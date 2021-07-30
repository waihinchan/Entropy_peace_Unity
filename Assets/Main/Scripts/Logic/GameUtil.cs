
using System;
using System.Collections.Generic;

public class GameUtil
{
    private static Dictionary<string, FactoryType> StrToTypeDict; 
    private static Dictionary<FactoryType, string> TypeToStrDict; 

    static GameUtil()
    {
        StrToTypeDict = new Dictionary<string, FactoryType>();
        TypeToStrDict = new Dictionary<FactoryType, string>();
    }

    public static void RegisterType(string typeName, FactoryType type)
    {
        StrToTypeDict.Add(typeName, type);
        TypeToStrDict.Add(type, typeName);
    }
    
    public static string ConvertFactoryToString(FactoryType factoryType)
    {
        string typeName = "";
        bool ok = TypeToStrDict.TryGetValue(factoryType, out typeName);
        return typeName;
    }

    public static FactoryType ConvertStringToFactory(string factoryTypeName)
    {
        FactoryType type;
        bool ok = StrToTypeDict.TryGetValue(factoryTypeName, out type);
        return type;
    }

    public static List<ValueTuple<FactoryType, ValueTuple<int, int>>> ConvertChessList(
        List<ChessInfo> chessInfoList)
    {
        List<ValueTuple<FactoryType, ValueTuple<int, int>>> chessList = new List<ValueTuple<FactoryType, ValueTuple<int, int>>>();
        foreach (var chessInfo in chessInfoList)
        {
            chessList.Add((ConvertStringToFactory(chessInfo.typeName), (chessInfo.x,chessInfo.y)));
        }

        return chessList;
    }
    
    public static List<ChessInfo> ConvertChessList(
        List<ValueTuple<FactoryType, ValueTuple<int, int>>> chessNameList)
    {
        List<ChessInfo> chessList = new List<ChessInfo>();
        foreach (var chessNameInfo in chessNameList)
        {
            chessList.Add(new ChessInfo()
            {
                typeName = chessNameInfo.Item1.Name,
                x = chessNameInfo.Item2.Item1,
                y = chessNameInfo.Item2.Item2,
            });
        }
        return chessList;
    }
}