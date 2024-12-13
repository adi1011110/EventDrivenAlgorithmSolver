using System.ComponentModel.DataAnnotations;

namespace EDAS.WebApp.Models;

public class CombinatronicsVM
{
    [Range(1, 20, ErrorMessage = "Enter an integer between 1 and 20")]
    public int N {  get; set; }
    [Range(1, 20, ErrorMessage = "Enter an integer between 1 and 20")]
    public int K { get; set; }
    [Display(Name = "Comma separated integers")]
    public string ElementsCSV {  get; set; }
}
