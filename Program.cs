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

    var initOrder = model.GetRandomInitOrder();
    var searchTime = TimeSpan.FromSeconds(model.GetProblemSize()); // 問題サイズの秒数で探索

    var searchNames = new string[] { "local", "tabu", "rpns" };

    var searchResults = searchNames
        .Select<string, ISearch>(name => name switch
            {
                "local" => new LocalSearch(model),
                "tabu" => new TabuSearch(model, searchTime),
                "rpns" => new RandomPartialNeighborhoodSearch(model, searchTime),
                _ => throw new Exception("Invalid searcher name")
            })
        .AsParallel()
        .WithDegreeOfParallelism(3) // 同時実行数
        .Select(searcher => searcher.Search(initOrder))
        .ToList();

    var writer = new SearchResultWriter(dirController.GetResultDirPath());
    writer.WriteResult(problem.info, searchResults);
}