namespace QAP.CharacterUserInterface
{
    internal class PercentageShower
    {
        public int MaxCount { get; init; }
        public DateTime StartTime { get; init; }

        public PercentageShower(int maxCount)
        {
            MaxCount = maxCount;
            StartTime = DateTime.Now;
        }

        public void UpdateConsole(int count)
        {
            var secondSpan = (DateTime.Now - StartTime).TotalSeconds;
            Console.WriteLine($"{count} / {MaxCount} ({MathF.Round((float)count / MaxCount * 100, 2)}%), {secondSpan} seconds elapsed.");
        }
    }
}