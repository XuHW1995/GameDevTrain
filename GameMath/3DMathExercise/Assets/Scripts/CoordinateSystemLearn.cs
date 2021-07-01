using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    //坐标系学习
    public class CoordinateSystemLearn : MonoBehaviour
    {
        #region Position
        public Transform testFatherTrans;
        public Transform testSonTrans;

        [ContextMenu("测试Position")]
        public void TestPosition()
        {
            Debug.Log("Father" + testFatherTrans.position.ToString() + testFatherTrans.localPosition.ToString());
            Debug.Log("Son" + testSonTrans.position.ToString() + testSonTrans.localPosition.ToString());
        }
        #endregion

        #region EulerAngle
        [ContextMenu("测试EulerAngle")]
        public void TestEulerAngle()
        {
            Debug.Log("Father" + testFatherTrans.rotation.eulerAngles + testFatherTrans.localRotation.eulerAngles);
            Debug.Log("Son" + testSonTrans.rotation.eulerAngles + testSonTrans.localRotation.eulerAngles);
        }
        #endregion
        
        #region Rotation??? 四元数转换了解原理
        [ContextMenu("测试Rotation")]
        public void TestRotation()
        {
            Debug.Log("Father" + testFatherTrans.rotation.ToString() + testFatherTrans.localRotation.ToString());
            Debug.Log("Son" + testSonTrans.rotation.ToString() + testSonTrans.localRotation.ToString());
        }
        #endregion

        #region 坐标系转换??了解原理
        [ContextMenu("测试坐标系转换 local to world ")]
        public void TestCoordSwitch()
        {
            Debug.Log(" Son: TransformDirection" + testSonTrans.TransformDirection(new Vector3(1,2,3)));
            Debug.Log(" Son: TransformPoint" + testSonTrans.TransformPoint(new Vector3(1,2,3)));
            Debug.Log(" Son: TransformVector" + testSonTrans.TransformVector(new Vector3(1,2,3)));
        }
        #endregion

        #region 变换

        public Vector3 from;
        public Vector3 to;

        //新坐标系Y轴正方向
        public Vector2 newCoordSystemYPositive;
        //新坐标系中的位置
        public Vector2 newCoordSystemPos;
        [ContextMenu("测试变换Transform")]
        public void TestTransform()
        {
            float _2dangle = Vector2.SignedAngle(from, to);
            Debug.Log("2D夹角" + _2dangle);

            float _3dangle = Vector3.SignedAngle(from, to, Vector3.up);
            Debug.Log("3D夹角, 相对轴为up" + _3dangle);

            float _3dangle2 = Vector3.SignedAngle(from, to, Vector3.down);
            Debug.Log("3D夹角, 相对轴为down" + _3dangle2);
            
            TransformVector(newCoordSystemYPositive, newCoordSystemPos);
        }
        private void TransformVector(Vector2 newForward, Vector2 newCoordSystemPos)
        {
            float rotateRad = Vector2.Angle(Vector2.up, newForward)/Mathf.Rad2Deg;
            float worldPosX = newCoordSystemPos.x * Mathf.Cos(rotateRad) +
                              newCoordSystemPos.y * Mathf.Sin(rotateRad);


            float worldPosY = newCoordSystemPos.y * Mathf.Cos(rotateRad) -
                              newCoordSystemPos.x * Mathf.Sin(rotateRad);
                              
            
            Debug.Log("原始坐标系中的位置为" + worldPosX + "---" + worldPosY);
        }
        #endregion
        
        public Text text;
        public void ShowText()
        {
            text.text = "Hello world!";
        }
    }
}