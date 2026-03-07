namespace Computer_Inheritance
{
    abstract class Computer
    {
        string type;
        string model;
        string cpu;
        bool isTurnedOn = false;

        Computer(string type, string model, string cpu)
        {
            this.type = type;
            this.model = model;
            this.cpu = cpu;
        }

        public abstract string getComputerType();
        public abstract string getComputerModel();
        public abstract string getComputerCpu();
        public abstract bool getComputerStatus();
        public abstract void SwitchComputerStatus();
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
