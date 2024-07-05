using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


public struct CollisionData
{
    public float4 position;
    public float3 normal;
    public float distance;
};

public struct CollisionRequest{
    public float4 position;
    public float3 direction;
    public float radius;
    
};

public class HedronCollisionManager : MonoBehaviour
{

    public Texture3DBuilder volumeBuilder;
    public Hedronizer hedronizer;
    GraphicsBuffer collisionBuffer;
    GraphicsBuffer collisionRequestBuffer;

    public int maximumCollisions;
    public ComputeShader shader;
    public SpoofedSurface[] spoofingSurfaces;

    public List<CollisionRequest> requests;
    public CollisionData[] collisions;
    
    void Start(){
        requests = new List<CollisionRequest>();
        collisions = new CollisionData[maximumCollisions];
        collisionBuffer  = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                                            GraphicsBuffer.UsageFlags.None,
                                            maximumCollisions ,  sizeof(float) * 8);

        collisionRequestBuffer  = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                                            GraphicsBuffer.UsageFlags.None,
                                            maximumCollisions ,  sizeof(float) * 8);

        volumeBuilder = FindFirstObjectByType<Texture3DBuilder>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RunBatchedCollisions();
    }

    public void AppendCollisionRequest(CollisionRequest request){
        requests.Add(request);
    }

    void RunBatchedCollisions(){
        //change to asyncreadback
        if(requests.Count == 0) return;

        collisionRequestBuffer.SetData(requests);
        int kernel_id = shader.FindKernel("collisionize");
        shader.SetBuffer(kernel_id, "_Spheres", collisionRequestBuffer);
        shader.SetBuffer(kernel_id, "_Collisions", collisionBuffer);
        shader.SetTexture(kernel_id, "_SDF", volumeBuilder.volume);
        hedronizer.FillShaderSDFParameters(shader);
        shader.Dispatch(kernel_id, (requests.Count / 8) + 1, 1, 1);

        collisionBuffer.GetData(collisions, 0, 0, requests.Count);

        MoveSurfaces(requests.Count);

        requests.Clear();
    }


    void MoveSurfaces(int quantity){

        for (int i = 0; i < quantity; i++)
        {
            spoofingSurfaces[i].SpoofCollision(collisions[i]);
        }

    }


    void OnDestroy(){
        collisionRequestBuffer?.Dispose();
        collisionBuffer?.Dispose();
    }


}
