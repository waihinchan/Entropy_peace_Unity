using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSelect : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera _camera;
    RaycastHit _hit;
    public LayerMask selectMask;
    private Material currentMaterial;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("aaa");
        if(Input.GetKey(KeyCode.Mouse0)){
            if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),out _hit)){
                // _drawMaterial.SetVector("_Coord",new Vector4(_hit.textureCoord.x,_hit.textureCoord.y,0,0));
                Material tempMaterial = _hit.collider.gameObject.GetComponent<MeshRenderer>().material;
                if(currentMaterial==null){
                    currentMaterial = tempMaterial;
                }
                if(tempMaterial!=currentMaterial){
                    currentMaterial.SetFloat("_OnSelect",0);
                    tempMaterial.SetFloat("_OnSelect",1);
                    currentMaterial = tempMaterial;
                }
                else{
                    currentMaterial.SetFloat("_OnSelect",1);
                }
                Debug.Log(tempMaterial);
                
            }
        }
    }
}
