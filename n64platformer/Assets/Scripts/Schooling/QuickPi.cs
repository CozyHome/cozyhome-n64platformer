using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;

public class QuickPi : MonoBehaviour
{
    // approximate pi via the following pythagorean relation: (a + bi)
    // b = sqrt(1 - x^2) / i; we'll ignore i for now and focus on the positive real and positive ith plane
    void Start() {
        MonoConsole.InsertCommand(
            "quickpi",
            (string[] modifiers, out string output) => {
                output = "=================";
                if(string.IsNullOrEmpty(modifiers[0])) {
                    MonoConsole.PrintToScreen("error: quickpi() no iteration amount declared");
                }
                else {
                    if(float.TryParse(modifiers[0], out float result)) {
                        
                    }
                }
            }
        );
    }
}
