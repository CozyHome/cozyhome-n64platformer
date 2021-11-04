using System.IO;
using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;

public class JSON_DEBUG : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // MonoConsole.InsertCommand(
        //     "json_test",
        //     (string[] modifiers, out string output) => 
        //     {
        //         output = "========================";

        //     }
        // );
        
        string file_path = Application.streamingAssetsPath + "/json_test.json";

        try
        {
            using(StreamReader sr = new StreamReader(file_path)) 
            {
                string json = sr.ReadToEnd();
                JSON_OBJ obj = JsonUtility.FromJson<JSON_OBJ>(json);
                MonoConsole.PrintToScreen("Name: " + obj.name);
            }
        }
        catch(IOException e)
        {
            Debug.Log(e.StackTrace);
        }

    }

}

[System.Serializable]
public class JSON_OBJ 
{
    public string name;
}
