using QAP.CharacterUserInterface;
using QAP.Controller;

var dirController = new DirectoryController();
var paths = dirController.GetProblemsPath();

// 問題選択
var selectProblemCUI = new SelectProblemCUI(dirController.GetProblemDirPath());
var problems = await selectProblemCUI.ReadProblems(paths);

Console.WriteLine("Read problem count: " + problems.Count);

// 探索モデルの選択
var selectedSearchModels = await new SelectSearchCUI().ReadAddaptSearchModels();

foreach (var problem in problems)
{
    var model = problem.model;
    Console.WriteLine($"{problem.info.ProblemName}-{problem.info.ProblemSize}{problem.info.ProblemNumber}, Size: {model.GetProblemSize()}");

    var initOrders = Enumerable.Range(0, 5)
        .Select(_ => model.GetRandomInitOrder())
        .ToList();
    var searchTime = TimeSpan.FromSeconds(5); // 問題サイズの秒数で探索

    var shower = new PercentageShower(selectedSearchModels.Count * initOrders.Count);
    int currentCount = 0;

    // 進捗表示
    shower.UpdateConsole(currentCount);

    // 並列して計算する
    var searchResultModels = selectedSearchModels
        .AsParallel()
        .WithDegreeOfParallelism(6) // 同時実行数
        .Select(searcher => Enumerable.Range(0, initOrders.Count)
            .Select(i =>
            {
                var result = searcher.Search(problem.model, initOrders[i], searchTime);
                shower.UpdateConsole(++currentCount);
                return result;
            })
            .ToArray())
        .Select(SearchResultWriter.SearchResultModel.FromResults)
        .ToList();

    var writer = new SearchResultWriter(dirController.GetResultDirPath());
    writer.WriteResult(problem.info, searchResultModels);
}