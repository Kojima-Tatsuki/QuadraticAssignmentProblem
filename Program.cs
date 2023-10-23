// See https://aka.ms/new-console-template for more information
using QAP.CharacterUserInterface;
using QAP.Controller;
using QAP.Model.Search;

var dirController = new DirectoryController();
var paths = dirController.GetProblemsPath();

var selectProblemCUI = new SelectProblemCUI(dirController.GetProblemDirPath());
var problems = await selectProblemCUI.ReadProblems(paths);

Console.WriteLine("Read problem count: " + problems.Count);

foreach (var problem in problems)
{
    var model = problem.model;
    Console.WriteLine("ProblemSize: " + model.GetProblemSize());

    var initOrders = Enumerable.Range(0, 5)
        .Select(_ => model.GetRandomInitOrder())
        .ToList();
    var searchTime = TimeSpan.FromSeconds(model.GetProblemSize()); // 問題サイズの秒数で探索

    var searchNames = new string[] { "local", "tabu", "rpns" };

    // 並列して計算する
    var searchResultModels = searchNames
        .Select<string, ISearch>(name => name switch
            {
                "local" => new LocalSearch(model),
                "tabu" => new TabuSearch(model, searchTime),
                "rpns" => new RandomPartialNeighborhoodSearch(model, searchTime),
                _ => throw new Exception("Invalid searcher name")
            })
        .AsParallel()
        .WithDegreeOfParallelism(3) // 同時実行数
        .Select(searcher => Enumerable.Range(0, initOrders.Count)
            .Select(i => searcher.Search(initOrders[i]))
            .ToArray())
        .Select(SearchResultWriter.SearchResultModel.FromResults)
        .ToList();

    var writer = new SearchResultWriter(dirController.GetResultDirPath());
    writer.WriteResult(problem.info, searchResultModels);
}