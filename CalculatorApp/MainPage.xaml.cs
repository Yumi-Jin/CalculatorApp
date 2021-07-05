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
        ArrayList operands = new ArrayList();
        int opindex = 0;

        void OnButtonClicked(object sender, EventArgs e)
        {
            string tempEquation = this.equation.Text;
            string pressed = ((Button)sender).Text;

            switch (pressed)
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
                        operands.Add(pressed); // creates a new element in the array list
                        opindex++;
                        this.equation.Text = tempEquation + pressed;
                    }
                    else if (state == 2) // if the character previously entered is an operator
                    {
                        // Replaces the previous operator (excluding the negative sign)
                        operands[opindex] = pressed; // replaces the last element in the arraylist
                        this.equation.Text = tempEquation.Substring(0, tempEquation.Length - 1) + pressed;
                    }
                    break;
                case "-":
                    Console.WriteLine("Case -");
                    if ((state == 0) || (state == 2)) // previous isn't an operator or has nothing
                    {
                        state = 3; //set to negative
                        this.equation.Text = tempEquation + pressed;

                        operands.Add(pressed);
                        opindex++;
                    }
                    else if (state == 1) // if previous was a number
                    {
                        // Sets the state to be an operator pressed
                        // Add to the equation label
                        state = 2;
                        operands.Add(pressed); // creates a new element in the array list
                        opindex++;
                        this.equation.Text = tempEquation + pressed;

                    }
                    break;
                case "=":
                    if (!((state == 0) || ((tempEquation.Length == 1) && (state == 3)))) //if there is at least something
                    {
                        if (!(state == 1)) // if equation ended with an operator
                        {
                            this.equation.Text = tempEquation.Substring(0, tempEquation.Length - 1);
                            operands.RemoveAt(operands.Count - 1);
                        }
                        state = 0;
                        Console.WriteLine("Case =");

                        // Calculates the equation
                        var result = Calculate.InfixToPrefix(operands);

                        this.equation.Text = "";
                        this.answer.Text = result;


                        foreach(string token in operands)
                        {
                            Console.WriteLine("token: " + token);
                        }

                        // clears out the array list
                        opindex = 0;
                        operands.Clear();
                    }
                    break;
                default:
                    if ((state == 3)|| (state == 1)) // if it's negative or a number was pressed
                    {
                        operands[opindex - 1] = operands[opindex - 1] + pressed;
                    }
                    else
                    {
                        operands.Add(pressed);
                        opindex++;
                    }
                    state = 1;

                    Console.WriteLine("Numbers");
                    this.equation.Text = tempEquation + pressed;
                    break;
            }
        }
    }

        
    public static class Calculate
    {
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
                            temp = op.Pop();
                            output.Push(temp);
                        }
                    }
                }
            }
            int stacksize = op.Count();

            for (int i = 0; i < stacksize; i++) //pop out all the tokens in op stack
            {
                temp = op.Pop();
                output.Push(temp);
            }

            foreach(string token in output) //pop out all the tokens in the output into an arraylist
            {
                prefix.Add(token);
            }


            foreach (string token in prefix)
            {
                Console.WriteLine("token: " + token);
            }

            return calculatePrefix(prefix);
        }

        private static string calculatePrefix(ArrayList tokens)
        {
            string answer = "";
            Stack<double> stack = new Stack<double>();
            tokens.Reverse();
            foreach (string token in tokens)
            {
                if (!isOperater(token)) //checks if it is an operand
                {
                    double operand = double.Parse(token);
                    stack.Push(operand);
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

            double a = stack.Peek();
            stack.Clear();
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
