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
        private double a, b, c, d, f, g;
        private int enterCount = 0;
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
        private string ShowExp()
        {//显示用户输入的表达式
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
        private void startERYuan()
        {
            isERYuan = true;
            closeOp();
            label_all.Visible = false;
            label_erci.Text = "__x + __y  = __";
            label_erci.Visible = true;
            label_eryuan.Visible = true;
            button_erci.Enabled = false;
        }
        private void endERYuan()
        {
            isERYuan = false;
            label_all.Visible = true;
            label_erci.Visible = false;
            label_eryuan.Visible = false;
            openEnter();
            openOp();
            button_equal.Enabled = true;
            button_delete.Enabled = true;
            button_clear_enter.Enabled = true;
            button_eryuan.Enabled = true;
            label_one.Visible = true;
            label_er.Visible = false;
            enterCount = 0;
            button_erci.Enabled = true;
            Reset();
        }
        private void startERCI()
        {
            isERCi = true;
            closeOp();
            label_all.Visible = false;
            label_erci.Text = "__x² + __x + __ = 0";
            label_erci.Visible = true;
            button_eryuan.Enabled = false;
        }
        private void endERCI()
        {
            isERCi = false;   
            label_all.Visible = true;
            label_erci.Visible = false;
            openEnter();
            openOp();
            button_equal.Visible = true;
            button_delete.Enabled = true;
            button_clear_enter.Enabled = true;
            button_eryuan.Enabled = true;
            label_one.Visible = true;
            label_er.Visible = false;
            enterCount = 0;
            button_erci.Enabled = true;
            Reset();
        }
        private void openEnter()
        {
            button_0.Enabled = true;
            button_1.Enabled = true;
            button_2.Enabled = true;
            button_3.Enabled = true;
            button_4.Enabled = true;
            button_5.Enabled = true;
            button_6.Enabled = true;
            button_7.Enabled = true;
            button_8.Enabled = true;
            button_9.Enabled = true;
            button_dot.Enabled = true;
            isEntering = false;
        }
        private void closeEnter()
        {
            button_0.Enabled = false;
            button_1.Enabled = false;
            button_2.Enabled = false;
            button_3.Enabled = false;
            button_4.Enabled = false;
            button_5.Enabled = false;
            button_6.Enabled = false;
            button_7.Enabled = false;
            button_8.Enabled = false;
            button_9.Enabled = false;
            button_dot.Enabled = false;
            isEntering = false;
        }
        private void openOp()
        {
            button_add.Enabled = true;
            button_sub.Enabled = true;
            button_mul.Enabled = true;
            button_mod.Enabled = true;
            button_reciprocal.Enabled = true;
            button_sqrt.Enabled = true;
            button_pow2.Enabled = true;
            button_div.Enabled = true;
            button_back.Enabled = true;
        }
        private void closeOp()
        {
            button_add.Enabled = false;
            button_sub.Enabled = false;
            button_mul.Enabled = false;
            button_mod.Enabled = false;
            button_reciprocal.Enabled = false;
            button_sqrt.Enabled = false;
            button_pow2.Enabled = false;
            button_div.Enabled = false;
            button_back.Enabled = false;
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
            if (isERCi)
            {
                if (isEntering) //已经输入了部分，将要继续输入
                {
                    if (button.Text != "." || !label_one.Text.Contains("."))  //不是. 或是还没有.，添加到尾部即可
                    {
                        label_one.Text += button.Text;
                    }
                }
                else //之前还未输入，是开始新的输入
                {
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
                return;
            }
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
            if (isERCi)
            {
                label_erci.Text = "__x² + __x + __ = 0";
                label_one.Text = "0";
                isEntering = false;
                enterCount = 0;
                button_equal.Enabled = true;
                button_clear_enter.Enabled = true;
                button_delete.Enabled = true;
                label_er.Visible = false;
                label_one.Visible = true;
                openEnter();
                return;
            }
            if (isERYuan)
            {
                label_erci.Text = "__x + __y = __";
                label_eryuan.Text = "__x + __y = __";
                label_one.Text = "0";
                isEntering = false;
                enterCount = 0;
                button_equal.Enabled = true;
                button_clear_enter.Enabled = true;
                button_delete.Enabled = true;
                label_er.Visible = false;
                label_one.Visible = true;
                openEnter();
                return;
            }
            if (isErrored)
            {
                ErrorHandled();
            }
            Reset();
        }


        private void button_erci_Click(object sender, EventArgs e)
        {
            if (isERCi)
            {
                endERCI();
            }
            else
            {
                startERCI();
            }
        }

        private void button_eryuan_Click(object sender, EventArgs e)
        {
            if (isERYuan)
            {
                endERYuan();
            }
            else
            {
                startERYuan();
            }
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
            if (isCalculated) { //已经计算过，只是更改运算符
                ops.RemoveAt(ops.Count - 1);
                nowOp = ops.Last();
                label_all.Text = label_all.Text.Remove(label_all.Text.Length - 1) + op;
            }else {//还未计算
                if (isFirstClick) { //是第一次点
                    result = double.Parse(label_one.Text);
                    if (isSingledClicked) { //点了单目运算符，直接在后面添加+，不用把输入数在添加进表达式显示框中，应为前面单目运算处理时已经添加过了
                        label_all.Text += op;
                    }else {
                        label_all.Text += result.ToString() + op;
                        numbers.Add(label_one.Text);
                    }
                    isFirstClick = false;
                }else {
                    DoFunc();
                    if (isErrored)
                        return;
                    if (isSingledClicked)
                        label_all.Text += op;
                    else{
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
            if (isERCi)
            {
                enterCount++;
                switch (enterCount)
                {
                    case 1:
                        a = double.Parse(label_one.Text);
                        label_erci.Text = a.ToString() + " x² + __x + __ = 0";
                        break;
                    case 2:
                        b = double.Parse(label_one.Text);
                        label_erci.Text = a.ToString() + " x² + " + b.ToString() + " x + __ = 0";
                        break;
                    case 3:
                        c = double.Parse(label_one.Text);
                        label_erci.Text = a.ToString() + " x² + " + b.ToString() + " x + " + c.ToString() + " = 0";
                        break;
                    default:
                        if ((b * b - 4 * a * c) < 0)
                        {
                            label_er.Text = "方程无解";
                        }
                        else
                        {
                            label_er.Text = "x₁ = " + Function.erci(a, b, c, 1).ToString() + " x₂ = " + Function.erci(a, b, c, 0);
                        }
                        label_er.Visible = true;
                        label_one.Visible = false;
                        button_equal.Enabled = false;
                        button_clear_enter.Enabled = false;
                        button_delete.Enabled = false;
                        break;
                }
                isEntering = false;
                if(enterCount == 3)
                {
                    closeEnter();
                }
                return;
            }
            if (isERYuan)
            {
                enterCount++;
                switch (enterCount)
                {
                    case 1:
                        a = double.Parse(label_one.Text);
                        label_erci.Text = a.ToString() + " x + __y = __";
                        break;
                    case 2:
                        b = double.Parse(label_one.Text);
                        label_erci.Text = a.ToString() + " x + " + b.ToString() + " y = __";
                        break;
                    case 3:
                        c = double.Parse(label_one.Text);
                        label_erci.Text = a.ToString() + " x + " + b.ToString() + " y  = " + c.ToString();
                        break;
                    case 4:
                        d = double.Parse(label_one.Text);
                        label_eryuan.Text = d.ToString() + " x + __y = __";
                        break;
                    case 5:
                        f = double.Parse(label_one.Text);
                        label_eryuan.Text = d.ToString() + " x + " + f.ToString() + " y = __";
                        break;
                    case 6:
                        g = double.Parse(label_one.Text);
                        label_eryuan.Text = d.ToString() + " x + " + f.ToString() + " y  = " + g.ToString();
                        break;
                    default:
                        if ((a * f - b * d) == 0)
                        {
                            if((b*g - c*f) == 0)
                            {
                                label_er.Text = "方程有无穷解";
                            }
                            else
                            {
                                label_er.Text = "方程无解";
                            }
                            
                        }
                        else
                        {
                            label_er.Text = "x = " + Function.eryuan(a, b, c, d, f, g, 1).ToString() + " y = " + Function.eryuan(a, b, c, d, f, g, 0);
                        }
                        label_er.Visible = true;
                        label_one.Visible = false;
                        button_equal.Enabled = false;
                        button_clear_enter.Enabled = false;
                        button_delete.Enabled = false;
                        break;
                }
                isEntering = false;
                if (enterCount == 7)
                {
                    closeEnter();
                }
                return;
            }
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
