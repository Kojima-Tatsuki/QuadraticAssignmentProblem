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

            var _ = await SelectProblems(problemPattern); // 同期処理だが、かなり長い

            var controller = new ReadProblemController(problemDirPath);

            var problems = controller.ReadProblems();

            return problems;
        }

        private async Task<IReadOnlyList<ProblemInfo>> SelectProblems(IReadOnlyDictionary<string, List<ProblemInfo>> pattern)
        {
            bool willLoop = true;
            var selectedList = new Dictionary<string, List<ProblemInfo>>();
            int selectIndex = pattern.Count - 1;

            while (willLoop)
            {
                // UI表示
                Console.WriteLine("Select Problem");
                for (int i = pattern.Count - 1; 0 <= i; i--)
                {
                    var key = pattern.Keys.ElementAt(i);

                    var selected = selectedList.ContainsKey(key)? '*' : ' ';

                    if (i == selectIndex)
                        Console.WriteLine($"- [{selected}] {key} {pattern[key].Count} {selectIndex}");
                    else
                        Console.WriteLine($"  [{selected}] {key} {pattern[key].Count}");
                }
                Console.WriteLine("Exit by pressing Enter key ...");

                // 入力
                var input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (selectIndex < pattern.Count - 1)
                            selectIndex++;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (0 < selectIndex)
                            selectIndex--;
                        break;

                    // 選択
                    case ConsoleKey.Spacebar:
                        var key = pattern.Keys.ElementAt(selectIndex);
                        if (selectedList.ContainsKey(key))
                            selectedList.Remove(key);
                        else
                            selectedList.Add(key, pattern[key]);
                        break;
                    // 終了処理
                    case ConsoleKey.Enter:
                        willLoop = false;
                        break;
                    default:
                        break;
                }

                await Task.Delay(100);

                Console.Clear();
            }

            // 平坦化 (Flatten)
            return selectedList.Values.SelectMany(x => x).ToList();
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
