using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private Point point;    //当前位置，用于窗口移动
        private double result = 0;
        private bool isEntering = false;    //是否处于输出状态中
        private bool isCalculated = false; //是否已经计算
        private bool isFirstClick = true;   //是否是第一次点击 + - x / %
        private bool isSingledClicked = false;  //是否已经点击单目运算符
        private bool isFirstEqualClick = true;  //是否是第一次点=
        private double secondeValue = 0;    //记住值，连续按=时有用
        private bool isErrored = false;
        private char nowOp = '+';
        private List<String> numbers = new List<String>();
        private List<char> ops = new List<char>();
        private bool isERYuan = false; //是否是二元一次方程组模式
        private bool isERCi = false; //是否是一元二次方程模式
        private double a, b, c, d, e, f;
        public Form1()
        {
            InitializeComponent();
        }

        private bool IsInt(double a)
        {
            const double ESP = 1e-6;
            if(a - Math.Floor(a) < ESP)
            {
                return true;
            }
            return false;
        }
        private void DoFunc()
        {
            double nowNumber = double.Parse(label_one.Text);
            switch (nowOp)
            {
                case '+':
                    result =  Function.add(result, nowNumber);
                    break;
                case '-':
                    result = Function.sub(result, nowNumber);
                    break;
                case '×':
                    result = Function.mul(result, nowNumber);
                    break;
                case '÷':                  
                    try
                    {
                        if (nowNumber == 0)
                        {
                            throw new CalculatorException("除数不能为零");
                        }
                        result = Function.div(result, nowNumber);
                    }
                    catch(CalculatorException ex)
                    {
                        ErrorHandler(ex.Message);
                    }
                    break;
                default:
                    try
                    {
                        if (!IsInt(nowNumber) || !IsInt(result))
                        {
                            throw new CalculatorException("取余的操作数不能为小数");
                        }
                        result =  Function.mod(Convert.ToInt32(result), Convert.ToInt32(nowNumber));
                    }
                    catch(CalculatorException ex)
                    {
                        ErrorHandler(ex.Message);
                    }
                    break;
            }
        }

        private void Reset()
        {
            result = 0;
            isEntering = false;    //是否处于输出状态中
            isCalculated = false; //是否已经计算
            isFirstClick = true;   //是否是第一次点击 + - x / %
            isSingledClicked = false; //是否已经点击单目运算符
            nowOp = '+';
            numbers.Clear();
            ops.Clear();
            label_one.Text = "0";
            label_all.Text = "";
            isFirstEqualClick = true;
            secondeValue = 0;
        }
        private void ErrorHandler(String message)
        {
            isErrored = true;
            button_add.Enabled = false;
            button_sub.Enabled = false;
            button_mul.Enabled = false;
            button_div.Enabled = false;
            button_mod.Enabled = false;
            button_sqrt.Enabled = false;
            button_equal.Enabled = false;
            button_pow2.Enabled = false;
            button_delete.Enabled = false;
            button_reciprocal.Enabled = false;
            label_one.Text = message;
            label_all.Text = "";
        }

        private void ErrorHandled()
        {
            isErrored = false;
            button_add.Enabled = true;
            button_sub.Enabled = true;
            button_mul.Enabled = true;
            button_div.Enabled = true;
            button_mod.Enabled = true;
            button_sqrt.Enabled = true;
            button_equal.Enabled = true;
            button_pow2.Enabled = true;
            button_delete.Enabled = true;
            button_reciprocal.Enabled = true;
            Reset();
        }
        private string ShowExp() //显示用户输入的表达式
        {
            String exp = "";
            for(int i=0; i<numbers.Count; i++)
            {
                if (i < ops.Count)
                {
                    exp += numbers.ElementAt(i) + ops.ElementAt(i);
                }
                else
                {
                    exp += numbers.ElementAt(i);
                }
            }
            return exp;
        }

        private double ToNumber(String num)
        {
            if (num.Contains("²") || num.Contains("³"))
            {
                num = num.Substring(1, num.Length-3);
            }else if (num.Contains("/"))
            {
                num = num.Substring(3, num.Length-4);
            }else if (num.Contains("√"))
            {
                num = num.Substring(2, num.Length - 3);
            }
            return double.Parse(num);
        }
        private void CalculateExp()
        {
            if(numbers.Count == 0)
            {
                return;
            }
            double tempResult = ToNumber(numbers.ElementAt(0));
            int p = 0;
            foreach(char op in ops)
            {
                p++;
                if (p >= numbers.Count)
                {
                    break;
                }
                switch (op)
                {
                    case '+':
                        tempResult = Function.add(tempResult, ToNumber(numbers.ElementAt(p)));
                        break;
                    case '-':
                        tempResult = Function.sub(tempResult, ToNumber(numbers.ElementAt(p)));
                        break;
                    case '×':
                        tempResult = Function.mul(tempResult, ToNumber(numbers.ElementAt(p)));
                        break;
                    case '÷':
                        tempResult = Function.div(tempResult, ToNumber(numbers.ElementAt(p)));
                        break;
                    default:
                        tempResult = Function.mod(Convert.ToInt32(tempResult), Convert.ToInt32(ToNumber(numbers.ElementAt(p))));
                        break;
                }
            }
            result = tempResult;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            point = new Point(e.X, e.Y);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - point.X, this.Location.Y + e.Y - point.Y);
            }
        }

        private void button_min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button_number_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (isErrored)
            {
                ErrorHandled();
            }
            if (isEntering) //已经输入了部分，将要继续输入
            {
                if(button.Text != "." || !label_one.Text.Contains("."))  //不是. 或是还没有.，添加到尾部即可
                {
                    label_one.Text += button.Text;
                }
            }
            else //之前还未输入，是开始新的输入
            {
                if (isSingledClicked)
                {
                    numbers.RemoveAt(numbers.Count - 1);
                    label_all.Text = ShowExp();
                    isSingledClicked = false;
                }
                if (button.Text == ".")  //
                {
                    label_one.Text = "0.";
                }
                else
                {
                    label_one.Text = button.Text;
                }
                isEntering = true;
            }
            isCalculated = false;
            isFirstEqualClick = true;
        }

        private void button_back_Click(object sender, EventArgs e)  //回退一步计算
        {
            if (isErrored)
            {
                ErrorHandled();
            }
            if (numbers.Count <= 1)
            {
                Reset();
            }
            else
            {
                if(ops.Count == numbers.Count)
                {
                    ops.RemoveAt(ops.Count - 1);
                    nowOp = ops.Last();
                }
                numbers.RemoveAt(numbers.Count - 1);
                label_all.Text = ShowExp();
                CalculateExp();
                label_one.Text = "0";
                isCalculated = true;
                isEntering = false;
                
            }

        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            if (isErrored)
            {
                ErrorHandled();
            }
            Reset();
        }

        private void button_clear_enter_Click(object sender, EventArgs e)
        {
            if (isErrored)
            {
                ErrorHandled();
            }
            if (isCalculated)
            {
                Reset();
            }
            else
            {
                label_one.Text = "0";
                isEntering = false;
            }
        }

        private void button_single_op_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            String op = button.Text;
            String number = "";
            switch (op)
            {
                case "1/x":
                    number = "1/(" + label_one.Text + ")";
                    label_one.Text = Function.reciprocal(double.Parse(label_one.Text)).ToString();
                    break;
                case "√x":
                    number = "√(" + label_one.Text + ")";
                    label_one.Text = Function.sqrt(double.Parse(label_one.Text)).ToString();
                    break;
                case "x²":
                    number = "(" + label_one.Text + ")²";
                    label_one.Text = Function.pow2(double.Parse(label_one.Text)).ToString();
                    break;
                case "x³":
                    number = "(" + label_one.Text + ")³";
                    label_one.Text = Function.pow3(double.Parse(label_one.Text)).ToString();
                    break;
            }
            if (label_all.Text == "")
            {
                label_all.Text += number;
                numbers.Add(number);
            }
            else
            {
                char last = label_all.Text.Last();
                if (last == '+' || last == '-' || last == '×' || last == '÷' || last == '%')
                {
                    label_all.Text += number;
                    numbers.Add(number);
                }
                else
                {
                    numbers.RemoveAt(numbers.Count - 1);
                    numbers.Add(number);
                    label_all.Text = ShowExp();
                }
            }
            isEntering = false;
            isSingledClicked = true;
            isFirstEqualClick = true;
        }

        private void button_double_op_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            String op = button.Text;
            if (isCalculated)  //已经计算过，只是更改运算符
            {
                ops.RemoveAt(ops.Count - 1);
                nowOp = ops.Last();
                label_all.Text = label_all.Text.Remove(label_all.Text.Length - 1) + op;
            }
            else //还未计算
            {
                if (isFirstClick)  //是第一次点
                {
                    result = double.Parse(label_one.Text);
                    if (isSingledClicked)  //点了单目运算符，直接在后面添加+，不用把输入数在添加进表达式显示框中，应为前面单目运算处理时已经添加过了
                    {
                        label_all.Text += op;
                    }
                    else
                    {
                        label_all.Text += result.ToString() + op;
                        numbers.Add(label_one.Text);
                    }
                    isFirstClick = false;
                }
                else
                {
                    DoFunc();
                    if (isErrored)
                        return;
                    if (isSingledClicked)
                    {
                        label_all.Text += op;
                    }
                    else
                    {
                        label_all.Text += label_one.Text + op;
                        numbers.Add(label_one.Text);
                    }
                    label_one.Text = result.ToString();
                    isCalculated = true;
                }
            }
            ops.Add(op.First());
            nowOp = ops.Last();
            isEntering = false;
            isSingledClicked = false;
            isFirstEqualClick = true;
        }

        private void button_equal_Click(object sender, EventArgs e)
        {
            isEntering = false;    //是否处于输出状态中
            isCalculated = false; //是否已经计算
            isFirstClick = true;   //是否是第一次点击 + - x / %
            isSingledClicked = false; //是否已经点击单目运算符
            numbers.Clear();
            ops.Clear();
            label_all.Text = "";
            if (isFirstEqualClick)
            {
                secondeValue = double.Parse(label_one.Text);
                isFirstEqualClick = false;
            }
            switch (nowOp)
            {
                case '+':
                    result = Function.add(result, secondeValue);
                    label_one.Text = result.ToString();
                    break;
                case '-':
                    result = Function.sub(result, secondeValue);
                    label_one.Text = result.ToString();
                    break;
                case '×':
                    result = Function.mul(result, secondeValue);
                    label_one.Text = result.ToString();
                    break;
                case '÷':
                    try
                    {
                        if (secondeValue == 0)
                        {
                            throw new CalculatorException("除数不能为零");
                        }
                        result = Function.div(result, secondeValue);
                        label_one.Text = result.ToString();
                    }
                    catch (CalculatorException ex)
                    {
                        ErrorHandler(ex.Message);
                    }
                    break;
                default:
                    try
                    {
                        if (!IsInt(secondeValue) || !IsInt(result))
                        {
                            throw new CalculatorException("取余的操作数不能为小数");
                        }
                        result = Function.mod(Convert.ToInt32(result), Convert.ToInt32(secondeValue));
                        label_one.Text = result.ToString();
                    }
                    catch (CalculatorException ex)
                    {
                        ErrorHandler(ex.Message);
                    }
                    break;
            }
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (isErrored)
            {
                ErrorHandled();
            }
            if (isCalculated)
            {
                Reset();
            }
            else
            {
                if (label_one.Text.Length <= 1)
                {
                    label_one.Text = "0";
                    isEntering = false;
                }
                else
                {
                    label_one.Text = label_one.Text.Remove(label_one.Text.Length - 1, 1);
                }
            }
        }
    }
}
