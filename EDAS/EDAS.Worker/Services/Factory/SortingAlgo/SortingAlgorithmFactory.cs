using EDAS.Sorting;

namespace EDAS.Worker.Services.Factory.SortingAlgo;

public class SortingAlgorithmFactory : 
    BaseAlgorithmFactory<EDAS.Sorting.SortingAlgo, SortingAlgoInput>,
    ISortingAlgorithmFactory
{
    public SortingAlgorithmFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        
    }

    ISortingAlgo IAlgorithmFactory<ISortingAlgo, SortingAlgoInput>.Create(SortingAlgoInput input)
    {
        return base.Create(input);
    }
}
