namespace EDAS.Worker.Utils;

public static class QueueHelper
{
    public static QueueType ConvertStringToEnum(string enumString)
    {
        var algoTypeLower = enumString.ToLower();

        var algoType = algoTypeLower.Substring(0, 1).ToUpper() + algoTypeLower.Substring(1);

        Enum.TryParse<QueueType>(algoType, true, out QueueType queueType);

        return queueType;
    }
}
