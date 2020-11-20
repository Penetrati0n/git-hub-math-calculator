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
            input = input.Replace(" ", "");
            
            // Проверка на правильность ввода скобок
            {
                Stack<char> brackets = new Stack<char>();
                for (int i = 0; i < input.Length; i++)
                {
                    if (isOpenBracket(input[i]))
                    {
                        brackets.Push(input[i]);
                    }
                    else if (isCloseBracket(input[i]))
                    {
                        char br;
                        switch (input[i])
                        {
                            case ')':
                                br = '(';
                                break;
                            case '}':
                                br = '{';
                                break;
                            case '>':
                                br = '<';
                                break;
                            case ']':
                                br = '[';
                                break;
                            default:
                                br = input[i];
                                break;
                        }
                        if (brackets.Count == 0 || br != brackets.Pop())
                        {
                            throw new Exception("Ошибка с вводом скобок скобок.");
                        }
                    }
                }
                if (brackets.Count > 0)
                {
                    throw new Exception("Ошибка с вводом скобок скобок.");
                }
            }

            Stack<string> stack = new Stack<string>();

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


                    // Если после цифры идёт аргумент или функция, то вставляем между ними знак умножения. Пример: 2x = 2 * x
                    if (i < input.Length && (char.IsLetter(input[i]) || isOpenBracket(input[i])))
                    {
                        while (stack.Count > 0 && GetPrioritet(stack.Peek()) >= GetPrioritet("*"))
                        {
                            Output.Add(stack.Pop());
                        }
                        stack.Push("*");
                    }
                    //


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
                else if (isOpenBracket(c))
                {
                    stack.Push(c.ToString());
                }
                else if (isCloseBracket(c))
                {
                    string openBracket;
                    switch (c)
                    {
                        case ')':
                            openBracket = "(";
                            break;
                        case '}':
                            openBracket = "{";
                            break;
                        case ']':
                            openBracket = "[";
                            break;
                        case '>':
                            openBracket = "<";
                            break;
                        default:
                            openBracket = "";
                            break;
                    }

                    while (stack.Peek() != openBracket)
                    {
                        Output.Add(stack.Pop());
                    }
                    stack.Pop();
                    if (stack.Count > 0 && !isPrefFunc(stack.Peek()) && !isOpenBracket(stack.Peek()[0]))
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
                            if (Math.Tan(stack.Peek()) == 0) throw new Exception("Ошибка в котангенсе.");
                            stack.Push(1.0 / Math.Tan(stack.Pop()));
                            break;
                        case "asin":
                        case "arcsin":
                            if (stack.Peek() < -1 || stack.Peek() > 1) throw new Exception("Значение аргумента в asin or arcsin не принадлежит [-1; 1].");
                            stack.Push(Math.Asin(stack.Pop()));
                            break;
                        case "acos":
                        case "arccos":
                            if (stack.Peek() < -1 || stack.Peek() > 1) throw new Exception("Значение аргумента в acos or arccos не принадлежит [-1; 1].");
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
                            if (Math.Cos(stack.Peek()) == 0) throw new Exception("Проблема в sec().");
                            stack.Push(1 / Math.Cos(stack.Pop()));
                            break;
                        case "csc":
                            if (Math.Sin(stack.Peek()) == 0) throw new Exception("Проблема в csc().");
                            stack.Push(1 / Math.Sin(stack.Pop()));
                            break;
                        case "asec":
                            if (stack.Peek() == 0) throw new Exception("Проблема в asec().");
                            stack.Push(Math.Acos(1 / stack.Pop()));
                            break;
                        case "acsc":
                            if (stack.Peek() == 0) throw new Exception("Проблема в acsc().");
                            stack.Push(Math.Asin(1 / stack.Pop()));
                            break;
                        case "abs":
                            stack.Push(Math.Abs(stack.Pop()));
                            break;
                        case "sqrt":
                            if (stack.Peek() < 0) throw new Exception("Отрицательное значение под корнем.");
                            stack.Push(Math.Sqrt(stack.Pop()));
                            break;
                        case "cbrt":
                            stack.Push(Math.Pow(stack.Pop(), (double)1 / 3));
                            break;
                        case "root":
                            double n = stack.Pop();
                            if (n == 0 || (n % 2) == 0 && stack.Peek() < 0) throw new Exception("Что-то не так с корнем.");
                            stack.Push(Math.Pow(stack.Pop(), 1 / n));
                            break;
                        case "log":
                            double a = stack.Pop();
                            double b = stack.Pop();
                            if (a <= 0 || a == 1 || b <= 0) throw new Exception("Ошибка в log(,).");
                            stack.Push(Math.Log(a, b));
                            break;
                        case "ln":
                            if (stack.Peek() <= 0) throw new Exception("Ошибка в ln().");
                            stack.Push(Math.Log(stack.Pop()));
                            break;
                        case "lg":
                            if (stack.Peek() <= 0) throw new Exception("Ошибка в lg().");
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
                    if (stack.Peek() - Math.Truncate(stack.Peek()) > 0 || stack.Peek() < 0) throw new Exception("Ошибка при вычислении факториала.");
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
        public double Integral(int a, int b, double E)
        {
            if (Output.Count == 0) throw new Exception("Нет формулы.");

            Dictionary<string, string> var = new Dictionary<string, string>() { { "x", "" } };
            int n = 2 * Convert.ToInt32(Math.Pow(3, 8));
            double dx = (b - a) / n;
            double sumN = 0;
            double sumL;
            double temp;
            Dictionary<double, double> valuesFunction = new Dictionary<double, double>();

            do
            {
                n *= 3;
                sumL = sumN;
                sumN = 0;

                for (double x = a + dx / 2; x < b; x += dx)
                {
                    if (valuesFunction.ContainsKey(x))
                    {
                        sumN += valuesFunction[x] * dx;
                    }
                    else
                    {
                        var["x"] = x.ToString();
                        temp = Computing(var);
                        sumN += temp * dx;
                        valuesFunction.Add(x, temp);
                    }
                }
            }
            while (Math.Abs(sumN - sumL) > E);

            return sumN;
        }
        public double Integral(int a, int b, double E, Dictionary<string, string> var)
        {
            if (Output.Count == 0) throw new Exception("Нет формулы.");

            if (!var.ContainsKey("x")) var.Add("x", "");

            int n = 2 * Convert.ToInt32(Math.Pow(3, 8));
            double dx = (b - a) / n;
            double sumN = 0;
            double sumL;
            double temp;
            Dictionary<double, double> valuesFunction = new Dictionary<double, double>();

            do
            {
                n *= 3;
                sumL = sumN;
                sumN = 0;

                for (double x = a + dx / 2; x < b; x += dx)
                {
                    if (valuesFunction.ContainsKey(x))
                    {
                        sumN += valuesFunction[x] * dx;
                    }
                    else
                    {
                        var["x"] = x.ToString();
                        temp = Computing(var);
                        sumN += temp * dx;
                        valuesFunction.Add(x, temp);
                    }
                }
            }
            while (Math.Abs(sumN - sumL) > E);

            return sumN;
        }
        public double Integral(int a, int b, double E, string function)
        {
            ReversePolish(function);

            Dictionary<string, string> var = new Dictionary<string, string>() { { "x", "" } };
            int n = 2 * Convert.ToInt32(Math.Pow(3, 8));
            double dx = (b - a) / n;
            double sumN = 0;
            double sumL;
            double temp;
            Dictionary<double, double> valuesFunction = new Dictionary<double, double>();

            do
            {
                n *= 3;
                sumL = sumN;
                sumN = 0;

                for (double x = a + dx / 2; x < b; x += dx)
                {
                    if (valuesFunction.ContainsKey(x))
                    {
                        sumN += valuesFunction[x] * dx;
                    }
                    else
                    {
                        var["x"] = x.ToString();
                        temp = Computing(var);
                        sumN += temp * dx;
                        valuesFunction.Add(x, temp);
                    }
                }
            }
            while (Math.Abs(sumN - sumL) > E);

            return sumN;
        }
        public double Integral(int a, int b, double E,string function, Dictionary<string, string> var)
        {
            ReversePolish(function);

            if (!var.ContainsKey("x")) var.Add("x", "");

            int n = 2 * Convert.ToInt32(Math.Pow(3, 8));
            double dx = (b - a) / n;
            double sumN = 0;
            double sumL;
            double temp;
            Dictionary<double, double> valuesFunction = new Dictionary<double, double>();

            do
            {
                n *= 3;
                sumL = sumN;
                sumN = 0;

                for (double x = a + dx / 2; x < b; x += dx)
                {
                    if (valuesFunction.ContainsKey(x))
                    {
                        sumN += valuesFunction[x] * dx;
                    }
                    else
                    {
                        var["x"] = x.ToString();
                        temp = Computing(var);
                        sumN += temp * dx;
                        valuesFunction.Add(x, temp);
                    }
                }
            }
            while (Math.Abs(sumN - sumL) > E);

            return sumN;
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

+ - * / () {} [] <>
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
        private bool isOpenBracket(char bracket)
        {
            return bracket == '(' || bracket == '{' || bracket == '[' || bracket == '<';
        }
        private bool isCloseBracket(char bracket)
        {
            return bracket == ')' || bracket == '}' || bracket == ']' || bracket == '>';
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
