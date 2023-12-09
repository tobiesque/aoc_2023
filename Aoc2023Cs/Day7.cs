namespace Aoc2023Cs;

using Card = char;
using CardValue = int;

public static class CardMap
{
    public static int ToValue(this Card card) => cardToValueMap[card];
    public static int ToCard(this int value) => valueToCardMap[value];
    
    public static Card[] cards = { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };
    public static Card[] cardsReversed = cards.Reverse().ToArray();
    public static Dictionary<Card, CardValue> cardToValueMap = cards.ToDictionary(c => c, c => Array.IndexOf(cardsReversed, c) );
    public static Dictionary<CardValue, Card> valueToCardMap = cardToValueMap.Inverse();
}

public class Hand(Span<Card> hand, ulong bid) : IComparer<Hand>
{
    public enum Rank { Five, Four, FullHouse, Three, TwoPair, OnePair, HighCard };
    
    public string hand = hand.ToString();
    public CardValue[] values = hand.AsEnumerable().Select(c => c.ToValue()).ToArray();
    public ulong bid = bid;

    public Rank GetRank()
    {
        Rank tempRank = Rank.HighCard;
        foreach (Card card in CardMap.cards)
        {
            int count = hand.Count(c => (c == card));
            switch (count)
            {
                case 5: return Rank.Five;
                case 4: return Rank.Four;
                case 3 when (tempRank == Rank.OnePair) : return Rank.FullHouse;
                case 2 when (tempRank == Rank.Three)   : return Rank.FullHouse;
                case 2 when (tempRank == Rank.OnePair) : return Rank.TwoPair;
                
                case 3: tempRank = Rank.Three; break;
                case 2: tempRank = Rank.OnePair; break;
            }
        }
        return tempRank;
    }  
    
    public int Compare(Hand? x, Hand? y) => ((int)x!.GetRank()).CompareTo((int)y!.GetRank());
}


public static class Day7
{
    public static void Run()
    {
        List<Hand> hands = new();
        foreach (var lineStr in "7".ReadLinesArray(test: false))
        {
            Span<char> line = lineStr.AsSpan();
            line.ExtractStringRef(out Span<Card> handChar).SkipWhiteRef().ExtractInt(out ulong bid);
            Hand hand = new(handChar, bid);
            hands.Add(hand);
            
            Console.WriteLine($"{handChar} - {hand.GetRank()}");            
        }
        
        // hands.Sort();
    }
}