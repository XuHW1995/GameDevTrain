using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace XSRP.Runtime
{
    public partial class XCameraRender
    {
#if UNITY_EDITOR
        private static ShaderTagId[] legacyShaderTagIds = {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")
        };
        
        private static Material errorMaterial;
        private void DrawUnsupportedShader()
        {
            if (errorMaterial == null)
            {
                errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
            }
            
            var drawSettings = new DrawingSettings
            (
                legacyShaderTagIds[0], new SortingSettings(_camera)
            )
            {
                overrideMaterial = errorMaterial
            };
            
            for (int i = 1; i < legacyShaderTagIds.Length; i++) {
                drawSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
            }

            var filteringSettings = FilteringSettings.defaultValue;
            _context.DrawRenderers(_cullingResults, ref drawSettings, ref filteringSettings);
        }

        private void DrawGizmos()
        {
            if (Handles.ShouldRenderGizmos())
            {
                _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
                _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
            }
        }

        private void PrepareForSceneWindows()
        {
            if (_camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
            }
        }
#endif
    }
}