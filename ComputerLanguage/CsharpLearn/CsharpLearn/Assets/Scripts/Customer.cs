namespace DefaultNamespace
{
    public class NewCus
    {
        private Customer _customer;

        public NewCus(Customer customer)
        {
            _customer = customer;
        }

        public void OldTestFun()
        {
            _customer.OnOrder("hah", "small");
        }


    }

    public partial class Customer
    {
        private readonly NewCus _newCus;

        public NewCus NewCus
        {
            get { return _newCus; }
        }
        
        /// <summary>
        /// 事件执行器，一般用OnXXX命名
        /// </summary>
        /// <param name="dishName"></param>
        /// <param name="size"></param>
        public void OnOrder(string dishName, string size)
        {
            if (EventOrder != null)
            {
                OrderEventArgs e = new OrderEventArgs();
                e.name = dishName;
                e.size = size;
                EventOrder(this, e);
            }
        }
    }
}