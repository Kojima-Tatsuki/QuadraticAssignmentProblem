using QAP.Model;

namespace QAP.Controller
{
    internal class ReadProblemController
    {
        private readonly string dirPath;

        public ReadProblemController(string dirPath)
        {
            this.dirPath = dirPath;
        }

        public IReadOnlyList<ProblemModel> ReadProblems(ProblemInfo? problemType = null)
        {
            // ファイル名は " 問題名 + 問題サイズ + 問題番号 " で表されている

            var paths = GetPathTobeRead(problemType);
            var result = new List<ProblemModel>(paths.Length);

            foreach (var path in paths)
            {
                using var sr = new StreamReader(dirPath + path);

                var str = sr.ReadToEnd();

                Console.WriteLine("Problem read completed.\n" + "File Path: " + dirPath + path + "\n" + str);

                var model = ProblemSeparator.ToModel(str);
                result.Add(model);
            }

            return result;
        }

        private string[] GetPathTobeRead(ProblemInfo? problemType = null)
        {
            return new string[] { "smp4.dat" };
        }
    }

    internal class ProblemInfo
    {
        public string ProblemName { get; init; } // bur, chr, els, ...
        public int ProblemSize { get; init; } // 4, 5, 6, ...
        public char? ProblemNumber { get; init; } // a, b, c, ...

        public ProblemInfo(string problemName, int problemSize, char? problemNumber = null)
        {
            ProblemName = problemName;
            ProblemSize = problemSize;
            ProblemNumber = problemNumber;
        }
    }
}
