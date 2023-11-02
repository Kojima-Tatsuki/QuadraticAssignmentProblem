namespace QAP.Model.Search
{
    internal class LocalSearch : ISearch
    {
        private string SearchName => "LocalSearch";

        public SearchResult Search(ProblemModel problem, IReadOnlyList<int> initOrder, TimeSpan? searchTime)
        {
            var bestOrder = initOrder;
            var bestScore = problem.GetScore(bestOrder);
            var loop = true;

            var loopCount = 0;

            while (loop)
            {
                var includeOptimal = IsIncludeMoreOptimal(bestOrder, bestScore, problem);

                // 改善解が存在しない場合は、無意味な代入となる
                loop = includeOptimal.isInclude;
                bestOrder = includeOptimal.order;
                bestScore = includeOptimal.score;
            }

            return new SearchResult(SearchName, initOrder, bestOrder, bestScore, problem, loopCount, problem.GetProblemSize());
        }

        /// <summary>
        /// 改善解が与えられた解近傍中に存在するか
        /// </summary>
        /// <param name="targetOrder"></param>
        /// <returns></returns>
        private (bool isInclude, int score, IReadOnlyList<int> order) IsIncludeMoreOptimal(IReadOnlyList<int> targetOrder, int targetScore, ProblemModel problem)
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

                    var newScore = problem.GetScore(newOrder);

                    // 改善解が近傍中に存在する場合
                    if (newScore < score)
                        return (true, newScore, newOrder);
                }
            }

            return (false, targetScore, targetOrder);
        }
    }
}
