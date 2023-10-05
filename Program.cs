// See https://aka.ms/new-console-template for more information
using QAP.Controller;
using QAP.Model.Search;

// Console.WriteLine("Hello, World!");

var parentPath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? throw new Exception("パスの取得に失敗しました");
var controller = new ReadProblemController(parentPath + "/Problems/");

var problems = controller.ReadProblems();

Console.WriteLine("Read problem count: " + problems.Count);

var oprimalOrder = new List<int> { 3, 4, 1, 2 };
Console.WriteLine("OptimalScore: " + problems[0].GetScore(oprimalOrder));
Console.WriteLine("OptimalOrder: " + string.Join(", ", oprimalOrder));

var local = new LocalSearch(problems[0]);
var result = local.Search(new List<int> { 1, 2, 3, 4 });

Console.WriteLine("ResultScore: " + result.BestScore);
Console.WriteLine("ResultOrder: " + string.Join(", ", result.BestOrder));
