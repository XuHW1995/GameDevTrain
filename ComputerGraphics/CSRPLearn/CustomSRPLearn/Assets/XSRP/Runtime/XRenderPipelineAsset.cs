using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using XSRP.Runtime;

[CreateAssetMenu(menuName = "XRendering/XRenderingAssets")]
public class XRenderPipelineAsset : RenderPipelineAsset
{
    public Color TestColor;
    public Material TestMaterial;
    public Shader TestShader;
    public Mesh TestMesh;
    public string TestName = "XRednerAssets";
    public bool SRPBatches = true;
    
    protected override RenderPipeline CreatePipeline()
    {
        return new XRenderPipeline(this);
    }
}
