namespace QAP.Model.Search
{
    internal interface ISearch
    {
        public ProblemModel Problem { get; init; }

        public SearchResult Search(IReadOnlyList<int> initOrder);
    }

    internal record SearchResult
    {
        public string SearchName { get; init; }
        public string? SearchModelOption { get; init; }
        public IReadOnlyList<int> InitOrder { get; init; }
        public IReadOnlyList<int> BestOrder { get; init; }
        public int BestScore { get; init; }
        public ProblemModel Problem { get; init; }
        public int LoopCount { get; init; }

        public SearchResult(string searchName, IReadOnlyList<int> initOrder, IReadOnlyList<int> bestOrder, int bestScore, ProblemModel problem, int loopCount, string? modelOption = null)
        {
            SearchName = searchName;
            SearchModelOption = modelOption;
            InitOrder = initOrder;
            BestOrder = bestOrder;
            BestScore = bestScore;
            Problem = problem;
            LoopCount = loopCount;
        }
    }
}
