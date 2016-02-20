using System;

namespace LogFileService.Model
{
    public class DataBlock
    {
        public DataBlock(int size)
        {
            Data = new char[size];
        }

        public char[] Data;

        public long StartPosition
        {
            get
            {
                if (PreviousBlock == null)
                    return 0;
                return PreviousBlock.EndPosition + 1;
            }
        }

        public int Length => Data.Length;

        public long EndPosition => StartPosition + Length;

        public void ResizeData(int newLength)
        {
            Array.Resize(ref Data, newLength);
        }

        public DataBlock PreviousBlock { get; set; }
    }
}
