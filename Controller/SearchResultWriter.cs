using QAP.Model;
using QAP.Model.Search;
using System.Text;

namespace QAP.Controller
{
    internal class SearchResultWriter
    {
        private readonly string ResultDirPath;

        public SearchResultWriter(string resultDirPath)
        {
            ResultDirPath = resultDirPath;
        }

        public void WriteResult(ProblemInfo info, IReadOnlyList<SearchResultModel> results)
        {
            var now = DateTimeOffset.Now.ToLocalTime();
            var fileName = $"{info.ProblemName}{info.ProblemSize}{info.ProblemNumber}-{now.ToString("yyyyMMdd-HHmmss")}.txt";
            var filePath = ResultDirPath + fileName;

            using var sw = new StreamWriter(path: filePath, append: true, encoding: Encoding.UTF8);

            sw.WriteLine($"ProblemInfo: {info.ProblemName} {info.ProblemSize} {info.ProblemNumber}");
            sw.WriteLine($"ResultTime: {now.ToString("yyyy/MM/dd-HH:mm:ss")}");

            results.ToList()
                .ForEach(result =>
                {
                    sw.WriteLine($"{result.SearchName}");
                    sw.WriteLine($"  BestScore: {result.BestScore}, Ave: {result.AveScore}");
                    sw.WriteLine($"  Time/Loop: {result.Time} / {result.AveLoops}");
                });

            sw.WriteLine();
            for (int i = 0; i < results.First().Results.Length; i++)
            {
                var initOrder = results.First().Results[i].InitOrder;
                sw.WriteLine($"[{i}]: InitOrder: {string.Join(" ", initOrder)}");
                sw.WriteLine($"[{i}]: BestOrder: {string.Join(" ", results.First().Problem.GetScore(initOrder))}");
                for (int k = 0; k < results.Count; k++)
                {
                    sw.WriteLine($"[{i}]: {results[k].SearchName}");
                    sw.WriteLine($"[{i}]: Score: {results[k].Results[i].BestScore}");
                    sw.WriteLine($"[{i}]: BestOrder: {string.Join(" ", results[k].Results[i].BestOrder)}");
                    sw.WriteLine($"[{i}]: Time/Loop: {results[k].Time}, {results[k].Results[i].LoopCount}");
                }
            }

            sw.Close();
        }

        public record SearchResultModel
        {
            public string SearchName { get; init; }
            public IReadOnlyList<int> InitOrder { get; init; }
            public IReadOnlyList<int> BestOrder { get; init; }
            public ProblemModel Problem { get; init; }
            public int Time { get; init; }
            public float AveLoops { get; init; }
            public float AveScore { get; init; }
            public int BestScore { get; init; }
            public SearchResult[] Results { get; init; }

            private SearchResultModel(string searchName, IReadOnlyList<int> initOrder, IReadOnlyList<int> bestOrder, ProblemModel problem, int time, float aveLoops, float aveScore, int bestScore, SearchResult[] results)
            {
                SearchName = searchName;
                InitOrder = initOrder;
                BestOrder = bestOrder;
                Problem = problem;
                Time = time;
                AveLoops = aveLoops;
                AveScore = aveScore;
                BestScore = bestScore;
                Results = results;
            }

            public static SearchResultModel FromResults(SearchResult[] results)
            {
                float sumLoops = 0;
                float sumScore = 0;
                int bestScore = int.MaxValue;
                IReadOnlyList<int> bestOrder = null!; 

                foreach (var result in results)
                {
                    sumLoops += result.LoopCount;
                    sumScore += result.BestScore;

                    if (result.BestScore < bestScore)
                    {
                        bestScore = result.BestScore;
                        bestOrder = result.BestOrder;
                    }
                }

                return new SearchResultModel(
                    searchName: results[0].SearchName + (results[0].SearchModelOption ?? ""),
                    initOrder: results[0].InitOrder,
                    bestOrder: bestOrder,
                    problem: results[0].Problem,
                    time: results[0].Problem.GetProblemSize(),
                    aveLoops: sumLoops / results.Length,
                    aveScore: sumScore / results.Length,
                    bestScore: bestScore,
                    results: results);
            }
        }
    }
}
