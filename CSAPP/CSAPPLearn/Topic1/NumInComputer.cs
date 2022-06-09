using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CSAPPLearn.Topic1
{
    [XHWTest(age = 27, name = "xuhongwei dadiu")]
    public static class NumInComputer
    {
        public static void IntPrint()
        {
            //TestIntAndLong();
            //TestDataInMemoryStruct();
            //TestValueAndReference();
            //Complatement();
            //UintTest();

            TestAttribute();
        }

        #region int和long在计算机中的表示
        public static void TestIntAndLong()
        {
            int a = 40000 * 40000;
            Console.WriteLine($"a = {a}");

            //32位整数会越界，所以改成long 64位整数
            long b = 2500000000;
            int c = 2147483647;
            Console.WriteLine($"b = {b}");
        }
        #endregion
        
        #region 数据在内存中的存储方式，如果不检测越界会出现数据错误的问题

        //测试数据在内存中的存储方式
        public static void TestDataInMemoryStruct()
        {
            for (int i = 0; i <= 6; i++)
            {
                double result = Test(i);
                Console.WriteLine($" i= {i} 结果 = {result}");
            }
        }
        
        //测试内存引用错误
        public struct TestMemoryReferencing
        {
            public int[] a;
            public double b;
        }
        
        public static double Test(int i)
        {
            TestMemoryReferencing testMemoryReferencing = new TestMemoryReferencing();
            //声明区块大小
            testMemoryReferencing.a = new int[2];

            testMemoryReferencing.b = 3.14;
            testMemoryReferencing.a[i] = 1073741824;
            return testMemoryReferencing.b;
        }
        #endregion
        
        #region 值类型和引用类型
        public class ReferenceObj
        {
            public string name;
            public int age;
        }

        //变量分为值类型的变量  和引用类型的变量
        //值类型的变量 存的东西就是数据本身， 引用类型的变量存的数据是 数据的地址，后面通过地址索引的方式进行数据访问
        //当通过变量访问数据的时 值类型的变量直接就可以进行数据访问，引用类型的变量还需要多进行一次寻址
        //总结：分清楚，值类型数据，引用类型数据，函数参数传递默认是以值拷贝的形式取进行的（值类型数据直接进行数据拷贝，引用类型数据把地址进行拷贝），但是引用类型变量的值本身是一个 地址，所以即便拷贝了还是一个地址，通过这个地址取取数据，进行修改就会改变外部的数据
        //但是如果在内部new了一个新的，就相当于，把存参数的值改成了 另外一个地址，这个之后，再去改就不会影响到外部了
        //string 比较特殊  给string 赋值其实就相当于在new 一个新的 string类型的 数据，相当于改了一次这个string变量引用的地址
        
        //参数里带上ref 相当于通过引用进行参数传递，传递过去的参数不再是复制出来的一个新的东西，而是用的传递时候的那个地址
        public static void TestValueAndReference()
        {
            ReferenceObj a = new ReferenceObj();
            a.age = 6;
            a.name = "小丢丢";
            
            ChangeAll(a);
            //ChangeAll(ref a);
            
            //ChangeName(a.name);
            //ChangeName(ref a.name);
            
            //ChangeAgeVal(a.age);
            //ChangeAgeRef(ref a.age);
            Console.WriteLine(a.name + "age = "+ a.age);
        }
        
        //引用类型对象，默认以引用类型参数进行传递
        
        //以传值(值拷贝)的方式进行参数传递，但是传递的参数本身是一个数据的引用，也就是地址，所以在内部进行数据修改，可以影响到外部
        private static void ChangeAll(ReferenceObj referenceObj)
        {
            referenceObj.age = 12;
            referenceObj.name = "大丢丢";
        }
        
        private static void ChangeAll(ref ReferenceObj referenceObj)
        {
            referenceObj = new ReferenceObj();
            referenceObj.age = 12;
            referenceObj.name = "大丢丢";
        }

        //以传值的方式进行参数传递
        private static void ChangeAgeVal(int age)
        {
            age = 15;
        }
        
        //以传引用的方式进行参数传递
        private static void ChangeAgeRef(ref int age)
        {
            age = 15;
        }

        private static void ChangeName(string name)
        {
            //此处的name = "大丢丢" 相当于是  进行了一次new操作，所以并不会改变外边的数据
            name = "大丢丢";
        }
        
        private static void ChangeName(ref string name)
        {
            name = "大丢丢";
        }
        
        //Hello world的一生
        #endregion
        
        #region 有符号数在计算机中的表示方式，都是用补码的方式来表示的

        //正数的补码是本身，负数的补码是  原码除符号位取反再+1
        public static void Complatement()
        
        {
            int result = -3 << 1;
            
            // -3 = 1011(原) ————1100（除符号位取反） + 1 = 1101（-3的补码，也就是计算机中的有符号数表示） 左移一位 = 1010（-3<<1）= -8 + 2 = -6(1010 补)  如果想补码再求原码 则对补码再求一次补
            //目的是为了  把正负号全部统一再一个域内，并且+-法也再一个域内
            
            //result = -6
            Console.WriteLine(result);
        }

        #endregion

        public static void UintTest()
        {
            uint x = 1;
            object y = 2U;
            int result = x.CompareTo(y);
            List<int> lx = new List<int>();
        }

        public static void TestAttribute()
        {
            Type x = typeof(NumInComputer);
            object[] attrs = x.GetCustomAttributes(typeof(XHWTestAttribute), false);
            foreach (var attrData in attrs)
            {
                XHWTestAttribute xhwTestAttribute = attrData as XHWTestAttribute;
                if (xhwTestAttribute != null)
                {
                    Console.WriteLine($"XHW age = {xhwTestAttribute.age}, name = {xhwTestAttribute.name}");
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class XHWTestAttribute : Attribute
    {
        public int age;
        public string name;
    }
}