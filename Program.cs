// See https://aka.ms/new-console-template for more information
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
    Console.WriteLine("ProblemSize: " + model.GetProblemSize());

    var initOrders = Enumerable.Range(0, 5)
        .Select(_ => model.GetRandomInitOrder())
        .ToList();
    var searchTime = TimeSpan.FromSeconds(model.GetProblemSize()); // 問題サイズの秒数で探索

    // 並列して計算する
    var searchResultModels = selectedSearchModels
        .AsParallel()
        .WithDegreeOfParallelism(6) // 同時実行数
        .Select(searcher => Enumerable.Range(0, initOrders.Count)
            .Select(i => searcher.Search(problem.model, initOrders[i], searchTime))
            .ToArray())
        .Select(SearchResultWriter.SearchResultModel.FromResults)
        .ToList();

    var writer = new SearchResultWriter(dirController.GetResultDirPath());
    writer.WriteResult(problem.info, searchResultModels);
}