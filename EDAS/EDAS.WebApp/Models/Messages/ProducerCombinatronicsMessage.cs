namespace EDAS.WebApp.Models.Messages;

public class ProducerCombinatronicsMessage : BaseMessage
{
    public int N { get; set; }

    public int K { get; set; }

    public string ElementsCSV { get; set; }
}