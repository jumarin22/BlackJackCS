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
        public int HandValue;
        public int HandCount; // number of cards in a player's hand, kind of. Actually, marks player's List<Card> index value. 
        public bool HasBlackjack; // True if player has BlackJack on initial deal. 

        // Constructor. 
        public Player(string name, int handValue, int handcount, bool hasBlackJack)
        {
            Name = name;
            HandValue = handValue;
            HandCount = handcount;
            HasBlackjack = hasBlackJack;
        }
    }

    class Program
    {
        // Create BlackJack method to call from Main.
        static void PlayBlackJack()
        {
            Console.WriteLine("***Let's play Blackjack!***");

            Console.WriteLine("Creating Deck...");
            var deck = CreateDeck(); // deck is a List<Card>.

            Console.WriteLine("Shuffling Cards...");
            ShuffleCards(deck); // Fisher–Yates.

            // Create two players.
            var player = new Player("Player", 0, -1, false); // Player(Name, HandValue, HandCount, Status).
            var house = new Player("House", 0, -1, false);

            // Create player hands.
            var playerHand = new List<Card>();
            var houseHand = new List<Card>();

            // Deal Cards.
            DealCard(deck, player, playerHand);
            DealCard(deck, house, houseHand);
            DealCard(deck, player, playerHand);
            DealCard(deck, house, houseHand);

            Console.WriteLine($"You are dealt {playerHand[0].ShowName()} and {playerHand[player.HandCount].ShowName()}.");

            // Adventure mode: Reveal one of house's cards to the player when cards dealt.
            Console.WriteLine($"House shows {houseHand[0].ShowName()}");

            // Calculate hand values. 
            player.HandValue = CalcVal(player, playerHand);

            // Do you have BlackJack?
            SeeIfBlackJack(player, playerHand);
            if (player.HasBlackjack == true)
                Console.WriteLine("BlackJack!");

            // Display hand value. 
            if (player.HasBlackjack == false)
                Console.WriteLine($"Your hand value is {player.HandValue}.");

            // Did you bust?
            if (SeeIfBust(player.HandValue) == true)
            {
                Console.WriteLine("You bust! Game Over.");
                return;
            }
            // else
            // Take your turn.
            if (player.HasBlackjack == false)
            {
                Console.WriteLine("Hit or stay? (h / s)");
                var hitOrStay = Console.ReadLine();
                while (hitOrStay.Contains("h") && SeeIfBust(player.HandValue) == false)
                {
                    DealCard(deck, player, playerHand);
                    CalcVal(player, playerHand);
                    Console.WriteLine($"You are dealt {playerHand[player.HandCount].ShowName()}.");
                    Console.WriteLine($"Your hand value is {player.HandValue}.");
                    if (player.HandValue < 21)
                    {
                        Console.WriteLine("Hit or stay? (h / s)");
                        hitOrStay = Console.ReadLine();
                    }
                }

                if (SeeIfBust(player.HandValue) == true)
                {
                    Console.WriteLine("You bust! Game Over.");
                    return;
                }
            }

            // Computer's turn.
            Console.WriteLine();
            Console.WriteLine("***House's Turn***");
            Console.WriteLine($"House reveals {houseHand[0].ShowName()} and {houseHand[house.HandCount].ShowName()}");
            CalcVal(house, houseHand);
            SeeIfBlackJack(house, houseHand);
            if (house.HasBlackjack == true)
                Console.WriteLine("BlackJack!");
            if (house.HasBlackjack == false)
            {
                Console.WriteLine($"House hand value is {house.HandValue}.");
                if (SeeIfBust(house.HandValue) == true)
                {
                    Console.WriteLine("House busts. Your win! Game Over.");
                    return;
                }

                while (house.HandValue < 17 && SeeIfBust(house.HandValue) == false)
                {
                    Console.WriteLine("House hits.");
                    DealCard(deck, house, houseHand);
                    CalcVal(house, houseHand);
                    Console.WriteLine($"House is dealt {houseHand[house.HandCount].ShowName()}.");
                    Console.WriteLine($"House hand value is {house.HandValue}.");
                }

                if (SeeIfBust(house.HandValue) == true)
                {
                    Console.WriteLine("House busts. You win! Game Over.");
                    return;
                }

                Console.WriteLine("House stays.");
            }

            // Display hand values.
            Console.WriteLine();
            Console.WriteLine("***And now to reveal the winner!***");
            Console.WriteLine($"Your hand value is {player.HandValue}.");
            Console.WriteLine($"House's hand value is {house.HandValue}.");

            // Determine winner. 
            // Adventure Mode: Improve the win requirements. 
            if (player.HasBlackjack == true && house.HasBlackjack == false)
            {
                Console.WriteLine("You have BlackJack and House doesn't. You win!");
                return;
            }
            else if (house.HasBlackjack == true && player.HasBlackjack == false)
            {
                Console.WriteLine("House has BlackJack and you don't. House wins.");
                return;
            }
            else if (player.HandValue > house.HandValue)
            {
                Console.WriteLine("You win!");
                return;
            }
            else if (player.HandValue < house.HandValue)
            {
                Console.WriteLine("House wins.");

                return;
            }
            else
            {
                Console.WriteLine("Hand values are the same. It's a push.");
            }
        }

        // Create deck method.
        static List<Card> CreateDeck()
        {
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
                        deck.Add(new Card("Ace", suit[suitCount].ToString(), 11)); // Card(Rank, Suit, Value).
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

            return deck;
        }

        // Shuffle cards method.
        static void ShuffleCards(List<Card> deck)
        {
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

        }

        // Deal cards method.
        static void DealCard(List<Card> deck, Player player, List<Card> hand)
        {
            var topCard = deck[0];
            deck.Remove(topCard);
            hand.Add(topCard);
            player.HandCount++;
        }

        // Calculate hand value method.
        static int CalcVal(Player player, List<Card> playerHand)
        {
            player.HandValue = 0;
            var aceCount = 0; // Count aces in hand.

            for (int cardCount = 0; cardCount < playerHand.Count; cardCount++)
            {
                player.HandValue += playerHand[cardCount].Value;

                if (playerHand[cardCount].ShowName().Contains("Ace"))
                    aceCount++;
            }

            while (aceCount > 0) // Adventure mode: Consider Aces to be 1 or 11. 
            {
                if (player.HandValue > 21)
                {
                    player.HandValue -= 10;
                    aceCount--;
                }
                else
                    break;
            }

            return player.HandValue;
        }
        // See if Blackjack method.
        static void SeeIfBlackJack(Player player, List<Card> playerHand)
        {
            foreach (var card in playerHand)
            {
                if (card.ShowName().Contains("Ace") && player.HandValue == 21)
                    player.HasBlackjack = true;
            }
        }

        // See if a player busts.
        static bool SeeIfBust(int handValue)
        {
            if (handValue > 21)
                return true;
            else
                return false;
        }

        // Main loops the BlackJack game until player quits.
        static void Main(string[] args)
        {
            var runGame = true;
            while (runGame)
            {
                PlayBlackJack();

                Console.WriteLine("Do you want to play again? (y / n)");
                string answer = Console.ReadLine();

                if (answer == "n")
                    runGame = false;
            }

            Console.WriteLine("Goodbye!");
        }
    }
}

