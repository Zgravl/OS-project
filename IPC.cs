using System;
using System.IO.Pipes;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please specify 'producer' or 'consumer'.");
            return;
        }

        if (args[0].ToLower() == "producer")
        {
            RunProducer();
        }
        else if (args[0].ToLower() == "consumer")
        {
            RunConsumer();
        }
        else
        {
            Console.WriteLine("Invalid argument. Please specify 'producer' or 'consumer'.");
        }
    }

    static void RunProducer()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("myPipe", PipeDirection.Out))
        {
            Console.WriteLine("Waiting for connection...");
            pipeServer.WaitForConnection();
            Console.WriteLine("Connected to consumer.");

            for (int i = 1; i <= 10; i++)
            {
                string message = i.ToString() + "\n";
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                pipeServer.Write(buffer, 0, buffer.Length);
                Console.WriteLine($"Produced: {i}");
            }

            Console.WriteLine("Data sent to consumer.");
        }
    }

    static void RunConsumer()
    {
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "myPipe", PipeDirection.In))
        {
            Console.WriteLine("Connecting to producer...");
            pipeClient.Connect();
            Console.WriteLine("Connected to producer.");

            byte[] buffer = new byte[256];
            int bytesRead;
            while ((bytesRead = pipeClient.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Consumed: {message.Trim()}");
            }

            Console.WriteLine("Finished consuming data.");
        }
    }
}
