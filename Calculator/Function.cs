using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Function
    {
        [DllImport("FunctionDll.dll", EntryPoint = "add", CallingConvention = CallingConvention.Cdecl)]
        public static extern double add(double a, double b);

        [DllImport("FunctionDll.dll", EntryPoint = "sub", CallingConvention = CallingConvention.Cdecl)]
        public static extern double sub(double a, double b);


        [DllImport("FunctionDll.dll", EntryPoint = "mul", CallingConvention = CallingConvention.Cdecl)]
        public static extern double mul(double a, double b);


        [DllImport("FunctionDll.dll", EntryPoint = "mydiv", CallingConvention = CallingConvention.Cdecl)]
        public static extern double div(double a, double b);

        [DllImport("FunctionDll.dll", EntryPoint = "mod", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mod(int a, int b);

        [DllImport("FunctionDll.dll", EntryPoint = "mysqrt", CallingConvention = CallingConvention.Cdecl)]
        public static extern double sqrt(double a);

        [DllImport("FunctionDll.dll", EntryPoint = "pow2", CallingConvention = CallingConvention.Cdecl)]
        public static extern double pow2(double a);

        [DllImport("FunctionDll.dll", EntryPoint = "pow3", CallingConvention = CallingConvention.Cdecl)]
        public static extern double pow3(double a);

        [DllImport("FunctionDll.dll", EntryPoint = "reciprocal", CallingConvention = CallingConvention.Cdecl)]
        public static extern double reciprocal(double a);
    }
}
