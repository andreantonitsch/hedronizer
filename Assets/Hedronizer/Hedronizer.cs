using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering;
public class Hedronizer : MonoBehaviour
{
    public Texture3DBuilder volumeBuilder;
    public MeshFilter obj;
    public Mesh mesh;
    public Material material;

    GraphicsBuffer computeBuffer;
    GraphicsBuffer vertexBuffer;
    GraphicsBuffer argsBuffer;

    public ComputeShader shader;
    public int thread_groups = 8;
    public int cells_per_axis;

    public float4 cell_size;
    public float4 origin;
    public float4 size;

    [Range(0,1)]
    public float isovalue = 0.3f;

    [Range(0,1)]
    public float gradientH = 0.001f;

    public int bufferSize;

    public bool ready = false;

    // Start is called before the first frame update
    void Start()
    {   
        // each facet is 3 x vertices of 2 x float4 each
        int quantity_of_vec4s = 3;
        computeBuffer  = new GraphicsBuffer(GraphicsBuffer.Target.Structured | 
                                            GraphicsBuffer.Target.Append | 
                                            GraphicsBuffer.Target.CopySource,
                                            GraphicsBuffer.UsageFlags.None,
                                            bufferSize ,  sizeof(float) * 3 * 4 * quantity_of_vec4s);
        //computeBuffer = new ComputeBuffer(bufferSize,  sizeof(float) * 3 * 4 * quantity_of_vec4s, ComputeBufferType.Append);
        //data_array = new Vector4[bufferSize * 3 * quantity_of_vec4s];
        vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopyDestination, GraphicsBuffer.UsageFlags.None, bufferSize,  sizeof(float) * 3 * 4 * quantity_of_vec4s);

    }
    
    public void Run()
    {

        // 'clears' the buffer
        computeBuffer.SetCounterValue(0);

        cell_size = size / (float4)cells_per_axis;
        //Debug.Log(cell_size);
        int kernel_id = shader.FindKernel("hedronize");
        shader.SetBuffer(kernel_id, "vertex_buffer", computeBuffer);
        shader.SetVector("_Origin", origin);
        shader.SetVector("_CellSize", cell_size);
        shader.SetFloat("_Time", Time.time);
        shader.SetFloat("_Isovalue", isovalue);
        shader.SetFloat("_GradientH", gradientH);
        shader.SetVector("_Size", size);
        shader.SetTexture(kernel_id, "_SDF", volumeBuilder.volume);

        //var graphicsFence = Graphics.CreateGraphicsFence(GraphicsFenceType.CPUSynchronisation, SynchronisationStageFlags.ComputeProcessing);

        shader.Dispatch(kernel_id, cells_per_axis / thread_groups, cells_per_axis / thread_groups, cells_per_axis / thread_groups);

        //while(!graphicsFence){}

        //Graphics.CopyBuffer(computeBuffer, vertexBuffer);

        // mesh = new Mesh();
        // obj.mesh = mesh;

        // vertex_array = new Vector3[bufferSize * 3];
        // normal_array = new Vector3[bufferSize * 3];
        // color_array = new Color[bufferSize * 3];
        // tris = new int[bufferSize * 3];


        // computeBuffer.GetData(data_array);
        // // walks 24 steps of data
        // // // reads 3 vertices, with 2 vector4 each
        // for (int i = 0; i < data_array.Length; i+=9)
        // {
        //     int tri_index = i/3;

        //     tris[tri_index] = tri_index;
        //     tris[tri_index + 1] = tri_index + 1;
        //     tris[tri_index + 2] = tri_index + 2;

        //     //consumes 1 triangle, 6 float4, 
        //     vertex_array[tri_index ] =  data_array[i];
        //     normal_array[tri_index ] =  data_array[i + 1];
        //     color_array[tri_index]   =  data_array[i + 2];

        //     vertex_array[tri_index + 1] = data_array[i + 3];
        //     normal_array[tri_index + 1] = data_array[i + 4];
        //     color_array[tri_index  + 1] = data_array[i + 5];

        //     vertex_array[tri_index + 2] = data_array[i + 6];
        //     normal_array[tri_index + 2] = data_array[i + 7];
        //     color_array[tri_index  + 2] = data_array[i + 8];

        // }

        // mesh.vertices = vertex_array;
        // mesh.triangles = tris;
        // mesh.normals = normal_array;
        // mesh.colors = color_array;
    }

    void OnDestroy(){
        computeBuffer?.Dispose();
        vertexBuffer?.Dispose();
    }

    void Update()
    {
        if(ready)
        {
            Run();
            Draw();
        }

    }


    void Draw() {
        RenderParams rp = new RenderParams(material);

        Vector3 b1 = new Vector3(origin.x, origin.y, origin.z);
        Vector3 b2 =  new Vector3(size.x, size.y, size.z) + b1; 

        rp.worldBounds = new Bounds(b1, b2);
        //rp.worldBounds = new Bounds(Vector3.zero, 10000*Vector3.one); // use tighter bounds
        rp.matProps = new MaterialPropertyBlock();

        rp.matProps.SetBuffer("_Data", computeBuffer);
        rp.matProps.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
        Graphics.RenderPrimitives(rp, MeshTopology.Triangles, bufferSize * 3, 1);
    }

    public void SetupTexture(){
        //volumeBuilder.Initialize();
        //volumeBuilder.GenerateProcedural();
        if(volumeBuilder.volume.IsCreated())
            ready = true;
    }

    void writeCommandBuffer(){

    }

}
