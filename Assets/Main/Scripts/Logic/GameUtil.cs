
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
        List<ValueTuple<string, ValueTuple<int, int>>> chessNameList)
    {
        List<ValueTuple<FactoryType, ValueTuple<int, int>>> chessList = new List<ValueTuple<FactoryType, ValueTuple<int, int>>>();
        foreach (var chessNameInfo in chessNameList)
        {
            chessList.Add((ConvertStringToFactory(chessNameInfo.Item1), chessNameInfo.Item2));
        }

        return chessList;
    }
    
    public static List<ValueTuple<string, ValueTuple<int, int>>> ConvertChessList(
        List<ValueTuple<FactoryType, ValueTuple<int, int>>> chessNameList)
    {
        List<ValueTuple<string, ValueTuple<int, int>>> chessList = new List<ValueTuple<string, ValueTuple<int, int>>>();
        foreach (var chessNameInfo in chessNameList)
        {
            chessList.Add((ConvertFactoryToString(chessNameInfo.Item1), chessNameInfo.Item2));
        }
        return chessList;
    }
}