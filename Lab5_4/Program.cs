using System;
using System.Threading;

class Program
{
    static int[,] matrixA = new int[3, 3] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
    static int[,] matrixB = new int[3, 3] { { 9, 8, 7 }, { 6, 5, 4 }, { 3, 2, 1 } };
    static int[,] resultMatrix = new int[3, 3];

    static SemaphoreSlim semaphore = new SemaphoreSlim(2); // дозволяє одночасно виконувати максимум 2 потоки
    static object lockObj = new object(); // для блокування доступу до результату

    static void Main()
    {
        int numThreads = 2; // кількість потоків
        Thread[] threads = new Thread[numThreads];

        for (int i = 0; i < numThreads; i++)
        {
            threads[i] = new Thread(new ParameterizedThreadStart(MatrixMultiplication));
            threads[i].Start(i);
        }

        for (int i = 0; i < numThreads; i++)
        {
            threads[i].Join();
        }

        // друк результату
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write(resultMatrix[i, j] + " ");
            }
            Console.WriteLine();
        }

        Console.ReadLine();
    }

    static void MatrixMultiplication(object threadIndex)
    {
        int index = (int)threadIndex;

        for (int i = index; i < 3; i += 2) // кожен потік обчислює окрему частину результату
        {
            for (int j = 0; j < 3; j++)
            {
                int sum = 0;
                for (int k = 0; k < 3; k++)
                {
                    sum += matrixA[i, k] * matrixB[k, j];
                }

                semaphore.Wait(); // чекаємо на дозвіл на доступ до результату
                lock (lockObj) // блокуємо доступ до результату
                {
                    resultMatrix[i, j] = sum;
                }
                semaphore.Release(); // відпускаємо семафор
            }
        }
    }
}

