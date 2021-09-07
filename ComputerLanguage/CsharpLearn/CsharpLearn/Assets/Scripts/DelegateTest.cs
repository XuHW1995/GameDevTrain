using System;
using System.Threading;
using UnityEngine;

namespace DefaultNamespace
{
    public delegate int TestCalc(int x, int y);
    
    public class DelegateTest : MonoBehaviour
    {
        public void Start()
        {
            Calculator c = new Calculator();
            
            //内置委托，无返回值
            Action action1 = new Action(c.Fun1);
            action1.Invoke();
            action1();
            Action<int, int> action2 = new Action<int, int>(c.Fun2);
            action2(100, 200);
            //内置委托有返回值
            Func<int, int, int> fun1 = new Func<int, int, int>(c.Fun3);
            int result = fun1(100, 200);
            Debug.Log(result);
            
            //自定义委托
            TestCalc tc = new TestCalc(c.Fun3);
            Debug.Log(tc(200, 100));
            
            //自定义委托模板方法调用方式
            Debug.Log("模板方法调用"+ TemplateFun(tc));
            //自定义委托回调调用方式
            CallBackFun(tc);
            
            //多播委托
            TestCalc tc2 = new TestCalc(c.Fun3);
            tc += tc2;
            Debug.Log(tc(200, 100));
            
            //同步调用
            //直接调用
            c.Fun1();
            Thread.Sleep(1000);
            c.Fun2(100,1);
            Thread.Sleep(1000);
            c.Fun3(100, 2);
            Thread.Sleep(1000);
            
            //通过委托进行间接同步调用
            tc(100, 2);
            
            //委托的隐式异步调用
            
            //显示异步调用 多线程

        }

        private int TemplateFun(TestCalc tc)
        {
            int templateFunResult = tc(100, 200);
            return templateFunResult + 1;
        }

        private void CallBackFun(TestCalc tcCallbacl)
        {
            int x = 100;
            int y = 200;
            if (tcCallbacl != null)
            {
                tcCallbacl(x + y, x - y);
            }
        }
    }

    public class Calculator
    {
        public void Fun1()
        {
            Debug.Log("测试下哈哈哈哈");
        }

        public void Fun2(int a, int b)
        {
            int all = a + b;
            Debug.Log(all);
        }

        public int Fun3(int a, int b)
        {
            Debug.Log("Fun3 invoke");
            return a - b;
        }
        
        public int Fun4(int a, int b)
        {
            Debug.Log("Fun4 invoke");
            return a + b;
        }
    }
}