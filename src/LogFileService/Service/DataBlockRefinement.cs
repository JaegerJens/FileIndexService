using LogFileService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogFileService.Service
{
    public class DataBlockRefinement
    {
        private EndOfLine _lastEol;

        public IEnumerable<DataEntry> Refine(DataBlock source)
        {
            if (_lastEol == null)
            {
                _lastEol = new EndOfLine(source);
                _lastEol.RelativePosition = 0;
            }
            foreach(var eol in FindLineEnds(source))
            {
                yield return Build(eol);
                _lastEol = eol;
            }
        }

        private DataEntry Build(EndOfLine end)
        {
            long absoluteStart = _lastEol.AbsolutePosition;
            // simple case: string is in only one block
            if (_lastEol.Block == end.Block)
            {
                return new DataEntry
                {
                    StartPosition = absoluteStart,
                    EndPosition = end.AbsolutePosition,
                    TextLine = GetFromStartToEnd(_lastEol, end)
                };
            }

            // build text from start block to end block
            var buffer = FindLast(end.Block);
            var sb = new StringBuilder();
            foreach (var b in buffer)
            {
                bool IsFirstBlock = (b == _lastEol.Block);
                bool IsLastBlock = (b == end.Block);
                if (IsFirstBlock)
                {
                    sb.Append(GetFromLastUntilEnd(_lastEol));
                    continue;
                }
                if (IsLastBlock)
                {
                    sb.Append(GetFromStart(end));
                    continue;
                }
                sb.Append(b.Data);
            }

            // return all text in StringBuilder
            return new DataEntry
            {
                StartPosition = absoluteStart,
                EndPosition = end.AbsolutePosition,
                TextLine = sb.ToString()
            };
        }

        private string GetFromStart(EndOfLine eol)
        {
            var len = eol.RelativePosition;
            if (eol.IsWindowsEndOfLine)
            {
                len -= 1;
            }
            var text = eol.Block.Data
                .Take(len)
                .ToArray();
            return new string(text);
        }

        private string GetFromLastUntilEnd(EndOfLine start)
        {
            var text = start.Block.Data
                .Skip(start.RelativePosition + 1)
                .ToArray();
            return new string(text);
        }

        private string GetFromStartToEnd(EndOfLine start, EndOfLine end)
        {
            if (start.Block != end.Block)
                throw new ArgumentException("start end end is not in the same block");
            var len = end.RelativePosition;
            if (end.IsWindowsEndOfLine)
            {
                len -= 1;
            }
            var text = start.Block.Data
                .Skip(start.RelativePosition)
                .Take(len)
                .ToArray();
            return new string(text);
        }

        private int GetTotalLength(EndOfLine start, EndOfLine end)
        {
            var total = start.AbsolutePosition - end.AbsolutePosition;
            if (end.IsWindowsEndOfLine)
            {
                total -= 1;
            }
            return (int)total;
        }


        private List<DataBlock> FindLast(DataBlock start)
        {
            var p = start;
            var buffer = new Stack<DataBlock>();
            while(p != _lastEol.Block)
            {
                buffer.Push(p);
                if (p.PreviousBlock == null)
                {
                    return buffer.ToList();
                }
                p = p.PreviousBlock;
            }
            buffer.Push(p);
            return buffer.ToList();
        }

        private IEnumerable<EndOfLine> FindLineEnds(DataBlock block)
        {
            var currentResult = new EndOfLine(block);
            for(int currentPos = 0; currentPos < block.Length; currentPos++)
            {
                if (block.Data[currentPos] == EndOfLine.LineFeed)
                {
                    currentResult.RelativePosition = currentPos;
                    yield return currentResult;
                    currentResult = new EndOfLine(block);
                }
                if (block.Data[currentPos] == EndOfLine.CarriageReturn)
                {
                    currentResult.IsWindowsEndOfLine = true;
                }
            }
            yield break;
        }

        private class EndOfLine
        {
            public const char CarriageReturn = '\r';
            public const char LineFeed = '\n';
            private const int Unknown = -1;

            public EndOfLine(DataBlock b)
            {
                Block = b;
                RelativePosition = Unknown;
                IsWindowsEndOfLine = false;
            }

            public readonly DataBlock Block;
            public int RelativePosition;

            public long AbsolutePosition
            {
                get
                {
                    if (Block == null)
                        return RelativePosition;

                    if (RelativePosition == Unknown)
                        return Unknown;

                    return Block.StartPosition + RelativePosition;
                }
            }
            public bool IsWindowsEndOfLine;
        }
    }
}
