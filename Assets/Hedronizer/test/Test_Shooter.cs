using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class Test_Shooter : MonoBehaviour
{
    
    public GameObject[] objects;
    public Camera camera;
    public InputActionAsset actions;

    public float Force = 100;
    public float offset = 5;
    public bool held = false;
    void Awake(){
        actions.FindActionMap("actionmap").FindAction("ShootLeft").performed += (InputAction.CallbackContext context) => {ShootObject(0);};
        //actions.FindActionMap("actionmap").FindAction("ShootLeft").performed += (InputAction.CallbackContext context) => {held = context.ReadValueAsButton();};
        actions.FindActionMap("actionmap").FindAction("ShootRight").performed += (InputAction.CallbackContext context) => {ShootObject(1);};
    }

    public void ShootObject(int index){
        float3 pos = camera.transform.position;

        Ray dir = camera.ScreenPointToRay(Input.mousePosition);

        pos += (float3)(dir.direction) * offset;

        var obj = Instantiate(objects[index]).GetComponent<Rigidbody>();

        obj.position = pos;
        obj.AddForce(dir.direction * Force);
    }
    void OnEnable()
    {
        actions.FindActionMap("actionmap").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("actionmap").Disable();
    }


    void Update(){
        //if(held)ShootObject(0);
    }


}
