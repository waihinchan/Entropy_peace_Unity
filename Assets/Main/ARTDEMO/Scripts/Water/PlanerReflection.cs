using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;
using System;

namespace WaterSystem{
    [ExecuteAlways]

    public class PlanerReflection : MonoBehaviour
    {   public static event Action<ScriptableRenderContext, Camera> BeginPlanarReflections;
        public LayerMask ReflectLyaers;
        public GameObject TargetPlane;
        private Camera _reflectionCamera; //update this
        private RenderTexture _reflectionTexture;

        // Start is called before the first frame update
        public bool useShadow = false;
        private readonly int _planarReflectionTextureId = Shader.PropertyToID("_PlanarReflectionTexture");
        private void OnEnable()
        {    
            RenderPipelineManager.beginCameraRendering += ExecutePlanarReflections;
        }
        Camera CreateReflectionCamera(){
            if(!_reflectionCamera){
                GameObject tempCamera = new GameObject("Planar Reflections",typeof(Camera));
                var cameraData = tempCamera.AddComponent(typeof(UniversalAdditionalCameraData)) as UniversalAdditionalCameraData;
                cameraData.requiresColorOption = CameraOverrideOption.Off;
                cameraData.requiresDepthOption = CameraOverrideOption.Off;
                cameraData.SetRenderer(0); //this is custom scriptable render index. TODO add scriptable render
                Transform t = transform; //can attach this at water plane or things like that.
                Camera reflectionCamera = tempCamera.GetComponent<Camera>(); //get the camera component
                reflectionCamera.transform.SetPositionAndRotation(t.position, t.rotation); //set the position.
                reflectionCamera.depth = -10; //make sure this camera render before all other render.
                reflectionCamera.enabled = false;
                tempCamera.hideFlags = HideFlags.HideAndDontSave;

                return reflectionCamera;
            }
            else{
                return _reflectionCamera;
            }
        }
        private void UpdateReflectionCamera(Camera realCamera){ //real camera can be our target camera;
            //prepare stage
            if (_reflectionCamera == null){
                _reflectionCamera = CreateReflectionCamera();
            }
            Vector3 pos = Vector3.zero;
            Vector3 normal = Vector3.up;
            if (TargetPlane != null) //make sure the plane is not empty
            {
                pos = TargetPlane.transform.position; //get the plane position
                normal = TargetPlane.transform.up; //the normal always goes up(in object space)
            }
            UpdateCamera(realCamera,_reflectionCamera); //pass the param form source camera to dst camera
            // d is the distance of targetCamera to a plane's point
            // -d = dot(POS,N), pos is a point at the plane
            // d = -dot(POS,N) 
            float d = -Vector3.Dot(pos, normal) -0.07f;
            //prepare stage : we have normal, pos d;
            //get the reflect matrix
            Matrix4x4 reflectMtrix = Matrix4x4.identity;
            //copy this from https://www.programmersought.com/article/49224297943/
            reflectMtrix.m00 = 1 - 2 * normal.x * normal.x;
            reflectMtrix.m01 = -2 * normal.x * normal.y;
            reflectMtrix.m02 = -2 * normal.x * normal.z;
            reflectMtrix.m03 = -2 * d * normal.x;
    
            reflectMtrix.m10 = -2 * normal.x * normal.y;
            reflectMtrix.m11 = 1 - 2 * normal.y * normal.y;
            reflectMtrix.m12 = -2 * normal.y * normal.z;
            reflectMtrix.m13 = -2 * d * normal.y;
    
            reflectMtrix.m20 = -2 * normal.x * normal.z;
            reflectMtrix.m21 = -2 * normal.y * normal.z;
            reflectMtrix.m22 = 1 - 2 * normal.z * normal.z;
            reflectMtrix.m23 = -2 * d * normal.z;
    
            reflectMtrix.m30 = 0;
            reflectMtrix.m31 = 0;
            reflectMtrix.m32 = 0;
            //copy this from https://www.programmersought.com/article/49224297943/

            _reflectionCamera.worldToCameraMatrix = realCamera.worldToCameraMatrix * reflectMtrix; //GET the Target Camera's matrix;
            //get the reflect matrix
            //this is MVRP. R is reflections. before projection correct the view matrix; //now this render will move to the opposite of our real camera;
            //NOTE: Why don't add a actual camera and follow the real camera forever..
            //move the reflect camera
            var clipPlane = CameraSpacePlane(_reflectionCamera, pos - Vector3.up * 0.1f, normal, 1.0f);
            var projection = realCamera.CalculateObliqueMatrix(clipPlane);
            _reflectionCamera.projectionMatrix = projection;
            _reflectionCamera.cullingMask = ReflectLyaers; // never render water layer
            _reflectionCamera.transform.position = new Vector3(realCamera.transform.position.x,-(realCamera.transform.position.y - pos.y*2),realCamera.transform.position.z);
            _reflectionCamera.transform.forward = Vector3.Scale(realCamera.transform.forward, new Vector3(1, -1, 1)); //inverse the direction
            //move the reflect camera

        }
        private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            var offsetPos = pos + normal *0.07f;
            var m = cam.worldToCameraMatrix;
            var cameraPosition = m.MultiplyPoint(offsetPos);
            var cameraNormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cameraNormal.x, cameraNormal.y, cameraNormal.z, -Vector3.Dot(cameraPosition, cameraNormal));
        }
        private float GetScaleValue()
        {
            // switch(m_settings.m_ResolutionMultiplier)
            // {
            //     case ResolutionMulltiplier.Full:
            //         return 1f;
            //     case ResolutionMulltiplier.Half:
            //         return 0.5f;
            //     case ResolutionMulltiplier.Third:
            //         return 0.33f;
            //     case ResolutionMulltiplier.Quarter:
            //         return 0.25f;
            //     default:
            //         return 0.5f; // default to half res
            // }
            return 0.5f; //sofar only return a half res
        }
        private int2 ReflectionResolution(Camera cam, float scale) //just copy this. don't wanna dig too depp
        {
            var x = (int)(cam.pixelWidth * scale * GetScaleValue());
            var y = (int)(cam.pixelHeight * scale * GetScaleValue());
            return new int2(x, y);
        }
        private void OnDisable()
        {
            Cleanup();
        }
        private static void SafeDestroy(UnityEngine.Object obj)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
        private void Cleanup()
        {
            RenderPipelineManager.beginCameraRendering -= ExecutePlanarReflections;

            if(_reflectionCamera)
            {
                _reflectionCamera.targetTexture = null;
                SafeDestroy(_reflectionCamera.gameObject);
            }
            if (_reflectionTexture)
            {
                RenderTexture.ReleaseTemporary(_reflectionTexture);
            }
        }
        private void PlanarReflectionTexture(Camera cam)
        {
            if (_reflectionTexture == null)
            {
                var res = ReflectionResolution(cam, UniversalRenderPipeline.asset.renderScale);
                const bool useHdr10 = true;
                const RenderTextureFormat hdrFormat = useHdr10 ? RenderTextureFormat.RGB111110Float : RenderTextureFormat.DefaultHDR;
                _reflectionTexture = RenderTexture.GetTemporary(res.x, res.y, 16,GraphicsFormatUtility.GetGraphicsFormat(hdrFormat, true));
            }
            _reflectionCamera.targetTexture =  _reflectionTexture;
        }

        private void ExecutePlanarReflections(ScriptableRenderContext context, Camera camera)
        {
            // we dont want to render planar reflections in reflections or previews
            if (camera.cameraType == CameraType.Reflection || camera.cameraType == CameraType.Preview)
                return;

            UpdateReflectionCamera(camera); // i rewrite this.
            PlanarReflectionTexture(camera); // create and assign RenderTexture

           
            RenderSet();

            BeginPlanarReflections?.Invoke(context, _reflectionCamera); // callback Action for PlanarReflection
            // https://www.cnblogs.com/lyyzhi/p/12876916.html
            //if BeginPlanarReflections==null. not invoke anything.
            UniversalRenderPipeline.RenderSingleCamera(context, _reflectionCamera); // render planar reflections
            GL.invertCulling = false;
            RenderSettings.fog = RenderSettings.fog;
            QualitySettings.maximumLODLevel = QualitySettings.maximumLODLevel;
            QualitySettings.lodBias = QualitySettings.lodBias;

            Shader.SetGlobalTexture(_planarReflectionTextureId, _reflectionTexture); // Assign texture to water shader
        }
        private void RenderSet(){
            GL.invertCulling = true;
            //The back cropping needs to be reversed, because only the vertices are changed, the normal vector is not changed, the winding order is reversed, and the cropping will be wrong
            RenderSettings.fog = false; //fog is not used temp TODO: fix the fog projections(if i can)
            QualitySettings.maximumLODLevel = 1; //should i set this to 0?
            QualitySettings.lodBias = QualitySettings.lodBias * 0.5f;
        }
        private void UpdateCamera(Camera src, Camera dest)
        {
            if (dest == null) return;

            dest.CopyFrom(src);
            dest.useOcclusionCulling = false;
            if (dest.gameObject.TryGetComponent(out UniversalAdditionalCameraData camData))
            {
                camData.renderShadows = useShadow; // turn off shadows for the reflection camera
            }
        }
    }
}
