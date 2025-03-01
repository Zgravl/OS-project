using System;
using System.Threading;

public class BankAccount
{
    // Properties for account balance and name
    public int Balance { get; set; }
    public string AccountName { get; set; }
    private readonly object balanceLock = new object(); // Lock object for thread safety

    // Constructor to initialize account details
    public BankAccount(string accountName, int balance)
    {
        AccountName = accountName;
        Balance = balance;
    }

    // Method to deposit money into the account
    public void Deposit(int amount)
    {
        lock (balanceLock)
        {
            Console.WriteLine($"Depositing {amount} to {AccountName}");
            Balance += amount;
            Console.WriteLine($"New balance of {AccountName}: {Balance}");
        }
    }

    // Method to withdraw money from the account)
    public void Withdraw(int amount)
    {
        lock (balanceLock)
        {
            if (Balance >= amount)
            {
                Console.WriteLine($"Withdrawing {amount} from {AccountName}");
                Balance -= amount;
                Console.WriteLine($"New balance of {AccountName}: {Balance}");
            }
            else
            {
                Console.WriteLine($"Insufficient funds for withdrawal from {AccountName}");
            }
        }
    }
}

class Program
{
    static BankAccount account1 = new BankAccount("Account1", 1000);
    static BankAccount account2 = new BankAccount("Account2", 1000);

    // Number of threads for parallel
    static int numberOfThreads = 10;
    static int[] numbers = new int[1000];
    static int[] squaredNumbers = new int[1000];

    // Method to attempt acquiring locks on two objects to prevent deadlock
    static bool TryAcquireLocks(object lock1, object lock2)
    {
        bool lock1Acquired = false;
        bool lock2Acquired = false;

        try
        {
            lock1Acquired = Monitor.TryEnter(lock1, TimeSpan.FromSeconds(1)); // Try acquiring first lock
            if (lock1Acquired)
            {
                lock2Acquired = Monitor.TryEnter(lock2, TimeSpan.FromSeconds(1)); // Try acquiring second lock
                if (!lock2Acquired)
                {
                    Monitor.Exit(lock1);  // Release first lock if second is not acquired
                }
            }
        }
        catch (Exception)
        {
            // Handle any exception that may arise
        }

        return lock1Acquired && lock2Acquired;
    }

    // Method to simulate bank transactions while avoiding deadlocks
    static void DeadlockResolution()
    {
        Thread thread1 = new Thread(() =>
        {
            if (TryAcquireLocks(account1, account2))
            {
                try
                {
                    Console.WriteLine("Thread1: Locking account1 and account2...");
                    account1.Deposit(300);
                    account2.Withdraw(200);
                }
                finally
                {
                    Monitor.Exit(account2);
                    Monitor.Exit(account1);
                }
            }
            else
            {
                Console.WriteLine("Thread1: Deadlock detected, unable to acquire locks.");
            }
        });

        Thread thread2 = new Thread(() =>
        {
            if (TryAcquireLocks(account2, account1))
            {
                try
                {
                    Console.WriteLine("Thread2: Locking account2 and account1...");
                    account2.Deposit(400);
                    account1.Withdraw(100);
                }
                finally
                {
                    Monitor.Exit(account1);
                    Monitor.Exit(account2);
                }
            }
            else
            {
                Console.WriteLine("Thread2: Deadlock detected, unable to acquire locks.");
            }
        });

        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();
    }

    // Method to perform parallel computation of squaring numbers using threads
    static void PerformComputation(int startIndex, int endIndex, int threadIndex)
    {
        Console.WriteLine($"Thread {threadIndex} started...");
        for (int i = startIndex; i < endIndex; i++)
        {
            squaredNumbers[i] = numbers[i] * numbers[i];
        }
        Console.WriteLine($"Thread {threadIndex} completed.");
    }

    static void Main(string[] args)
    {
        // Initialize the numbers array for parallel
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = i + 1
        }

        //Parallel Computation (Squaring numbers with 10 threads)
        Console.WriteLine("Performing Parallel Computation with 10 Threads...");
        Thread[] threads = new Thread[numberOfThreads];
        int chunkSize = numbers.Length / numberOfThreads;

        // Create and start 10 threads for parallel
        for (int i = 0; i < numberOfThreads; i++)
        {
            int startIndex = i * chunkSize;
            int endIndex = (i == numberOfThreads - 1) ? numbers.Length : (i + 1) * chunkSize;

            int threadIndex = i; // For identifying the thread
            threads[i] = new Thread(() => PerformComputation(startIndex, endIndex, threadIndex));
            threads[i].Start();
        }

        // Wait for all threads to complete
        for (int i = 0; i < numberOfThreads; i++)
        {
            threads[i].Join();
        }

        // Optionally print out the first 10 squared numbers for validation
        Console.WriteLine("\nFirst 10 squared numbers:");
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(squaredNumbers[i]);
        }

        //Simulate Bank Operations with Deadlock Detection and Resolution
        Console.WriteLine("\nDeadlock Detection and Resolution:");
        DeadlockResolution();
    }
}
