using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathCalculator;

namespace FuncAlgObrPolNot
{
    class Program
    {
        static void Main(string[] args)
        {
            Example();
        }

        public static void Example()
        {
            Calculator calculator = new Calculator();
            Console.WriteLine(calculator.Help(0) + "\n\n\n");                           // Вывод списка функций

            string function = "(sin(pi * x) ^ 2 * log(2, x + a)) / (root(x + 15.15, a))";
            Console.WriteLine($"F(x, a) = {function}\n");
            calculator.ReversePolish(function);                                         // Перевод функции в оратную польскую запись

            Dictionary<string, string> varValue = new Dictionary<string, string>();
            varValue.Add("x", "0.5676");                                                // Задаём значения
            varValue.Add("a", "1");                                                     //          аргументам

            foreach (var t in varValue)
            {
                Console.WriteLine($"{t.Key} = {t.Value}");
            }
            Console.WriteLine();

            double result = calculator.Computing(varValue);                             // Вычисляем значение функции при заданных аргумантах
            Console.WriteLine($"F = {result}");                                         // Выводим результат
            Console.ReadLine();
        }
    }
}
