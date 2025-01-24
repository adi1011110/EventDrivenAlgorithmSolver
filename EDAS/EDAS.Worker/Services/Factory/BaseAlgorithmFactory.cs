namespace EDAS.Worker.Services.Factory
{
    public abstract class BaseAlgorithmFactory<TAlgo, TInput> : 
        IAlgorithmFactory<TAlgo, TInput>
        where TAlgo : class
        where TInput : class
    {
        private readonly IServiceProvider _serviceProvider;

        protected BaseAlgorithmFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TAlgo Create(TInput input)
        {
            return ActivatorUtilities.CreateInstance<TAlgo>(_serviceProvider, input);
        }
    }
}
