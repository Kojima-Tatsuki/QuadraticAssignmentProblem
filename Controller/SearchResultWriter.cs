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

        public void WriteResult(ProblemInfo info, IReadOnlyList<SearchResult> results)
        {
            var now = DateTimeOffset.Now.ToLocalTime();
            var fileName = $"{info.ProblemName}{info.ProblemSize}{info.ProblemNumber}-{now.ToString("yyyyMMdd-HHmmss")}.txt";
            var filePath = ResultDirPath + fileName;

            using var sw = new StreamWriter(path: filePath, append: true, encoding: Encoding.UTF8);

            sw.WriteLine($"ProblemInfo: {info.ProblemName} {info.ProblemSize} {info.ProblemNumber}");
            sw.WriteLine($"ResultTime: {now.ToString("yyyy/MM/dd-HH:mm:ss")}");

            sw.WriteLine($"InitOrder: {string.Join(", ", results[0].InitOrder)}");
            sw.WriteLine($"InitScore: {results[0].Problem.GetScore(results[0].InitOrder)}\n");

            foreach (var result in results)
            {
                sw.WriteLine($"SearchName: {result.SearchName}");
                sw.WriteLine($"Score: {result.BestScore}");
                sw.WriteLine($"Order: {string.Join(", ", result.BestOrder)}");
                sw.WriteLine($"Time: {result.Problem.GetProblemSize()}, Loops: {result.LoopCount}\n");
            }

            sw.Close();
        }
    }
}
