using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklad
{
    internal class Program
    {
        static void Main ( string[] args )
        {
            List<Pallet> pallets = GenerateData();

            var groupedPallets = pallets
                .GroupBy(p => p.GetExpirationDate())
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    ExpirationDate = g.Key,
                    Pallets = g.OrderBy(p => p.Weight).ToList()
                }).ToList();

            Console.WriteLine("Паллеты, сгруппированные по сроку годности (возрастание) и отсортированные по весу (в каждой группе):");
            foreach (var group in groupedPallets)
            {
                Console.WriteLine($"Срок годности: {group.ExpirationDate.ToShortDateString()}");
                foreach (var pallet in group.Pallets)
                {
                    Console.WriteLine($"- Паллета ID: {pallet.ID}, Вес: {pallet.Weight}, Объем: {pallet.Volume()}");
                }
            }


            var top3Pallets = pallets
                .OrderByDescending(p => p.GetExpirationDate())
                .Take(3)
                .OrderBy(p => p.Volume())
                .ToList();

            Console.WriteLine("\n3 паллеты с наибольшим сроком годности (отсортированные по возрастанию объема):");
            foreach (var pallet in top3Pallets)
            {
                Console.WriteLine($"- Паллета ID: {pallet.ID}, Срок годности: {pallet.GetExpirationDate().ToShortDateString()}, Объем: {pallet.Volume()}");
            }

            Console.ReadKey();
        }


        static List<Pallet> GenerateData ( )
        {
            List<Pallet> pallets = new List<Pallet>();


            Box box1 = new Box(1, 10, 20, 30, 5, DateTime.Now.AddDays(-50), null); // Production Date
            Box box2 = new Box(2, 15, 25, 35, 7, null, DateTime.Now.AddDays(30)); // Expiration Date
            Box box3 = new Box(3, 12, 18, 24, 4, DateTime.Now.AddDays(20), null); // Production Date
            Box box4 = new Box(4, 10, 10, 10, 2, null, DateTime.Now.AddDays(60)); // Expiration Date
            Box box5 = new Box(5, 14, 22, 28, 6, DateTime.Now.AddDays(-20), null); // Production Date


            Pallet pallet1 = new Pallet(1, 100, 100, 100);
            pallet1.AddBox(box1);
            pallet1.AddBox(box2);

            Pallet pallet2 = new Pallet(2, 120, 120, 120);
            pallet2.AddBox(box3);
            pallet2.AddBox(box4);

            Pallet pallet3 = new Pallet(3, 110, 110, 110);
            pallet3.AddBox(box5);

            Pallet pallet4 = new Pallet(4, 90, 90, 90);


            pallets.Add(pallet1);
            pallets.Add(pallet2);
            pallets.Add(pallet3);
            pallets.Add(pallet4);

            return pallets;
        }

    
     }
    public abstract class AllClass
    {
        public int ID { get; set; }
        public double Width {  get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public double Weight { get; set; }

        public abstract double Volume ( );
    }
    public class Pallet:AllClass
    {
        private const double PalletWeight = 20;
        public double _width;
        public double _height;
        public double _depth;
        public List<Box> Boxes { get; set; } = new List<Box>();

        public Pallet ( int id, double width, double height, double depth )
        {
            ID = id;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public void AddBox ( Box box )
        {
            if (box.Width > Width || box.Depth > Depth)
            {
                Console.WriteLine("Коробка превышает размеры паллеты.");
            }
            Boxes.Add(box);
        }

        public DateTime GetExpirationDate ( )
        {
            if (Boxes.Count == 0)
            {
                return DateTime.MaxValue;
            }
            return Boxes.Min(b => b.GetTheExpirationDate());
        }

        public override double Volume ( )
        {
            double boxesVolume = Boxes.Sum(b => b.Volume());
            return boxesVolume + (Width * Height * Depth);
        }

        public  double Weight
        {
            get { return Boxes.Sum(b => b.Weight) + PalletWeight; }
            set { } 
        }

    }
    public class Box : AllClass
    {
        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public Box ( int id, double width, double height, double depth, double weight, DateTime? productionDate, DateTime? expirationDate)
        {
            ID = id;
            Width = width;
            Height = height;
            Depth = depth;
            Weight = weight;
            ProductionDate = productionDate;
            ExpirationDate = expirationDate;
        }


        public DateTime GetTheExpirationDate ( )
        {
            if (ExpirationDate.HasValue)
            {
                return ExpirationDate.Value;
            }
            else if (ProductionDate.HasValue)
            {
                return ProductionDate.Value.AddDays(100);
            }
            else
            {
                
                throw new InvalidOperationException("Не указаны ни дата производства, ни срок годности для коробки.");
            }
        }


        public override double Volume ( )
        {
            return Width * Height * Depth;
        }
    }

}
