namespace EDAS.Worker.Services.Factory;

public interface IAlgorithmFactory<TAlgo, TInput> 
    where TAlgo: class 
    where TInput : class
{
    TAlgo Create(TInput input);
}
