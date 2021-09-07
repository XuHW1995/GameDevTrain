using System;
using UnityEngine;
using  System.Timers;

namespace DefaultNamespace
{
    public class EventTest : MonoBehaviour
    {
        Timer timer = new Timer();
        public void Start()
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
            c.Order += w.Action;
            c.eventOrder += w.Action;
            c.eh2 += w.Action;
            c.BeginOrder();
            c.PayBill();
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
    public class Customer
    {
        private OrderEventHandler orderEventHandler;
        //事件旧的声明方式
        public event OrderEventHandler Order
        {
            add
            {
                this.orderEventHandler += value;
            }

            remove
            {
                this.orderEventHandler -= value;
            }
        }

        //简化版事件声明方式
        public event OrderEventHandler eventOrder;
        
        //委托声明方式
        public OrderEventHandler eh2;
        
        public double bill { get; set; }

        public void PayBill()
        {
            Debug.Log("付钱" + bill);
        }

        public void Walkin()
        {
            Debug.Log("客人进来啦");
            
        }

        public void BeginOrder()
        {
            Debug.Log("开始点菜");

            OrderEventArgs e = new OrderEventArgs();
            e.name = "水煮肉";
            e.size = "Big";
            this.orderEventHandler(this, e);

            e.name = "土豆丝";
            e.size = "Small";
            this.eventOrder(this, e);
            
            e.name = "炒黄瓜";
            e.size = "Small";
            this.eh2(this, e);
        }
    }

    //事件响应者
    public class Waiter
    {
        //事件处理器
        public void Action(Customer customer, OrderEventArgs e)
        {
            Debug.Log("服务员上菜"+  e.name);
            int price = 0;
            switch (e.size)
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

            customer.bill += price;
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