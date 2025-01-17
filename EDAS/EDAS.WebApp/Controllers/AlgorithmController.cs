using EDAS.WebApp.Models.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace EDAS.WebApp.Controllers;

[Authorize]
public class AlgorithmController : Controller
{
    private readonly BrokerConfig _brokerConfig;
    private readonly QueueConfigCollection _queueConfigCollection;
    private readonly IProducerFactory _producerFactory;
    private readonly UserManager<EDASWebAppUser> _userManager;

    public AlgorithmController(
        IOptions<BrokerConfig> brokerConfigOption,
        QueueConfigCollection queueConfigCollection,
        IProducerFactory producerFactory,
        UserManager<EDASWebAppUser> userManager)
    {
        _brokerConfig = brokerConfigOption.Value;
        _queueConfigCollection = queueConfigCollection;
        _producerFactory = producerFactory;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("[controller]/Backtracking/[action]")]
    public IActionResult Combinatronics()
    {
        
        var viewModel = new CombinatronicsVM();
        return View(viewModel);
    }

    [HttpPost]
    [Route("[controller]/Backtracking/[action]")]
    public async Task<IActionResult> Combinatronics([FromForm] CombinatronicsVM viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                ModelState.AddModelError("Authentication", "User could nout be found");
                //TO DO: redirect to error page
                return View(viewModel);
            }

            var email = user.Email;

            var producerType = ProducerType.Combinatronics;

            var queueConfig = _queueConfigCollection.QueuesConfig[producerType];

            var producerFactoryConfig = new ProducerFactoryConfig(queueConfig, producerType);

            var producerService = await _producerFactory.Create(producerFactoryConfig);

            var producerMessage = new ProducerCombinatronicsMessage
            {
                EmailAddress = email,
                N = viewModel.N,
                K = viewModel.K,
                ElementsCSV = viewModel.ElementsCSV
            };

            var message = JsonConvert.SerializeObject(producerMessage);

            await producerService.SendMessageAsync(message);

            return RedirectToAction(nameof(SubmissionSuccessful));
        }

        return View();
    }

    [HttpGet]
    [Route("[controller]/Sorting/[action]")]
    public IActionResult QuickSort()
    {
        var viewModel = new SortingVM();

        return View(viewModel);
    }

    [HttpPost]
    [Route("[controller]/Sorting/[action]")]
    public async Task<IActionResult> QuickSort([FromForm] SortingVM viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                ModelState.AddModelError("Authentication", "User could nout be found");
                //TO DO: redirect to error page
                return View(viewModel);
            }

            var email = user.Email;

            var producerType = ProducerType.Sorting;

            var queueConfig = _queueConfigCollection.QueuesConfig[producerType];

            var producerFactoryConfig = new ProducerFactoryConfig(queueConfig, producerType);

            var producerService = await _producerFactory.Create(producerFactoryConfig);

            var producerMessage = new ProducerSortingMessage
            {
                EmailAddress = email,
                ElementsCSV = viewModel.ElementsCSV
            };

            var message = JsonConvert.SerializeObject(producerMessage);

            await producerService.SendMessageAsync(message);

            return RedirectToAction(nameof(SubmissionSuccessful));
        }

        return View();
    }


    [HttpGet]
    public IActionResult SubmissionSuccessful()
    {
        return View();
    }
}
