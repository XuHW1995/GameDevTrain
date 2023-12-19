using UnityEngine;
using UnityEngine.Rendering;

namespace XSRP.Runtime
{
    public partial class XCameraRender
    {
        private ScriptableRenderContext _context;
        private Camera _camera;
        
        private const string bufferName = "XCameraRender renderCamera";
#if UNITY_EDITOR
        private string SampleName
        {
            get;
            set;
        }
#else
        private string SampleName = bufferName;
#endif
        private CommandBuffer cmb = new CommandBuffer{name = bufferName};
        
        private CullingResults _cullingResults;
        
        private static ShaderTagId[] allDrawShaderTagIds = {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("XHWLightMode"),
        };
        
        public void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            this._context = context;
            this._camera = camera;

#if UNITY_EDITOR
            PrepareForSceneWindows();
#endif
            PrepareCmdBuffer();
            
            if (!Cull())
            {
                return;
            }
            
            SetUp();
            DrawVisibleGeometry();
#if UNITY_EDITOR
            DrawUnsupportedShader();
            DrawGizmos();
#endif
            Submit();
        }
        
        private void PrepareCmdBuffer()
        {
            cmb.name = _camera.name;
            SampleName = _camera.name;
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

            CameraClearFlags flags = _camera.clearFlags;
            cmb.ClearRenderTarget(flags < CameraClearFlags.Depth, flags <= CameraClearFlags.Color, Color.clear);
            
            cmb.BeginSample(SampleName);
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
            var drawSettings = new DrawingSettings(allDrawShaderTagIds[0], sortingSettings);
            for (int i = 1; i < allDrawShaderTagIds.Length; i++) {
                drawSettings.SetShaderPassName(i, allDrawShaderTagIds[i]);
            }
            
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
        
        /// <summary>
        /// 执行渲染指令
        /// </summary>
        private void Submit()
        {
            cmb.EndSample(SampleName);
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