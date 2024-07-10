using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
public class Texture3DBuilder
{

    public RenderTexture volume;
    public Material material;
    int pass = 0;

    //use powers of 2
    int3 _resolution;

    public string fileName;
    
    public Texture3DBuilder(Material material) { this.material = material; }

    // Update is called once per frame
    public RenderTexture GenerateProcedural(Material material)
    {
        Blit3D(volume, _resolution.z, material);
        
        return volume; 
    }

    public void InitializeTexture(int3 resolution, RenderTextureFormat format)
    {
        //RenderTextureDescriptor desc = new RenderTextureDescriptor(resolution.x, resolution.y, format, 0, 0, RenderTextureReadWrite.Linear);
        //desc.volumeDepth = resolution.z;
        //desc.enableRandomWrite =true;
        //volume = new RenderTexture(desc);

        _resolution = resolution;
        volume = new RenderTexture(_resolution.x, _resolution.y, 0, format, RenderTextureReadWrite.Linear);
        volume.volumeDepth = _resolution.z;
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
