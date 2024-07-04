using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Texture3DBuilder : MonoBehaviour
{

    public RenderTexture volume;
    public Material material;
    public int pass = 0;

    //use powers of 2
    public Vector3Int resolution;

    public RenderTextureFormat format = RenderTextureFormat.RFloat;

    public string fileName;

    public void Start()
    {
        Initialize();
        GenerateProcedural();
    }
    
    // Update is called once per frame
    public void GenerateProcedural()
    {
        Blit3D(volume, resolution.z, material);
    }

    public void Initialize()
    {
        //RenderTextureDescriptor desc = new RenderTextureDescriptor(resolution.x, resolution.y, format, 0, 0, RenderTextureReadWrite.Linear);
        //desc.volumeDepth = resolution.z;
        //desc.enableRandomWrite =true;
        //volume = new RenderTexture(desc);

        volume = new RenderTexture(resolution.x, resolution.y, 0, format, RenderTextureReadWrite.Linear);
        volume.volumeDepth = resolution.z;
        volume.enableRandomWrite = true;
        volume.dimension = TextureDimension.Tex3D;

        volume.Create();

    }

    public void Load()
    {

    }

    public void Bake()
    {

    }


    void OnDestroy()
    {
        volume?.Release();
    }


    void Blit3D(RenderTexture tex, int size, Material blitMat) {
    GL.PushMatrix();
    GL.LoadOrtho();
    for (int i = 0; i < size; ++i) {
        // make sure tex.dimension is Tex3D. I think it works with Tex2DArray too
        Graphics.SetRenderTarget(tex, 0, CubemapFace.Unknown, i);
        blitMat.SetFloat("_DepthSlice", (float)i / (float)(size - 1));
        // because i was using this for noise generation, I wanted the uvs to be strictly 0 to 1
        // (i had to remap the x and y coords in the shader as well)
        // but you may want to change it to how textures normally go 
        // (something like 0.5 -> size - 0.5)
        float z = Mathf.Clamp01(i / (float)(size - 1));

        blitMat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.TexCoord3(0, 0, z);
        GL.Vertex3(0, 0, 0);
        GL.TexCoord3(1, 0, z);
        GL.Vertex3(1, 0, 0);
        GL.TexCoord3(1, 1, z);
        GL.Vertex3(1, 1, 0);
        GL.TexCoord3(0, 1, z);
        GL.Vertex3(0, 1, 0);

        GL.End();
    }
    GL.PopMatrix();
}
}
