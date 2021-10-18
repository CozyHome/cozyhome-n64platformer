using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
namespace com.cozyhome.Console
{
    public static class ConsoleHeader
    {
        const int NULLQUEUE = 8192;

        public delegate void Command(string[] modifiers, out string output);
        public delegate void OnConsoleToggled(bool B);

        // this is probably the most scuffed algorithm I have ever written.
        public static string[] Parse(string rawinput, out int wc)
        {
            // find action and subsequent modifiers
            const int MAXKEYS = 12;
            // I hate this but a potential fix is to 
            // regularly call GC.Collect() during our update timeline
            // to prevent spikes...

            // alongside that, maybe make a command that allows for GC.Collect to be ran
            // if it hasn't been manullay been ran for some time ? 

            // stack of quote identifiers to know if to ignore splitting
            
            // DISGUSTING. WRAP THIS IN A OBJECT THAT IS PASSED INTO THE FUNCTION YOUY FUCK -DC 8/29/2021
            Queue<int> quotequeue = new Queue<int>(); // quote stack
            Queue<int> charqueue = new Queue<int>();
            char[] txt = rawinput.ToCharArray();
            wc = 0; // word count

            string[] tmpbuffer = new string[MAXKEYS];
            tmpbuffer[0] = "";

            // first pass: determine quote attributes
            for (int i = 0; i < txt.Length; i++)
                if (txt[i] == '"')
                    quotequeue.Enqueue(i);
                else
                {
                    if (txt[i] == ' ') // ignore whitespace
                        continue;

                    // our cstack will work like this
                    // odd values will represent starting indices
                    // even values will represent ending indices

                    // how to determine end of signature?
                    // well...
                    // odd values will start AFTER a whitespace.
                    // even values will start BEFORE a whitespace.

                    // as well as this, if we're currently determing an even
                    // index, we automatically know to determine an odd index for next iteration

                    // starting index will begin first:

                    bool isInQuote = quotequeue.Count % 2 == 1;

                    bool isStartingIndex = (charqueue.Count & 0x0001) == 0;

                    if (isInQuote)
                        continue;
                    else
                    {
                        if (isStartingIndex)
                        {
                            if (i == 0 || // if is beginning OR
                               txt[i - 1] == ' ' || // prev is whitespace OR
                               txt[i - 1] == '"') // prev is quote
                                {
                                    charqueue.Enqueue(i);
                                    
                                    // alright, so we need to handle the case
                                    // where a token is one character long. This will
                                    // be how we do it:
                                    // DC 9/06/2021 @ 12:13 AM
                                    if(i + 1 >= txt.Length - 1 ||
                                        txt[i + 1] == ' ' ||
                                        txt[i + 1] == '"')
                                    {
                                        charqueue.Enqueue(i + 1);   
                                    }
                                }

                        }
                        else // if not starting index, then we must be an ending index 
                        {
                            if (i + 1 >= txt.Length - 1 ||
                                txt[i + 1] == ' ' ||
                                txt[i + 1] == '"')
                                charqueue.Enqueue(i + 1);
                        }
                    }
                }

            for (; wc < MAXKEYS; wc++)
            {
                int cindex = NULLQUEUE;
                int qindex = NULLQUEUE;

                if (charqueue.Count <= 1 && quotequeue.Count <= 1)
                    break;

                if (charqueue.Count > 0)
                    cindex = charqueue.Peek();

                if (quotequeue.Count > 0)
                    qindex = quotequeue.Peek();

                int delta = qindex - cindex;

                if (qindex - cindex > 0) // if quotequeue is ahead in index count, char queue goes first.
                {
                    if (charqueue.Count > 1 &&
                        cindex != NULLQUEUE)
                    {
                        // pop the endpoint indicies from the queue and read them
                        // into the substring function to determine the desired
                        // string
                        int c0 = charqueue.Dequeue();
                        int c1 = charqueue.Dequeue();

                        // assign it to the word at index wc.
                        tmpbuffer[wc] = rawinput.Substring(c0, c1 - c0);
                    }
                }
                else
                {
                    // doing the same thing as before but instead we're also including
                    // the quotes in the output
                    if (quotequeue.Count > 1 &&
                        qindex != NULLQUEUE)
                    {
                        int c0 = quotequeue.Dequeue();
                        int c1 = quotequeue.Dequeue();
                        tmpbuffer[wc] = rawinput.Substring(c0, c1 - c0 + 1);
                    }
                }
            }

            return tmpbuffer;
        }

        public static bool TryParseSingle(string input, out float parsed)
        {
            return float.TryParse(input, out parsed);
        }

        public static void SetDefaults(ref Dictionary<string, Command> commands)
        {
            commands.Add("print", Print);
        }

        public static void Print(string[] parameters, out string output)
        {
            output = "";
            for (int i = 0; i < parameters.Length; i++)
                output += (parameters[i]) + " ";
       
        }
    }
}
