namespace EDAS.Sorting;

public class SortingAlgoInput 
{
    public List<int> Numbers { get; set; }

    public SortingAlgoInput()
    {

    }
} 

public record SortingAlgoOutput(List<int> OutputSortedNumber);

public class SortingAlgo : ISortingAlgo
{
    private const int MAX = 1000;

    private SortingAlgoInput _algoInput;

    public SortingAlgo(SortingAlgoInput algoInput)
    {
        if(algoInput == null || 
            algoInput.Numbers == null || 
            (!algoInput.Numbers.Any()) ||
            algoInput.Numbers.Count() > MAX)
        {
            throw new ArgumentException("Input can't be null");
        }

       _algoInput = algoInput;
    }

    public SortingAlgoOutput Run()
    {
        QuickSort(_algoInput.Numbers, 0, _algoInput.Numbers.Count() - 1);

        return new SortingAlgoOutput(_algoInput.Numbers);
    }

    private void QuickSort(List<int> array, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(array, low, high);

            QuickSort(array, low, pivotIndex - 1);
            QuickSort(array, pivotIndex + 1, high);
        }
    }

    private int Partition(List<int> array, int low, int high)
    {
        int pivot = array[high];
        int i = low - 1; 

        for (int j = low; j < high; j++)
        {
            if (array[j] < pivot)
            {
                i++;
                Swap(array, i, j);
            }
        }

        Swap(array, i + 1, high);

        return i + 1;
    }

    private void Swap(List<int> array, int a, int b)
    {
        int temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }
}