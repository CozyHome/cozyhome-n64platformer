using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;


public class QuickEuclid : MonoBehaviour
{
    void Start() 
    {

        MonoConsole.InsertCommand(
            "morder",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result_A = 0;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result_A) : false;
                
                int result_B = 0;
                _ =  modifiers.Length > 1 && !string.IsNullOrEmpty(modifiers[1]) ? int.TryParse(modifiers[1], out result_B) : false;
                
                if(result_A != 0 && result_B != 0) 
                { 
                    MonoConsole.PrintToScreen($"computing mod({result_A}, {result_B})");
                    int rem = mod(result_A, result_B);
                    MonoConsole.PrintToScreen($"rem: {rem}");
                }
            });

        MonoConsole.InsertCommand(
            "mod",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result_A = 0;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result_A) : false;
                
                int result_B = 0;
                _ =  modifiers.Length > 1 && !string.IsNullOrEmpty(modifiers[1]) ? int.TryParse(modifiers[1], out result_B) : false;
                
                if(result_A != 0 && result_B != 0) 
                { 
                    MonoConsole.PrintToScreen($"computing mod({result_A}, {result_B})");
                    int rem = mod(result_A, result_B);
                    MonoConsole.PrintToScreen($"rem: {rem}");
                }
            });

        MonoConsole.InsertCommand(
            "lincon",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result_A = 0;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result_A) : false;
                
                int result_B = 0;
                _ =  modifiers.Length > 1 && !string.IsNullOrEmpty(modifiers[1]) ? int.TryParse(modifiers[1], out result_B) : false;
                
                int result_N = 0;
                _ =  modifiers.Length >= 2 && !string.IsNullOrEmpty(modifiers[2]) ? int.TryParse(modifiers[2], out result_N) : false;
            
                if(result_A != 0 && result_B != 0 && result_N != 0) 
                { 
                    MonoConsole.PrintToScreen($"computing lincon({result_A}, {result_B}, {result_N})");
                    (int z, int s, int d) tuple = lincon(result_A, result_B, result_N);
                    MonoConsole.PrintToScreen($"z: {tuple.z} step` = {tuple.s} other sols = {tuple.d}");
                }
            });

        MonoConsole.InsertCommand(
            "det_rinvert",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result_N = 0;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result_N) : false;
                
                if(result_N != 0) 
                { 
                    int cardinality = 0;

                    MonoConsole.PrintToScreen($"computing det_rinvert({result_N})");
                    for(int i = 0;i < result_N;i++)
                    {
                        (int z, int s, int d) tuple = lincon(i, 1, result_N);
                        MonoConsole.PrintToScreen($"i` = {i} z` = {tuple.z} " /*s` = {tuple.s} d` = {tuple.d}" */);
                        cardinality = tuple.z != 0 ? cardinality + 1 : cardinality;
                    }

                    MonoConsole.PrintToScreen($"cardinality of Z_*n = {cardinality}");
                }
            });

        MonoConsole.InsertCommand(
            "exeu",
            (string[] modifiers, out string output) => 
            {
                output = "==================";
                
                int result_A = 0;
                _ = !string.IsNullOrEmpty(modifiers[0]) ? int.TryParse(modifiers[0], out result_A) : false;
                int result_B = 0;
                _ = modifiers.Length > 1 && !string.IsNullOrEmpty(modifiers[1]) ? int.TryParse(modifiers[1], out result_B) : false;
                
                if(result_A != 0 && result_B != 0)
                {
                    MonoConsole.PrintToScreen($"computing exteuclid({result_A}, {result_B}");
                    int gcd = top_gcdExtended(result_A, result_B, out int x, out int y);
                    MonoConsole.PrintToScreen($"results: gcd({result_A},{result_B}) := {gcd} x := {x} y := {y} h := {height}");
                }
                else
                    MonoConsole.PrintToScreen("error: incorrect inputs provided for gcd!");
            }
        );
    }

    (int, int, int) lincon(int a, int b, int n)
    {
        (int d, int s, int t) ftuple;
        ftuple.d = top_gcdExtended(a, n, out ftuple.t, out ftuple.s);
    
        if(ftuple.d == 1)
            return (mod(ftuple.t * b, n), 0, 0);
        else if(ftuple.d > 1)
        {
            if(mod(b, ftuple.d) != 0)
            {
                // MonoConsole.PrintToScreen($"D : {ftuple.d} isn't a factor of B : {b}!?");
                return (0,0,0);
            }
            else 
            {
                (int aa, int bb, int nn) level = (a / ftuple.d, b / ftuple.d, n / ftuple.d);
                (int d, int s, int t) stuple;

                MonoConsole.PrintToScreen($"a` = {level.aa} n` = {level.nn} ");
                stuple.d = top_gcdExtended(level.aa, level.nn, out stuple.t, out stuple.s);
                MonoConsole.PrintToScreen($"d` = {stuple.d} s` = {stuple.s} t` = {stuple.t}");

                return (mod(stuple.t * level.bb, level.nn), level.nn, ftuple.d - 1);
            }
        }
        else
            return (0, 0, 0);
    }

    // Taken from: https://www.geeksforgeeks.org/euclidean-algorithms-basic-and-extended/
    // extended Euclidean Algorithm
    int top_gcdExtended(int a, int b, out int x, out int y)
    {
        height = 0;
        // Base Case
        if (a == 0)
        {
            x = 0;
            y = 1;
            return b;
        }
  
        // To store results of
        // recursive call
        int x1 = 1, y1 = 1; 
        int gcd = rec_gcdExtended(b % a, a, ref x1, ref y1);
  
        // Update x and y using 
        // results of recursive call
        x = y1 - (b / a) * x1;
        y = x1;
  
        return gcd;
    }

    int height = 0;

    int rec_gcdExtended(int a, int b, ref int x, ref int y)
    {
        height++;
        // Base Case
        if (a == 0)
        {
            x = 0;
            y = 1;
            return b;
        }
  
        // To store results of
        // recursive call
        int x1 = 1, y1 = 1; 
        int gcd = rec_gcdExtended(b % a, a, ref x1, ref y1);
  
        // Update x and y using 
        // results of recursive call
        x = y1 - (b / a) * x1;
        y = x1;


        return gcd;
    }

    int mod(int x, int m) 
    {
        return (x % m + m) % m;
    }

}
