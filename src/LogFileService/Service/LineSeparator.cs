using System.Collections.Generic;
using System.Linq;

namespace LogFileService.Service
{
    public class LineSeparator
    {
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';

        public IEnumerable<string> SeparateLines(IEnumerable<char[]> input)
        {
            return SeparateLines(input.Select(d => new string(d)));
        }

        public IEnumerable<string> SeparateLines(IEnumerable<string> input)
        {
            string resume = null;
            foreach(var currentInput in input)
            {
                // work only with variable inp
                var inp = currentInput;

                // if resume has left overs from last line
                // then concat inp with resume
                if (resume != null)
                {
                    inp = resume + inp;
                    resume = null;
                }

                while(inp.Length > 0)
                {
                    // find end of line
                    var pos = inp.IndexOf(LineFeed);

                    // if current inp has no end of line
                    // then remember left over in resume
                    // and continue with next line
                    if (pos == -1)
                    {
                        resume = inp;
                        inp = string.Empty;
                        continue;
                    }

                    // find windows end of line: \r\n
                    bool hasWindowsEndOfLine = false;
                    if (pos > 0)
                    {
                        hasWindowsEndOfLine = (inp[pos - 1] == CarriageReturn);
                    }

                    // yield return everything until end of line
                    if (hasWindowsEndOfLine)
                    {
                        yield return inp.Substring(0, pos - 1);
                    }
                    else
                    {
                        yield return inp.Substring(0, pos);
                    }
                    inp = inp.Substring(pos + 1);
                }
            }
        }
    }
}
