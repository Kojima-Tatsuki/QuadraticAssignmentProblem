namespace QAP.Model.Search
{
    internal interface ISearch
    {
        public ProblemModel Problem { get; init; }

        public SearchResult Search(IReadOnlyList<int> initAns);
    }

    internal record SearchResult
    {
        public IReadOnlyList<int> InitAns { get; init; }
        public IReadOnlyList<int> Ans { get; init; }
        public int Score { get; init; }
        public ProblemModel Problem { get; init; }

        public SearchResult(IReadOnlyList<int> initAns, IReadOnlyList<int> ans, int score, ProblemModel problem)
        {
            InitAns = initAns;
            Ans = ans;
            Score = score;
            Problem = problem;
        }
    }
}
