using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InClass10
{
    // We have created Entity for Product
    public class Product
    {
        public string ID { get; set; }
        public string productname { get; set; }
        public double price { get; set; }
        public ICollection<OrderProductDetail> relatedorders { get; set; }
    }
    // We have created Entity for Order
    public class Order
    {
        public string ID { get; set; }
        public DateTime orderdate { get; set; }
        public string customerName { get; set; }
        public ICollection<OrderProductDetail> orderedproducts { get; set; }
    }

    // We have created associate entity OrderDetail
    public class OrderProductDetail
    {
        public string ID { get; set; }
        public Order order { get; set; }
        public Product product { get; set; }
        public int quantity { get; set; }
    }

    class ApplicationDbContext : DbContext
    {
        public DbSet<Product> products { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderProductDetail> orderproductDetails { get; set; }

        string connectionString = "Server=(localdb)\\mssqllocaldb;Database=InClass10;Trusted_Connection=True;MultipleActiveResultSets=true";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.EnsureCreated();
                Product[] plist = new Product[]
               {
                    new Product{ID="1",productname="BudLight",price=5.49},
                    new Product{ID="2",productname="Corona",price=10.99},
                    new Product{ID="3",productname="Micheleb",price=9.99},
                    new Product{ID="4",productname="Modello",price=11.00},
                    new Product{ID="5",productname="Jack Daniel",price=33.00},
                    new Product{ID="6",productname="Borboun",price=25.00},
                    new Product{ID="7",productname="RedLabel",price=26.00}
               };

                Order[] olist = new Order[]
                {
                    new Order{ID="1",orderdate=DateTime.Parse("2021-04-05"),customerName="Samrat"},
                    new Order{ID="2",orderdate=DateTime.Parse("2021-03-26"),customerName="Michael"},
                    new Order{ID="3",orderdate=DateTime.Parse("2021-02-12"),customerName="Dharma"},
                    new Order{ID="4",orderdate=DateTime.Parse("2021-04-02"),customerName="Meghla"},
                    new Order{ID="5",orderdate=DateTime.Parse("2021-03-20"),customerName="Samarpan"},
                    new Order{ID="6",orderdate=DateTime.Parse("2021-03-20"),customerName="Phani"}
                };

                OrderProductDetail[] dlist = new OrderProductDetail[]
                {
                    new OrderProductDetail{ID="101",order=olist[1],product=plist[6],quantity=2},
                    new OrderProductDetail{ID="103",order=olist[0],product=plist[2],quantity=1},
                    new OrderProductDetail{ID="104",order=olist[0],product=plist[3],quantity=5},
                    new OrderProductDetail{ID="201",order=olist[1],product=plist[0],quantity=2},
                    new OrderProductDetail{ID="205",order=olist[1],product=plist[4],quantity=1},
                    new OrderProductDetail{ID="302",order=olist[2],product=plist[1],quantity=4},
                    new OrderProductDetail{ID="304",order=olist[3],product=plist[3],quantity=1},
                    new OrderProductDetail{ID="305",order=olist[2],product=plist[4],quantity=5},
                    new OrderProductDetail{ID="401",order=olist[3],product=plist[0],quantity=1},
                    new OrderProductDetail{ID="402",order=olist[3],product=plist[1],quantity=4},
                    new OrderProductDetail{ID="403",order=olist[3],product=plist[2],quantity=3},
                    new OrderProductDetail{ID="404",order=olist[3],product=plist[3],quantity=4},
                    new OrderProductDetail{ID="405",order=olist[3],product=plist[4],quantity=5},
                    new OrderProductDetail{ID="406",order=olist[5],product=plist[2],quantity=3},
                };
                if (!context.orders.Any())
                {
                    foreach (Order o in olist)
                    {
                        context.orders.Add(o);
                    }
                    context.SaveChanges();
                }

                if (!context.products.Any())
                {
                    foreach (Product p in plist)
                    {
                        context.products.Add(p);
                    }
                    context.SaveChanges();
                }

                if (!context.orderproductDetails.Any())
                {
                    foreach (OrderProductDetail d in dlist)
                    {
                        context.orderproductDetails.Add(d);
                    }
                    context.SaveChanges();
                }

                // List all orders where a product is sold.
                var a = context.orders
                    .Include(c => c.orderedproducts)
                    .Where(c => c.orderedproducts.Count != 0);
                Console.WriteLine("---------------Order List of sold products------------------");
                foreach (var i in a)
                {
                    Console.WriteLine("OrderID={0},OrderDate={1},CustomerName={2}", i.ID, i.orderdate, i.customerName);
                }

                Console.WriteLine("\nEnter Productname for Maximum Order : ");
                Console.WriteLine("\n | BudLight | Corona | Micheleb | Modello | Jack Daniel | Borboun | RedLabel |");
                string xyz = Console.ReadLine();

                // For a given product, find the order where it is sold the maximum.
                Order output = context.orderproductDetails
                    .Where(c => c.product.productname == xyz)
                    .OrderByDescending(c => c.quantity)
                    .Select(c => c.order)
                    .First();
                Console.WriteLine("---------------------Max {0} Order---------------------", xyz);
                Console.WriteLine("OrderID={0},OrderDate={1},CustomerName={2}", output.ID, output.orderdate, output.customerName);
            }
        }
    }
}