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
using System.Security.Cryptography.X509Certificates;

namespace Bank_Chat_Bot;
public interface IBankAccountOperation
{
    /* 
        Sample operations expected to be proceeded 
        Values may be different 
        -I want to see my balance 
        -I want to see money in my account 
        -I want to withdraw 200 
        -I want to pull 100 
        -I want to deposit 500 
        -I want to invest 600 
        -I want to transfer 100 to my account 
        -I want to deposit 500 dollars 
        -Pull 100 dollars 
        -Deposit 200 
    */
    void Deposit(decimal d);
    void Withdraw(decimal d);
    //returns the current balance after process. 
    decimal ProcessOperation(string message);
}

//Create BankOperations class by implementing IBankAccountOperation
class BankOperations : IBankAccountOperation 
 
{
    decimal _amount = 0;

    public decimal ProcessOperation(string message)
    {
        decimal amt = 0;

        string[] arr1 = message.Split(' ');

        foreach (string s in arr1)
        {
            if (decimal.TryParse(s, out amt))
            {
                break;
            }
            else
            {
                continue;
            }
        }


        if (message.Contains("see") || message.Contains("show"))
        {
            return _amount;
        }
        else if (message.Contains("deposit") || message.Contains("put") || message.Contains("invest") || message.Contains("transfer"))
        {
            Deposit(amt);
        }
        else if (message.Contains("withdraw") || message.Contains("pull"))
        {
            Withdraw(amt);
        }

        return _amount;
    }

        public void Deposit(decimal d)
    {
        _amount += d;
    }
        public void Withdraw(decimal d)
    {
        _amount -= d;
    }

    }



class Solution
{
    public static void Main(string[] args)
    {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string k = Console.ReadLine();
        int n = Convert.ToInt32(k);
        List<string> inputs = new List<string>();
        for (int i = 0; i < n; i++)
        {
            inputs.Add(Console.ReadLine());
        }
        BankOperations opt = new BankOperations();
        foreach (var item in inputs)
        {
            textWriter.WriteLine(opt.ProcessOperation(item));
        }

        textWriter.Flush();
        textWriter.Close();
    }
}