namespace Salary_Calculator
{
    class InvalidDayException : Exception
    {
        public InvalidDayException (string message) : base (message) { }
    }

    class InvalidSalaryException : Exception
    {
        public InvalidSalaryException (string message) : base (message) { }
    }

    class SalaryData
    {
        public string Name { get; set; }
        public int DaysInMonth { get; set; }
        public double Salary {  get; set; }

    }

    class Validator
    {
        public string ValidateSalaryData(SalaryData salaryData)
        {
            int days = salaryData.DaysInMonth;

            if(days != 28 && days != 30 && days != 31)
            {
                throw new InvalidDayException("Error: Invalid No of Days");
            }

            if(salaryData.Salary <= 0 || salaryData.Salary >= 1000000)
            {
                throw new InvalidSalaryException("Error: Invalid Salary");
            }

            return "Valid Data";
        }

        public double GetTotalSalary(SalaryData s)
        {
            ValidateSalaryData (s);

            double salary = s.DaysInMonth * s.Salary;

            return salary;
        }
    }



    internal class Program
    {
        static void Main(string[] args)
        {
            Validator validator = new Validator();

            // Test Case 1: Valid Data
            try
            {
                SalaryData s1 = new SalaryData
                {
                    Name = "Rahul",
                    DaysInMonth = 30,
                    Salary = 2000
                };

                Console.WriteLine(validator.ValidateSalaryData(s1));
                Console.WriteLine("Total Salary: " + validator.GetTotalSalary(s1));
            }
            catch (InvalidDayException ex)
            {
                Console.WriteLine("Day Exception: " + ex.Message);
            }
            catch (InvalidSalaryException ex)
            {
                Console.WriteLine("Salary Exception: " + ex.Message);
            }

            Console.WriteLine();

            // Test Case 2: Invalid Days
            try
            {
                SalaryData s2 = new SalaryData
                {
                    Name = "Sneha",
                    DaysInMonth = 29,
                    Salary = 2000
                };

                Console.WriteLine(validator.ValidateSalaryData(s2));
                Console.WriteLine("Total Salary: " + validator.GetTotalSalary(s2));
            }
            catch (InvalidDayException ex)
            {
                Console.WriteLine("Day Exception: " + ex.Message);
            }
            catch (InvalidSalaryException ex)
            {
                Console.WriteLine("Salary Exception: " + ex.Message);
            }

            Console.WriteLine();

            // Test Case 3: Salary Out of Range
            try
            {
                SalaryData s3 = new SalaryData
                {
                    Name = "Amit",
                    DaysInMonth = 31,
                    Salary = 2000000   // Above allowed range
                };

                Console.WriteLine(validator.ValidateSalaryData(s3));
                Console.WriteLine("Total Salary: " + validator.GetTotalSalary(s3));
            }
            catch (InvalidDayException ex)
            {
                Console.WriteLine("Day Exception: " + ex.Message);
            }
            catch (InvalidSalaryException ex)
            {
                Console.WriteLine("Salary Exception: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}
