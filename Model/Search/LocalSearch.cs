using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAP.Model.Search
{
    internal class LocalSearch : ISearch
    {
        public ProblemModel Problem { get; init; }

        public LocalSearch(ProblemModel problem)
        {
            Problem = problem;
        }

        public SearchResult Search(IReadOnlyList<int> initOrder)
        {
            var bestOrder = initOrder;
            var bestScore = Problem.GetScore(bestOrder);
            var loop = true;

            while (loop)
            {
                var includeOptimal = IsIncludeMoreOptimal(bestOrder, bestScore);

                Console.WriteLine("Score: " + includeOptimal.score + ", Order: " + string.Join(", ", includeOptimal.order));

                // 改善解が存在しない場合は、無意味な代入となる
                loop = includeOptimal.isInclude;
                bestOrder = includeOptimal.order;
                bestScore = includeOptimal.score;
            }

            return new SearchResult(initOrder, bestOrder, bestScore, Problem);
        }

        /// <summary>
        /// 改善解が与えられた解近傍中に存在するか
        /// </summary>
        /// <param name="targetOrder"></param>
        /// <returns></returns>
        private (bool isInclude, int score, IReadOnlyList<int> order) IsIncludeMoreOptimal(IReadOnlyList<int> targetOrder, int targetScore)
        {
            var score = targetScore;

            for (var i = 0; i < targetOrder.Count; i++)
            {
                for (var j = i + 1; j < targetOrder.Count; j++)
                {
                    var newOrder = targetOrder.ToArray();
                    var tmp = newOrder[i];
                    newOrder[i] = newOrder[j];
                    newOrder[j] = tmp;

                    var newScore = Problem.GetScore(newOrder);

                    // 改善解が近傍中に存在する場合
                    if (newScore < score)
                        return (true, newScore, newOrder);
                }
            }

            return (false, targetScore, targetOrder);
        }
    }
}
