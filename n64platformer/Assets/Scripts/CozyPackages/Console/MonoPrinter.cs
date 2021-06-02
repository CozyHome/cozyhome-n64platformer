using System;
using UnityEngine;

namespace com.cozyhome.Console
{
    [System.Serializable]
    public class MonoPrinter
    {
        [Header("Required References")]
        [SerializeField] private GameObject ConsolePanel;
        [System.NonSerialized] private UnityEngine.UI.Text[] Lines;

        public void CacheLines()
        {
            // attempt to grab lines
            if (ConsolePanel != null)
            {
                UnityEngine.UI.Text[] rl = ConsolePanel.GetComponentsInChildren<UnityEngine.UI.Text>();
                Lines = rl;
            }
        }

        public void Write(string output)
        {
            // bubble up
            for (int i = 0; i < Lines.Length - 2; i++)
                Lines[i].text = Lines[i + 1].text;
            // start at size - 2
            Lines[Lines.Length - 2].text = "> " + output;
        }

        public void AppendCommandString(string inputString)
        =>
            Lines[Lines.Length - 1].text += inputString;

        public void RemoveCharactersFromString(int amt)
        {
            string str = Lines[Lines.Length - 1].text;
            int ni = str.Length - (amt + 1);
            if (ni <= 1)
                return;
            else
                Lines[Lines.Length - 1].text = str.Substring(0, ni);
        }

        public string GetInputLine()
        {
            return Lines[Lines.Length - 1].text.Substring(2);
        }

        public void ClearInputLine()
        {
            Lines[Lines.Length - 1].text = ">:";
        }
    }
}

