namespace CleanseAndInvertKey
{
    internal class Program
    {
        public static string PassConverter(string input)
        {
            if(input == null || input.Length < 6)
            {
                return "";
            }
            foreach(var item in input)
            {
                if (!Char.IsLetter(item))
                {
                    return "";
                }
            }

            input = input.ToLower();

            string res = "";

            for(int i = 0; i < input.Length; i++)
            {
                if (input[i] % 2 != 0)
                {
                    res += input[i];
                }
            }

            char[] ch = res.ToCharArray();

            Array.Reverse(ch);

            for(int i = 0; i < ch.Length; i++)
            {
                if(i%2 == 0)
                {
                    ch[i] = (char)(ch[i] - 32);
                }
            }

            string result = "";

            foreach(var item in ch)
            {
                result += item;
            }
            

            return result;
             
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the word");
            string input = Console.ReadLine();

            string result = PassConverter(input);

// comment
            if (result == "")
            {
                Console.WriteLine("Invalid Input");
            }
            else
            {
                Console.WriteLine("The generated key is - " + result);
            }
        }
    }
}
