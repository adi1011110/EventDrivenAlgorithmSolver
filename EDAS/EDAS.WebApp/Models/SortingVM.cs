using System.ComponentModel.DataAnnotations;

namespace EDAS.WebApp.Models;

public class SortingVM
{
    [Display(Name = "Comma separated integers")]
    public string ElementsCSV { get; set; }
}
