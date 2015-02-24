using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Week1
{
    /// <summary>
    /// The Shell Trajector Calculator Program
    /// </summary>
    class Program
    {
        //An exit code if things go belly up
        //Using Microsoft System Error Codes as a guide
        private const int ERROR_INVALID_DATA = 0xD;

        /// <summary>
        /// The main method where all the input, calculations, and output are handled
        /// </summary>
        /// <param name="args">Args to pass into the main method</param>
        static void Main(string[] args)
        {
            //Declare the variables used for calculations, initialized inputs to 0
            float theta = 0;
            float speed = 0;
            float vox;
            float voy;
            float t;
            float h;
            float dx;

            //Gravity is a constant 9.8 m/s
            const float GRAVITY = 9.8F;

            //Introduce the program
            Console.WriteLine("Welcome to the Week 1 Shell Trajectory Calculator");
            Console.WriteLine("This calculator will calculate the maximum height and the ground distance of a fired shell.");

            //Utilizing the Retry Pattern for user input
            //User input should never be implicitly trusted
            for (; ; )
            {
                try
                {
                    //Prompt for angle input
                    Console.Write("Enter an initial angle in degrees: ");
                    //Try to get a valid number from the user and store in theta
                    theta = float.Parse(Console.ReadLine());
                    if (theta < 0 || theta > 180)
                    {
                        //Technically, angles should be above the ground, i.e. between 0 and 180 degrees
                        throw new ArgumentOutOfRangeException();
                    }
                    //Convert theta to radians
                    theta = ((float)Math.PI / 180) * theta;
                    //If we are successful, break out of the loop
                    break;
                }
                catch (FormatException e)
                {
                    //If the user entered something thats not a number, let them know and prompt for input again
                    Console.WriteLine("The input was not recognized as a valid number.");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    //If the user entered an invalid angle , let them know and prompt for input again
                    //Technically not an argument exception since it's not calling a method, but close enough for our purposes, and we're not ready for custom exceptions quite yet
                    Console.WriteLine("The angle must be between 0 and 180 degrees");
                }
                catch (Exception e)
                {
                    //A different error occurred on user input. Let the user know and exit the program because we don't know how else to handle it
                    Console.WriteLine("There was an error. Please report this to the developer.");
                    Console.WriteLine(e.ToString());
                    System.Environment.Exit(ERROR_INVALID_DATA);
                }
            }

            //Utilizing the Retry Pattern for user input
            //User input should never be implicitly trusted
            for (; ; )
            {
                try
                {
                    //Prompt for speed input
                    Console.Write("Enter an initial speed in m/s: ");
                    //Try to get a valid number from the user and store in speed
                    speed = float.Parse(Console.ReadLine());
                    if (speed < 0)
                    {
                        //Speed should be positive
                        throw new ArgumentOutOfRangeException();
                    }
                    //If we are successful, break out of the loop
                    break;
                }
                catch (FormatException e)
                {
                    //If the user entered something thats not a number, let them know and prompt for input again
                    Console.WriteLine("The input was not recognized as a valid number.");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    //If the user entered a negative number, let them know and prompt for input again
                    //Technically not an argument exception since it's not calling a method, but close enough for our purposes, and we're not ready for custom exceptions quite yet
                    Console.WriteLine("The speed cannot be a negative number.");
                }
                catch (Exception e)
                {
                    //A different error occurred on user input. Let the user know and exit the program because we don't know how else to handle it
                    Console.WriteLine("There was an error. Please report this to the developer.");
                    Console.WriteLine(e.ToString());
                    System.Environment.Exit(ERROR_INVALID_DATA);
                }
            }

            //Calculate the velocity's horizontal (x) component
            vox = speed * (float)Math.Cos(theta);
            //Calculate the velocity's vertical (y) component
            voy = speed * (float)Math.Sin(theta);

            //The time in seconds the shell is in the air is the vertical velocity component divided by acceleration due to gravity
            t = voy / g;
            //The height in meters of the shell is the square of the vertical velocity component / 2 x acceleration due to gravity
            h = (float)Math.Pow(voy, 2) / (2 * g);
            //The horizontal distance travelled of the shell is 2 x horizontal velocity component x time
            dx = 2 * vox * t;

            //Output our results, formatted to 3 decimal places
            Console.WriteLine("The calculated height of the shell is: " + h.ToString("F3") + " meters.");
            Console.WriteLine("The calculated distance of the shell is: " + dx.ToString("F3") + " meters.");

            //End with a blank line
            Console.WriteLine();
        }
    }
}
