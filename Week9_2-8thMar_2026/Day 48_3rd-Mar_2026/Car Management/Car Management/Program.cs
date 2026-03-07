using System;
using System.Collections.Generic;
using System.Linq;

namespace Car_Management
{
    public class Car
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Price { get; set; }

        public Car(string brand, string model, int price)
        {
            Brand = brand;
            Model = model;
            Price = price;
        }
    }

    public class CarManager
    {
        private List<Car> cars;

        public CarManager(List<Car> cars)
        {
            this.cars = cars;
        }

        // 1. Most Expensive Car
        public Car MostExpensiveCar()
        {
            return cars.OrderByDescending(c => c.Price).First();
        }

        // 2. Cheapest Car
        public Car CheapestCar()
        {
            return cars.OrderBy(c => c.Price).First();
        }

        // 3. Average Price
        public double AveragePriceOfCars()
        {
            return cars.Average(c => c.Price);
        }

        // 4. Most expensive car for each brand
        public Dictionary<string, Car> MostExpensiveModelForEachBrand()
        {
            return cars
                .GroupBy(c => c.Brand)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(c => c.Price).First()
                );
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Test Data
            List<Car> cars = new List<Car>()
            {
                new Car("Toyota", "Corolla", 20000),
                new Car("Toyota", "Camry", 30000),
                new Car("Honda", "Civic", 22000),
                new Car("Honda", "Accord", 28000),
                new Car("BMW", "X1", 40000),
                new Car("BMW", "X5", 60000)
            };

            CarManager manager = new CarManager(cars);

            // 1 Most Expensive Car
            var mostExpensive = manager.MostExpensiveCar();
            Console.WriteLine("Most Expensive Car:");
            Console.WriteLine($"{mostExpensive.Brand} {mostExpensive.Model} {mostExpensive.Price}");

            // 2 Cheapest Car
            var cheapest = manager.CheapestCar();
            Console.WriteLine("\nCheapest Car:");
            Console.WriteLine($"{cheapest.Brand} {cheapest.Model} {cheapest.Price}");

            // 3 Average Price
            var avg = manager.AveragePriceOfCars();
            Console.WriteLine("\nAverage Price:");
            Console.WriteLine(avg);

            // 4 Most expensive model per brand
            Console.WriteLine("\nMost Expensive Model For Each Brand:");

            var result = manager.MostExpensiveModelForEachBrand();

            foreach (var item in result)
            {
                Console.WriteLine($"{item.Key} -> {item.Value.Model} {item.Value.Price}");
            }
        }
    }
}

