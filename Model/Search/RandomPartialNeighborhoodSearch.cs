namespace QAP.Model.Search
{
    internal class RandomPartialNeighborhoodSearch : ISearch
    {
        private string SearchName => "RandomPartialNeighborhoodSearch";

        private Random Random { get; init; }

        private RpnsOption Option { get; init; }

        public RandomPartialNeighborhoodSearch(RpnsOption? option = null)
        {
            Random = new Random();
            Option = option ?? new RpnsOption(RpnsOption.RaitoType.Fix, fixedRaito: 0.2f);
        }

        public SearchResult Search(ProblemModel problem, IReadOnlyList<int> initOrder, TimeSpan? searchTime)
        {
            var time = searchTime ?? TimeSpan.FromSeconds(problem.GetProblemSize());

            var bestOrder = initOrder;
            var bestScore = problem.GetScore(bestOrder);

            var currentOrder = bestOrder;
            var currentScore = bestScore;

            var startTime = DateTime.Now;

            var loopCount = 0;

            while (DateTime.Now.Subtract(startTime) < time)
            {
                var timeRaito = DateTime.Now.Subtract(startTime).TotalSeconds / time.TotalSeconds;

                var raito = Option.Type switch
                {
                    RpnsOption.RaitoType.Fix => Option.FixedRaito,
                    RpnsOption.RaitoType.LinerUpdate => (float)timeRaito * (Option.RaitoMax - Option.RaitoMin) + Option.RaitoMin,
                    RpnsOption.RaitoType.ExponentialUpdate => Option.RaitoMin * (float)Math.Pow(Option.RaitoMax / Option.RaitoMin, timeRaito),
                    _ => throw new Exception("Rpnsの近傍率型に未知の型が代入されています")
                };

                var includeOptimal = IsIncludeMoreOptimal(currentOrder, raito, problem);

                currentOrder = includeOptimal.order;
                currentScore = includeOptimal.score;

                if (currentScore < bestScore)
                {
                    bestOrder = currentOrder;
                    bestScore = currentScore;
                }

                loopCount++;
            }

            return new SearchResult(SearchName, initOrder, bestOrder, bestScore, problem, loopCount, modelOption: Option.ToString());
        }

        public (int score, IReadOnlyList<int> order) IsIncludeMoreOptimal(IReadOnlyList<int> targetOrder, float partialRaito, ProblemModel problem)
        {
            var currentBestScore = -1;
            var currentBestOrder = new List<int[]>();

            for (var i = 0; i < targetOrder.Count; i++)
            {
                for(var k = i + 1; k < targetOrder.Count; k++)
                {
                    var raito = (float)Random.NextDouble();

                    if (partialRaito <= raito)
                        continue;

                    var newOrder = targetOrder.ToArray();
                    var tmp = newOrder[i];
                    newOrder[i] = newOrder[k];
                    newOrder[k] = tmp;

                    var newScore = problem.GetScore(newOrder);

                    // 改善解が近傍中に存在する場合
                    if (newScore < currentBestScore || currentBestScore == -1)
                    {
                        currentBestOrder.Clear();
                        currentBestOrder.Add(newOrder);
                        currentBestScore = newScore;
                    }
                    else if (newScore == currentBestScore)
                    {
                        currentBestOrder.Add(newOrder);
                    }
                }
            }

            if (currentBestOrder.Count == 0)
                return (problem.GetScore(targetOrder), targetOrder); // すべての近傍で探索が出来なかった場合

            // 複数候補から1つ選んで改善解とする
            var resultOrder = currentBestOrder[new Random().Next(0, currentBestOrder.Count)];

            return (currentBestScore, resultOrder);
        }

        public record RpnsOption
        {
            public RaitoType Type { get; init; }
            
            public float FixedRaito { get;init; } // 固定の場合のみ使用
            public float RaitoMin { get; init; } // 動的更新の場合のみ使用
            public float RaitoMax { get; init; } // 動的更新の場合のみ使用

            public RpnsOption(RaitoType type, float fixedRaito = 0.2f, float raitoMin = 0.005f, float raitoMax = 0.2f)
            {
                Type = type;
                FixedRaito = fixedRaito;
                RaitoMin = raitoMin;
                RaitoMax = raitoMax;
            }

            public override string ToString() => Type switch
            {
                RaitoType.Fix => $"Fix-{FixedRaito}",
                RaitoType.LinerUpdate => $"Liner-{RaitoMin}-{RaitoMax}",
                RaitoType.ExponentialUpdate => $"Exponential-{RaitoMin}-{RaitoMax}",
                _ => "Default"
            };

            public enum RaitoType
            {
                Fix, // 固定
                LinerUpdate, // 線形更新
                ExponentialUpdate, // 指数更新
            }
        }
    }
}
