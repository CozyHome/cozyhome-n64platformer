using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftArr : MonoBehaviour
{
    void Start() { }

    long odd_occur(int[] A) 
    {
        HashSet<int> box = new HashSet<int>();
        long val = 0;

        for(int i = 0;i < A.Length;i++)
        {
            if(box.Contains(A[i]))
            {
                val -= A[i];
                box.Remove(A[i]);
            }
            else
            {
                val += A[i];

                box.Add(A[i]);
            }
        }

        return val;
    }

    void printarr(int[] a) 
    {
        string o = "";

        foreach(int _val in a)
            o += " " + _val;
     
        Debug.Log(o);
    }

    public int[] shift_arr(int[] A, int K) 
    {
        // write your code in C# 6.0 with .NET 4.5 (Mono)
        
        K %= A.Length;

        Debug.Log("Executions actually made: " + K);

        while(K > 0)
        {
            int i = A.Length - 1;
            int last = A[i];

            for(;i > 0;i--)
            {
                A[i] = A[i - 1];
            }
            
            A[0] = last;
            K--;

        }

        return A;
    }
}
