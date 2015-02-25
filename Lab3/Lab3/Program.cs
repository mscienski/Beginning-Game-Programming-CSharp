using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            float originalF = 0F;
            float calculatedC;
            float calculatedF;

            for (; ; )
            {
                try
                {
                    Console.Write("Enter the temperature in Farenheight: ");
                    originalF = float.Parse(Console.ReadLine());
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("The input was not recognized as a valid number.");
                }
            }

            calculatedC = ((originalF - 32) / 9) * 5;
            Console.WriteLine(originalF.ToString("f") + " degrees Farenheight is " + calculatedC.ToString("f") + " degrees Celcius");

            calculatedF = ((calculatedC * 9) / 5) + 32;
            Console.WriteLine(calculatedC.ToString("f") + " degrees Celcius is " + calculatedF.ToString("f") + " degrees Farenheight");
            Console.WriteLine();
        }
    }
}
