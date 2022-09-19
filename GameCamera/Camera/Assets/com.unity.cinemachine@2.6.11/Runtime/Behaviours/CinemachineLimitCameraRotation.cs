
using UnityEngine;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that adjusts
    /// the FOV of the lens to keep the target object at a constant size on the screen,
    /// regardless of camera and target position.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [AddComponentMenu("")] // Hide in menu
    [SaveDuringPlay]
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class CinemachineLimitCameraRotation : CinemachineExtension
    {

        public float m_XRotation = 70f;

        public CinemachineCore.Stage m_Stage =  CinemachineCore.Stage.Body;


        class VcamExtraState
        {
            public float m_previousFrameZoom = 0;
        }

        /// <summary>Callback to preform the zoom adjustment</summary>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {

            if (m_Stage == stage)
            {
                var rota = state.RawOrientation;
                float angle = rota.eulerAngles.x - 180;

                if (angle > 0)
                     angle -= 180;
                else
                    angle += 180; 
                if(angle >= m_XRotation)
                {
                    rota.eulerAngles = new Vector3(m_XRotation, rota.eulerAngles.y, rota.eulerAngles.z);
                }
                else if(angle <= 0)
                {
                    rota.eulerAngles = new Vector3(0, rota.eulerAngles.y, rota.eulerAngles.z);
                }

                state.RawOrientation = rota;
            }
        }
    }
}
