namespace Aoc2023Cs;

public class Day4
{
    public class Card
    {
        public int number;
        public HashSet<int> drawn = new();
        public HashSet<int> own = new();
        public List<int> winning = new();

        public Card(int number)
        {
            this.number = number;
        }

        public Card(int number, string constLine)
        {
            this.number = number;
            
            string line = constLine.After(':');
            line = Util.SkipSpaces(line);

            while ((line.Length > 0) && (line.First() != '|'))
            {
                line = Util.ExtractInt(line, out var winningNumber);
                drawn.Add(winningNumber);
                
                line = Util.SkipSpaces(line);
            }
            line = line.Substring(1);
            line = Util.SkipSpaces(line);
            
            while (line.Length > 0)
            {
                line = Util.ExtractInt(line, out var drawnNumber);
                own.Add(drawnNumber);
                
                line = Util.SkipSpaces(line);
            }

            HashSet<int> winningSet = drawn;
            winningSet.IntersectWith(own);
            winning = winningSet.ToList();
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
        int resultPartOne = 0;
        foreach (Card card in cards.Values)
        {
            if (card.winning.Count > 0)
            {
                int value = 1;
                for (int i = 1; i < card.winning.Count; ++i)
                {
                    value *= 2;
                }
                resultPartOne += value;
            }
            
            string winning = Util.MakeList(card.winning);
            string drawn = Util.MakeList(card.drawn);
            string own = Util.MakeList(card.own);
            Console.WriteLine($"Card {card.number} - {card.winning.Count} winning - !{winning}! ({drawn} / {own})");
        }
        Console.WriteLine();
        Console.WriteLine($"Part One : {resultPartOne}");

        // part two
        int resultPartTwo = 0;
        Queue<Card> cardQueue = new();
        foreach (Card card in cards.Values)
        {
            cardQueue.Enqueue(card);
        }
        
        while (cardQueue.Count > 0)
        {
            ++resultPartTwo;            
            Card card = cardQueue.Dequeue();
            for (int i = 0; i < card.winning.Count; ++i)
            {
                Card requeueCard = cards[card.number + 1 +i];
                cardQueue.Enqueue(requeueCard);
            }
        }
        Console.WriteLine($"Part Two : {resultPartTwo}");
    }
}