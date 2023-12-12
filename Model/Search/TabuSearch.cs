namespace QAP.Model.Search
{
    internal class TabuSearch : ISearch
    {
        private string SearchName => "TabuSearch";

        private TabuOption Option { get; init; }

        public TabuSearch(TabuOption? option = null)
        {
            Option = option ?? new TabuOption { Type = TabuOption.TabuListType.OrderLength, SearchType = TabuOption.TabuListSearchType.Factory };
        }

        public SearchResult Search(ProblemModel problem, IReadOnlyList<int> initOrder, TimeSpan? searchTime)
        {
            var time = searchTime ?? TimeSpan.FromSeconds(problem.GetProblemSize()); // 問題サイズの秒数で探索

            var tabuList = Option.Type switch
            {
                TabuOption.TabuListType.OrderLength => TabuList.CreateByOrderLength(initOrder.Count),
                TabuOption.TabuListType.ListLength => TabuList.CreateByListLength(Option.TabuSize),
                _ => throw new Exception("TabuListの型に未知の型が代入されています")
            };
            var bestOrder = initOrder;
            var bestScore = problem.GetScore(bestOrder);

            var currentOrder = bestOrder;
            var currentScore = bestScore;

            var startTime = DateTime.Now;
            var loopCount = 0;

            while(DateTime.Now.Subtract(startTime) < time)
            {
                var includeOptimal = IsIncludeMoreOptimal(currentOrder, tabuList, problem);

                // タブーリストを満たす解が存在しない場合、探索終了
                if (!includeOptimal.HasValue)
                {
                    Console.WriteLine($"Not Found Optimal, {Option}");
                    break;
                }

                currentOrder = includeOptimal.Value.order;
                currentScore = includeOptimal.Value.score;
                tabuList = includeOptimal.Value.tabuList;

                if (currentScore < bestScore)
                {
                    bestOrder = currentOrder;
                    bestScore = currentScore;
                }

                loopCount++;
            }

            return new SearchResult(SearchName, initOrder, bestOrder, bestScore, problem, loopCount, (int)time.TotalSeconds, Option.ToString());
        }

        /// <summary>
        /// 近傍内の最善解を返す
        /// </summary>
        /// <param name="targetOrder"></param>
        /// <returns></returns>
        private (int score, IReadOnlyList<int> order, TabuList tabuList)? IsIncludeMoreOptimal(IReadOnlyList<int> targetOrder, TabuList tabuList, ProblemModel problem)
        {
            // 近傍探索、タブーリストの更新
            var currentBestScore = int.MaxValue;
            var currentBestOrder = new List<(int[] order, int i, int k)>();

            for (var i = 0; i < targetOrder.Count; i++)
            {
                for (var k = i + 1; k < targetOrder.Count; k++)
                {
                    Func<(int factory,int location), (int factory, int location), bool> inTabuFanc = Option.SearchType switch
                    {
                        TabuOption.TabuListSearchType.Factory => tabuList.InTabuFactory,
                        TabuOption.TabuListSearchType.Location => tabuList.InTabuLocation,
                        TabuOption.TabuListSearchType.FactoryAndLocation => tabuList.InTabuFactoryAndLocation,
                        _ => throw new Exception("TabuListの探索型に未知の型が代入されています")
                    };
                    if (inTabuFanc((targetOrder[i], i), (targetOrder[k], k)))
                        continue;

                    // i番目とk番目の要素を入れ替える
                    var newOrder = targetOrder.ToArray();
                    var tmp = newOrder[i];
                    newOrder[i] = newOrder[k];
                    newOrder[k] = tmp;

                    var newScore = problem.GetScore(newOrder);

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

            if (currentBestScore == int.MaxValue && currentBestOrder.Count == 0)
                return null;

            // 複数候補から1つ選んで改善解とする
            var index = new Random().Next(0, currentBestOrder.Count);
            var resultOrder = currentBestOrder[index];
            var resultTabuList = tabuList.AddTabuList(new Pair((resultOrder.order[resultOrder.i], resultOrder.i), (resultOrder.order[resultOrder.k], resultOrder.k)));

            return (currentBestScore, resultOrder.order, resultTabuList);
        }

        public record TabuOption
        {
            public TabuListType Type { get; init; }
            public TabuListSearchType SearchType { get; init; }

            public int TabuSize { get; init; }

            public enum TabuListType
            {
                OrderLength,
                ListLength
            }

            public enum TabuListSearchType
            {
                Factory,
                Location,
                FactoryAndLocation
            }

            public override string ToString()
            {
                var type = Type switch
                {
                    TabuListType.OrderLength => $"OrderLength",
                    TabuListType.ListLength => $"ListLength-{TabuSize}",
                    _ => "Default"
                };  
                var search = SearchType switch
                {
                    TabuListSearchType.Factory => $"Factory",
                    TabuListSearchType.Location => $"Location",
                    TabuListSearchType.FactoryAndLocation => $"FactoryAndLocation",
                    _ => "Default"
                };

                return $"{type}-{search}";
            }
        }

        private class TabuList
        {
            public int Length { get; init; }
            private Queue<(int factory, int location)> IndexQueue { get; init; }

            private TabuList(int listLength)
            {
                Length = listLength;
                IndexQueue = new Queue<(int factory, int location)>(Length);
            }

            public static TabuList CreateByOrderLength(int length) => new TabuList((int)Math.Sqrt(length));
            public static TabuList CreateByListLength(int length) => new TabuList(length);

            public TabuList AddTabuList(Pair pair)
            {
                foreach (var item in pair.ToArray())
                {
                    if (IndexQueue.Count >= Length)
                        IndexQueue.Dequeue();

                    IndexQueue.Enqueue(item);
                }

                return this;
            }

            /// <summary>
            /// 直前に操作した工場がタブーリストに含まれているか
            /// </summary>
            public bool InTabuFactory((int factory, int _) i, (int factory, int _) k) => IndexQueue.Any(_ => _.factory == i.factory || _.factory == k.factory);
            public bool InTabuLocation((int _, int location) i, (int _, int location) k) => IndexQueue.Any(_ => _.location == i.location || _.location == k.location);
            public bool InTabuFactoryAndLocation((int factory, int location) i, (int factory, int location) k) => IndexQueue.Any(_ => (_.factory == i.factory && _.location == i.location) || (_.factory == k.factory && _.location == k.location));
        }

        private record Pair
        {
            public (int factory, int location) First { get; init; }
            public (int factory, int location) Second { get; init; }

            public Pair((int factory, int location) a, (int factory, int location) b)
            {
                First = a;
                Second = b;
            }

            public (int factory, int location)[] ToArray()
            {
                var isFirst = new Random().Next(2) == 1;
                if (isFirst)
                    return new (int factory, int location)[] { First, Second };
                else
                    return new (int factory, int location)[] { Second, First };
            }
        }
    }
}
