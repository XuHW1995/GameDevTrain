using UnityEngine;
using UnityEngine.Rendering;

namespace XSRP.Runtime
{
    public class XCameraRender
    {
        private ScriptableRenderContext _context;
        private Camera _camera;

        private const string bufferName = "XCameraRender renderCamera";
        private CommandBuffer cmb = new CommandBuffer{name = bufferName};

        private CullingResults _cullingResults;
        private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
        
        private static ShaderTagId[] legacyShaderTagIds = {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")
        };
        
        public void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            this._context = context;
            this._camera = camera;

            if (!Cull())
            {
                return;
            }
            
            SetUp();
            DrawVisibleGeometry();
            DrawUnsupportedShader();
            Submit();
        }

        private bool Cull()
        {
            if (_camera.TryGetCullingParameters(out ScriptableCullingParameters cullingParameters))
            {
                _cullingResults = _context.Cull(ref cullingParameters);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 渲染配置设置
        /// </summary>
        private void SetUp()
        {
            _context.SetupCameraProperties(_camera);
            cmb.ClearRenderTarget(true, true, Color.clear);
            
            cmb.BeginSample(bufferName);
            ExecuteCommandBuffer();
        }

        /// <summary>
        /// 渲染对象
        /// </summary>
        private void DrawVisibleGeometry()
        {
            //不透明
            var sortingSettings = new SortingSettings(_camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };
            var drawSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            _context.DrawRenderers(_cullingResults, ref drawSettings, ref filteringSettings);
            
            //天空盒
            _context.DrawSkybox(_camera);

            //透明
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            _context.DrawRenderers(_cullingResults, ref drawSettings, ref filteringSettings);
        }

        private void DrawUnsupportedShader()
        {
            var drawSettings = new DrawingSettings
            (
                legacyShaderTagIds[0], new SortingSettings(_camera)
            );
            
            for (int i = 1; i < legacyShaderTagIds.Length; i++) {
                drawSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
            }

            var filteringSettings = FilteringSettings.defaultValue;
            _context.DrawRenderers(_cullingResults, ref drawSettings, ref filteringSettings);
        }
        
        /// <summary>
        /// 执行渲染指令
        /// </summary>
        private void Submit()
        {
            cmb.EndSample(bufferName);
            ExecuteCommandBuffer();
            _context.Submit();
        }

        /// <summary>
        /// 复制cmb当前的指令到context，并清除buffer
        /// </summary>
        private void ExecuteCommandBuffer()
        {
            _context.ExecuteCommandBuffer(cmb);
            cmb.Clear();
        }
    }
}