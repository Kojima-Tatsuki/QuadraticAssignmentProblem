// See https://aka.ms/new-console-template for more information
using QAP.Controller;

// Console.WriteLine("Hello, World!");

var parentPath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? throw new Exception("パスの取得に失敗しました");
var controller = new ReadProblemController(parentPath + "/Problems/");

var problems = controller.ReadProblems();

Console.WriteLine("Read problem count: " + problems.Count);

Console.WriteLine("Score: " + problems[0].GetScore(new List<int> { 3, 4, 1, 2 }) / 2);
