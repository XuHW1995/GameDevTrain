using UnityEngine;
using Cinemachine.Utility;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that adds a final offset to the camera
/// </summary>
[AddComponentMenu("")] // Hide in menu
#if UNITY_2018_3_OR_NEWER
[ExecuteAlways]
#else
[ExecuteInEditMode]
#endif
[HelpURL(Documentation.BaseURL + "api/Cinemachine.CinemachineCameraOffset.html")]
[SaveDuringPlay]
public class CinemachineCameraPosLimit : CinemachineExtension
{

    [Tooltip("Offset the camera's position by this much (camera space)")]
    public Vector3 m_Offset = Vector3.zero;
    
    /// <summary>
    /// If applying offset after aim, re-adjust the aim to preserve the screen position
    /// of the LookAt target as much as possible
    /// </summary>
    [Tooltip("If applying offset after aim, re-adjust the aim to preserve the screen position"
        + " of the LookAt target as much as possible")]
    public bool m_PreserveComposition;

    /// <summary>
    /// Applies the specified offset to the camera state
    /// </summary>
    /// <param name="vcam">The virtual camera being processed</param>
    /// <param name="stage">The current pipeline stage</param>
    /// <param name="state">The current virtual camera state</param>
    /// <param name="deltaTime">The current applicable deltaTime</param>
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {   
        if (stage == CinemachineCore.Stage.Finalize)
        {   
            var pos = state.RawPosition;
            if (pos.y < m_Offset.y)
            {   
                state.RawPosition = new Vector3( pos.x,m_Offset.y,pos.z );
            }   
        }   
    }
}
