namespace EDAS.Worker.Handlers.Commands.Combinations;

public class CombinationsInputCommand : IRequest<CombinationsOutput>
{
    public int N { get; set; }

    public int K { get; set; }

    public List<int> Elements { get; set; }

    public CombinationsInputCommand()
    {

    }
}

public class CombinationsOutput
{
    public List<List<int>> Elements { get; set; }

    public CombinationsOutput()
    {

    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var pair in Elements)
        {
            string current = string.Join(", ", pair) + "<br>";
            sb.Append(current);
        }

        string result = sb.ToString();

        return result;
    }
}


public class SolveCombinationsAlgorithmHandler(ICombinationsAlgorithmFactory combinationAlgoFactory,
    IMapper mapper)
    : IRequestHandler<CombinationsInputCommand, CombinationsOutput>
{
    public async Task<CombinationsOutput> Handle(CombinationsInputCommand request, CancellationToken cancellationToken)
    {
        if (request.Elements == null || request.Elements.Count == 0)
        {
            throw new ArgumentException("Bad arguments");
        }

        var input = mapper.Map<CombinationsInputCommand, CombinationAlgoInput>(request);

        var combinationService = combinationAlgoFactory.Create(input);

        var output = combinationService.Run();

        var result = mapper.Map<CombinationsOutput>(output);

        return result;
    }
}
