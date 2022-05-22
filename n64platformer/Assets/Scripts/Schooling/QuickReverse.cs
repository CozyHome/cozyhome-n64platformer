using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;

public class QuickReverse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MonoConsole.InsertCommand(
            "qrev",
            (string[] modifiers, out string output) => {
                output = "=============";
                if(string.IsNullOrEmpty(modifiers[0])) {
                    MonoConsole.PrintToScreen("error: qrev() invalid arguments list");
                }
                else {
                    if(int.TryParse(modifiers[0], out int result)) {
                        // int s = result > 0 ? 1 : -1;
                        // result *= s;

                        (int a , int b) sol = rec_reverse(result);
                        MonoConsole.PrintToScreen("input: " + result + " reverse: " + sol.a);
                    }
                    else {
                        MonoConsole.PrintToScreen("error: qrev() could not resolve int");
                    }
                }
            }
        );

        (int, int) rec_reverse(int x) {
            if(x == 0) {
                return (0, 1);
            }
            else {
                int div = x / 10; // 25
                int rem = x % 10; // 3

                (int a, int b) sol = rec_reverse(div);
                // Debug.Log(sol.a + " " + rem + " " + sol.b + " " + rem * sol.b + " " + (sol.a + rem * sol.b));
                int rise = sol.b * rem;
                int sum = sol.a + rise;

                if(sol.a > 0 && rise > 0 && sum < 0 ||
                   sol.a < 0 && rise < 0 && sum > 0 ||
                   rem != 0 && (rem * sol.b) / rem != sol.b) {
                    return (0, 0);
                }
                else {
                    return (sum, sol.b * 10);
                }
            }
        }
    }
}
