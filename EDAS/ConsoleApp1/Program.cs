using EDAS.BacktrackingCombinatronics;

const int MAX = 100;


List<int> input = new List<int>();
 int n, p;

Console.Write("n = ");
n = int.Parse(Console.ReadLine());

Console.Write("p = ");
p = int.Parse(Console.ReadLine());


for (int i = 0; i < n; i++)
{
    Console.Write($"Element #{i}: ");
    input.Add(int.Parse(Console.ReadLine()));
}

var algoInput = new CombinationAlgoInput(n, p, input);

CombinationAlgo myAlgo = new CombinationAlgo(algoInput);

var result = myAlgo.Run();

foreach(var current in result.Solution)
{
    foreach(var element in current)
    {
        Console.WriteLine(element + " ");
    }
    Console.WriteLine("=========");
}