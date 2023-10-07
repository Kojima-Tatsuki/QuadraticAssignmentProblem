using QAP.Controller;
using QAP.Model;
using System.Text.RegularExpressions;

namespace QAP.CharacterUserInterface
{
    internal class SelectProblemCUI
    {
        public async Task<IReadOnlyList<ProblemModel>> ReadProblems()
        {
            var parentPath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? throw new Exception("パスの取得に失敗しました");
            var problemDirPath = parentPath + "/Problems/";

            var files = Directory.GetFiles(problemDirPath, "*.dat", SearchOption.TopDirectoryOnly);

            var problemPattern = GetProblemPattern(files);

            var controller = new ReadProblemController(problemDirPath);

            var problems = controller.ReadProblems();

            return problems;
        }

        private IReadOnlyDictionary<string, List<ProblemInfo>> GetProblemPattern(string[] problemPaths)
        {
            var dict = new Dictionary<string, List<ProblemInfo>>();

            for (int i = 0; i < problemPaths.Length; i++)
            {
                var name = Path.GetFileName(problemPaths[i]);

                var pattern = @"([A-Za-z]+)(\d+)([A-Za-z]?).dat";
                var match = Regex.Match(name, pattern);

                var problemInfo = new ProblemInfo(match.Groups[1].Value, int.Parse(match.Groups[2].Value), match.Groups[3].Value == "" ? null: match.Groups[3].Value[0]);

                if (dict.ContainsKey(problemInfo.ProblemName))
                    dict[problemInfo.ProblemName].Add(problemInfo);
                else
                    dict.Add(problemInfo.ProblemName, new List<ProblemInfo> { problemInfo });
            }

            return dict;
        }
    }
}
