using System;
using System.Collections.Concurrent;
using System.Threading;

namespace XenoBot2
{
    // from https://stackoverflow.com/questions/3670057/does-console-writeline-block

    /// <summary>
    /// A console that queues console writes and executes them on a different thread.
    /// </summary>
    public static class NonBlockingConsole
    {
        private static readonly BlockingCollection<string> MQueue = new BlockingCollection<string>();

        static NonBlockingConsole()
        {
            var thread = new Thread(
                () =>
                {
                    while (true) Console.WriteLine(MQueue.Take());
                }) {IsBackground = true};
            thread.Start();
        }

        /// <summary>
        ///		Write a line to the console.
        /// </summary>
        /// <param name="value">The data to write to the console.</param>
        public static void WriteLine(string value)
        {
            MQueue.Add(value);
        }

        /// <summary>
        ///		Write a newline to the console.
        /// </summary>
        public static void WriteLine()
        {
            MQueue.Add(string.Empty);
        }
    }
}
