namespace EDAS.Worker.Services.Factory.CombinationsAlgo;

public class CombinationsAlgorithmFactory 
    : BaseAlgorithmFactory<CombinationAlgo, CombinationAlgoInput>, 
    ICombinationsAlgorithmFactory
{
    public CombinationsAlgorithmFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    ICombinationAlgo IAlgorithmFactory<ICombinationAlgo, CombinationAlgoInput>.Create(
        CombinationAlgoInput input)
    {
        return base.Create(input);
    }
}
