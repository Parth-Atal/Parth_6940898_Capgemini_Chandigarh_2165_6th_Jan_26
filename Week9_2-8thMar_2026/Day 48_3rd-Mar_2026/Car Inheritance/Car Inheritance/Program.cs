
using System;

namespace Car_Inheritance
{
    class Car
    {
        protected bool isSedan;
        protected string seats;

        public Car(bool isSedan, string seats)
        {
            this.isSedan = isSedan;
            this.seats = seats;
        }

        public bool GetIsSedan()
        {
            return isSedan;
        }

        public string GetSeats()
        {
            return seats;
        }
    }

    class WagonR : Car
    {
        private int mileage;

        public WagonR(int mileage) : base(false, "4")
        {
            this.mileage = mileage;
        }

        public string GetMileage()
        {
            return mileage + " kmpl";
        }
    }

    class HondaCity : Car
    {
        private int mileage;

        public HondaCity(int mileage) : base(true, "4")
        {
            this.mileage = mileage;
        }

        public string GetMileage()
        {
            return mileage + " kmpl";
        }
    }

    class InnovaCrysta : Car
    {
        private int mileage;

        public InnovaCrysta(int mileage) : base(false, "6")
        {
            this.mileage = mileage;
        }

        public string GetMileage()
        {
            return mileage + " kmpl";
        }
    }

    class Solution
    {
        static void Main(string[] args)
        {
            int carType = Convert.ToInt32(Console.ReadLine());
            int mileage = Convert.ToInt32(Console.ReadLine());

            if (carType == 0)
            {
                WagonR car = new WagonR(mileage);

                Console.WriteLine("WagonR is not Sedan, is " + car.GetSeats() +
                                  "-seater, and has a mileage of around " +
                                  car.GetMileage() + ".");
            }
            else if (carType == 1)
            {
                HondaCity car = new HondaCity(mileage);

                Console.WriteLine("HondaCity is Sedan, is " + car.GetSeats() +
                                  "-seater, and has a mileage of around " +
                                  car.GetMileage() + ".");
            }
            else if (carType == 2)
            {
                InnovaCrysta car = new InnovaCrysta(mileage);

                Console.WriteLine("InnovaCrysta is not Sedan, is " + car.GetSeats() +
                                  "-seater, and has a mileage of around " +
                                  car.GetMileage() + ".");
            }
        }
    }
}
