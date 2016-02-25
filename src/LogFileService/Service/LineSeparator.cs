using System.Collections.Generic;
using System.Text;

namespace LogFileService.Service
{
    public class LineSeparator
    {
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';

        private StringBuilder _builder;

        public LineSeparator()
        {
            _builder = new StringBuilder();
        }

        public IEnumerable<string> Separate(char inputChar)
        {
            if (inputChar == CarriageReturn)
            {
                yield return _builder.ToString();
                _builder = new StringBuilder();
                yield break;
            }
            if (inputChar == LineFeed)
            {
                yield break;
            }
            _builder.Append(inputChar);
            yield break;
        }
    }
}
