using System;
using UnityEngine;
using  System.Timers;

namespace DefaultNamespace
{
    public class EventTest : MonoBehaviour
    {
        Timer timer = new Timer();

        protected void Start()
        {

            // timer.Interval = 1000;
            //
            // EventHandler1 eh1 = new EventHandler1();
            // timer.Elapsed += eh1.TimerHandler;
            //
            // EventHandler2 eh2 = new EventHandler2();
            // timer.Elapsed += eh2.Sing;
            //
            // timer.Start();
            
            Customer c = new Customer();
            Waiter w = new Waiter();
            //事件订阅
            //c.Order += w.Action;
            //c.eh2 += w.Action;
            c.EventOrder += w.Action;

            Debug.Log("开始点菜");
            c.OnOrder("水煮肉", "Big");
            c.OnOrder("土豆丝", "Small");
            c.OnOrder("炒黄瓜", "Small");
            c.NewCustormer.PayBill();
        }

        private void OnDisable()
        {
            timer.Stop();
        }
    }

    public class OrderEventArgs : EventArgs
    {
        public string name;
        public string size;
    }

    //委托类型的事件处理器
    public delegate void OrderEventHandler(Customer customer, OrderEventArgs e);
    
    //事件拥有者
    public interface ICustomer
    {
        void BeginOrder2();
    }

    public class HAHA
    {
        private Customer _customer;

        public HAHA(Customer customer)
        {
            _customer = customer;
        }

        public void BeginOrder2()
        {
            Debug.Log("开始点菜");
            _customer.OnOrder("水煮肉", "Big");
            _customer.OnOrder("土豆丝", "Small");
            _customer.OnOrder("炒黄瓜", "Small");
        }
    }

    public class NewCustormer
    {
        public NewCustormer()
        {
        }

        public double bill { get; set; }

        public void PayBill()
        {
            Debug.Log("付钱" + bill);
        }
    }

    public partial class Customer : ICustomer
    {
        private readonly HAHA _haha;
        private readonly NewCustormer _newCustormer;

        public Customer()
        {
            _haha = new HAHA(this);
            _newCus = new NewCus(this);
        }
        
        // //完整声明形式
        // private OrderEventHandler orderEventHandler;
        // //事件旧的声明方式
        // public event OrderEventHandler Order
        // {
        //     add
        //     {
        //         this.orderEventHandler += value;
        //     }
        //
        //     remove
        //     {
        //         this.orderEventHandler -= value;
        //     }
        // }

        //简化版事件声明方式
        public event EventHandler EventOrder;
        //
        // //委托声明方式（不安全，外部可以访问，并且执行，这就相当于暴露出一些隐患）
        // public OrderEventHandler eh2;

        public NewCustormer NewCustormer
        {
            get { return _newCustormer; }
        }

        public void BeginOrder2()
        {
            _haha.BeginOrder2();
        }
    }

    //事件响应者
    public class Waiter
    {
        //事件处理器
        public void Action(object customer, EventArgs e)
        {
            Customer c = customer as Customer;
            OrderEventArgs oe = e as OrderEventArgs;

            if (oe != null)
            {
                Debug.Log("服务员上菜" + oe.name);
                int price = 0;
                switch (oe.size)
                {
                    case "Small":
                        price = 10;
                        break;
                    case "Big":
                        price = 20;
                        break;
                    default:
                        price = 5;
                        break;
                }

                c.NewCustormer.bill += price;
            }
        }
    }
    
    
    public class EventHandler1
    {
        public void TimerHandler(object sender, ElapsedEventArgs e)
        {
            Debug.Log("EventHandler1 响应");
        }
    }

    public class EventHandler2
    {
        public void Sing(object sender, ElapsedEventArgs e)
        {
            Debug.Log("EventHandler2 响应唱歌");
        }
    }

    public class EventHandler3
    {
        
    }
}