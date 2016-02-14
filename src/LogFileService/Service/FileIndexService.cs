using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogFileService.Service
{
    public class FileIndexService
    {
        public IObservable<string> ReadLines(string filename)
        {
            return Observable.FromAsync(ct => ReadAsyncLines(filename, ct));
        }

        private async Task<string> ReadAsyncLines(string filename, CancellationToken t)
        {
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadLineAsync();
                }
            }
        }

        public IObservable<long> FindEndOfLines(string filename, CancellationToken t)
        {
            var data = ReadFileBlockWise(filename, t)
                .SelectMany(b => FindAll(b, '\n', t));
            return data.ToObservable();
        }

        private IEnumerable<Block> ReadFileBlockWise(string filename, CancellationToken t)
        {
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                const int bufferLen = 0x10000;
                long pos = 0;
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        if (t.IsCancellationRequested)
                        {
                            yield break;
                        }
                        var buffer = new char[bufferLen];
                        var readByteCount = reader.Read(buffer, 0, bufferLen);
                        var b = new Block
                        {
                            Data = buffer,
                            StartPosition = pos,
                            Length = readByteCount
                        };
                        yield return b;
                        pos += readByteCount;
                    }
                }
            }
        }

        private struct Block
        {
            public char[] Data;
            public long StartPosition;
            public int Length;
        }

        private IEnumerable<long> FindAll(Block block, char search, CancellationToken t)
        {
            for(int i = 0; i< block.Length; i++)
            {
                if (t.IsCancellationRequested)
                {
                    yield break;
                }
                if (block.Data[i] == search)
                {
                    yield return i + block.StartPosition;
                }
            }
            yield break;
        }
    }
}
