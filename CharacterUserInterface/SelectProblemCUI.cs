using QAP.Controller;
using QAP.Model;
using System.Text.RegularExpressions;

namespace QAP.CharacterUserInterface
{
    internal class SelectProblemCUI
    {
        private readonly string ProblemDirPath;

        public SelectProblemCUI(string problemDirPath)
        {
            ProblemDirPath = problemDirPath;
        }

        public async Task<IReadOnlyList<(ProblemModel model, ProblemInfo info)>> ReadProblems(string[] problemsPath)
        {
            var problemInfos = GetProblemPattern(problemsPath);
            var flattenProblemInfos = await SelectProblems(problemInfos); // 問題を選択する

            var controller = new ReadProblemController(ProblemDirPath);
            var problemModels = controller.ReadProblems(flattenProblemInfos);

            var result = new List<(ProblemModel model, ProblemInfo info)>(problemModels.Count);
            for (int i = 0; i < problemModels.Count; i++)
                result.Add((problemModels[i], flattenProblemInfos[i]));

            return result;
        }

        private async Task<IReadOnlyList<ProblemInfo>> SelectProblems(IReadOnlyDictionary<string, List<ProblemInfo>> pattern)
        {
            bool willLoop = true;
            var selectedDict = new Dictionary<string, List<ProblemInfo>>();
            int selectIndex = pattern.Count - 1;

            while (willLoop)
            {
                // UI表示
                Console.WriteLine("Select Problem");
                for (int i = pattern.Count - 1; 0 <= i; i--)
                {
                    var key = pattern.Keys.ElementAt(i);

                    var selected = selectedDict.ContainsKey(key)? '*' : ' ';

                    if (i == selectIndex)
                        Console.WriteLine($"- [{selected}] {key} {pattern[key].Count}");
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
                        if (selectedDict.ContainsKey(key))
                            selectedDict.Remove(key);
                        else
                            selectedDict.Add(key, pattern[key]);
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
            var flatten = selectedDict.Values.SelectMany(x => x).ToList();
            selectIndex = flatten.Count - 1;
            willLoop = true;

            var result = new ProblemInfo[flatten.Count]; // 要素をすべてnullで初期化

            while (willLoop)
            {
                // UI表示
                Console.WriteLine("Select Problem");
                for (int i = result.Length - 1; 0 <= i; i--)
                {
                    var selected = result[i] != null ? '*' : ' ';

                    if (i == selectIndex)
                        Console.WriteLine($"- [{selected}] {flatten[i].ProblemName} {flatten[i].ProblemNumber} {flatten[i].ProblemSize}");
                    else
                        Console.WriteLine($"  [{selected}] {flatten[i].ProblemName} {flatten[i].ProblemNumber} {flatten[i].ProblemSize}");
                }
                Console.WriteLine("Exit by pressing Enter key ...");

                // 入力
                var input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (selectIndex < result.Length - 1)
                            selectIndex++;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (0 < selectIndex)
                            selectIndex--;
                        break;

                    // 選択
                    case ConsoleKey.Spacebar:
                        if (result[selectIndex] == null)
                            result[selectIndex] = flatten[selectIndex];
                        else
                            result[selectIndex] = null!;
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

            return result.Where(x => x != null).ToList() ?? new List<ProblemInfo>(); // すべてnullの場合は空リストを返す
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
