using System;
using UnityEngine;

namespace MoveAndRotateTest
{
    public class MoveAndRotateTest: MonoBehaviour
    {
        //目标向量
        public Vector3 lineA = new Vector3(6,6,6);
        //待投影向量
        public Vector3 lineB = Vector3.right;
        //待投影平面
        public Vector3 planXYNormalVector = Vector3.up;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(Vector3.zero, lineA);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Vector3.zero, lineB);
            
            Gizmos.color = Color.magenta;
            Vector3 projectOnLineB = Vector3.Project(lineA, lineB);
            Gizmos.DrawLine(Vector3.zero, projectOnLineB);
            
            Gizmos.color = Color.green;
            Vector3 projectOnPlaneXY = Vector3.ProjectOnPlane(lineA, planXYNormalVector);
            Gizmos.DrawLine(Vector3.zero, projectOnPlaneXY);
        }
    }
}