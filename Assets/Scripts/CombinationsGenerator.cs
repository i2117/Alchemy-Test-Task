using System.Collections.Generic;
using UnityEngine;

public class CombinationsGenerator: MonoBehaviour
{
    // Сочетания без повторений
    public static List<int[]> GetAllCombinations(int k, int n)
    {
        var result = new List<int[]>();
        var buf = new int[k];

        Recursive(0, 0);

        void Recursive(int ind, int begin)
        {
            for (int i = begin; i < n; i++)
            {
                buf[ind] = i;
                if (ind + 1 < k)
                    Recursive(ind + 1, buf[ind] + 1);
                else
                {
                    result.Add(buf);
                    //Debug.Log(string.Join(",", buf));
                }
            }
        }

        return result;
    }

    // Сочетания с повторениями
    public static List<int[]> GetAllCombinationsWithRepeats(int length, int maxNumber)
    {
        var maxPos = length - 1;

        var result = new List<int[]>();
        var buf = new int[maxPos+1];

        rec(0, 0);

        void rec(int sPos, int sDig)
        {
            for (int i = sDig; i <= maxNumber; i++)
            {
                for (int j = sPos; j <= maxPos; j++)
                    buf[j] = i;

                if (sPos == maxPos)
                {
                    // Filling the result list with array
                    int[] newArr = (int[])buf.Clone();
                    result.Add(newArr);
                    //Debug.Log(string.Join(",", buf));
                }

                if (sPos < maxPos)
                    rec(sPos + 1, i);
            }
        }

        return result;
    }

}
