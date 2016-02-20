using LogFileService.Model;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LogFileService.Service
{
    public class BlockReader
    {
        const int BufferSize = 8*1024;

        public IObservable<DataBlock> ReadBlocksFromFile(string filename)
        {
            var obs = Observable.Create<DataBlock>(async (subject, token) =>
            {
                try
                {
                    using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            DataBlock prev = null;
                            while (!reader.EndOfStream)
                            {
                                if (token.IsCancellationRequested)
                                    break;

                                var block = new DataBlock(BufferSize);
                                block.PreviousBlock = prev;
                                block = await ReadBlockAsync(reader, block);
                                subject.OnNext(block);
                                prev = block;
                            }
                            subject.OnCompleted();
                        }
                    }
                } catch (Exception ex)
                {
                    subject.OnError(ex);
                }
            });
            return obs;
        }

        private async Task<DataBlock> ReadBlockAsync(StreamReader reader, DataBlock target)
        {
            var readBytes = await reader.ReadAsync(target.Data, 0, BufferSize);
            if (readBytes != BufferSize)
            {
                target.ResizeData(readBytes);
            }
            return target;
        }
    }
}
