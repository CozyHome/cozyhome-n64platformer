using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;

public class QuickFib : MonoBehaviour
{
    void Start() 
    {
        MonoConsole.InsertCommand(
            "fib",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result = 0;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result) : false;

                if(result != 0)
                    MonoConsole.PrintToScreen($"computing fib({result}) = {fibLin(result)}");
                else
                    MonoConsole.PrintToScreen("error: no valid int provided for fib!");
            }
        );

        MonoConsole.InsertCommand(
            "gfib",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result = -1;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result) : false;

                if(result != -1)
                    MonoConsole.PrintToScreen($"computing gfib({result}) = {gfib(12, 34, result)}");
                else
                    MonoConsole.PrintToScreen("error: no valid int provided for gfib!");
            }
        );

        MonoConsole.InsertCommand(
            "linfib",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result = -1;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result) : false;

                if(result != -1)
                    MonoConsole.PrintToScreen($"computing linfib({result}) = {linfib(result)}");
                else
                    MonoConsole.PrintToScreen("error: no valid int provided for fib!");
            }
        );

        MonoConsole.InsertCommand(
            "cplx",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result = 0;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result) : false;

                if(result != 0)
                    MonoConsole.PrintToScreen($"computing time_complex({result}) = {time_complex(result)}");
                else
                    MonoConsole.PrintToScreen("error: no valid int provided for fib!");
            }
        );   
    }

    int time_complex(int n)
    {
        if(n <= 1)
            return 1;
        else
            return 1 + time_complex(n - 1) + time_complex(n - 2);
    }

    int linfib(int n) 
    {
        if(n == 0)
            return 0;
        if(n == 1)
            return 1;

        (int a, int b) tuple = (1,1);

        for(int i = 2;i < n;i++)
        {
            int _next = tuple.a + tuple.b;
            tuple.b = tuple.a;
            tuple.a = _next;
        }

        return tuple.a;
    }

    int glinfib(int g0, int g1, int n) 
    {
        if(n == 0)
            return g0;
        if(n == 1)
            return g1;

        (int a, int b) tuple = (g0, g1);

        for(int i = 1;i < n;i++)
        {
            int _next = tuple.a + tuple.b;
            tuple.b = tuple.a;
            tuple.a = _next;
        }

        return tuple.a;
    }

    int quickfib(int n) => recfib(n - 1).a;

    (int a, int b) recfib(int i) 
    {
        if(i <= 1)
            return (1, 1);
        else 
        {
            if((i & 0x1) == 0) // even
            {
                (int a, int b) res1 = recfib(i / 2);
                int d = res1.a - res1.b;
                return (
                    a: (res1.a * res1.a) + (res1.b * res1.b),
                    b: (res1.b * res1.a) + (d * res1.b)
                );
            }
            else // odd
            {
                (int a, int b) res1 = recfib((i / 2) + 1); // even
                (int a, int b) res2 = recfib(i / 2); // odd
                int d = res1.a - res1.b;

                return (
                    a: (res1.a * res2.a) + (res1.b * res2.b),
                    b: (res1.b * res2.a) + (d * res2.b)
                );
            }
        }
    }

    
    int gfib(int a, int b, int n)
    {
        int g0 = a;
        int g1 = b;

        if(n == 0)
            return g0;
        if(n == 1)
            return g1;

        // formula : g(n) = f(n+1)(b) + f(n)(a)
        int gB = linfib(n) * b;
        int gA = linfib(n - 1) * a;

        // the way it is defined
        (int current, int previous) tuple = (g1, g0);

        if(n == 0)
            tuple.current = g0;
        if(n == 1)
            tuple.current = g1;

        for(int i = 2;i <= n;i++)
        {
            int oldcur     = tuple.current;
            tuple.current  = tuple.current + tuple.previous;
            tuple.previous = oldcur;
        }

        MonoConsole.PrintToScreen($"other procedure produced: {tuple.current}");

        int g = gA + gB;

        return g;
    }

    
    int fibLin(int n)
    {
        return fibPair(n - 1)[1];        
    }

    int[] fibPair(int n)
    {
        if(n == 1)
            return new int[] { 1, 1 };
        else
        {
            int[] pair = fibPair(n - 1);
            int old = pair[1]; // fn- 1
            pair[1] = pair[0] + pair[1];
            pair[0] = old;

            return pair;
        }
    }
}
