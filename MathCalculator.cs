using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncAlgObrPolNot
{
    public class MathCalculator
    {
        private List<string> Output;                        // Обратная польская запись
        private readonly string[] PrefixFunctions = { "sin", "cos", "tg", "tan", "ctg", "cot",
            "asin", "arcsin", "acos", "arccos", "atg", "atan", "arctg", "arctan", "actg", "acot", "arcctg", "arccot",
            "sec", "csc", "asec", "acsc", "abs", "sqrt", "cbrt", "root", "log", "ln", "lg", "exp" };    // Список функций

        public MathCalculator()
        {
            Output = new List<string>();
        }

        /// <summary>
        /// Перевод строки в обратную польскую запись
        /// </summary>
        /// <param name="input">Исходная формула</param>
        public void ReversePolish(string input)
        {
            Output.Clear();
            Stack<string> stack = new Stack<string>();
            input = input.Replace(" ", "");

            char c;
            string temp;
            for (int i = 0; i < input.Length; i++)
            {
                c = input[i];
                if (char.IsDigit(c) || c == '.')
                {
                    temp = $"{c}";
                    while (++i < input.Length && (char.IsDigit(input[i]) || input[i] == '.'))
                    {
                        if (input[i] == '.')
                        {
                            temp += ',';
                        }
                        else
                        {
                            temp += input[i];
                        }
                    }

                    Output.Add(temp);
                    i--;
                }
                else if (c == '!')
                {
                    Output.Add(c.ToString());
                }
                else if (char.IsLetter(c))
                {
                    temp = $"{c}";
                    while (++i < input.Length && char.IsLetter(input[i]))
                    {
                        temp += input[i];
                    }

                    if (temp == "pi" || temp == "e")
                    {
                        Output.Add(temp);
                    }
                    else if (temp.Length == 1)
                    {
                        Output.Add(temp);
                    }
                    else
                    {
                        stack.Push(temp);
                    }

                    i--;
                }
                else if (c == ',')
                {
                    while (stack.Peek() != "(")
                    {
                        Output.Add(stack.Pop());
                    }
                    stack.Pop();
                    stack.Push("(");
                }
                else if (c == '(')
                {
                    stack.Push(c.ToString());
                }
                else if (c == ')')
                {
                    while (stack.Peek() != "(")
                    {
                        Output.Add(stack.Pop());
                    }
                    stack.Pop();
                    if (stack.Count > 0 && !isPrefFunc(stack.Peek()))
                    {
                        Output.Add(stack.Pop());
                    }
                }
                else
                {
                    if (i - 1 < 0 || (i - 1 > 0 && input[i - 1] == '('))
                    {
                        Output.Add("0");
                    }

                    while (stack.Count > 0 && (isPrefFunc(stack.Peek()) || GetPrioritet(stack.Peek()) >= GetPrioritet(c.ToString())))
                    {
                        Output.Add(stack.Pop());
                    }
                    stack.Push(c.ToString());

                }
            }

            while (stack.Count > 0 && stack.Peek() != "(")
            {
                Output.Add(stack.Pop());
            }
        }
        /// <summary>
        /// Вычисление функции
        /// </summary>
        /// <param name="dict">Коллекция: переменная - значение</param>
        /// <returns></returns>
        public double Computing(Dictionary<string, string> dict)
        {
            Stack<double> stack = new Stack<double>();

            string c;
            for (int i = 0; i < Output.Count; i++)
            {
                c = Output[i];

                if (char.IsDigit(c[0]) || (c.Length > 1 && char.IsDigit(c[1])))
                {
                    stack.Push(Convert.ToDouble(c));
                }
                else if (c == "pi" || c == "e")
                {
                    switch (c)
                    {
                        case "pi":
                            stack.Push(Math.PI);
                            break;
                        case "e":
                            stack.Push(Math.E);
                            break;
                    }
                }
                else if (isPrefFunc(c))
                {
                    switch (c)
                    {
                        case "sin":
                            stack.Push(Math.Sin(stack.Pop()));
                            break;
                        case "cos":
                            stack.Push(Math.Cos(stack.Pop()));
                            break;
                        case "tg":
                        case "tan":
                            stack.Push(Math.Tan(stack.Pop()));
                            break;
                        case "ctg":
                        case "cot":
                            stack.Push(1.0 / Math.Tan(stack.Pop()));
                            break;
                        case "asin":
                        case "arcsin":
                            stack.Push(Math.Asin(stack.Pop()));
                            break;
                        case "acos":
                        case "arccos":
                            stack.Push(Math.Acos(stack.Pop()));
                            break;
                        case "atg":
                        case "atan":
                        case "arctg":
                        case "arctan":
                            stack.Push(Math.Atan(stack.Pop()));
                            break;
                        case "actg":
                        case "acot":
                        case "arcctg":
                        case "arccot":
                            stack.Push(Math.PI / 2 - Math.Atan(stack.Pop()));
                            break;
                        case "sec":
                            stack.Push(1 / Math.Cos(stack.Pop()));
                            break;
                        case "csc":
                            stack.Push(1 / Math.Sin(stack.Pop()));
                            break;
                        case "asec":
                            stack.Push(Math.Acos(1 / stack.Pop()));
                            break;
                        case "acsc":
                            stack.Push(Math.Asin(1 / stack.Pop()));
                            break;
                        case "abs":
                            stack.Push(Math.Abs(stack.Pop()));
                            break;
                        case "sqrt":
                            stack.Push(Math.Sqrt(stack.Pop()));
                            break;
                        case "cbrt":
                            stack.Push(Math.Pow(stack.Pop(), (double)1 / 3));
                            break;
                        case "root":
                            double n = stack.Pop();
                            stack.Push(Math.Pow(stack.Pop(), 1 / n));
                            break;
                        case "log":
                            stack.Push(Math.Log(stack.Pop(), stack.Pop()));
                            break;
                        case "ln":
                            stack.Push(Math.Log(stack.Pop()));
                            break;
                        case "lg":
                            stack.Push(Math.Log10(stack.Pop()));
                            break;
                        case "exp":
                            stack.Push(Math.Exp(stack.Pop()));
                            break;
                    }
                }
                else if (char.IsLetter(c[0]) && c.Length == 1)
                {
                    stack.Push(double.Parse(dict[c].Contains('.') ? dict[c].Replace('.', ',') : dict[c]));
                }
                else if (c == "!")
                {
                    stack.Push(Factorial(stack.Pop()));
                }
                else
                {
                    double a, b;

                    if (stack.Count == 1)
                    {
                        a = stack.Pop();
                        b = 0;
                    }
                    else
                    {
                        a = stack.Pop();
                        b = stack.Pop();
                    }

                    switch (c)
                    {
                        case "-":
                            stack.Push(b - a);
                            break;
                        case "+":
                            stack.Push(b + a);
                            break;
                        case "*":
                            stack.Push(b * a);
                            break;
                        case "/":
                            stack.Push(b / a);
                            break;
                        case "^":
                            stack.Push(Math.Pow(b, a));
                            break;

                    }
                }
            }

            return stack.Pop();
        }
        /// <summary>
        /// Информация о возможностях калькулятора
        /// </summary>
        /// <param name="i">0 - показать всю информацию</param>
        /// <returns></returns>
        public string Help(int i)
        {
            switch (i)
            {
                case 0:
                    return @"--------------------------------------------------------------

Простейшие математические операции

+ - * / ()
сложение, вычитание, умножение, деление и группирующие символы

Десятичные дроби записываются через точку:
0.5 - правильная запись;
0,5 - неправильная запись.

--------------------------------------------------------------

Элементарные функции

x^n - Возведение в степень
sqrt(x) - Квадратный корень
cbrt(x) - Кубический корень
root(x,n) - Корень n-той степени из x
log(a,x) - Логарифм от x по основанию a
ln(x) - Натуральный логарифм (логарифм c основанием e)
lg(x) - Десятичный логарифм (логарифм по основанию 10)
exp(x) - Экспоненциальная функция, эквивалентно e^x

--------------------------------------------------------------

Тригонометрические функции

sin(x) - Синус от x
cos(x) - Косинус от x
tg(x) - Тангенс от x. Можно вводить tg(x) или tan(x)
ctg(x) - Котангенс от x. Можно вводить ctg(x) или cot(x)
sec(x) - Секанс от x, определяется как 1/cos(x)
csc(x) - Косеканс от x, определяется как 1/sin(x)
arcsin(x) - Арксинус от x. Можно вводить arcsin(x) или asin(x)
arccos(x) - Арккосинус от x. Можно вводить arccos(x) или acos(x)
atan(x) - Арктангенс от x. Можно вводить atan(x) или atg(x) или arctan(x) или arctg(x)
arcctg(x) - Арккотангенс от x. Можно вводить actg(x) или acot(x) или arcctg(x) или arccot(x)
asec(x) - Арксеканс от x
acsc(x) - Арккосеканс от x

--------------------------------------------------------------

Некоторые константы

e - Число Эйлера e = 2.718281828459045...
pi - Число pi = 3.141592653589793...

--------------------------------------------------------------
";
                default:
                    return "";
            }
        }


        private int GetPrioritet(string op)
        {
            int p;
            switch (op)
            {
                case "+":
                case "-":
                    p = 1;
                    break;
                case "*":
                case "/":
                    p = 2;
                    break;
                case "^":
                    p = 3;
                    break;
                default:
                    p = 0;
                    break;
            }

            return p;
        }
        private bool isPrefFunc(string oper)
        {
            return PrefixFunctions.Contains(oper);
        }
        private double Factorial(double i)
        {
            double result = 1;
            for (; i > 1; i--)
            {
                result *= i;
            }

            return result;
        }
    }
}
