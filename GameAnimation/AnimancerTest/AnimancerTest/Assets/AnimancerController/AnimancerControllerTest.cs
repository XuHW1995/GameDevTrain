using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Animancer;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimancerController
{
    public class AnimancerControllerTest : MonoBehaviour
    {
        public Animator TestAnimator;
        public AnimationClip TestClip;
        public PlayableGraph Graph;
        public AnimationPlayableOutput Output;
        public AnimationClipPlayable ClipPlayable;
            
        public float speed = 0.5f;
        [Button]
        public void InitGraph()
        {
            Graph = PlayableGraph.Create("XHWTestGraph");
            Output = AnimationPlayableOutput.Create(Graph, "XHWAniOutPut", TestAnimator);
            ClipPlayable = AnimationClipPlayable.Create(Graph, TestClip);
            Output.SetSourcePlayable(ClipPlayable);
            
            var out2 = ScriptPlayableOutput.Create(Graph, "XHWCustomOutPut");
            var selfPlayable = ScriptPlayable<TestScriptPlayableBehavior>.Create(Graph, 1);
            out2.SetSourcePlayable(selfPlayable);
        }

        [Button]
        public void PlayGraph()
        {
            Graph.Play();
        }

        [Button]
        public void StopGraph()
        {
            Graph.Stop();
        }
        
        [Button]
        public void ChangeClipSpeed()
        {
            PlayableExtensions.SetSpeed(ClipPlayable, speed);
            //ClipPlayable.Play();
        }
        
        public float graphSpeed = 2;
        public void Update()
        {
            if (Graph.IsValid())
            {
                Debug.Log("XHW graph update");
                Graph.Evaluate(graphSpeed * Time.deltaTime);
            }
        }
        
        //反射
        // public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        // {
        //     //通过反射程序集找到所有继承自BTNodeBase的类 也就是找到所有节点类
        //     List<Type> types = new List<Type>();
        //     foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        //     {
        //         List<Type> result = assembly.GetTypes().Where(type =>
        //         {
        //             return type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BTNodeBase));
        //         }).ToList();
        //         types.AddRange(result);
        //     }
        // }
    }
}