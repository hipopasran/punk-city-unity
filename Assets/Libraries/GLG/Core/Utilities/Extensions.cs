using System.Collections.Generic;
using UnityEngine;

public static class GLGExtensions
{
    public static T GetRandom<T>(this T[] array)
    {
        return array.Length > 0 ? array[Random.Range(0, array.Length)] : default;
    }
    public static T GetRandom<T>(this List<T> array)
    {
        return array.Count > 0 ? array[Random.Range(0, array.Count)] : default;
    }
}
