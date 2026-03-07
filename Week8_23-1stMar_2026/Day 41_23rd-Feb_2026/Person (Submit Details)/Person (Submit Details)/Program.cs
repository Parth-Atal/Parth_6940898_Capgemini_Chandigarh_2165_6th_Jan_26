using System.Text.RegularExpressions;

namespace Person__Submit_Details_
{

    class InvalidDateException : Exception
    {
        public InvalidDateException(string message) : base(message) { }
    }

    class InvalidEmailException : Exception
    {
        public InvalidEmailException(string message) : base(message) { }
    }

    class Person
    {
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }

    }

    class Immplementation
    {
        public string Validator(Person details)
        {
            string pattern = @"^(0[1-9]|[12][0-9]|3[01])-(0[1-9]|1[0-2])-(19\d{2})$";

            var res = Regex.Match(details.DateOfBirth, pattern);

            if (!res.Success)
            {
                throw new InvalidDateException("Invalid Date Format");
            }

            string pattern2 = @"^[A-Za-z0-9._%+-]+@doselect\.com$";
            var res2 = Regex.Match(details.Email, pattern2);

            if (!res2.Success)
            {
                throw new InvalidEmailException("Invalid Email Format");
            }

            return "Valid Data";
        }

        public string SubmitDetails(Person details)
        {
            Validator(details);

            return "Valid Data";
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Immplementation impl = new Immplementation();

            // Test Case 1: Valid Data
            try
            {
                Person p1 = new Person
                {
                    Name = "Rahul",
                    DateOfBirth = "12-05-1998",
                    Email = "rahul@doselect.com"
                };

                Console.WriteLine(impl.SubmitDetails(p1));
            }
            catch (InvalidDateException ex)
            {
                Console.WriteLine("Date Exception: " + ex.Message);
            }
            catch (InvalidEmailException ex)
            {
                Console.WriteLine("Email Exception: " + ex.Message);
            }

            Console.WriteLine();

            // Test Case 2: Invalid Date
            try
            {
                Person p2 = new Person
                {
                    Name = "Sneha",
                    DateOfBirth = "12/05/1998",
                    Email = "sneha@doselect.com"
                };

                Console.WriteLine(impl.SubmitDetails(p2));
            }
            catch (InvalidDateException ex)
            {
                Console.WriteLine("Date Exception: " + ex.Message);
            }
            catch (InvalidEmailException ex)
            {
                Console.WriteLine("Email Exception: " + ex.Message);
            }

            Console.WriteLine();

            // Test Case 3: Invalid Email
            try
            {
                Person p3 = new Person
                {
                    Name = "Amit",
                    DateOfBirth = "10-10-1995",
                    Email = "amit@gmail.com"
                };

                Console.WriteLine(impl.SubmitDetails(p3));
            }
            catch (InvalidDateException ex)
            {
                Console.WriteLine("Date Exception: " + ex.Message);
            }
            catch (InvalidEmailException ex)
            {
                Console.WriteLine("Email Exception: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}
