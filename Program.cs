// See https://aka.ms/new-console-template for more information
using QAP.CharacterUserInterface;
using QAP.Model.Search;

// Console.WriteLine("Hello, World!");

var selectProblemCUI = new SelectProblemCUI();

var problems = await selectProblemCUI.ReadProblems();

Console.WriteLine("Read problem count: " + problems.Count);

var oprimalOrder = new List<int> { 3, 4, 1, 2 };
Console.WriteLine("OptimalScore: " + problems[0].GetScore(oprimalOrder));
Console.WriteLine("OptimalOrder: " + string.Join(", ", oprimalOrder));

var initOrder = new List<int> { 1, 2, 3, 4 };

var local = new LocalSearch(problems[0]);
var localResult = local.Search(initOrder);

Console.WriteLine("Loacl ResultScore: " + localResult.BestScore);
Console.WriteLine("Local ResultOrder: " + string.Join(", ", localResult.BestOrder));

var tabu = new TabuSearch(problems[0]);
var tabuResult = tabu.Search(initOrder);

Console.WriteLine("Tabu ResultScore: " + tabuResult.BestScore);
Console.WriteLine("Tabu ResultOrder: " + string.Join(", ", tabuResult.BestOrder));

var rpns = new RandomPartialNeighborhoodSearch(problems[0], TimeSpan.FromSeconds(3));
var rpnsResult = rpns.Search(initOrder);

Console.WriteLine("Tabu ResultScore: " + rpnsResult.BestScore);
Console.WriteLine("Tabu ResultOrder: " + string.Join(", ", rpnsResult.BestOrder));