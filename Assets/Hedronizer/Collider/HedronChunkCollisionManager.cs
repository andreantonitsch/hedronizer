using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;


public class HedronChunkCollisionManager : MonoBehaviour
{
    public GameObject ChunkPrefab;
    private Hedronizer hedronizer;
    public List<HedronChunkCollider> Chunks;
    public Texture3DBuilder volumeBuilder;
    public int3 ChunksPerAxis;
    public int chunk_cells_per_axis = 4;

    void Start(){
        Chunks = new List<HedronChunkCollider>();
        hedronizer = GetComponent<Hedronizer>();
    }
    // public void DirtyAllChunks(){

    // }

    // public void DirtyChunk(){

    // }

    // public void UpdateChunk(){

    // }

    public void Initialize(){
        float3 ori = hedronizer.transform.position;
        float3 sizes = hedronizer.size.xyz;
        Chunkify(new Bounds(ori, sizes), ChunksPerAxis);
    }

    public void Chunkify(Bounds bounds, int3 divisions)
    {
        float3 chunksize = bounds.size / (float3)divisions;
        float3 halfchunksize = chunksize /  2.0f;

        for(int i = 0; i < divisions.x; i++){
            for(int j = 0; j < divisions.z; j++){
                float3 origin = chunksize * new int3(i,0, j) - halfchunksize;
                GameObject chunk = Instantiate(ChunkPrefab);
                chunk.transform.parent = transform;
                HedronChunkCollider chunkCollider = chunk.GetComponent<HedronChunkCollider>();
                Chunks.Add(chunkCollider);
                chunkCollider.Initialize(origin, chunksize, chunk_cells_per_axis);
            }
        }

    }

}
