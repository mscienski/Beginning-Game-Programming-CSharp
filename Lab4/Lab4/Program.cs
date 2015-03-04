using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab4
{
	/// <summary>
	/// Demonstrates classes and objects
	/// </summary>
    class Program
    {
		/// <summary>
		/// Demonstrates use of Deck and Card objects
		/// </summary>
		/// <param name="args">command-line args</param>
        static void Main(string[] args)
        {
            // create a new deck and print the contents of the deck
            Deck deck = new Deck();

            deck.Print();

            Console.WriteLine();

            // shuffle the deck and print the contents of the deck
            deck.Shuffle();

            deck.Print();

            Console.WriteLine();

            // take the top card from the deck and print the card rank and suit
            Card card = deck.TakeTopCard();

            Console.WriteLine(card.Rank);
            Console.WriteLine(card.Suit);
            Console.WriteLine();

            // take the top card from the deck and print the card rank and suit
            Card card2 = deck.TakeTopCard();

            Console.WriteLine(card2.Rank);
            Console.WriteLine(card2.Suit);
            Console.WriteLine();


        }
    }
}
