namespace EDAS.Common.Models;

public class SortingInputModel : BaseCSVElementsInputModel
{
    public SortingInputModel()
    {
        Subject = "Sorting solution";
    }

    public override string ToString()
    {
        return ElementsCSV;
    }
}
