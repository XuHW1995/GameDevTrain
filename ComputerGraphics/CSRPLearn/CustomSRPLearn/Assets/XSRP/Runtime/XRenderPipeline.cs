﻿using UnityEngine;
using UnityEngine.Rendering;

namespace XSRP.Runtime
{
    public class XRenderPipeline : RenderPipeline
    {
        private XRenderPipelineAsset useAssets;
        
        public XRenderPipeline(XRenderPipelineAsset asset)
        {
            this.useAssets = asset;
        }
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            //Debug.Log("XHW XRender do renderLoop" + useAssets.TestName);

            var cmd = new CommandBuffer();
            cmd.ClearRenderTarget(true, true, Color.black);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();

            foreach (var oneCamera in cameras)
            {
                oneCamera.TryGetCullingParameters(out var cullingParameters);
                var cullingResult = context.Cull(ref cullingParameters);
                
                context.SetupCameraProperties(oneCamera);

                ShaderTagId shaderTagId = new ShaderTagId("XHWLightMode");
                var sortSetting = new SortingSettings(oneCamera);

                DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortSetting);
                FilteringSettings filteringSettings = FilteringSettings.defaultValue;
                
                context.DrawRenderers(cullingResult, ref drawingSettings, ref filteringSettings);

                if (oneCamera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
                {
                    context.DrawSkybox(oneCamera);
                }
                
                context.Submit();
            }
        }
    }
}