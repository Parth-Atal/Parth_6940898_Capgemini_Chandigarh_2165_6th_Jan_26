using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace Authentication_State
{
    interface IUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int IncorrectAttempt { get; set; }
        public string Location { get; set; }
        public int Count { get; set; }
    }

    interface IApplicationAuthState
    {
        public List<IUser> RegisteredUsers { get; set; }
        public List<IUser> UsersLoggedIn { get; set; }
        public List<string> AllowedLocations { get; set; }
        string Register(IUser user);
        string Login(IUser user);
        string Logout(IUser user);
    }

    /* 
     * These strings can be copied and pasted to avoid typing errors. 
     * User1@email.com should be replaced with the correct user email. 
     * 
     * User1@email.com registered successfully! 
     * User1@email.com is not registered! 
     * User1@email.com is not logged in! 
     * User1@email.com is already registered! 
     * User1@email.com logged in successfully! 
     * User1@email.com logged out successfully! 
     * User1@email.com is already logged in! 
     * User1@email.com is already logged in from another location! 
     * User1@email.com is blocked! 
     * User1@email.com is not allowed to login from this location! 
     * User1@email.com password is incorrect! 
     */


    class User : IUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int IncorrectAttempt { get; set; }
        public string Location { get; set; }
        public int Count { get; set; }
    }

    class ApplicationAuthState
    {
        public List<IUser> RegisteredUsers { get; set; }
        public List<IUser> UsersLoggedIn { get; set; }
        public List<string> AllowedLocations { get; set; }

        public string Register(IUser user)
        {
            if (RegisteredUsers.Contains(user))
            {
                return $"{user.Email} is already registered!";
            }
            RegisteredUsers.Add(user);
            return $"{user.Email} registered successfully!";
        }

        public string Login(IUser user)
        {
            if (!RegisteredUsers.Contains(user))
            {
                return $"{user.Email} is not registered!";
            }
            if (UsersLoggedIn.Contains(user))
            {
                return $"{user.Email} is already logged in!";
            }
            if (!AllowedLocations.Contains(user.Location))
            {
                return $"{user.Email} is not allowed to login from this location!";
            }
            UsersLoggedIn.Add(user);
            return $"{user.Email} logged in successfully!";
        }

        public string Logout(IUser user)
        {
            if (!RegisteredUsers.Contains(user))
            {
                return $"{user.Email} is not registered!";
            }
            if (!UsersLoggedIn.Contains(user))
            {
                return $"{user.Email} is not logged in!";
            }
            if (UsersLoggedIn.Count == 1)
            {
                UsersLoggedIn.RemoveAt(0);
                return $"{user.Email} logged out successfully!";
            }
            else
            {
                UsersLoggedIn.RemoveAt(0);
                return $"{user.Email} logged out successfully!";
            }
        }
    }

    class Solution
    {
        public static void Main(string[] args)
        {
            TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);
            List<IUser> users = new List<IUser>();
            List<string> allowedLocations = new List<string>();
            int allowedLocationCount = Convert.ToInt32(Console.ReadLine().Trim());
            for (int i = 0; i < allowedLocationCount; i++)
            {
                var a = Console.ReadLine().Trim();
                allowedLocations.Add(a);
            }
            ApplicationAuthState authState = new ApplicationAuthState(allowedLocations);

            int usersCount = Convert.ToInt32(Console.ReadLine().Trim())
    ;
            for (int i = 0; i < usersCount; i++)
            {
                var a = Console.ReadLine().Trim().Split(',');
                users.Add(new User(Convert.ToInt32(a[0]), a[1], a[2], a[3]));
            }

            int actionCount = Convert.ToInt32(Console.ReadLine().Trim());
            for (int i = 0; i < actionCount; i++)
            {
                var a = Console.ReadLine().Trim().Split(':');
                var uIndex = Convert.ToInt32(a[1]);
                if (a[0] == "Register")
                {
                    textWriter.WriteLine(authState.Register(users[uIndex]));
                }
                else if (a[0] == "Login")
                {
                    textWriter.WriteLine(authState.Login(users[uIndex]));
                }
                else if (a[0] == "Logout")
                {
                    textWriter.WriteLine(authState.Logout(users[uIndex]));
                }
            }
            textWriter.Flush();
            textWriter.Close();
        }
    }
}