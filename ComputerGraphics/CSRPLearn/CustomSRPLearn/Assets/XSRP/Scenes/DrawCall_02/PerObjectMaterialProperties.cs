using System;
using UnityEngine;

namespace Scenes.DrawCall_02
{
    public class PerObjectMaterialProperties : MonoBehaviour
    {
        private static int baseColorId = Shader.PropertyToID("_BaseColor");
        
        public Color baseColor = Color.black;

        static MaterialPropertyBlock block;

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (block == null)
            {
                block = new MaterialPropertyBlock();
            }
            
            block.SetColor(baseColorId, baseColor);
            GetComponent<Renderer>().SetPropertyBlock(block);
        }
    }
}