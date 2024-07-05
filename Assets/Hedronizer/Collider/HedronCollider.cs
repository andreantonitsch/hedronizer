using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HedronCollider : MonoBehaviour
{

    public float Radius = 1.0f;
    HedronCollisionManager manager;
    Rigidbody rb;

    bool test = false;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindFirstObjectByType<HedronCollisionManager>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // test = !test;
        // if(test){
            var v = rb.position + (rb.velocity.normalized * Radius) + ( rb.velocity *  Time.fixedDeltaTime);
            var p = new float4(v.x, v.y, v.z, 1.0f);
            manager.AppendCollisionRequest(new() {position = p, direction = rb.velocity, radius=Radius});
    //    }

    }
}
