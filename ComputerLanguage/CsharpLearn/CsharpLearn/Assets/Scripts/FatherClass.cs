using System;

namespace DefaultNamespace
{
    public class FatherClass
    {
        
    }

    public class SonClass : FatherClass
    {
        public void main()
        {
            SonClass a = new SonClass();
            Type x = typeof(SonClass);
            x.GetMembers();
        }
    }
}