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
        float3 origin = hedronizer.transform.position;
        float3 scales = hedronizer.size.xyz;
        Chunkify(origin, scales, ChunksPerAxis);
    }

    public void Chunkify(float3 origin, float3 scale, int3 divisions)
    {
        float3 chunksize = scale / (float3)divisions;
        float3 halfchunksize = chunksize /  2.0f;

        for(int j = 0; j < divisions.z; j++){
        for(int i = 0; i < divisions.x; i++){
                float3 position = chunksize * new int3(i,0, j);
                GameObject chunk = Instantiate(ChunkPrefab);
                chunk.transform.parent = transform;
                HedronChunkCollider chunkCollider = chunk.GetComponent<HedronChunkCollider>();
                Chunks.Add(chunkCollider);
                float3 cell_size = new float3(chunksize.x / chunk_cells_per_axis, 0, chunksize.z / chunk_cells_per_axis);
                chunkCollider.Initialize(position, chunksize, chunk_cells_per_axis);
            }
        }

    }

}
