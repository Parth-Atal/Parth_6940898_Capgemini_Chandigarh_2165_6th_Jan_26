using System;

namespace User_Inheritance
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public abstract class User
    {
        private string type;
        private string name;
        private Gender gender;
        private int age;

        public User(string type, string name, Gender gender, int age)
        {
            this.type = type;
            this.name = name;
            this.gender = gender;
            this.age = age;
        }

        public string GetUserName()
        {
            return name;
        }

        public string GetUserType()
        {
            return type;
        }

        public int GetAge()
        {
            return age;
        }

        public Gender GetGender()
        {
            return gender;
        }
    }

    public class Admin : User
    {
        public Admin(string name, Gender gender, int age)
            : base("Admin", name, gender, age)
        {
        }
    }

    public class Moderator : User
    {
        public Moderator(string name, Gender gender, int age)
            : base("Moderator", name, gender, age)
        {
        }
    }

    class Program
    {
        static void PrintUser(User user)
        {
            Console.WriteLine("Type of user " + user.GetUserName() + " is " + user.GetUserType());
            Console.WriteLine("Age of user " + user.GetUserName() + " is " + user.GetAge());
            Console.WriteLine("Gender of user " + user.GetUserName() + " is " + user.GetGender());
        }

        static void Main(string[] args)
        {
            // Test Case 1 (Same as Sample Input)
            Admin admin1 = new Admin("Oscar", Gender.Male, 23);
            Moderator mod1 = new Moderator("Abel", Gender.Female, 36);

            PrintUser(admin1);
            PrintUser(mod1);

            Console.WriteLine();

            // Test Case 2
            Admin admin2 = new Admin("Ace", Gender.Male, 29);
            Moderator mod2 = new Moderator("Ali", Gender.Female, 36);

            PrintUser(admin2);
            PrintUser(mod2);
        }

    }
}