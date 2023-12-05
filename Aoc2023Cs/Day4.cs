﻿namespace Aoc2023Cs;

public class Day4
{
    public class Card
    {
        public int number;
        public HashSet<int> winning = new();
        
        private HashSet<int> drawn = new();
        private HashSet<int> own = new();

        public Card(int number, string constLine)
        {
            this.number = number;
            
            string line = constLine.After(':');
            line = Util.SkipSpaces(line);

            // read drawn numbers
            while ((line.Length > 0) && (line.First() != '|'))
            {
                line = Util.ExtractInt(line, out var drawnNumber);
                drawn.Add(drawnNumber);
                
                line = Util.SkipSpaces(line);
            }
            
            // skip | and spaces
            line = line.Substring(1);
            line = Util.SkipSpaces(line);
            
            // read own numbers
            while (line.Length > 0)
            {
                line = Util.ExtractInt(line, out var ownedNumbers);
                own.Add(ownedNumbers);
                
                line = Util.SkipSpaces(line);
            }

            // determine winning numbers
            winning = drawn;
            winning.IntersectWith(own);
        }
    };
    
    public static void Run()
    {
        var lines = Util.ReadLines("4", test: false);

        Dictionary<int, Card> cards = new();
        
        int cardNumber = 0;
        foreach (string constLine in lines)
        {
            ++cardNumber;
            Card card = new(cardNumber, constLine);
            cards.Add(card.number, card);
        }

        // part one
        int resultPartOne = cards.Values.Sum(c => Util.GeometricSequence(c.winning.Count, 2));
        Console.WriteLine();
        Console.WriteLine($"Part One : {resultPartOne}");

        // part two
        int resultPartTwo = 0;
        Queue<Card> cardQueue = new(cards.Values);
        while (cardQueue.Count > 0)
        {
            ++resultPartTwo;            
            Card card = cardQueue.Dequeue();
            for (int i = 0; i < card.winning.Count; ++i)
            {
                Card requeueCard = cards[card.number + 1 + i];
                cardQueue.Enqueue(requeueCard);
            }
        }
        Console.WriteLine($"Part Two : {resultPartTwo}");
    }
}