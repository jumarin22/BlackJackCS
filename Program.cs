using System;
using System.Collections.Generic;

namespace BlackJack
{
    // Create Card class. 
    public class Card
    {
        // Properties. 
        public string Rank;
        public string Suit;
        public int Value;

        // Constructor.
        public Card(string rank, string suit, int value)
        {
            Rank = rank;
            Suit = suit;
            Value = value;
        }

        // Methods.
        public string ShowName()
        {
            return $"{Rank}{Suit}";
        }
    }

    // Create Player class.
    public class Player
    {
        // Properties.
        public string Name;
        public string Status;
        public int HandValue;

        // Constructor. 
        public Player(string name, string status, int handValue)
        {
            Name = name;
            Status = status; // Can be Plays or Stays. This property *almost* completely useless.
            HandValue = handValue;
        }
    }

    class Program
    {
        // Create BlackJack method to call from Main.
        static void PlayBlackJack()
        {
            Console.WriteLine("***Let's play Blackjack!***");
            Console.WriteLine("Shuffling deck and dealing Cards...");

            // Create the deck as a List of Cards.
            var deck = new List<Card>();

            // Create an array of suits.
            var suit = new string[] { " of Spades", " of Hearts", " of Clubs", " of Diamonds" };

            // Iterate through the suits, then ranks. Add each Card to the deck.
            for (int suitCount = 0; suitCount < 4; suitCount++) // Iterate through the 4 suits.
            {
                for (int rankCount = 1; rankCount < 12; rankCount++) // Iterate through the ranks.
                {
                    if (rankCount == 1) // Make the first Card an Ace.
                    {
                        deck.Add(new Card("Ace", suit[suitCount].ToString(), 11)); // Card(Name, Rank, Value).
                    }
                    else if (rankCount >= 2 && rankCount <= 10) // Make 2-10 their normal values.
                    {
                        deck.Add(new Card(rankCount.ToString(), suit[suitCount].ToString(), rankCount));
                    }
                    else if (rankCount == 11) // Once we reach the face Cards, just Add them to the deck.
                    {
                        deck.Add(new Card("Jack", suit[suitCount].ToString(), 10));
                        deck.Add(new Card("Queen", suit[suitCount].ToString(), 10));
                        deck.Add(new Card("King", suit[suitCount].ToString(), 10));
                    }
                }
            }

            // Length of deck.
            int numberOfCards = deck.Count;

            // Shuffle deck.
            for (var end = numberOfCards - 1; end >= 0; end--) // Start at the end of the List and decrement.
            {
                var somePlace = new Random().Next(0, end); // Generate a random number less than end's value.

                var copiedCard = deck[end]; // Copy the Card from the end of the List.

                deck[end] = deck[somePlace]; // Change the Card at the end of the List to some other random Card to its left. 

                deck[somePlace] = copiedCard; // That random Card's old spot is now replaced with our copied Card (which was at the end of the List).
            }

            // Create two players.
            var player = new Player("Player", "Plays", 0); // Player(Name, Status, HandValue).
            var house = new Player("House", "Plays", 0);

            // Deal initial Cards.
            // Deals to Player, then House, then Player, then House.
            // Dealt Cards are represented by their value being added to the Player.HandValue.

            player.HandValue = deck[0].Value + deck[2].Value;
            house.HandValue = deck[1].Value + deck[3].Value;

            // Describe the player's Hand.
            Console.WriteLine($"You are dealt the {deck[0].ShowName()} and the {deck[2].ShowName()}");
            Console.WriteLine($"Your hand value is {player.HandValue}.");

            SeeIfBust(player.Name, player.HandValue); // See if the player busted. 

            // The player's turn.
            int hitCount = 4; // Keep track of where we are in the deck to "deal" the Cards by incrementing. 

            while (player.Status != "Stays" && player.HandValue < 22) // Keep asking "hit or stay" until bust or "stay".
            {
                Console.WriteLine("Hit or Stay?");
                string choice = Console.ReadLine();

                if (choice == "hit" || choice == "Hit")
                {
                    Console.WriteLine($"You are dealt {deck[hitCount].ShowName()}");
                    player.HandValue += deck[hitCount].Value;
                    Console.WriteLine($"Your hand value is {player.HandValue}");
                    hitCount++;
                }
                else
                    player.Status = "Stays";
            }

            SeeIfBust(player.Name, player.HandValue);
            Console.WriteLine();

            // House's turn.
            Console.WriteLine("***Now it is the House's Turn.***");
            Console.WriteLine($"The House reveals: {deck[1].ShowName()} and {deck[3].ShowName()}");
            Console.WriteLine($"The House's hand value is {house.HandValue}");

            SeeIfBust(house.Name, house.HandValue); // See if the House busted.

            while (house.HandValue < 17)
            {
                Console.WriteLine("The House hits.");
                Console.WriteLine($"The House is dealt {deck[hitCount].ShowName()}");
                house.HandValue += deck[hitCount].Value;
                Console.WriteLine($"The House's hand value is {house.HandValue}");
                hitCount++;
            }

            SeeIfBust(house.Name, house.HandValue);

            Console.WriteLine("The House stays."); // If the house doesn't bust, it will eventually stay.

            // Display hand values.
            Console.WriteLine();
            Console.WriteLine("***And now to reveal the winner!***");
            Console.WriteLine($"Your hand value is {player.HandValue}");
            Console.WriteLine($"The House's hand value is {house.HandValue}");

            // Determine winner. 
            if (player.HandValue > house.HandValue)
            {
                Console.WriteLine("You win!");
            }
            else
                Console.WriteLine("The House wins.");

            InquireReplay(); // Ask for rematch.
        }

        // Method to see if a Player busts.
        static void SeeIfBust(string moniker, int ifBust)
        {
            string handle = moniker;
            int bust = ifBust;

            if (handle == "Player" && bust > 21) // If the player busts.
            {
                Console.WriteLine("You bust! The House wins.");
                InquireReplay();
            }
            else if (handle == "House" && bust > 21) // If the House busts.
            {
                Console.WriteLine("The House busts. You win!");
                InquireReplay();
            }
        }

        // Method to ask if the player wants a rematch.
        static void InquireReplay()
        {
            Console.WriteLine("Do you want to play again? (y / n)?");
            string answer = Console.ReadLine();
            if (!answer.Contains("y") && !answer.Contains("Y"))
                Console.WriteLine("Goodbye!");
            else
                PlayBlackJack();
            System.Environment.Exit(1);
        }

        // Main method just starts the BlackJack game.
        static void Main(string[] args)
        {
            PlayBlackJack();
        }
    }
}
