using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            BatchBlock<Tuple<string, string>> batchBlock;
            ActionBlock<Tuple<string, string>[]> actionBlock;

            var inputBuffer1 = new BufferBlock<Tuple<string, string>>();
            var inputBuffer2 = new BufferBlock<Tuple<string, string>>();
            var inputBuffer3 = new BufferBlock<Tuple<string, string>>();


            //this batch block will create a batch when when each producer that is linked to it has posted at least 1 message
            batchBlock = new BatchBlock<Tuple<string, string>>(3, new GroupingDataflowBlockOptions { Greedy = false });

            actionBlock = new ActionBlock<Tuple<string, string>[]>(
            data =>
            {
                foreach (var obj in data)
                    Console.WriteLine(obj.Item1 + " : " + obj.Item2);
                Console.WriteLine("----------------------------------");
                System.Threading.Thread.Sleep(1000);
            });

            //Link all of these input buffers to the batch block this should equal the batch size defined in the BatchBlock constructor
            inputBuffer1.LinkTo(batchBlock);
            inputBuffer2.LinkTo(batchBlock);
            inputBuffer3.LinkTo(batchBlock);

            //the output of the batch block will get sent to the uploader blocks
            batchBlock.LinkTo(actionBlock);


            for(int i = 0; i <= 200; i++)
            {
                inputBuffer1.SendAsync(new Tuple<string, string>(String.Format("A test {0}", i), String.Format("A test {0}", i)));
                System.Threading.Thread.Sleep(new Random().Next(0, 1000));
                inputBuffer2.SendAsync(new Tuple<string, string>(String.Format("B test {0}", i), String.Format("B test {0}", i)));
                if(i % 2 == 0)
                    inputBuffer3.SendAsync(new Tuple<string, string>(String.Format("C test {0}", i), String.Format("C test {0}", i)));

                System.Threading.Thread.Sleep(new Random().Next(0, 1000));
                Console.WriteLine("1: {0}, 2: {1}, 3: {2}", inputBuffer1.Count, inputBuffer2.Count, inputBuffer3.Count);
            }

            Console.ReadLine();
        }
    }
}
