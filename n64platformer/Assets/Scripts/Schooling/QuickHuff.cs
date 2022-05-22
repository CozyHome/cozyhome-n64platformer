using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;

public class QuickHuff : MonoBehaviour
{   
    struct kvp {
        public char key;
        public int num;
    }

    // Start is called before the first frame update
    void Start()
    {

        MonoConsole.InsertCommand(
            "qfreq",
            (string[] modifiers, out string output) => {
                
                if(!string.IsNullOrEmpty(modifiers[0]))
                {
                    Dictionary<char, int> set = new Dictionary<char, int>();
                    for(int i = 0;i < modifiers[0].Length;i++){
                        if(set.ContainsKey(modifiers[0][i]))
                            set[modifiers[0][i]] += 1;
                        else
                            set.Add(modifiers[0][i], 1);
                    }

                    List<kvp> entries = new List<kvp>();
                
                    var en = set.Keys.GetEnumerator();
                    while(en.MoveNext())
                    {
                        kvp k; k.key = en.Current;
                        k.num = set[en.Current];
                        entries.Add(k);
                    
                    }

                    entries.Sort( (kvp x, kvp y) => { return y.num.CompareTo(x.num); } );
                    entries.ForEach( (kvp a) => MonoConsole.PrintToScreen($"{a.key} = {a.num}"));

                    en.Dispose();
                }
                else
                {
                    MonoConsole.PrintToScreen("error: no input string provided for qfreq");
                }

                output = "==============";
            }
        );  
    }

}
