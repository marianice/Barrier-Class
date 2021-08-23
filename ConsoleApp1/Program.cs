using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Barrier Class - позволяет нескольким задачам параллельно работать с алгоритмом, используя несколько фаз.
/*
 * Cледующая программа подсчитывает количество итераций
 * (или этапов), необходимых для того, чтобы каждый
 * из двух потоков одновременно нашел свое частичное решение,
 * используя алгоритм перемешивания слов. 
 * В каждом потоке перемешиваются свои слова, 
 * а затем операция барьера после этапа сравнивает 
 * два результата и проверяет, правильно ли 
 * собрано полное предложение.
 */
namespace ConsoleApp1
{
    class Program
    {
        static string[] words1 = new string[] { "brown", "jumps", "the", "fox", "quick" };
        static string[] words2 = new string[] { "dog", "lazy", "the", "over" };
        static string solution = "the quick brown fox jumps over the lazy dog.";

        static bool success = false;
        static Barrier barrier = new Barrier(2, (b) =>
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < words1.Length; i++)
            {
                sb.Append(words1[i]);
                sb.Append(" ");
            }
            for (int i = 0; i < words2.Length; i++)
            {
                sb.Append(words2[i]);

                if (i < words2.Length - 1)
                    sb.Append(" ");
            }
            sb.Append(".");
#if TRACE
            System.Diagnostics.Trace.WriteLine(sb.ToString());
#endif
            Console.CursorLeft = 0;
            Console.Write("Количество попыток: {0}", barrier.CurrentPhaseNumber);
            if (String.CompareOrdinal(solution, sb.ToString()) == 0)
            {
                success = true;
                Console.WriteLine("\r\nРешение найдено с {0}-ой попытки", barrier.CurrentPhaseNumber);
            }
        });

        static void Main(string[] args)
        {
            Thread t1 = new Thread(() => Solve(words1));
            Thread t2 = new Thread(() => Solve(words2));
            t1.Start();
            t2.Start();
            Console.ReadLine();
        }
        // Перемешивание Кнута-Фишера-Йетса для случайного изменения порядка каждого массива.
        // Успех с правой или левой стороны не учитывается   
        static void Solve(string[] wordArray)
        {
            while (success == false)
            {
                Random random = new Random();
                for (int i = wordArray.Length - 1; i > 0; i--)
                {
                    int swapIndex = random.Next(i + 1);
                    string temp = wordArray[i];
                    wordArray[i] = wordArray[swapIndex];
                    wordArray[swapIndex] = temp;
                }

                barrier.SignalAndWait();
            }
        }
    }
}
