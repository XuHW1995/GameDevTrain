using System.Collections.Generic;
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
    }
}