using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering;


//Generates a vertexBuffer with a triangulated SDF at some isosurface level
public class Hedronizer : MonoBehaviour
{
    //public Texture3DBuilder volumeBuilder;
    public int3 VolumeResolution = new int3(256,256,256);
    public VolumeManager volumeManager;
    public GraphicsBuffer computeBuffer;
    //public GraphicsBuffer vertexBuffer;
    GraphicsBuffer argsBuffer;

    public ComputeShader shader;
    public ComputeShader argsBufferCorrectshader;
    uint3 visualization_thread_groups = 8;
    uint3 collision_thread_groups = 8;
    public int cells_per_axis;

    float4 cell_size;
    //public float4 origin;
    public float4 size;
    public float3 sample_region;

    [Range(-1000,1000)]
    public float isovalue = 0.3f;

    [Range(0,1)]
    public float gradientH = 0.001f;

    public int bufferSize;
    public int max_triangles;
    public int min_triangles;
    public bool ready = false;
    private int quantity_of_vec4s = 2;
    public bool IsChild = true;

    bool cached_buffer_count = false;
    private int  buffer_count = 0;
    public int ItensInBuffer {
        get{  
            if(!cached_buffer_count){
                int[] args = new int[]{ 0, 1, 0, 0 };
                argsBuffer.SetData(args);
                GraphicsBuffer.CopyCount(computeBuffer, argsBuffer, 0);
                // TODO inneficient
                argsBuffer.GetData(args);
                buffer_count = args[0];
                cached_buffer_count = true;
            }

            return buffer_count;
        }
    }

    bool cached_args_buffer = false;
    public GraphicsBuffer ArgsBuffer {
        get{  
            if(!cached_args_buffer){

                GraphicsBuffer.CopyCount(computeBuffer, argsBuffer, 0);
                // corrects the instance count
                argsBufferCorrectshader.SetBuffer(0, "_ArgsBuffer", argsBuffer);
                argsBufferCorrectshader.Dispatch(0, 1,1,1);
                cached_args_buffer = true;
            }

            return argsBuffer;
        }
    }

    // Start is called before the first frame update
    void Start()
    {   
        if(!IsChild){
            volumeManager.Initialize(VolumeResolution, size.xyz);
            Initialize();
        }
    }

    public void Initialize(){

        sample_region = size.xyz;
        if(IsChild){

            var parent_hedronizer = transform.parent.gameObject.GetComponentInParent<Hedronizer>();
            isovalue = parent_hedronizer.isovalue;
            gradientH = parent_hedronizer.gradientH;
            //origin = parent_hedronizer.origin;
            size = parent_hedronizer.size;
            volumeManager = parent_hedronizer.volumeManager;
            quantity_of_vec4s = 1;
            
        }

        argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments | 
                                        GraphicsBuffer.Target.CopyDestination |
                                        GraphicsBuffer.Target.Structured,  
                                        GraphicsBuffer.UsageFlags.None,  1, 4 * sizeof(int));

       
        // each facet is 3 x vertices of 2 x float4 each
        bufferSize = EstimateComputeBufferSize(max_triangles);

        computeBuffer  = new GraphicsBuffer(GraphicsBuffer.Target.Structured 
                                            | GraphicsBuffer.Target.Append
                                            | GraphicsBuffer.Target.CopySource
                                            ,GraphicsBuffer.UsageFlags.None,
                                            bufferSize ,  ComputeStrideSize());

        //computeBuffer = new ComputeBuffer(bufferSize,  sizeof(float) * 3 * 4 * quantity_of_vec4s, ComputeBufferType.Append);
        //data_array = new Vector4[bufferSize * 3 * quantity_of_vec4s];
        //vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopyDestination, GraphicsBuffer.UsageFlags.None, bufferSize,  ComputeStrideSize());
        
