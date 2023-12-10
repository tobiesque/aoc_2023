using System.Diagnostics;

namespace Aoc2023Cs;

using Card = char;
using CardValue = int;

public static class Day7
{
    public static bool partOne = true;
    
    public static void Run(int part)
    {
        partOne = (part == 1);
        
        Setup();
        
        List<Hand> hands = new();
        foreach (var lineStr in "7".ReadLinesArray(test: false))
        {
            Span<char> line = lineStr.AsSpan();
            line.ExtractStringRef(out Span<Card> handStr).SkipWhiteRef().ExtractIntRef(out ulong bid);
            Hand hand = new(handStr, bid);
            hands.Add(hand);
        }

        hands.Sort();

        ulong score = 0;
        for (int i = 0; i < hands.Count; i++)
        {
            ulong handScore = hands[i].bid * (ulong)(i + 1);
            Console.WriteLine($"{new String(hands[i].hand)} - {hands[i].GetStrength()} - {i+1} * {hands[i].bid} = {handScore}");
            score += handScore;
        }
        Console.WriteLine($"Part {part}: {score}");
    }

    public enum Strength { Unknown, HighCard, OnePair, TwoPair, Three, FullHouse, Four, Five };

    public static CardValue ToValue(Card card) => cardToValueMap[card];

    private static Card[] GetCards() => cards;
    private static Card[] cards = null!;
    private static Card[] cardsPartOne = { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };
    private static Card[] cardsPartTwo = { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' };
    private static Dictionary<Card, CardValue> cardToValueMap = null!;
    
    public static bool IsJoker(this Card c) => (c == 'J');

    public static void Setup()
    {
        cards = partOne ? cardsPartOne : cardsPartTwo;
        cardToValueMap = cards.ToDictionary(c => c, c => Array.IndexOf(cards.Reverse().ToArray(), c));
    }

    public class Hand(Span<Card> handSpan, ulong bid = 0) : IComparable<Hand>
    {
        public Card[] hand = handSpan.ToArray();
        public ulong bid = bid;
        public Strength strength = Strength.Unknown;

        public static Strength GetStrength(Card[] hand)
        {
            Strength tempStrength = Strength.HighCard;
            foreach (Card card in GetCards())
            {
                // skip jokers in part two
                if (!partOne && card.IsJoker()) continue;
                
                int count = hand.Count(c => (c == card));
                switch (count)
                {
                    case 5: return Strength.Five;
                    case 4: return Strength.Four;
                    case 3 when (tempStrength == Strength.OnePair) : return Strength.FullHouse;
                    case 2 when (tempStrength == Strength.Three)   : return Strength.FullHouse;
                    case 2 when (tempStrength == Strength.OnePair) : return Strength.TwoPair;
                    
                    case 3: tempStrength = Strength.Three; break;
                    case 2: tempStrength = Strength.OnePair; break;
                }
            }
            return tempStrength;
        }

        public Strength GetStrength()
        {
            if (strength == Strength.Unknown)
            {
                strength = partOne ? GetStrength(hand) : StrongestJokerHand();
            }
            return strength;
        }

        public bool IsStrongerSecondary(Hand other)
        {
            for (var i = 0; i < hand.Length; i++)
            {
                CardValue thisCard = ToValue(hand[i]);
                CardValue otherCard = ToValue(other.hand[i]);
                if (thisCard != otherCard) return (thisCard > otherCard);
            }

            Debug.Fail("Two hands are exactly the same");
            return false;
        }

        public Strength StrongestJokerHand()
        {
            Strength strength = GetStrength(hand);
            
            Card[] sortedHand = hand.ToList().SortSelf().ToArray();
            int jokerCount = sortedHand.Count(IsJoker);
            
            // no jokers, it's just the hand
            if (jokerCount == 0) return strength;
            
            // five jokers, go for the best five
            if (jokerCount == 5) return Strength.Five;

            // four jokers, make a five
            if (jokerCount == 4) return Strength.Five;

            if (jokerCount == 3)
            {
                return strength == Strength.OnePair ? 
                    Strength.Five : // three jokers and a pair, make a five
                    Strength.FullHouse; // three jokers and two different cards, make a full house
            }
            
            if (jokerCount == 2)
            {
                return strength switch
                {
                    Strength.Three => Strength.Five, // two jokers and three, make five
                    Strength.OnePair => Strength.Four, // two jokers and a pair, make four
                    _ => Strength.Three
                };

                // two jokers and three different cards, make a three
            }
            
            if (jokerCount == 1)
            {
                int jokerPosition = Array.IndexOf(sortedHand, 'J');
                Card[] currentHand = sortedHand;

                Strength bestStrength = Strength.Unknown;
                foreach (Card c in cards.Where(c => !IsJoker(c)))
                {
                    currentHand[jokerPosition] = c;
                    Strength currentStrength = GetStrength(currentHand);
                    if (currentStrength > bestStrength)
                    {
                        bestStrength = currentStrength;
                    }
                }
                return bestStrength;
            }

            return strength;
        }

        public int CompareTo(Hand? other)
        {
            Strength thisStrength = GetStrength();
            Strength otherStrength = other!.GetStrength();
            int result = ((int)thisStrength).CompareTo((int)otherStrength);
            if (result != 0) return result;
            return IsStrongerSecondary(other!) ? 1 : -1;
        }
    }
}