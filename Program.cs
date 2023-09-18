// See https://aka.ms/new-console-template for more information
using QAP.Controller;

Console.WriteLine("Hello, World!");

var parentPath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? throw new Exception("パスの取得に失敗しました");
var controller = new ReadProblemController(parentPath + "/Problems/");

var problems = controller.ReadProblems();

Console.WriteLine(problems.Count);
