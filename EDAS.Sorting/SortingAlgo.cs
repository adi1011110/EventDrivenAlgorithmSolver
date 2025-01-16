namespace EDAS.Sorting;

public record SortingAlgoInput(int[] InputNumbers);

public record SortingAlgoOutput(int[] OutputSortedNumber);

public class SortingAlgo : ISortingAlgo
{
    private const int MAX = 1000;

    private SortingAlgoInput _algoInput;

    public SortingAlgo(SortingAlgoInput algoInput)
    {
        if(algoInput == null || 
            algoInput.InputNumbers == null || 
            (!algoInput.InputNumbers.Any()) ||
            algoInput.InputNumbers.Length > MAX)
        {
            throw new ArgumentException("Input can't be null");
        }

       _algoInput = algoInput;
    }

    public SortingAlgoOutput Run()
    {
        QuickSort(_algoInput.InputNumbers, 0, _algoInput.InputNumbers.Length - 1);

        return new SortingAlgoOutput(_algoInput.InputNumbers);
    }

    private void QuickSort(int[] array, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(array, low, high);

            QuickSort(array, low, pivotIndex - 1);
            QuickSort(array, pivotIndex + 1, high);
        }
    }

    private int Partition(int[] array, int low, int high)
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

    private void Swap(int[] array, int a, int b)
    {
        int temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }
}