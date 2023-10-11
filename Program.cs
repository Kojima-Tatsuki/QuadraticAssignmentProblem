// See https://aka.ms/new-console-template for more information
using QAP.CharacterUserInterface;
using QAP.Model.Search;

var dirController = new DirectoryController();
var paths = dirController.GetProblemsPath();

var selectProblemCUI = new SelectProblemCUI(dirController.ParentPath);
var problems = await selectProblemCUI.ReadProblems(paths);

Console.WriteLine("Read problem count: " + problems.Count);

foreach (var problem in problems)
{
    Console.WriteLine("ProblemSize: " + problem.GetProblemSize());

    // var oprimalOrder = new List<int> { 3, 4, 1, 2 };
    // Console.WriteLine("OptimalScore: " + problems[0].GetScore(oprimalOrder));
    // Console.WriteLine("OptimalOrder: " + string.Join(", ", oprimalOrder));

    var initOrder = problem.GetRandomInitOrder();
    var searchTIme = TimeSpan.FromSeconds(problem.GetProblemSize()); // 問題サイズの秒数で探索

    var local = new LocalSearch(problem);
    var localResult = local.Search(initOrder);

    Console.WriteLine("Loacl ResultScore: " + localResult.BestScore);
    Console.WriteLine("Local ResultOrder: " + string.Join(", ", localResult.BestOrder));

    var tabu = new TabuSearch(problem, searchTIme);
    var tabuResult = tabu.Search(initOrder);

    Console.WriteLine("Tabu ResultScore: " + tabuResult.BestScore);
    Console.WriteLine("Tabu ResultOrder: " + string.Join(", ", tabuResult.BestOrder));

    var rpns = new RandomPartialNeighborhoodSearch(problem, searchTIme);
    var rpnsResult = rpns.Search(initOrder);

    Console.WriteLine("RPNS ResultScore: " + rpnsResult.BestScore);
    Console.WriteLine("RPNS ResultOrder: " + string.Join(", ", rpnsResult.BestOrder));
}