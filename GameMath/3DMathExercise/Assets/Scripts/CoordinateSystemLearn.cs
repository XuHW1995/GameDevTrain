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

        public Text text;
        public void ShowText()
        {
            text.text = "Hello world!";
        }
    }
}