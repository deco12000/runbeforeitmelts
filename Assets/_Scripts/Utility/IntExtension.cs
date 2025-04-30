using System;
using System.Collections.Generic;

public static class IntExtension
{
    private static List<Tuple<int, string>> units = new List<Tuple<int, string>>()
    {
        new Tuple<int, string>(15, "P"),
        new Tuple<int, string>(12, "T"),
        new Tuple<int, string>(9, "G"),
        new Tuple<int, string>(6, "M"),
        new Tuple<int, string>(3, "K"),
        new Tuple<int, string>(0, "")
    };
    
    public static void ToStringKilo(this int num, out string intPart, out string decPart, out string unitPart)
    {
        int zerocount = num.ToString().Length;
        intPart = "";
        decPart = "";
        unitPart = "";

        if (zerocount < 4)
        {
            intPart = num.ToString("N0");
            return;
        }

        for (int i = 0; i < units.Count; i++)
        {
            if (zerocount > units[i].Item1)
            {
                double result = (double)num / Math.Pow(10, units[i].Item1);
                double truncatedResult = Math.Truncate(result * 100) / 100;
                intPart = ((int)truncatedResult).ToString();
                decPart = (truncatedResult - Math.Truncate(truncatedResult)).ToString("F2").Substring(1);
                unitPart = units[i].Item2;
                return;
            }
        }
    }
}