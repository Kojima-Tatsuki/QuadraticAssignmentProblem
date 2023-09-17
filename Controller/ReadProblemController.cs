using QAP.Model;
using System.Security.Cryptography.X509Certificates;

namespace QAP.Controller
{
    internal class ReadProblemController
    {
        private readonly string dirPath;

        public ReadProblemController(string dirPath)
        {
            this.dirPath = dirPath;
        }

        public IReadOnlyList<ProblemModel> ReadProblems(ProblemType? problemType = null)
        {
            // ファイル名は " 問題名 + 問題サイズ + 問題番号 " で表されている

            var paths = GetPathTobeRead(problemType);
            var result = new List<ProblemModel>(paths.Length);

            foreach (var path in paths)
            {
                using var sr = new StreamReader(dirPath + path);

                var str = sr.ReadToEnd();

                Console.WriteLine(dirPath + path + ", " + str);

                ProblemModel model = null; // new ProblemModel(null, null);
                result.Add(model);
            }

            return result;
        }

        private string[] GetPathTobeRead(ProblemType? problemType = null)
        {
            return new string[] { "nug12.dat" };
        }

        public class ProblemType
        {
            public string? ProblemName { get; init; } // bur, chr, els, ...
            public char? ProblemNumber { get; init; } // a, b, c, ...

            public ProblemType(string? problemName = null, char? problemNumber = null)
            {
                ProblemName = problemName;
                ProblemNumber = problemNumber;
            }
        }
    }
}
