using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    // Модель Клиент
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }

    // Модель Услуга 
    public class Service
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal BasePrice { get; set; }
    }

    // Модель Заказ
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public string ItemDescription { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal ServiceCost { get; set; }
        public string Status { get; set; }
        public DateTime? CompletionDate { get; set; }
    }

    // Модель Квитанция
    public class Receipt
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime PrintDate { get; set; }
    }
}
