using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class collision_text_adder : MonoBehaviour
{

    public enum ShapeFunction{
        SphereAdd,
        SphereRemove
    };
    
    public float radius = 3.0f;
    public ShapeFunction shape;

    void OnCollisionEnter(Collision collision){
        
        if(collision.gameObject.CompareTag("SDF")){
            switch(shape){
            case ShapeFunction.SphereAdd:
                    FindAnyObjectByType<VolumeManager>().AddSphere(collision.contacts[0].point, new float3(1, 1, 1), radius);
                    break;
            case ShapeFunction.SphereRemove:
                    FindAnyObjectByType<VolumeManager>().RemoveSphere(collision.contacts[0].point, new float3(1, 1, 1), radius);
                    break;
            }
            collision.gameObject.GetComponent<HedronChunkCollider>().SetDirty();
            Destroy(gameObject);
        }

    }

}
