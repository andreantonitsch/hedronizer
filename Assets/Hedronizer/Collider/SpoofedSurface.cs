using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoofedSurface : MonoBehaviour
{

    Transform t;

    public void Start(){
        t = transform; 
    }
    public void SpoofCollision(CollisionData data)
    {
        if(data.distance > 0.01 || data.distance < -0.01) { t.position = Vector3.one * 1000; return;}
        t.position = new Vector3(data.position.x, data.position.y, data.position.z);
        var v = new Vector3(data.normal.x,
                            data.normal.y,
                            data.normal.z);
        t.localRotation *= Quaternion.FromToRotation(t.forward, v);
        //t.position -= t.forward * t.localScale.z/2;
    }


}
