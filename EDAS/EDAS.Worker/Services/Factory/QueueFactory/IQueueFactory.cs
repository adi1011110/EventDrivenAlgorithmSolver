using EDAS.Worker.Services.Queues;

namespace EDAS.Worker.Services.Factory.QueueFactory;

public interface IQueueFactory : IAlgorithmFactory<IRabbitMQueue, QueueFactoryConfig>;
