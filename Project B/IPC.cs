using System;
using System.IO.Pipes;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Check if an argument is provided
        if (args.Length == 0)
        {
            Console.WriteLine("Please specify 'producer' or 'consumer'.");
            return;
        }

        // Determine whether to run as producer or consumer
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
        // Create a named pipe server for one-way communication
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("myPipe", PipeDirection.Out))
        {
            Console.WriteLine("Waiting for connection...");
            pipeServer.WaitForConnection(); // Wait for a consumer to connect
            Console.WriteLine("Connected to consumer.");

            // Send 10 messages to the consumer
            for (int i = 1; i <= 10; i++)
            {
                string message = i.ToString() + "\n"; // Append newline for readability
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                pipeServer.Write(buffer, 0, buffer.Length); // Write message to the pipe
                Console.WriteLine($"Produced: {i}");
            }

            Console.WriteLine("Data sent to consumer.");
        } // Pipe server is automatically closed when exiting the using block
    }

    static void RunConsumer()
    {
        // Create a named pipe client for one-way communication
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "myPipe", PipeDirection.In))
        {
            Console.WriteLine("Connecting to producer...");
            pipeClient.Connect(); // Connect to the producer's pipe
            Console.WriteLine("Connected to producer.");

            byte[] buffer = new byte[256]; // Buffer to store received data
            int bytesRead;
            
            // Read data from the pipe until there is no more
            while ((bytesRead = pipeClient.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Consumed: {message.Trim()}"); // Trim to remove extra whitespace
            }

            Console.WriteLine("Finished consuming data.");
        } 
    }
}
