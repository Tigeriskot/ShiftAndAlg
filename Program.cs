using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShiftAndAlg
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("************* Лабораторные работы по 6 лекции *************\n" +
                "************* Реализация алгоритмов ShiftAnd и ShiftAndFz *************\n");

            // считываем исходный текст из файла в переменную TextInFile

            Console.WriteLine("Введите название файла в котором будет происходить поиск: ");


            try
            {   // чтение данных из файла
                using (StreamReader sr = new StreamReader(Console.ReadLine()))
                {
                    string TextInFile = sr.ReadToEnd();
                    Console.WriteLine("Текст файла: ");
                    Console.WriteLine(TextInFile);
                    // метод, который демонстрирует работу алгоритмов
                    Realization(TextInFile);

                }


            }
            catch (Exception ex)
            {

                Console.WriteLine("Файл не найден");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Если хотите ввести текст вручную, напишите 1");

                if (Console.ReadLine() == "1")
                {
                    Console.WriteLine("Введите текст для работы с алгоритмами");
                    string Text = Console.ReadLine();
                    Realization(Text);
                }


            }





            Console.Read();
        }




        public static void Realization(string Text)
        {
            string SearchingText;

            Console.WriteLine("Полученный текст: " + Text);
            Console.WriteLine("Введите текст, которые хотите искать в " + Text);
            SearchingText = Console.ReadLine();

            Console.WriteLine("Демонстрация работы алгоритма Shift-and");
            ShiftAnd(Text, SearchingText);

            Console.WriteLine("Демонстрация работы алгоритма Shift-and модифицированный");
            ShiftAndFz(Text, SearchingText, 1);

            Console.WriteLine();
        }


        // Метод(вспомогательный) для вывода на экран содержимого массива массива граней и т.п.
        public static void Show(int[] Array)
        {
            for (int i = 0; i < Array.Length; ++i)
            {
                Console.Write(Array[i] + " ");
            }
            Console.WriteLine();
        }
        // Метод, который реализует работу алгоритма ShiftAnd
        public static void ShiftAnd(string OriginalText, string SearchingText)
        {
            int OriginalTextLength = OriginalText.Length;
            int SearchingTextLength = SearchingText.Length;
            if (SearchingText.Length > 64) // ограничение long
                return;

            char BeginChar = '0', EndChar = 'z';    // Алфавит: от цифр до букв латиницы
            int AlphSize = EndChar - BeginChar + 1; // размер алфавита
            long[] InpArr = new long[AlphSize];          // массив вхождений

            // Подготовка массива вхождений (битовая 1 если в этой позиции в подстроке совпадающий символ)
            // т.е. индекс - буква из алфавита, а # позиции в числе - позиция в подстроке
            for (int i = 0; i < SearchingTextLength; i++)
                InpArr[SearchingText[i]-BeginChar] |= 1 << (SearchingTextLength - 1 - i);

            long uHigh = 1 << (SearchingTextLength - 1); // Константа для установки 1 в старший (для подстроки) разряд
            long M = 0; // строка матрицы в виде битового поля (обычное число) для подстрок до 64 символов


            Console.WriteLine("  " + SearchingText); // NOTE VIZ

            // Вычисление «строк матрицы» и фиксация вхождений
            for (int i = 0; i < OriginalTextLength; i++)
            {
                // (сдвиг вправо, установка старшего бита в 1) AND (строка массива текущей буквы)
                // по сути выполняется учёт предыдущих бит при сдвиге
                // если суффикс прекратился раньше чем дошли до конца подстроки, то в M будут записываться нули
                
                M = (M >> 1 | uHigh) & InpArr[OriginalText[i]-BeginChar];

                Console.WriteLine(OriginalText[i] + " " + IntToString(M, SearchingTextLength-1)); 
                
                if ((M & 1) == 1) // если младший бит = 1 -> нашли вхождение
                    Console.WriteLine("Совпадение с позиции: " + (i - SearchingTextLength + 1));
            }
        }


        // Поиск неточных совпадений (k - кол-во несовпадающих букв)
        public static void ShiftAndFz(string OriginalText, string SearchingText, int CountDifference)
        {
            int OriginalTextLength = OriginalText.Length;
            int SearchingTextLength = SearchingText.Length;
            if (CountDifference > SearchingTextLength) // если CountDifference > длины паттерна
                return;

            if (SearchingText.Length > 64) // ограничение long
                return;

            char BeginChar = '0', EndChar = 'z';    // Алфавит: от цифр до букв латиницы
            int AlphSize = EndChar - BeginChar + 1; // размер алфавита
            long[] InpArr = new long[AlphSize];          // массив вхождений

            // Подготовка массива вхождений (битовая 1 если в этой позиции в подстроке совпадающий символ)
            // т.е. индекс - буква из алфавита, а # позиции в числе - позиция в подстроке
            for (int i = 0; i < SearchingTextLength; i++)
                InpArr[SearchingText[i] - BeginChar] |= 1 << (SearchingTextLength - 1 - i);

            long uHigh = 1 << (SearchingTextLength - 1); // Константа для установки 1 в старший разряд

            long[] M = new long[CountDifference + 1];
            long[] M1 = new long[CountDifference + 1];

            // Вычисление «строк матрицы» и фиксация вхождений
            for (int i = 0; i < OriginalTextLength; i++)
            {
                for (int j = 0; j <= CountDifference; j++)
                {
                    M1[j] = M[j]; // Запомнить (i-1) строку
                    
                    M[j] = (M[j] >> 1 | uHigh) & InpArr[OriginalText[i] - BeginChar];
                    
                    
                    if (j != 0) // если строка не первая
                        M[j] |= M1[j - 1] >> 1 | uHigh; // Используем предыдущую строку
                   
                    Console.WriteLine(" -> " + IntToString(M[j], SearchingTextLength-1)); 

                    if ((j == CountDifference) && (M[j] & 1) == 1) // если дошли до CountDifference и последний бит строки = 1
                        Console.WriteLine("Совпадение с позиции: " + (i - SearchingTextLength + 1));
                }

                Console.WriteLine(); 
            }
        }

        
        public static string IntToString(long number, int numSize)
        {
            StringBuilder result = new StringBuilder();

            for (int i = numSize; i >= 0; i--)
            {
                long mask = 1 << i;
                result.Append((number & mask) != 0 ? "1" : "0");

                if (i % 4 == 0)
                    result.Append(" ");
            }
           

            return result.ToString();
        }
    }
}
