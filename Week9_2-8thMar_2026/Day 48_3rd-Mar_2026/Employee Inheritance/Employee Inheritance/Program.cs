using System;

namespace Employee_Inheritance
{

    public abstract class Employee
    {
        protected string department;
        protected string name;
        protected string location;
        protected bool isOnVacation = false;

        public Employee(string department, string name, string location)
        {
            this.department = department;
            this.name = name;
            this.location = location;
        }

        public abstract string GetDepartment();
        public abstract string GetName();
        public abstract string GetLocation();
        public abstract bool GetStatus();
        public abstract void SwitchStatus();
    }

    class FinanceEmployee : Employee
    {
        public FinanceEmployee(string name, string location) : base("Finance", name, location)
        {
        }

        public override string GetDepartment()
        {
            return department;
        }

        public override string GetName()
        {
            return name;
        }

        public override string GetLocation()
        {
            return location;
        }

        public override bool GetStatus()
        {
            return isOnVacation;
        }

        public override void SwitchStatus()
        {
            isOnVacation = !isOnVacation;
        }
    }

    class MarketingEmployee : Employee
    {
        public MarketingEmployee(string name, string location) : base("Marketing", name, location)
        {
        }

        public override string GetDepartment()
        {
            return department;
        }

        public override string GetName()
        {
            return name;
        }

        public override string GetLocation()
        {
            return location;
        }

        public override bool GetStatus()
        {
            return isOnVacation;
        }

        public override void SwitchStatus()
        {
            isOnVacation = !isOnVacation;
        }
    }

    class Solution
    {
        static void Main()
        {
            string str = Console.ReadLine();
            string[] arr = str.Split(' ');

            Employee financeEmployee = new FinanceEmployee(arr[0], arr[1]);

            Console.WriteLine($"FinanceEmployee info: Department - {financeEmployee.GetDepartment()}, Name - {financeEmployee.GetName()}, Location - {financeEmployee.GetLocation()}");

            PrintStatus(financeEmployee);

            Console.WriteLine("Switching");
            financeEmployee.SwitchStatus();
            PrintStatus(financeEmployee);

            Console.WriteLine("Switching");
            financeEmployee.SwitchStatus();
            PrintStatus(financeEmployee);

            str = Console.ReadLine();
            arr = str.Split(' ');

            Employee marketingEmployee = new MarketingEmployee(arr[0], arr[1]);

            Console.WriteLine($"MarketingEmployee info: Department - {marketingEmployee.GetDepartment()}, Name - {marketingEmployee.GetName()}, Location - {marketingEmployee.GetLocation()}");

            PrintStatus(marketingEmployee);

            Console.WriteLine("Switching");
            marketingEmployee.SwitchStatus();
            PrintStatus(marketingEmployee);
        }

        static void PrintStatus(Employee emp)
        {
            string status = emp.GetStatus() ? "on" : "not on";
            Console.WriteLine($"{emp.GetName()} is {status} vacation");
        }
    }
}