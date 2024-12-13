using EDAS.WebApp.Models;
using EDAS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EDAS.WebApp.Controllers;

public class AlgorithmController : Controller
{
    private readonly IProducerService _producerSerice;

    public AlgorithmController(IProducerService producerSerice)
    {
        _producerSerice = producerSerice;
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
            //var elements = viewModel.ElementsCSV.Split(',').Select(int.Parse).ToList();

            var content = JsonConvert.SerializeObject(viewModel);

            var message = new ProducerMessage
            {
                Message = content
            };      

            await _producerSerice.SendMessageAsync(message);

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