        int kernel_id = shader.FindKernel("hedronize");
        shader.GetKernelThreadGroupSizes(kernel_id, out visualization_thread_groups.x, out visualization_thread_groups.y, out visualization_thread_groups.z);
        kernel_id = shader.FindKernel("collisionizer");
        shader.GetKernelThreadGroupSizes(kernel_id, out collision_thread_groups.x, out collision_thread_groups.y, out collision_thread_groups.z);
    }

    public void FillShaderSDFParameters(ComputeShader target_shader){
        target_shader.SetVector("_Position", transform.position);
        //target_shader.SetVector("_SampleRegion", sample_region.xyzz);
        target_shader.SetVector("_CellSize", cell_size);
        //target_shader.SetVector("_Cells", (sample_region / size.xyz * cells_per_axis).xyzz);
        target_shader.SetFloat("_Time", Time.time);
        target_shader.SetFloat("_Isovalue", isovalue);
        target_shader.SetFloat("_GradientH", gradientH);
        target_shader.SetVector("_Scale", size);
    }

    public void RunCollision()
    {        
        cached_buffer_count = false;
        cached_args_buffer = false;
        // 'clears' the buffer
        computeBuffer.SetCounterValue(0);
        cell_size = sample_region.xyzz / (float4)cells_per_axis;
        int kernel_id = shader.FindKernel("collisionizer");
        shader.SetBuffer(kernel_id, "collider_buffer", computeBuffer);
        shader.SetTexture(kernel_id, "_SDF", volumeManager.volume);
        FillShaderSDFParameters(shader);


        shader.Dispatch(kernel_id, (int)math.max(cells_per_axis   / collision_thread_groups.x,1),
                                   (int)math.max(cells_per_axis   / collision_thread_groups.y,1),
                                   (int)math.max(cells_per_axis   / collision_thread_groups.z,1));
    }

    public void RunVisualization()
    {        
        // 'clears' the buffer
        cached_buffer_count = false;
        cached_args_buffer = false;
        computeBuffer.SetCounterValue(0);

        cell_size = size / (float4)cells_per_axis;
        //Debug.Log(cell_size);
        int kernel_id = shader.FindKernel("hedronize");
        shader.SetBuffer(kernel_id, "vertex_buffer", computeBuffer);
        shader.SetTexture(kernel_id, "_SDF", volumeManager.volume);
        FillShaderSDFParameters(shader);


        shader.Dispatch(kernel_id, (int)math.max(cells_per_axis / visualization_thread_groups.x,1),
                                   (int)math.max(cells_per_axis / visualization_thread_groups.y,1),
                                   (int)math.max(cells_per_axis / visualization_thread_groups.z,1));

    }

    void OnDestroy(){
        argsBuffer?.Dispose();
        computeBuffer?.Dispose();
    }

 
    public int ComputeStrideSize(){
        //each stride holds 1 triangle.
        return sizeof(float) * 3 * 4 * quantity_of_vec4s;
    }

    int ComputerBufferSize(){
        //quanitty of cube cells
        int cells = cells_per_axis * cells_per_axis * cells_per_axis;
        int hedrons = cells * 6;
        int max_facets = hedrons * 2;
        return max_facets;
    }

    public int EstimateComputeBufferSize(int max_tris){        

        float estimated_occupation = 5.5f;
        int computed_buffer_size = ComputerBufferSize();
        int estimate = (int)(estimated_occupation * computed_buffer_size);
        estimate += (3 * quantity_of_vec4s) - (estimate % quantity_of_vec4s);
        if(max_tris != 0)
            return math.max(min_triangles, math.min(max_tris, estimate));
        else
            return estimate;
    }

    //copy shared parameters of target hedronizer
    //for use with children hedronizers.
    //TODO  swap for scriptable object or something.
    // so each set of hedronizers can share a single configuration set of vars
    public void CopyParameters(Hedronizer target){
        shader = target.shader;
        size = target.size;
        isovalue = target.isovalue;
        gradientH = target.gradientH;
        volumeManager = target.volumeManager;
    }

}
