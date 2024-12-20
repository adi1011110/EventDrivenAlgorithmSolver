using System.ComponentModel.DataAnnotations;

namespace EDAS.WebApp.Models;

public class ProducerMessage
{
    public string EmailAddress {  get; set; }

    public int N { get; set; }

    public int K { get; set; }

    public string ElementsCSV { get; set; }
}
