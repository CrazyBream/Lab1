using IO;
using System;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string filePath = "data.txt";

            using (var fileHandler = new FileHandler(filePath))
            {
                var consoleMenu = new ConsoleMenu(fileHandler);
                consoleMenu.ShowMenu();
            }
        }
    }
}