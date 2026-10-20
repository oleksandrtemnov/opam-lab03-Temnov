using System;
using System.Collections.Generic;
using System.IO;

namespace Lab03Recursion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int N = 7;
            int K = 19;
            int a = N % 3; // 1
            int b = N % 4; // 3
            int c = N % 5; // 2

            Console.WriteLine($"=== Лабораторна робота 3 ===");
            Console.WriteLine($"N = {N}, K = {K}, a = {a}, b = {b}, c = {c}\n");

            // --- Завдання 1 ---
            Console.WriteLine("--- Завдання 1: Вкладена структура (Пошук .cs файлів) ---");
            Console.Write("Введіть шлях до директорії: ");
            string? dirPath = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(dirPath) && Directory.Exists(dirPath))
            {
                (int csCount, long totalSize) = Task1_CountCsFiles(dirPath);
                Console.WriteLine($"Кількість .cs файлів: {csCount}");
                Console.WriteLine($"Сумарний розмір: {totalSize} байт");
            }
            else
            {
                Console.WriteLine("Помилка: Вказана директорія не існує.");
            }

            Console.WriteLine();

            // --- Завдання 2 ---
            Console.WriteLine("--- Завдання 2: Розбиття навпіл (Елементи > 9) ---");
            int[] array16 = GenerateArray(16, N);
            Console.WriteLine("Згенерований масив (16 елементів):");
            Console.WriteLine(string.Join(", ", array16));

            int maxDepth = 0;
            int threshold = K % 10; // 9
            int countGreaterThanK = Task2_CountGreaterThanK(array16, 0, array16.Length - 1, 1, ref maxDepth, threshold);

            Console.WriteLine($"Кількість елементів, більших за {threshold}: {countGreaterThanK}");
            Console.WriteLine($"Максимальна глибина рекурсії: {maxDepth}");
            Console.WriteLine($"Теоретичне значення log2(16): {Math.Log2(16)}");

            Console.WriteLine();

            // --- Завдання 3 ---
            Console.WriteLine("--- Завдання 3: Перебір з поверненням (Сходинки) ---");
            int M = (K % 8) + 5; // (19 % 8) + 5 = 8
            Console.WriteLine($"Усі способи піднятися на {M} сходинок кроками по 1 або 2:");

            int totalWays = Task3_StairsBacktracking(M, new List<int>());
            Console.WriteLine($"Усього способів: {totalWays}");

            Console.WriteLine();

            // --- Завдання 4 ---
            Console.WriteLine("--- Завдання 4: Трасування (Стек викликів) ---");
            int[] array4 = GenerateArray(4, N);
            Console.WriteLine("Масив з 4 елементів: " + string.Join(", ", array4));
            Console.WriteLine("Трасування рекурсії:");
            Task4_TraceCountGreaterThanK(array4, 0, array4.Length - 1, 0, threshold);

            Console.WriteLine();

            // --- Завдання 5 ---
            Console.WriteLine("--- Завдання 5: Ітеративна версія (Цикл) ---");
            int iterativeCount = Task5_CountGreaterThanKIterative(array16, threshold);
            Console.WriteLine($"Результат циклу: {iterativeCount}");
            Console.WriteLine($"Результат рекурсії: {countGreaterThanK}");
            Console.WriteLine($"Збігаються: {iterativeCount == countGreaterThanK}");
        }

        // --- Генератор масиву ---
        static int[] GenerateArray(int size, int seed)
        {
            Random rnd = new Random(seed);
            int[] arr = new int[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = rnd.Next(-20, 21);
            }
            return arr;
        }

        // --- Завдання 1: Вкладена структура ---
        public static (int count, long size) Task1_CountCsFiles(string currentDir)
        {
            int csCount = 0;
            long totalSize = 0;

            try
            {
                string[] files = Directory.GetFiles(currentDir);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        csCount++;
                        FileInfo fi = new FileInfo(file);
                        totalSize += fi.Length;
                    }
                }

                string[] subDirs = Directory.GetDirectories(currentDir);
                foreach (string subDir in subDirs)
                {
                    (int subCount, long subSize) = Task1_CountCsFiles(subDir);
                    csCount += subCount;
                    totalSize += subSize;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Пропускаємо папки, до яких немає доступу
            }
            catch (DirectoryNotFoundException)
            {
                // Пропускаємо, якщо папка була видалена під час обходу
            }

            return (csCount, totalSize);
        }

        // --- Завдання 2: Розбиття навпіл ---
        public static int Task2_CountGreaterThanK(int[] a, int lo, int hi, int currentDepth, ref int maxDepth, int threshold)
        {
            if (currentDepth > maxDepth)
            {
                maxDepth = currentDepth;
            }

            // Базовий випадок: залишився один елемент
            if (lo == hi)
            {
                return a[lo] > threshold ? 1 : 0;
            }

            int mid = lo + (hi - lo) / 2;

            int leftCount = Task2_CountGreaterThanK(a, lo, mid, currentDepth + 1, ref maxDepth, threshold);
            int rightCount = Task2_CountGreaterThanK(a, mid + 1, hi, currentDepth + 1, ref maxDepth, threshold);

            return leftCount + rightCount;
        }

        // --- Завдання 3: Перебір з поверненням ---
        public static int Task3_StairsBacktracking(int remainingStairs, List<int> path)
        {
            // Базовий випадок 1: досягли вершини
            if (remainingStairs == 0)
            {
                Console.WriteLine(string.Join(" + ", path));
                return 1;
            }

            // Базовий випадок 2: переліт через сходинки
            if (remainingStairs < 0)
            {
                return 0;
            }

            int ways = 0;
            for (int step = 1; step <= 2; step++)
            {
                path.Add(step); // Зробити вибір
                ways += Task3_StairsBacktracking(remainingStairs - step, path); // Піти вглиб
                path.RemoveAt(path.Count - 1); // Скасувати вибір (backtrack)
            }

            return ways;
        }

        // --- Завдання 4: Трасування ---
        public static int Task4_TraceCountGreaterThanK(int[] a, int lo, int hi, int depth, int threshold)
        {
            string indent = new string(' ', depth * 2);
            Console.WriteLine($"{indent}enter Count({lo}..{hi})");

            int result;
            if (lo == hi)
            {
                result = a[lo] > threshold ? 1 : 0;
                Console.WriteLine($"{indent}  enter Count({lo}..{lo}) -> {result}");
                Console.WriteLine($"{indent}exit  Count({lo}..{hi}) -> {result}");
                return result;
            }

            int mid = lo + (hi - lo) / 2;
            int left = Task4_TraceCountGreaterThanK(a, lo, mid, depth + 1, threshold);
            int right = Task4_TraceCountGreaterThanK(a, mid + 1, hi, depth + 1, threshold);

            result = left + right;
            Console.WriteLine($"{indent}exit  Count({lo}..{hi}) -> {result}");
            return result;
        }

        // --- Завдання 5: Ітеративна версія ---
        public static int Task5_CountGreaterThanKIterative(int[] a, int threshold)
        {
            int count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] > threshold)
                {
                    count++;
                }
            }
            return count;
        }
    }
}