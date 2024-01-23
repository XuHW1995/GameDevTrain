using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TESTFPX
{
    public class TestIEnumarator : MonoBehaviour
    {
        [Button]
        public void TestEnumaratorCall()
        {
            IEnumerable<int> iterable = CreateEnumerable();
            IEnumerator<int> iterator = iterable.GetEnumerator();
            Debug.Log("Start to iterate");
            while (true)
            {
                Debug.Log("Calling MoveNext()");
                bool result = iterator.MoveNext();
                Debug.Log($"MoveNext result = {result}");

                if (!result)
                {
                    break;
                }
                
                Debug.Log($"Fetching current = {iterator.Current}");
            }
        }

        private readonly string padding = "【迭代器内部逻辑】";

        IEnumerable<int> CreateEnumerable()
        { 
            Debug.Log($"{padding} Start of CreateEnumerable");
            for (int i = 0; i < 3; i++)
            {
                Debug.Log($"{padding}Before to yield {i}");
                yield return i;
                Debug.Log($"{padding}After to yield {i}");
            }
            
            Debug.Log($"{padding}Yielding final value");
            yield return -1;
            Debug.Log($"{padding}End of CreateEnumerable");
        }

                
        [DllImport("XCplusplusforUnity.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetDistance(float x1, float y1, float x2, float y2);

        [DllImport("XCplusplusforUnity.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AddInt(int a, int b);
        
        [Button]
        public void TestCallNativeCode(Transform a, Transform b)
        {
            var pos1 = a.transform.position;
            var pos2 = b.transform.position;
            Debug.Log("Distance of Two Cube");
            float dis = GetDistance(pos1.x, pos1.y, pos2.x, pos2.y);
            Debug.Log("Distance:" + dis);
        }

        [Button]
        public void TestCAdd(int a, int b)
        {
            int x = AddInt(a, b);
            Debug.Log($"Call c++ add result =  {x}");
        }
    }
}