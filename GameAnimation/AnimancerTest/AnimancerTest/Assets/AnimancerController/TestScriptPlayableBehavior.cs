using UnityEngine;
using UnityEngine.Playables;

namespace AnimancerController
{
    public class TestScriptPlayableBehavior : PlayableBehaviour
    {
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            Debug.Log("XHW OnBehaviourPlay");
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            Debug.Log("XHW PrepareFrame");
        }
    }
}