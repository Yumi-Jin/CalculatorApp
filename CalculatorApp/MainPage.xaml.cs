using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CalculatorApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        int state = 0;
        //0: nothing, 1: number, 2: operator pressed, 3: negative

        void OnButtonClicked(object sender, EventArgs e)
        {
            string tempEquation = this.equation.Text;
            string pressed = ((Button)sender).Text;

            switch (((Button)sender).Text)
            {
                case "÷":
                case "*":
                case "+":
                    Console.WriteLine("Case ÷*+");
                    if (state == 1) // if the character previously entered is a number
                    {
                        // Sets the state to be an operator pressed
                        // Add to the equation label
                        state = 2;
                        this.equation.Text = tempEquation + pressed;
                    }
                    else if (state == 2) // if the character previously entered is an operator
                    {
                        // Replaces the previous operator (excluding the negative sign)
                        this.equation.Text = tempEquation.Substring(0, tempEquation.Length - 1) + pressed;
                    }
                    break;
                case "-":
                    Console.WriteLine("Case -");
                    if (state != 3) // if equation isn't negative
                    {
                        state = 3; //set to negative
                        this.equation.Text = tempEquation + pressed;
                    }
                    break;
                case "=":
                    if (!((state == 0) || ((tempEquation.Length == 1) && (state == 3)))) //if there is at least something
                    {
                        if (!(state == 1)) // if equation ended with an operator
                        {
                            this.equation.Text = tempEquation.Substring(0, tempEquation.Length - 1);
                        }
                        state = 0;
                        Console.WriteLine("Case =");
                        var result = Calculate.ConvertToArray(this.equation.Text);
                        //this.equation.Text = result;
                        this.equation.Text = "";
                        this.answer.Text = result;
                    }
                    break;
                default:
                    state = 1;
                    Console.WriteLine("Numbers");
                    this.equation.Text = tempEquation + pressed;
                    break;
            }
        }
    }
    public static class Calculate
    {
        public static string ConvertToArray(string equation)
        {
            string eq = equation;
            ArrayList operands = new ArrayList();
            string previous = "";
            int opindex = 0;
            bool negative = false;

            //separate all the numbers as elements
            //save all 

            for (int counter = 0; counter < equation.Length; counter++)
            {
                string current = equation[counter].ToString();
                Console.WriteLine(current);
                switch (current)
                {
                    case "÷":
                    case "*":
                    case "+":
                        negative = false;
                        opindex++;
                        operands.Add(current);
                        break;
                    case "-":
                        if (isOperater(previous) || previous.Equals(""))
                        {
                            negative = true;
                        }
                        if (!previous.Equals(""))
                        {
                            opindex++;
                        }
                        operands.Add(current);
                        break;
                    default: // if current character is a number
                        if (negative || !(isOperater(previous) || previous.Equals(""))) // if previous is a number or negative
                        {
                            negative = false;
                            operands[opindex] = operands[opindex] + current;
                        }
                        else
                        {
                            if (!previous.Equals(""))
                            {
                                opindex++;
                            }
                            operands.Add(current);
                        }
                        break;
                }
                previous = current;
            }

            return InfixToPrefix(operands);
        }

        public static string InfixToPrefix(ArrayList operands)
        {
            string answer = "";
            string temp = "";
            ArrayList prefix = new ArrayList();
            if (operands.Count == 1) // if there is only 1 element
            {
                return (string)operands[0];
            }
            operands.Reverse();

            Stack<String> output = new Stack<String>();
            Stack<String> op = new Stack<String>();

            foreach (string token in operands)
            {
                if(!isOperater(token)) // if the token is an operand
                {
                    output.Push(token);
                }
                else // if the token is an operator
                {
                    if (op.Count == 0)
                    {
                        op.Push(token);
                    }
                    else
                    {
                        temp = op.Peek();
                        if (getPriority(temp) <= getPriority(token))
                        {
                            op.Push(token);
                        }
                        else
                        {
                            output.Push(op.Pop());
                        }
                    }
                }
            }
            foreach (string token in op)
            {
                output.Push(op.Pop());
            }

            foreach(string token in output)
            {
                prefix.Add(token);
            }


            return calculatePrefix(prefix);
        }

        private static string calculatePrefix(ArrayList tokens)
        {
            string answer = "";
            Stack<double> stack = new Stack<double>();
            foreach (string token in tokens)
            {
                if (!isOperater(token))
                {
                    stack.Push(int.Parse(token));
                }
                else
                {
                    double ans = 0;
                    double op1 = stack.Pop();
                    double op2 = stack.Pop();
                    switch (token)
                    {
                        case "+":
                             ans = op1 + op2;
                            break;
                        case "-":
                            ans = op1 - op2;
                            break;
                        case "*":
                            ans = op1 * op2;
                            break;
                        case "÷":
                            ans = op1 / op2;
                            break;
                        default:
                            ans = 0;
                            break;
                    }
                    stack.Push(ans);

                }


            }

            double a = stack.Pop();
            answer = a.ToString();

            return answer;
        }

        private static int getPriority(string s)
        {
            switch (s)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "÷":
                    return 2;
                default:
                    return 0;
            }
        }
        private static bool isOperater(string s)
        {
            switch (s)
            {
                case "÷":
                case "*":
                case "+":
                case "-":
                    return true;
                default:
                    return false;
            }
        }
    }
}
