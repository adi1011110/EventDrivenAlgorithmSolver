using System.Text;

namespace EDAS.Common.Models;

public class CombinationsInputModel : BaseCSVElementsInputModel
{
    public int N { get; set; }
    public int K { get; set; }
    

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"N = {N}");
        sb.AppendLine($"K = {K}");
        sb.AppendLine($"Elements = {ElementsCSV}");

        return sb.ToString();
    }
}
