using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            //A comment so git will commit
            int age;
            const int MAX_SCORE = 100;
            int score = 97;
            float percent = (float)score/(float)MAX_SCORE * 100;
            age = 29;
            Console.WriteLine("My age is: " + age.ToString("N0"));
            Console.WriteLine("The score percent is: " + percent.ToString("F2"));
        }
    }
}