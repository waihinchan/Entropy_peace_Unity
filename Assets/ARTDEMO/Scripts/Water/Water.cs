using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;
namespace WaterSystem
{
    [ExecuteAlways]
    public class Water : MonoBehaviour
    {
        //这个水体系统主要是用来赋予全局变量.
        //不负责drawMesh,和原来的系统相比有所简化.泛用性更强.
        public Gradient _absorptionRamp;
        public Gradient _scatterRamp;
        private Texture2D _rampTexture; //we assemble this because we can custom the color of our water
        public Texture2D _tempBindFoamRamp;
        private Camera _depthCam;
        public float depthExtra = 4.0f; //这个可以方便调整.
        public bool DebugMode = false;
        public float maxVisibleSize = 2; 
        public float waterMaxDepthVisibility = 20;
        public LayerMask preDepthLayer; //这个是加入预烘焙深度的图层 不在这个图层的不写入深度.
        [SerializeField] 
        public RenderTexture _depthTex; //保持实例化. 不太确认如果说有新的内容的时候是否需要实时更新.可以留在后面测试.
        private static readonly int AbsorptionScatteringRamp = Shader.PropertyToID("_AbsorptionScatteringRamp");
        private static readonly int WaterDepthMap = Shader.PropertyToID("_WaterDepthMap");
        private static readonly int DepthCamZParams = Shader.PropertyToID("_DepthCamParams");
        private static readonly int MaxDepth = Shader.PropertyToID("_MaxDepth");
        
        private void OnEnable()
        {   
            GetInstanceDepth();
            if (_depthTex)
            {
                Shader.SetGlobalTexture(WaterDepthMap, _depthTex);
            }
            GenerateColorRamp();
        }
        public void GetInstanceDepth(){
            // if(!_depthCam){
                var tempDepthCamera = new GameObject("depthCamera") {hideFlags = HideFlags.HideAndDontSave};
                // 
                // 
                _depthCam = tempDepthCamera.AddComponent<Camera>(); //添加camera组件
                if(DebugMode){
                    Debug.Log("succeed create a depth camera !");
                }
            // }
            //照抄ws
            var additionalCamData = _depthCam.GetUniversalAdditionalCameraData();
            additionalCamData.renderShadows = false;
            additionalCamData.requiresColorOption = CameraOverrideOption.Off;
            additionalCamData.requiresDepthOption = CameraOverrideOption.Off;
            //照抄ws
            //TODO : 测试一下这些项是用来做什么的.
            var newTransform = _depthCam.transform;
            newTransform.position = transform.position + Vector3.up * (transform.position.y + depthExtra); //往上移一定距离.
            newTransform.up = Vector3.forward; //垂直照着地面
            _depthCam.enabled = true;
            _depthCam.orthographic = true;
            if(maxVisibleSize<=0){maxVisibleSize = 250;}
            _depthCam.orthographicSize = maxVisibleSize; //这个是最大可视范围
            _depthCam.nearClipPlane =0.01f;
            _depthCam.farClipPlane = waterMaxDepthVisibility + depthExtra; //这里改了一下变量名应该比较好理解.
            _depthCam.allowHDR = false;
            _depthCam.allowMSAA = false;
            _depthCam.cullingMask = preDepthLayer;
            if(!_depthTex){
                _depthTex = new RenderTexture(1024, 1024, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear); //记住写入是线性

            }
            //平台差异 暂时不知道原因 照抄
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
            {
                _depthTex.filterMode = FilterMode.Point;
            }
            //平台差异 暂时不知道原因 照抄
            _depthTex.wrapMode = TextureWrapMode.Clamp;
            _depthTex.name = "PreBakeWaterDepthMap";
            _depthCam.targetTexture = _depthTex;
            _depthCam.Render();
            _depthCam.enabled = false;
            _depthCam.targetTexture = null;
            // DestroyImmediate(tempDepthCamera);
             var _params = new Vector4(newTransform.position.y, 250, 0, 0);
            Shader.SetGlobalVector(DepthCamZParams, _params);
            Shader.SetGlobalFloat(MaxDepth, waterMaxDepthVisibility);
            if(DebugMode){

                Debug.Log(tempDepthCamera);
            }
        }
        private void GenerateColorRamp()
        {
            if(_rampTexture == null)
                _rampTexture = new Texture2D(128, 3, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
            _rampTexture.wrapMode = TextureWrapMode.Clamp;

            // var defaultFoamRamp = resources.defaultFoamRamp;
            var defaultFoamRamp = _tempBindFoamRamp;

            var cols = new Color[384];
            for (var i = 0; i < 128; i++)
            {
                cols[i] = _absorptionRamp.Evaluate(i / 128f);
            }
            for (var i = 0; i < 128; i++)
            {
                cols[i + 128] = _scatterRamp.Evaluate(i / 128f);
            }
            for (var i = 0; i < 128; i++)
            {
                cols[i + 256] = defaultFoamRamp.GetPixelBilinear(i / 128f , 0.5f);
            


            }
            _rampTexture.SetPixels(cols);
            _rampTexture.Apply();
            Shader.SetGlobalTexture(AbsorptionScatteringRamp, _rampTexture);
        }
    }
    
}
