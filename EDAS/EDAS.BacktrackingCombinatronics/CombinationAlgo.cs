namespace EDAS.BacktrackingCombinatronics;

public class CombinationAlgoInput 
{
    public int N { get; set; }

    public int K { get; set; }

    public List<int> Elements { get; set; }
} 

public class CombinationAlgoOutput
{
    public List<List<int>> Solution { get; set; }

    public CombinationAlgoOutput()
    {
        Solution = new List<List<int>>();
    }
}

public class CombinationAlgo : ICombinationAlgo
{
    private const int MAX = 100;

    private int[] _algoStack = new int[MAX];

    private CombinationAlgoInput _algoInput;

    private CombinationAlgoOutput _algoOutput;

    public CombinationAlgo(CombinationAlgoInput input)
    {
        _algoInput = input;
        _algoInput.Elements.Insert(0, 0);
        _algoOutput = new CombinationAlgoOutput();
    }

    public CombinationAlgoOutput Run()
    {
        try
        {
            Backtracking(1);
            return _algoOutput;
        }
        catch (Exception e) 
        {
            return new CombinationAlgoOutput();
        }
    }

    private void Backtracking(int position)
    {
        _algoStack[position] = 0;
        for (int i = position; i <= _algoInput.N; i++)
        {
            _algoStack[position] = _algoInput.Elements[i];

            if (_IsValid(position))
            {
                if (_IsSolution(position))
                {
                    _StoreSolution(position);
                }
                else
                {
                    Backtracking(position + 1);
                }
            }
        }
    }

    private bool _IsValid(int k)
    {
        for (int i = 1; i < k; i++)
        {
            if (_algoStack[i] == _algoStack[k] || 
                _algoStack[i] > _algoStack[k])
            {
                return false;
            }
        }
        return true;
    }

    private bool _IsSolution(int index)
    {
        return index == _algoInput.K;
    }

    private void _StoreSolution(int k)
    {
        var tempSolution = new List<int>();
        for (int i = 1; i <= k; i++)
        {
            tempSolution.Add(_algoStack[i]);
        }
        _algoOutput.Solution.Add(tempSolution);
    }
}