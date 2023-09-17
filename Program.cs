// See https://aka.ms/new-console-template for more information
using QAP.Controller;

Console.WriteLine("Hello, World!");

var controller = new ReadProblemController(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Problems/");

var problems = controller.ReadProblems();

Console.WriteLine(problems.Count);
