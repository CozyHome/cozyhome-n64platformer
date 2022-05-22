using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;

public class QuickStr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MonoConsole.InsertCommand("qstr", (string[] modifiers, out string output) => {
            output = "==================";
        
            MonoConsole.PrintToScreen("exec qstr()");
            string st1 = "aabaaabaaacac";
            string st2 = "aabaaac";

            int idx = StrStr(st1, st2);
            MonoConsole.PrintToScreen("id: " + idx);
        });
    }

    public int StrStr(string haystack, string needle) {
        // since the question asks for the lowest possible index, we minimize lja instead
        // of simply returning it when it's immediately detected. -DC @ jan 4th 2021

        // base case #1: needle string is empty
        if(needle.Length == 0) {
            return 0;
        }
        // base case #2: haystack string is empty
        else if(haystack.Length == 0) {
            return -1;
        }
        // base case #3: haystack is smaller than needle
        else if(haystack.Length < needle.Length)
            return -1;
        // begin while
        else {
            // this algorithm uses a dual scheme. 
            // it uses a forward march in combination with a backward march
            // algorithm that simply sweeps in linear time to detect needles in both directions

            int i   = 0, j  = 0; // local indices of needle
            int i0  = 0, j0 = haystack.Length - 1; // offsets
            int lja = int.MaxValue; // minimal index found via leftward sweep
            
            while(i0 < lja && i0 < haystack.Length && j0 >= 0) {
                // if we are in rightward bounds of haystack
                if(i0 + i < haystack.Length) {
                    if(haystack[i0 + i] == needle[i])
                        i++;
                    else {
                        i0 += (i > 1) ? i - 1 : 1;
                        i = 0;
                    }
                } 
                // i would return -1 here but it would skip j's logic. 
                // Maybe try to figure out a cheap way to do so without ignoring logic
                
                // if we are in leftward bounds of haystack
                if(j0 - j >= 0) {
                    if(haystack[j0 - j] == needle[needle.Length - j - 1])
                        j++;
                    else {
                        j0 -= (j > 1) ? j - 1 : 1;
                        j = 0;
                    }
                }
                else // outside of bounds means needle can't fit in remaining string
                    return -1;

                // if rightward march detects string, this is the first of their kind. return it.
                if(i == needle.Length) {
                    return i0;
                }
                // however, if this was not discovered, let's check along our leftward march instead
                else if(j == needle.Length) {
                    // minimize lja
                    if(j0 - j + 1 < lja)
                        lja = j0 - j + 1;

                    // march left, reset offset.
                    j0 -= 1;
                    j = 0;
                }
            }

            // if lja was not minimized, then nothing was discovered. If it was, then return it.
            return lja != int.MaxValue ? lja : -1;
        }
    }
}
