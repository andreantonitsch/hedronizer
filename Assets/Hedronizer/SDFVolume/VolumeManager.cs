using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

//[RequireComponent(typeof(Texture3DBuilder))]
public class VolumeManager : MonoBehaviour
{

    struct Shape{
        public float4 rotation;
        public float3 position;
        public float3 scale;
        public float value1;
        public float value2;
    }

    RenderTextureFormat format = RenderTextureFormat.RFloat;
    // RenderTextureFormat format = RenderTextureFormat.ARGBFloat;


    public RenderTexture volume;

    [HideInInspector]
    public int3 resolution;
    [HideInInspector]
    public float3 scale;
    public Material TextureStartMaterial;
    public int MaximumShapes = 128;
    
    List<Shape> addSpheres = new List<Shape>();
    List<Shape> removeSpheres = new List<Shape>();
    List<Shape> shapes = new List<Shape>();

    public ComputeShader ShapeWriter; 
    GraphicsBuffer shapeBuffer;

    public bool dirty= false;

    public void Initialize(int3 resolution, float3 scale){
        this.resolution = resolution;
        this.scale = scale;
        var textureBuilder = new Texture3DBuilder(TextureStartMaterial);
        textureBuilder.InitializeTexture(resolution, format);
        textureBuilder.GenerateProcedural(TextureStartMaterial);
        volume = textureBuilder.volume;
        shapeBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured 
                                            ,GraphicsBuffer.UsageFlags.None,
                                            MaximumShapes ,  sizeof(float) * 12);
    }



    public void AddSphere(float3 position, float3 scale, float radius){
        
        Debug.Log("Test");
        addSpheres.Add(new Shape{
            position = position,
            rotation = new float4(Quaternion.identity.x,Quaternion.identity.y ,Quaternion.identity.z ,Quaternion.identity.w) ,
            scale =  scale,
            value1 = radius,
            value2 = 0.0f
        });

        dirty = true;

    }

    public void RemoveSphere(float3 position, float3 scale, float radius){
        

        removeSpheres.Add(new Shape{
            position = position,
            rotation = new float4(Quaternion.identity.x,Quaternion.identity.y ,Quaternion.identity.z ,Quaternion.identity.w) ,
            scale =  scale,
            value1 = radius,
            value2 = 0.0f
        });

        dirty = true;

    }

    public void DispatchShapes(){

        SetShaderParameters(resolution, scale);

        int sphereStart = shapes.Count;
        int sphereCount = addSpheres.Count;
        shapes.AddRange(addSpheres);	

        


        int remSphereStart = shapes.Count;
        int remSphereCount  = removeSpheres.Count;
        shapes.AddRange(removeSpheres);
        
        shapeBuffer.SetData<Shape>(shapes);

        if(sphereCount > 0){
            int kernel_id = ShapeWriter.FindKernel("add_spheres");
            ShapeWriter.SetBuffer(kernel_id, "_Shapes", shapeBuffer);
            ShapeWriter.SetTexture(kernel_id, "_VolumeTexture", volume);

            SetBufferLocation(sphereStart, sphereCount);
            ShapeWriter.Dispatch(kernel_id, resolution.x / 8, resolution.y / 8, resolution.z /8);

        }

        if(remSphereCount > 0){
            int kernel_id = ShapeWriter.FindKernel("remove_spheres");
            ShapeWriter.SetBuffer(kernel_id, "_Shapes", shapeBuffer);
            ShapeWriter.SetTexture(kernel_id, "_VolumeTexture", volume);
            SetBufferLocation(remSphereStart, remSphereCount);
            ShapeWriter.Dispatch(kernel_id, resolution.x / 8, resolution.y / 8, resolution.z /8);
        }
        addSpheres.Clear();
        removeSpheres.Clear();
        shapes.Clear();
        dirty = false;
    }


    void SetShaderParameters(int3 resolution, float3 scale)
    {
        ShapeWriter.SetVector("_Resolution", new Vector4(resolution.x, resolution.y, resolution.z, 0));
        ShapeWriter.SetVector("_Scale", new Vector4(scale.x, scale.y, scale.z, 0));
    }

    void SetBufferLocation(int startIndex, int shapeCount){
        ShapeWriter.SetInt("_ShapeCount", shapeCount);
        ShapeWriter.SetInt("_StartIndex", startIndex);
    }

    void Update(){
        if(dirty)
            DispatchShapes();
    }

    void OnDestroy(){

        volume?.Release();
        shapeBuffer?.Dispose();
    }

}
