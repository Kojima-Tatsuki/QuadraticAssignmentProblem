using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAP.Model.Search
{
    internal class RandomPartialNeighborhoodSearch : ISearch
    {
        private string SearchName => "RandomPartialNeighborhoodSearch";
        public ProblemModel Problem { get; init; }
        public TimeSpan SearchTime { get; init; }

        private Random Random { get; init; }

        public RandomPartialNeighborhoodSearch(ProblemModel problem, TimeSpan searchTime)
        {
            Problem = problem;
            SearchTime = searchTime;
            Random = new Random();
        }

        public SearchResult Search(IReadOnlyList<int> initOrder)
        {
            var bestOrder = initOrder;
            var bestScore = Problem.GetScore(bestOrder);

            var currentOrder = bestOrder;
            var currentScore = bestScore;

            var partialRaito = 0.2f; // 部分近傍率
            var startTime = DateTime.Now;

            var loopCount = 0;

            while (DateTime.Now.Subtract(startTime) < SearchTime)
            {
                var includeOptimal = IsIncludeMoreOptimal(currentOrder, partialRaito);

                currentOrder = includeOptimal.order;
                currentScore = includeOptimal.score;

                if (currentScore < bestScore)
                {
                    bestOrder = currentOrder;
                    bestScore = currentScore;
                }

                loopCount++;
            }

            return new SearchResult(SearchName, initOrder, bestOrder, bestScore, Problem, loopCount);
        }

        public (int score, IReadOnlyList<int> order) IsIncludeMoreOptimal(IReadOnlyList<int> targetOrder, float partialRaito)
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

                    var newScore = Problem.GetScore(newOrder);

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
                return (Problem.GetScore(targetOrder), targetOrder); // すべての近傍で探索が出来なかった場合

            // 複数候補から1つ選んで改善解とする
            var resultOrder = currentBestOrder[new Random().Next(0, currentBestOrder.Count)];

            return (currentBestScore, resultOrder);
        }

        public record RPNSOptions
        {
            public RaitoType Type { get; init; }
            
            public float? FixedRaito { get;init; } // 固定の場合のみ使用
            public float? RaitoMin { get; init; } // 動的更新の場合のみ使用
            public float? RaitoMax { get; init; } // 動的更新の場合のみ使用


            public enum RaitoType
            {
                Fix, // 固定
                LinerUpdate, // 線形更新
                ExponentialUpdate, // 指数更新
            }
        }
    }
}
