using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hedronizer))]
public class HedronizerRenderer : MonoBehaviour
{

    [SerializeField]
    Hedronizer hedronizer;

    public Material material;

    void Start(){
        hedronizer = GetComponent<Hedronizer>();
        //GraphicsSettings.useScriptableRenderPipelineBatching = false;
    }

    void Update(){
        hedronizer.RunVisualization();
        Draw();
    }

    void Draw() {
        RenderParams rp = new RenderParams(material);

        Vector3 b1 = hedronizer.transform.position;
        Vector3 b2 = hedronizer.size.xyz; 

        rp.worldBounds = new Bounds(b1, b2);
        rp.matProps = new MaterialPropertyBlock();

        rp.matProps.SetBuffer("_Data", hedronizer.computeBuffer);
        rp.matProps.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
        Graphics.RenderPrimitives(rp, MeshTopology.Triangles, hedronizer.ItensInBuffer * 3, 1);
        //Graphics.RenderPrimitivesIndirect(rp, MeshTopology.Triangles, hedronizer.ArgsBuffer, 1);



    }


}
