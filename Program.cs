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
        public int SplitValue; // Adventure mode: Allow the player to "Split". (Player can only split once).
        public int SplitCount;
        public bool HasSplitJack; // If split hand makes BlackJack. 

        // Constructor. 
        public Player(string name, int handValue, int handcount, bool hasBlackJack, int splitValue, int splitCount, bool hasSplitJack)
        {
            Name = name;
            HandValue = handValue;
            HandCount = handcount;
            HasBlackjack = hasBlackJack;
            SplitValue = splitValue;
            SplitCount = splitCount;
            HasSplitJack = hasSplitJack;
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
            var player = new Player("Player", 0, -1, false, 0, 0, false); // Player(name, handValue, handcount, hasBlackJack, SplitValue, splitCount, hasSplitJack)
            var house = new Player("House", 0, -1, false, 0, 0, false);

            // Create player hands.
            var playerHand = new List<Card>();
            var houseHand = new List<Card>();
            var splitHand = new List<Card>();

            // Deal Cards.
            DealCard(deck, player, playerHand);
            DealCard(deck, house, houseHand);
            DealCard(deck, player, playerHand);
            DealCard(deck, house, houseHand);

            Console.WriteLine($"You are dealt {playerHand[0].ShowName()} and {playerHand[player.HandCount].ShowName()}.");

            // Calculate hand values. 
            player.HandValue = CalcVal(playerHand);

            // Do you have BlackJack? (Ace + 10 value card)
            player.HasBlackjack = SeeIfBlackJack(playerHand);
            if (player.HasBlackjack == true)
                Console.WriteLine("BlackJack!");

            // Adventure mode: Reveal one of house's cards to the player when cards dealt.
            Console.WriteLine($"House shows {houseHand[0].ShowName()}.");

            // Display hand value. 
            if (player.HasBlackjack == false)
                Console.WriteLine($"Your hand value is {player.HandValue}.");

            // See if cards can be split: if both cards have same value and start with the same character. 
            if (playerHand[0].Value == playerHand[1].Value && playerHand[0].ShowName()[..1] == playerHand[1].ShowName()[..1])
            {
                Console.WriteLine("Do you want to split? (y / n)");
                if (Console.ReadLine().Contains("y"))
                {
                    // Take the last card dealt from the first hand and put it in the split hand.
                    var splitCard = playerHand[1];
                    playerHand.Remove(splitCard);
                    splitHand.Add(splitCard);
                    // Deal a new card to the first hand and the split hand.
                    DealCard(deck, player, playerHand);
                    DealCard(deck, player, splitHand);
                    // Reset count/index values. 
                    player.HandCount = 1;
                    player.SplitCount = 1;
                    // Recalculate hands. 
                    player.HandValue = CalcVal(playerHand);
                    player.SplitValue = CalcVal(splitHand);
                    // Provide a wall of text. 
                    Console.WriteLine($"You split. Your first hand is dealt {playerHand[1].ShowName()}.");
                    Console.WriteLine($"Your split hand is dealt {splitHand[1].ShowName()}.");
                    Console.WriteLine();
                    Console.WriteLine("***First hand turn***");
                    Console.WriteLine($"Your first hand is {playerHand[0].ShowName()} and {playerHand[1].ShowName()}.");
                    Console.WriteLine($"Your first hand value is { player.HandValue }.");
                }
            }

            // Check for BlackJack again in case of split. 
            player.HasBlackjack = SeeIfBlackJack(playerHand);
            if (player.HasBlackjack == true && player.SplitValue > 0)
                Console.WriteLine("BlackJack!");

            // Take your turn. First hand. 
            if (player.HasBlackjack == false) // End turn immediately if you already have BlackJack. 
            {
                while (player.HandValue < 21)
                {
                    Console.WriteLine("Hit or stay? (h / s)");
                    if (Console.ReadLine().Contains("h") == false)
                        break;
                    else
                    {
                        DealCard(deck, player, playerHand);
                        player.HandValue = CalcVal(playerHand);
                        Console.WriteLine($"You are dealt {playerHand[player.HandCount].ShowName()}.");
                    }

                    if (player.SplitValue == 0)
                        Console.WriteLine($"Your hand value is {player.HandValue}.");
                    else
                        Console.WriteLine($"Your first hand value is {player.HandValue}.");
                }

                if (player.HandValue > 21 && player.SplitValue == 0)
                {
                    Console.WriteLine("You bust! Game Over.");
                    return;
                }
                else if (player.HandValue > 21 && player.SplitValue > 0)
                    Console.WriteLine("Your first hand busts.");
            }

            // Split hand turn.
            if (player.SplitValue > 1)
            {
                Console.WriteLine("Split hand turn.");
                Console.WriteLine($"Your split hand is {splitHand[0].ShowName()} and {splitHand[1].ShowName()}.");
                Console.WriteLine($"Your split hand value is {player.SplitValue}.");

                // Do you have BlackJack? If so end turn immediately. 
                player.HasBlackjack = SeeIfBlackJack(splitHand);
                if (player.HasBlackjack == true)
                    Console.WriteLine("BlackJack!");

                while (player.SplitValue < 21)
                {
                    Console.WriteLine("Hit or stay? (h / s)");
                    if (Console.ReadLine().Contains("h") == false)
                        break;
                    else
                    {
                        DealCard(deck, player, splitHand);
                        player.SplitCount++; // DealCard() does not increase player.SplitCount 
                        player.SplitValue = CalcVal(splitHand);
                        Console.WriteLine($"You are dealt {splitHand[player.SplitCount].ShowName()}.");
                    }

                    Console.WriteLine($"Your split hand value is {player.SplitValue}.");
                }

                if (player.HandValue > 21 && player.SplitValue > 21) // If first and split hands busted, you lose. 
                {
                    Console.WriteLine("You bust! Game Over.");
                    return;
                }
                else if (player.SplitValue > 21)
                    Console.WriteLine("Your split hand busts.");
            }

            // Computer's turn.
            Console.WriteLine();
            Console.WriteLine("***House's Turn***");
            Console.WriteLine($"House reveals {houseHand[0].ShowName()} and {houseHand[house.HandCount].ShowName()}.");
            house.HandValue = CalcVal(houseHand);
            house.HasBlackjack = SeeIfBlackJack(houseHand);
            if (house.HasBlackjack == true)
                Console.WriteLine("BlackJack!");
            if ((player.HasBlackjack == true && player.SplitValue == 0) && house.HasBlackjack == false)
            {
                Console.WriteLine("You have BlackJack and House doesn't. You win!");
                return;
            }

            if (house.HasBlackjack == false)
            {
                Console.WriteLine($"House hand value is {house.HandValue}.");

                while (house.HandValue < 17)
                {
                    Console.WriteLine("House hits.");
                    DealCard(deck, house, houseHand);
                    house.HandValue = CalcVal(houseHand);
                    Console.WriteLine($"House is dealt {houseHand[house.HandCount].ShowName()}.");
                    Console.WriteLine($"House hand value is {house.HandValue}.");
                }

                if (house.HandValue > 21)
                {
                    Console.WriteLine("House busts. You win! Game Over.");
                    return;
                }

                Console.WriteLine("House stays."); // If the house doesn't bust it will eventually stay. 
            }

            // Display hand values.
            Console.WriteLine();
            Console.WriteLine("***And now to reveal the winner!***");
            if (player.SplitValue > 1)
                Console.WriteLine($"Your first hand value is {player.HandValue}.");
            else
                Console.WriteLine($"Your hand value is {player.HandValue}.");
            if (player.SplitValue > 1)
                Console.WriteLine($"Your split hand value is {player.SplitValue}.");
            Console.WriteLine($"House's hand value is {house.HandValue}.");

            // Determine winner. 
            // Adventure Mode: Improve the win requirements. 
            if (player.SplitValue == 0)
            {
                if (player.HasBlackjack == true && house.HasBlackjack == false)
                    Console.WriteLine("You have BlackJack and House doesn't. You win!");
                else if (house.HasBlackjack == true && player.HasBlackjack == false)
                    Console.WriteLine("House has BlackJack and you don't. House wins.");
                else if (player.HandValue > house.HandValue)
                    Console.WriteLine("You win!");
                else if (player.HandValue < house.HandValue)
                    Console.WriteLine("House wins.");
                else
                    Console.WriteLine("Hand values are the same. It's a push.");
                return;
            }

            // First hand results.
            if (player.HasBlackjack == true && house.HasBlackjack == false)
                Console.WriteLine("Your first hand has BlackJack and House doesn't. You win!");
            else if (house.HasBlackjack == true && player.HasBlackjack == false)
                Console.WriteLine("House has BlackJack and your first hand doesn't. House wins.");
            else if (player.HandValue > 21)
                Console.WriteLine("Your first hand busted. House wins.");
            else if (player.HandValue > house.HandValue)
                Console.WriteLine("Your first hand wins!");
            else if (player.HandValue < house.HandValue)
                Console.WriteLine("House wins against your first hand.");
            else
                Console.WriteLine("Your first hand and house hand values are the same. It's a push.");

            // Split hand results.
            if (player.HasSplitJack == true && house.HasBlackjack == false)
                Console.WriteLine("Your split hand has BlackJack and House doesn't. You win!");
            else if (house.HasBlackjack == true && player.HasSplitJack == false)
                Console.WriteLine("House has BlackJack and your split hand doesn't. House wins.");
            else if (player.SplitValue > 21)
                Console.WriteLine("Your split hand busted. House wins.");
            else if (player.SplitValue > house.HandValue)
                Console.WriteLine("Your split hand wins!");
            else if (player.SplitValue < house.HandValue)
                Console.WriteLine("House wins against your split hand.");
            else
                Console.WriteLine("Your split hand and house hand values are the same. It's a push.");

            return;
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
        static int CalcVal(List<Card> hand)
        {
            var value = 0;
            var aceCount = 0; // Count aces in hand.

            foreach (var card in hand)
            {
                value += card.Value;

                if (card.ShowName().Contains("Ace"))
                    aceCount++;
            }

            while (aceCount > 0) // Adventure mode: Consider Aces to be 1 or 11. 
            {
                if (value > 21)
                {
                    value -= 10;
                    aceCount--;
                }
                else
                    break;
            }

            return value;
        }
        // See if Blackjack method.
        static bool SeeIfBlackJack(List<Card> hand)
        {
            if ((hand[0].ShowName().Contains("Ace") || hand[1].ShowName().Contains("Ace")) && (hand[0].Value + hand[1].Value == 21))
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

