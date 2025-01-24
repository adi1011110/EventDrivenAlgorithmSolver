using EDAS.Sorting;
using EDAS.Worker.Services.Factory.SortingAlgo;
using System.Runtime.ConstrainedExecution;

namespace EDAS.Worker.Handlers.Commands.Sorting;

public class SortingInputCommand : IRequest<SortingOutputResult>
{
    public List<int> Numbers { get; set; }

    public SortingInputCommand()
    {
    }
}

public class SortingOutputResult 
{
    public List<int> OutputSortedNumber { get; set; }

    public override string ToString()
    {
        return string.Join(", ", OutputSortedNumber);
    }
}

public class SolveSortingAlgorithmHandler(ISortingAlgorithmFactory factory, IMapper mapper)
    : IRequestHandler<SortingInputCommand, SortingOutputResult>
{
    public async Task<SortingOutputResult> Handle(SortingInputCommand request, CancellationToken cancellationToken)
    {
        if(request.Numbers == null || request.Numbers.Count() == 0)
        {
            throw new ArgumentException("Bad arguments");
        }

        var input = mapper.Map<SortingAlgoInput>(request);

        var algorithmService = factory.Create(input);

        var output = algorithmService.Run();

        var result = mapper.Map<SortingOutputResult>(output);

        return result;
    }
}
