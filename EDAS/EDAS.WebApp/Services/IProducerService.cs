using EDAS.WebApp.Models;

namespace EDAS.WebApp.Services;

public interface IProducerService
{
    Task SendMessageAsync(ProducerMessage message);
}
