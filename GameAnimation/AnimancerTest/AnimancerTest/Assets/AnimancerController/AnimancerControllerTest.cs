using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimancerController
{
    public class AnimancerControllerTest : MonoBehaviour
    {
        public Animator testAnimator;
        public AnimationClip testClip;
        
        [Button]
        public void InitGraph()
        {
            var graph = PlayableGraph.Create("XHWTestGraph");

            var output = AnimationPlayableOutput.Create(graph, "XHWAniOutPut", testAnimator);

            var clipPlayable = AnimationClipPlayable.Create(graph, testClip);

            output.SetSourcePlayable(clipPlayable);
            
                
            var out2 = ScriptPlayableOutput.Create(graph, "XHWCustomOutPut");
            var selfPlayable = ScriptPlayable<TestScriptPlayableBehavior>.Create(graph, 1);
            out2.SetSourcePlayable(selfPlayable);
            
            graph.Play();
            
            testAnimator.Play("haha");
            
        }
    }
}