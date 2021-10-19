using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Text;

namespace StringManipulations
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Password123! => Pas*********

#if DEBUG
            var test = new ToBenchMark();          
           
            Console.WriteLine(test.StringExtensionUsage());
            Console.WriteLine(test.ManipulateArray());
            Console.WriteLine(test.ManipulateSpan());
            Console.WriteLine(test.StringCreationCallBackSpanWithRangeOperator());
            Console.WriteLine(test.StringCreationCallBackSpanWithLoop());
            Console.WriteLine(test.StringBuilderWithLoop());
            Console.WriteLine(test.StringBuilderWithRepeat());
            Console.WriteLine(test.UnsafeManipulateString());

            Environment.Exit(0);
#endif
            BenchmarkRunner.Run<ToBenchMark>();
        }
    }

    [MemoryDiagnoser]
    public class ToBenchMark
    {

        public string Password = "Password123!";
        public ToBenchMark()
        {
            Password = File.ReadAllText("test.txt");
        }

        [Benchmark]
        public string StringExtensionUsage()
        {
            return Password.Substring(0, 3).PadRight(Password.Length, '*');
        }

        [Benchmark]
        public string ManipulateArray()
        {
            char[] e = Password.ToCharArray();

            for (int i = 3; i < Password.Length; i++)
            {
                e[i] = '*';
            }

            return new string(e);
        }

        [Benchmark]
        public string ManipulateSpan()
        {
            var a = new Span<char>(Password.ToCharArray());

            for (int i = 3; i < Password.Length; i++)
            {
                a[i] = '*';
            }

            return a.ToString();
        }

        [Benchmark]
        public string StringCreationCallBackSpanWithRangeOperator()
        {
            return string.Create(Password.Length, Password, (c, s) =>
            {
                s.AsSpan().CopyTo(c);
                c[3..].Fill('*');
            });
        }

        [Benchmark]
        public string StringCreationCallBackSpanWithLoop()
        {
            return string.Create(Password.Length, Password, (c, s) =>
            {
                s.AsSpan().CopyTo(c);

                for (int i = 3; i < Password.Length; i++)
                {
                    c[i] = '*';
                }
            });
        }

        [Benchmark]
        public string StringBuilderWithLoop()
        {
            var e = new StringBuilder(Password.Substring(0, 3));

            for (int i = 0; i < Password.Length - 3; i++)
            {
                e.Append('*');
            }
            return e.ToString();
        }

        [Benchmark]
        public string StringBuilderWithRepeat()
        {
            var e = new StringBuilder(Password.Substring(0, 3));
            e.Append('*', Password.Length-3);

            return e.ToString();
        }

        [Benchmark]
        public unsafe string UnsafeManipulateString()
        {
            var result = new string(Password);
            fixed (char* chars = result)
            {
                for (int i = 3; i < result.Length; i++)
                {
                    chars[i] = '*';
                }
            }
            return result;
        }
    }
}
