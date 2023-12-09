namespace Aoc2023Cs;

public class Day4
{
    public class Card
    {
        public int number;
        public HashSet<int> winning = new();
        
        private HashSet<int> drawn = new();
        private HashSet<int> own = new();

        public Card(int number, Span<char> line)
        {
            this.number = number;
            
            line.After(':').SkipWhiteRef();

            // read drawn numbers
            while ((line.Length > 0) && (line[0] != '|'))
            {
                line.ExtractIntRef(out int drawnNumber).SkipWhiteRef();
                drawn.Add(drawnNumber);
            }
            
            // skip | and spaces
            line.SkipRef(1);
            line.SkipWhiteRef();
            
            // read own numbers
            while (line.Length > 0)
            {
                line.ExtractIntRef(out int ownedNumbers);
                own.Add(ownedNumbers);
                
                line.SkipWhiteRef();
            }

            // determine winning numbers
            winning = drawn;
            winning.IntersectWith(own);
        }
    };
    
    public static void Run()
    {
        var lines = "4".ReadLinesEnumerable(test: false);

        Dictionary<int, Card> cards = new();
        
        int cardNumber = 0;
        foreach (string line in lines)
        {
            ++cardNumber;
            Card card = new(cardNumber, line.AsSpan());
            cards.Add(card.number, card);
        }

        // part one
        int resultPartOne = cards.Values.Sum(c => Util.GeometricSequence(c.winning.Count, 2));
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