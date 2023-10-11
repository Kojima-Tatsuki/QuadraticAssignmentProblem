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
    var searchTIme = TimeSpan.FromSeconds(model.GetProblemSize()); // 問題サイズの秒数で探索

    var local = new LocalSearch(model);
    var localResult = local.Search(initOrder);

    Console.WriteLine("Loacl ResultScore: " + localResult.BestScore);
    Console.WriteLine("Local ResultOrder: " + string.Join(", ", localResult.BestOrder));

    var tabu = new TabuSearch(model, searchTIme);
    var tabuResult = tabu.Search(initOrder);

    Console.WriteLine("Tabu ResultScore: " + tabuResult.BestScore);
    Console.WriteLine("Tabu ResultOrder: " + string.Join(", ", tabuResult.BestOrder));

    var rpns = new RandomPartialNeighborhoodSearch(model, searchTIme);
    var rpnsResult = rpns.Search(initOrder);

    Console.WriteLine("RPNS ResultScore: " + rpnsResult.BestScore);
    Console.WriteLine("RPNS ResultOrder: " + string.Join(", ", rpnsResult.BestOrder));

    var writer = new SearchResultWriter(dirController.GetResultDirPath());
    writer.WriteResult(problem.info, new List<SearchResult> { localResult, tabuResult, rpnsResult });
}