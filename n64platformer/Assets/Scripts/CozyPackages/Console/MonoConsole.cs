using UnityEngine;
using com.cozyhome.Singleton;
using System.Collections.Generic;
using System;

namespace com.cozyhome.Console
{
    [DefaultExecutionOrder(-2000)]
    public class MonoConsole : SingletonBehaviour<MonoConsole>
    {
        [Header("Console References")]
        [SerializeField] MonoPrinter Printer;

        private Dictionary<string, ConsoleHeader.Command> Commands;
        private ConsoleHeader.OnConsoleToggled ToggleRelay;

        protected override void OnAwake()
        {
            Commands = new Dictionary<string, ConsoleHeader.Command>();

            Printer.CacheLines();
            Printer.Write("Console initialized at frame: " + Time.frameCount);

            ConsoleHeader.SetDefaults(ref Commands);
        }

        private void AttemptInvokation(string rawinput)
        {
            string[] keys = ConsoleHeader.Parse(rawinput, out int wc);

            // if action is described:
            if (keys.Length > 0)
            {
                // if action exists:
                if (Commands.TryGetValue(keys[0], out ConsoleHeader.Command cmd))
                {
                    int size = wc - 1;
                    if(size <= 0) 
                    {
                        // This means the user didn't input any commands... send a null ref
                        cmd.Invoke(null, out string output);
                        WriteToScreen(output);
                    }
                    else
                    {
                        string[] modifiers = new string[size];

                        // deep copy strings bc C# forces me to
                        for (int i = 0; i < size; i++)
                            modifiers[i] = keys[i + 1];

                        cmd.Invoke(modifiers, out string output);
                        WriteToScreen(output);
                    }
                }
                else
                    WriteToScreen("Error: '" + keys[0] + "' is not a recognized command.");
            }
            else
                WriteToScreen("Error: The input string provided could not be parsed.");
        }

        private void InsertCMD(string key, ConsoleHeader.Command command)
            => Commands?.Add(key.ToLower(), command);

        private void RemoveCMD(string key)
            => Commands?.Remove(key.ToLower());

        private void WriteToScreen(string output)
            => Printer.Write(output);

        public void AppendCommandString(string inputString)
        =>
            Printer.AppendCommandString(inputString);

        public void RemoveCharacterFromString(int amt)
        =>
            Printer.RemoveCharactersFromString(amt);

        public void SubmitLineForParsing()
        {
            string raw = Printer.GetInputLine();
            Printer.ClearInputLine();
            AttemptInvokation(raw);
        }

        public void NotifyConsoleToggled(bool B)
        {
            ToggleRelay?.Invoke(B);
            return;
        }

        public static void AttemptExecution(string rawinput) => _instance?.AttemptInvokation(rawinput);

        public static void PrintToScreen(string output) => _instance?.WriteToScreen(output);

        public static void InsertCommand(string key, ConsoleHeader.Command command)
            => _instance?.InsertCMD(key, command);

        public static void RemoveCommand(string key)
            => _instance?.RemoveCMD(key);

        // add listeners to let end user's know whether the console is active or not.

        public static void InsertToggleListener(ConsoleHeader.OnConsoleToggled listener)
        {
            if (_instance)
                _instance.ToggleRelay += listener;
            else
                return;
        }

        public static void RemoveToggleListener(ConsoleHeader.OnConsoleToggled listener)
        {
            if (_instance)
                _instance.ToggleRelay -= listener;
            else
                return;
        }
    }
}