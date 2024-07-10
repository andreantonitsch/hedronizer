using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class collision_text_adder : MonoBehaviour
{
    

    void OnCollisionEnter(Collision collision){
        
        if(collision.gameObject.CompareTag("SDF")){
            FindAnyObjectByType<VolumeManager>().AddSphere(collision.contacts[0].point, new float3(1,1,1), 4);

            Destroy(gameObject);
        }

    }



}
