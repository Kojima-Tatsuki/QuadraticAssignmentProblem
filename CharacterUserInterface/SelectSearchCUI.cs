using QAP.Model.Search;

namespace QAP.CharacterUserInterface
{
    using RpnsOption = RandomPartialNeighborhoodSearch.RpnsOption;

    internal class SelectSearchCUI
    {
        private readonly float[] RpnsFixedRaitoPatterns = new float[] { 0.01f, 0.02f, 0.03f, 0.05f, 0.1f, 0.2f, 0.3f, 0.5f, 1.0f };
        private readonly float RpnsRaitoMin = 0.005f;
        private readonly float[] RpnsRaitoMaxPatterns = new float[] {0.005f, 0.01f, 0.015f, 0.02f, 0.03f, 0.05f, 0.07f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f, 0.4f};

        private readonly int[] TabuListLengthPatterns = new int[] { 3, 5, 7, 11, 13, 17, 23 };

        public async Task<IReadOnlyList<ISearch>> ReadAddaptSearchModels()
        {
            // RPNS
            var fixedOptions = RpnsFixedRaitoPatterns
                .Select(fixedRaito => new RpnsOption(RpnsOption.RaitoType.Fix, fixedRaito));
            var linerOptions = RpnsRaitoMaxPatterns
                .Select(raitomax => new RpnsOption(RpnsOption.RaitoType.LinerUpdate, raitoMin: RpnsRaitoMin, raitoMax: raitomax));
            var exponentialOptions = RpnsRaitoMaxPatterns
                .Select(raitomax => new RpnsOption(RpnsOption.RaitoType.ExponentialUpdate, raitoMin: RpnsRaitoMin, raitoMax: raitomax));

            var rpnsOptions = fixedOptions.Concat(linerOptions).Concat(exponentialOptions)
                .ToDictionary(option => option.ToString(), option => option);
            var selectedRpnsOptions = await CUIModel.Input(rpnsOptions);
            var rpnsSearches = selectedRpnsOptions.Select(option => new RandomPartialNeighborhoodSearch(option));

            // Tabu
            var tabuOptions = TabuListLengthPatterns
                .Select(length => new TabuSearch.TabuOption { Type = TabuSearch.TabuOption.TabuListType.ListLength, TabuSize = length })
                .ToDictionary(option => option.ToString(), option => option);
            var selectedTabuOptions = await CUIModel.Input(tabuOptions);
            var tabuSearches = selectedTabuOptions.Select(option => new TabuSearch(option));

            return rpnsSearches.Concat<ISearch>(tabuSearches)
                .Concat(new ISearch[] { new LocalSearch() })
                .ToList();
        }
    }

    internal class CUIModel
    {
        // ALL CHECK 未実装
        public static async Task<IReadOnlyList<T>> Input<T>(IReadOnlyDictionary<string, T> patterns, bool hasAllCheckbox = false)
        {
            bool willLoop = true;
            var selectedDict = new Dictionary<string, T>();
            int selectIndex = patterns.Count - 1;

            while (willLoop)
            {
                // UI表示
                Console.WriteLine("Select Problem");
                for (int i = patterns.Count - 1; 0 <= i; i--)
                {
                    var key = patterns.Keys.ElementAt(i);

                    var selected = selectedDict.ContainsKey(key) ? '*' : ' ';

                    if (i == selectIndex)
                        Console.WriteLine($"- [{selected}] {key}");
                    else
                        Console.WriteLine($"  [{selected}] {key}");
                }
                Console.WriteLine("Exit by pressing Enter key ...");

                // 入力
                var input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (selectIndex < patterns.Count - 1)
                            selectIndex++;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (0 < selectIndex)
                            selectIndex--;
                        break;

                    // 選択
                    case ConsoleKey.Spacebar:
                        var key = patterns.Keys.ElementAt(selectIndex);
                        if (selectedDict.ContainsKey(key))
                            selectedDict.Remove(key);
                        else
                            selectedDict.Add(key, patterns[key]);
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
            return selectedDict.Values.ToList();
        }
    }
}
