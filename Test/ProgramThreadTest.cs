using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{


    class ProgramThreadTest
    {
        static void Main(string[] args)
        {
            //RunableTest test = new RunableTest();

            Thread th1 = new Thread(ProgramThreadTest.Run);
            Thread th2 = new Thread(ProgramThreadTest.Run);
            Thread th3 = new Thread(ProgramThreadTest.Run);
            th1.Start();
            th2.Start();
            th3.Start();
        }

        private static int tick = 60;

        public static object _lock = new object();
        public static void Run()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (tick == 0)
                        break;

                    if (tick == 10)
                    {
                        int a1 = 0;
                        int a = 100 / a1;
                    }

                    System.Threading.Thread.Sleep(10);
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ":" + tick--);
                }

            }
        }
    }
}
