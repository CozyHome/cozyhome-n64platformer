using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;

public class QuickBust : MonoBehaviour
{
    void Start() {
        MonoConsole.InsertCommand(
            "qbust",
            (string[] modifiers, out string output) => {
                output = "================";
            
                int result = 0;
                _ = string.IsNullOrEmpty(modifiers[0]) ? false : int.TryParse(modifiers[0], out result); 

                int totalgames = 0;

                if(result > 0)
                {
                    // int games = 0;
                    var rand = new System.Random();
                    
                    for(int i = 0;i < result;i++) {
                        int bal = 10;
                        while(bal >= 2) {
                            bal -= 2;
                            totalgames++;
                            // ++games;

                            if(rand.NextDouble() < (1D/3D)) {
                                bal += 5;
                            }
                        }
                    }
                    
                    MonoConsole.PrintToScreen($"{totalgames} have been played with simulation count {result}");
                    MonoConsole.PrintToScreen($"expected number of games played: {(double) (totalgames) / (double) result }");
                }
                else
                    MonoConsole.PrintToScreen("error: input for qbust is invalid. Provide a number for sim count!");
            }
        );

        MonoConsole.InsertCommand(
            "qsubsum",
            (string[] modifiers, out string output) => {
                output = "================";

                // ITERATIVE ALGORITHM
                /*

                */
                int[] vals = { 4, 3, 1, 3, 4 };
                int n = vals.Length;
                int t = 11;

                int[][] arr = new int[n + 1][];
                for(int i = 0;i < arr.Length;i++)
                    arr[i] = new int[t + 1];

                // algorithm
                for(int i = 0;i < t;i++)
                    arr[0][i] = 0;
                
                for(int x = 1; x <= n;x++) {
                    for(int y = 0; y <= t;y++) {
                        int ai = vals[x - 1];
                        if(ai <= y && ai + arr[x-1][y - ai] > arr[x - 1][y])
                            arr[x][y] = ai + arr[x-1][y - ai];
                        else
                            arr[x][y] = arr[x - 1][y];
                    }
                }

                MonoConsole.PrintToScreen($"{arr[n][t]}");
            
                string fs = "  ";
                for(int z = 0; z < t + 1;z++) {
                    fs += " " + (z + 1);
                }
                MonoConsole.PrintToScreen(fs);

                for(int z = 0; z < n + 1;z++) {
                    // line by line
                    string line = " " + (z + 1);
                    for(int zz = 0; zz < t + 1;zz++) {
                        line += " " + arr[z][zz];
                    }

                    line = string.Format("{0, 2}",line);

                    MonoConsole.PrintToScreen(line);
                }
            }
        );
    }
}
