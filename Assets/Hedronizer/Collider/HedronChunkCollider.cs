using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshCollider), typeof(MeshFilter), typeof(BoxCollider)),
RequireComponent(typeof(Hedronizer))]
public class HedronChunkCollider : MonoBehaviour
{

    private struct Tri{
        public float4 v1;
        public float4 v2;
        public float4 v3;
    }

    public Hedronizer parent_hedronizer;
    public Hedronizer hedronizer;
    Mesh[] meshes;
    Texture3DBuilder volumeBuilder;
    BoxCollider eventCollider;
    MeshCollider collider;
    MeshFilter filter;
    bool dirty = true;
    Transform transform;
    int currentMesh = 0;
    Tri[] triangles;
    List<Vector3> listVertices;
    List<int> listIndexes; 

    public float3 Size;

    public void Initialize(float3 origin, float3 sizes, int cell_per_axis){
        transform = GetComponent<Transform>();
        collider = GetComponent<MeshCollider>();
        filter = GetComponent<MeshFilter>();
        volumeBuilder = FindFirstObjectByType<Texture3DBuilder>();
        hedronizer  = GetComponent<Hedronizer>();
        parent_hedronizer  = transform.parent.gameObject.GetComponentInParent<Hedronizer>();
        hedronizer.IsChild = true;
        hedronizer.cells_per_axis = cell_per_axis;
        hedronizer.CopyParameters(parent_hedronizer);
        hedronizer.Initialize();
        
        meshes = new Mesh[2];
        //breaking triangles into vertices sooner
        triangles = new Tri[hedronizer.EstimateComputeBufferSize(0)];
        transform.position = origin;
        Size = sizes;
        hedronizer.sample_region = sizes;
        meshes[0] = new Mesh();
        meshes[1] = new Mesh();

        listIndexes = new List<int>();
        listVertices = new List<Vector3>();

        Bake(0);
        filter.mesh = meshes[0];
        collider.sharedMesh = meshes[0];

        // Rebake(0);
        // Rebake(1);

    }

    //initializes stuff
    public void Bake(int index)
    {
        listIndexes.Clear();
        listVertices.Clear();

        hedronizer.RunCollision();
        hedronizer.computeBuffer.GetData(triangles);

        for (int i = 0; i < hedronizer.ItensInBuffer; i++)
        {

                listIndexes.Add(i*3);
                listIndexes.Add(i*3+1);
                listIndexes.Add(i*3+2);
                listVertices.Add(triangles[i].v1.xyz);
                listVertices.Add(triangles[i].v2.xyz);
                listVertices.Add(triangles[i].v3.xyz);

        }

        meshes[index].Clear() ;
        meshes[index].SetVertices(listVertices);
        meshes[index].SetTriangles(listIndexes,0);
        filter.mesh = meshes[index];
        //meshes[index].UploadMeshData(false);

        //Physics.BakeMesh(meshes[index].GetInstanceID(), false, MeshColliderCookingOptions.WeldColocatedVertices);
        // MeshColliderCookingOptions.CookForFasterSimulation 
        //                                                       | 
        //                                                       | MeshColliderCookingOptions.EnableMeshCleaning
        //                                                       | MeshColliderCookingOptions.UseFastMidphase);
    }

    //does it async
    public void Rebake(int index){

        

    }

    public void MeshSwap(){

        currentMesh = (currentMesh + 1) % 2; 
        filter.mesh = meshes[currentMesh];
    }

    void OnDrawGizmos(){    
        Gizmos.color = Color.white;
        float3 flat = new float3(Size.x, 0.0f, Size.z);
        float3 pos = new float3(transform.position.x, 0.0f, transform.position.z);
        Gizmos.DrawWireCube(pos, flat);
    }

    void OnDrawGizmosSelected(){
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Size);
    }

    void BuildMesh  (){



    }


}
