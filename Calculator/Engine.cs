using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    static class Engine
    {
        #region Operations Methods

        private static double Add(double lArg, double rArg) => lArg + rArg;
        private static double Subtract(double lArg, double rArg) => lArg - rArg;
        private static double Multiply(double lArg, double rArg) => lArg * rArg;
        private static double Divide(double lArg, double rArg) => lArg / rArg;
        private static double Modulo(double lArg, double rArg) => lArg % rArg;

        #endregion

        #region Delegates

        private delegate double UnaryFunction(double arg);
        private delegate double BinaryFunction(double lArg, double rArg);

        #endregion

        #region Nested Types

        private abstract class Operation : IComparable<Operation>
        {
            public Operation(int priority, int rightOperandIndex, int orderIndex)
            {
                Priority = priority;
                RightOperandIndex = rightOperandIndex;
                OrderIndex = orderIndex;
            }
            
            public int Priority { get; private set; }
            public int RightOperandIndex { get; set; }
            public int OrderIndex { get; private set; }

            public int CompareTo(Operation other)
            {
                if (Priority == other.Priority)
                {
                    if (RightOperandIndex == other.RightOperandIndex)
                        return -OrderIndex.CompareTo(other.OrderIndex);
                    else
                        return RightOperandIndex.CompareTo(other.RightOperandIndex);
                }
                else
                    return -Priority.CompareTo(other.Priority);
            }

            public abstract void Execute(List<double> operands);
        }

        private class UnaryOperation : Operation
        {
            private UnaryFunction function;
            
            public UnaryOperation(int priority, int rightOperandIndex, int orderIndex, UnaryFunction function) : base(priority, rightOperandIndex, orderIndex)
            {
                this.function = function;
            }

            public override void Execute(List<double> operands)
            {
                operands[RightOperandIndex] = function(operands[RightOperandIndex]);
            }
        }

        private class BinaryOperation : Operation
        {
            private BinaryFunction function;

            public BinaryOperation(int priority, int rightOperandIndex, int orderIndex, BinaryFunction function) : base(priority, rightOperandIndex, orderIndex)
            {
                this.function = function;
            }

            public override void Execute(List<double> operands)
            {
                operands[RightOperandIndex] = function(operands[RightOperandIndex - 1], operands[RightOperandIndex]);
                operands.RemoveAt(RightOperandIndex - 1);
            }
        }

        #endregion

        #region Readonly Variables

        private static readonly Dictionary<char, int> priorities = new Dictionary<char, int>();

        #endregion

        #region Static Constructor

        static Engine()
        {
            priorities.Add('+', 0);
            priorities.Add('-', 0);
            priorities.Add('*', 1);
            priorities.Add('/', 1);
            priorities.Add('%', 1);
            priorities.Add('√', 2);
            priorities.Add('^', 2);
            priorities.Add('s', 3);
            priorities.Add('c', 3);
            priorities.Add('t', 3);
            priorities.Add('l', 3);
            priorities.Add('a', 3);
        }

        #endregion

        #region Static Methods

        public static double Compute(string expression)
        {
            int currentIndex = 0;
                
            try
            {
                return Process(expression, ref currentIndex);
            }
            catch
            {
                return double.NaN;
            }
        }

        private static double Process(string expression, ref int currentIndex)
        {
            List<double> operands = new List<double>();
            List<Operation> operations = new List<Operation>();
            bool wasNumber = false;
            int index, operationIndex = 0;

            if (expression[currentIndex] == '-')
            {
                operands.Add(0);
                operations.Add(ParseBinaryOperator('-', 1, operationIndex++));
                ++currentIndex;
            }
            
            while (true)
            {
                if (currentIndex >= expression.Length)
                    break;

                if (expression[currentIndex] == '(')
                {
                    ++currentIndex;
                    operands.Add(Process(expression, ref currentIndex));
                }

                if (char.IsLetter(expression[currentIndex]))
                {
                    if (wasNumber)
                        operations.Add(ParseBinaryOperator('*', operands.Count, operationIndex++));
                    wasNumber = false;
                    operations.Add(ParseFunction(expression[currentIndex], operands.Count, operationIndex++));
                    currentIndex += 4;
                    operands.Add(Process(expression, ref currentIndex));
                    continue;
                }
                else if (expression[currentIndex] == '√')
                {
                    if (wasNumber)
                        operations.Add(ParseBinaryOperator('*', operands.Count, operationIndex++));
                    wasNumber = false;
                    operations.Add(ParseSqrt(operands.Count, operationIndex++));
                    ++currentIndex;
                    continue;
                }
                else if (expression[currentIndex] == '^')
                {
                    wasNumber = false;
                    operations.Add(ParsePower(operands.Count, operationIndex++));
                    ++currentIndex;
                    continue;
                }
                else
                {
                    var binaryOperator = ParseBinaryOperator(expression[currentIndex], operands.Count, operationIndex++);
                    if (binaryOperator != null)
                    {
                        wasNumber = false;
                        operations.Add(binaryOperator);
                        ++currentIndex;
                        continue;
                    }
                }

                index = currentIndex;
                while (index < expression.Length && (char.IsDigit(expression[index]) || expression[index] == ','))
                    ++index;
                operands.Add(double.Parse(expression.Substring(currentIndex, index - currentIndex)));
                currentIndex = index;
                wasNumber = true;

                if (currentIndex >= expression.Length)
                    break;

                if (expression[currentIndex] == ')')
                {
                    ++currentIndex;
                    break;
                }
            }

            operations.Sort();

            foreach (Operation operation in operations)
            {
                operation.Execute(operands);
                if (operation is BinaryOperation)
                {
                    foreach (var otherOperation in operations.Where(otherOperation => otherOperation.RightOperandIndex > operation.RightOperandIndex))
                        --otherOperation.RightOperandIndex;
                }
            }

            return operands[0];
        }

        private static Operation ParseSqrt(int rightOperandIndex, int orderIndex) => new UnaryOperation(priorities['√'], rightOperandIndex, orderIndex, Math.Sqrt);

        private static Operation ParsePower(int rightOperandIndex, int orderIndex) => new BinaryOperation(priorities['^'], rightOperandIndex, orderIndex, Math.Pow);

        private static Operation ParseFunction(char operationSymbol, int rightOperandIndex, int orderIndex)
        {
            switch (operationSymbol)
            {
                case 's': return new UnaryOperation(priorities['s'], rightOperandIndex, orderIndex, Math.Sin);
                case 'c': return new UnaryOperation(priorities['c'], rightOperandIndex, orderIndex, Math.Cos);
                case 't': return new UnaryOperation(priorities['t'], rightOperandIndex, orderIndex, Math.Tan);
                case 'l': return new UnaryOperation(priorities['l'], rightOperandIndex, orderIndex, Math.Log10);
                case 'a': return new UnaryOperation(priorities['a'], rightOperandIndex, orderIndex, Math.Abs);
                default: return null;
            }
        }

        private static Operation ParseBinaryOperator(char operationSymbol, int rightOperandIndex, int orderIndex)
        {
            switch (operationSymbol)
            {
                case '+': return new BinaryOperation(priorities['+'], rightOperandIndex, orderIndex, Add);
                case '-': return new BinaryOperation(priorities['-'], rightOperandIndex, orderIndex, Subtract);
                case '*': return new BinaryOperation(priorities['*'], rightOperandIndex, orderIndex, Multiply);
                case '/': return new BinaryOperation(priorities['/'], rightOperandIndex, orderIndex, Divide);
                case '%': return new BinaryOperation(priorities['%'], rightOperandIndex, orderIndex, Modulo);
                default: return null;
            }
        }

        #endregion
    }
}
