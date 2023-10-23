namespace QAP.Model.Search
{
    internal class TabuSearch : ISearch
    {
        private string SearchName => "TabuSearch";

        public ProblemModel Problem { get; init; }
        public TimeSpan SearchTime { get; init; }

        public TabuSearch(ProblemModel problem, TimeSpan searchTIme)
        {
            Problem = problem;
            SearchTime = searchTIme;
        }

        public SearchResult Search(IReadOnlyList<int> initOrder)
        {
            var tabuList = new TabuList(initOrder.Count);
            var bestOrder = initOrder;
            var bestScore = Problem.GetScore(bestOrder);

            var currentOrder = bestOrder;
            var currentScore = bestScore;

            var startTime = DateTime.Now;
            var loopCount = 0;

            while(DateTime.Now.Subtract(startTime) < SearchTime)
            {
                var includeOptimal = IsIncludeMoreOptimal(currentOrder, tabuList);

                currentOrder = includeOptimal.order;
                currentScore = includeOptimal.score;
                tabuList = includeOptimal.tabuList;

                if (currentScore < bestScore)
                {
                    bestOrder = currentOrder;
                    bestScore = currentScore;
                }

                loopCount++;
            }

            return new SearchResult(SearchName, initOrder, bestOrder, bestScore, Problem, loopCount);
        }

        /// <summary>
        /// 近傍内の最善解を返す
        /// </summary>
        /// <param name="targetOrder"></param>
        /// <returns></returns>
        private (int score, IReadOnlyList<int> order, TabuList tabuList) IsIncludeMoreOptimal(IReadOnlyList<int> targetOrder, TabuList tabuList)
        {
            // 近傍探索、タブーリストの更新
            var currentBestScore = int.MaxValue;
            var currentBestOrder = new List<(int[] order, int i, int k)>();

            for (var i = 0; i < targetOrder.Count; i++)
            {
                for (var k = i + 1; k < targetOrder.Count; k++)
                {
                    if (tabuList.InTabu(i, k))
                        continue;

                    // i番目とk番目の要素を入れ替える
                    var newOrder = targetOrder.ToArray();
                    var tmp = newOrder[i];
                    newOrder[i] = newOrder[k];
                    newOrder[k] = tmp;

                    var newScore = Problem.GetScore(newOrder);

                    // 改善解が近傍中に存在する場合
                    if (newScore < currentBestScore)
                    {
                        currentBestOrder.Clear();
                        currentBestOrder.Add((newOrder, i, k));
                        currentBestScore = newScore;
                    }
                    else if (newScore == currentBestScore)
                    {
                        currentBestOrder.Add((newOrder, i, k));
                    }
                }
            }

            // 複数候補から1つ選んで改善解とする
            var index = new Random().Next(0, currentBestOrder.Count);
            var resultOrder = currentBestOrder[index];
            var resultTabuList = tabuList.AddTabuList(new Pair(resultOrder.i, resultOrder.k));

            return (currentBestScore, resultOrder.order, resultTabuList);
        }

        private class TabuList
        {
            public int Length { get; init; }
            private Queue<int> IndexQueue { get; init; }

            public TabuList(int orderLength)
            {
                Length = (int)Math.Sqrt(orderLength); // 平方根を取る
                IndexQueue = new Queue<int>(Length);
            }

            public TabuList AddTabuList(Pair pair)
            {
                if (IndexQueue.Count >= Length)
                {
                    IndexQueue.Dequeue();
                    IndexQueue.Dequeue();
                }

                IndexQueue.Enqueue(pair.First);
                IndexQueue.Enqueue(pair.Second);

                return this;
            }

            public bool InTabu(int i, int k) => IndexQueue.Any(_ => _ == i || _ == k);
        }

        private record Pair
        {
            public int First { get; init; }
            public int Second { get; init; }

            public Pair(int a, int b)
            {
                First = a;
                Second = b;
            }
        }
    }
}
