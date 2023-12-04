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

        List<Card> cards = new();
        
        int cardNumber = 0;
        foreach (string constLine in lines)
        {
            ++cardNumber;
            Card card = new(cardNumber, constLine);
            cards.Add(card);
        }

        int resultPartOne = 0;
        foreach (Card card in cards)
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
            Console.WriteLine($"{card.number} - !{winning}! ({drawn} / {own})");
        }
        Console.WriteLine();
        
        Console.WriteLine($"Part One : {resultPartOne}");
    }
}