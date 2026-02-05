using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumUtils
{
    public static T GetRandomEnumValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        return values[UnityEngine.Random.Range(0, values.Length)];
    }

    public static T GetRandomEnumFromList<T>(List<T> values) where T : Enum
    {
        return values[UnityEngine.Random.Range(0, values.Count)];
    }

    public static List<T> TakeRandomFromEnumValues<T>(int n) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        return values.Take(n).ToList();
    }
}