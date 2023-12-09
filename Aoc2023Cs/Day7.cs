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
            Console.WriteLine($"{hands[i].hand} - {hands[i].GetStrength()} - {i+1} * {hands[i].bid} = {handScore}");
            score += handScore;
        }
        Console.WriteLine($"Part One: {score}");
    }

    public enum Strength { HighCard, OnePair, TwoPair, Three, FullHouse, Four, Five };

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

    public class Hand(Span<Card> hand, ulong bid) : IComparable<Hand>
    {
        public string hand = hand.ToString();
        public ulong bid = bid;

        public Strength GetStrength()
        {
            Strength tempStrength = Strength.HighCard;
            foreach (Card card in GetCards())
            {
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

        public int DecodePermutation(int value, int digit)
        {
            return value / 12.Pow(digit);
        } 

        public string StrongestJokerHand()
        {
            int jokerCount = hand.Count(IsJoker);
            if (jokerCount == 0) return hand;

            for (var i = 0; i < hand.Length; i++)
            {
                if (!IsJoker(hand[i])) continue;

                
                foreach (Card card in cards)
                {
                    if (IsJoker(card)) continue;

                    
                }
            }

            if (jokerCount == 5) return "AAAAA";

            if (jokerCount == 4)
            {
                
            }

            return "";
        }

        public int CompareTo(Hand? other)
        {
            if (partOne)
            {
                Strength thisStrength = GetStrength();
                Strength otherStrength = other!.GetStrength();
                int result = ((int)thisStrength).CompareTo((int)otherStrength);
                if (result != 0) return result;
                return IsStrongerSecondary(other!) ? 1 : -1;
            }
            
            
            // Part Two
            return 0;
        }
    }
}