using System.Collections.Generic;

namespace LogFileService.Service
{
    public class LineSeparator
    {
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';

        private string _buffer;

        public LineSeparator()
        {
            _buffer = null;
        }

        public IEnumerable<string> Separate(char inputChar)
        {
            if (inputChar == CarriageReturn)
            {
                yield return _buffer;
                _buffer = null;
                yield break;
            }
            if (inputChar == LineFeed)
            {
                yield break;
            }
            _buffer = _buffer + inputChar;
            yield break;
        }
    }
}
