// See https://aka.ms/new-console-template for more information

public delegate void ThreadStart();

class ThreadTest
{
    static void Main()
    {
        Thread t = new Thread (Go);
        t.Start(); // Run Go() on the new thread.
        Go(); // Simultaneously run Go() in the main thread.
    }
    static void Go()
    {
        Console.WriteLine ("hello!");
    }
}