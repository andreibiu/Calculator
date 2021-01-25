using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator
{
    public class MainWindow : Window
    {
        #region Nested Types

        static class Buttons
        {
            public const int Sin = 0;
            public const int Cos = 1;
            public const int Tan = 2;
            public const int Log = 3;
            public const int Abs = 4;
            public const int LeftParenthesis = 5;
            public const int RightParenthesis = 6;
            public const int Add = 14;
            public const int Substract = 19;
            public const int Multiply = 24;
            public const int Divide = 29;
            public const int Modulo = 9;
            public const int Sqrt = 7;
            public const int Pow = 8;
            public const int Equal = 28;
            public const int Comma = 26;
            public const int Seven = 11;
            public const int Eight = 12;
            public const int Nine = 13;
            public const int Four = 16;
            public const int Five = 17;
            public const int Six = 18;
            public const int One = 21;
            public const int Two = 22;
            public const int Three = 23;
            public const int Zero = 27;
            public const int Clear = 5;
            public const int ClearExpresion = 10;
            public const int ClearHistory = 15;
            public const int HistoryUp = 20;
            public const int HistoryDown = 25;

            public const int Count = 30;
        }

        static class DesignConstants
        {
            public const double WindowWidth = 370;
            public const double WindowHeight = 550;
            public const double MarginThickness = 10;
            public const double ContentWidth = WindowWidth - 2 * MarginThickness;
            public const double ContentHeight = WindowHeight - 2 * MarginThickness;
            public const double ButtonWidth = 70;
            public const double ButtonHeight = 70;
            public const double ResultTextBlockHeight = 65;
            public const double HexResultTextBlockHeight = 30;
            public const double BinResultTextBlockHeight = 30;
            public const double ResultsTextBlockWidth = OperationTextBlockWidth / 2;
            public const double OperationTextBlockHeight = 30;
            public const double OperationTextBlockWidth = WindowWidth - 2 * MarginThickness;

            public const string WindowTitle = "Calculator";
            public const int ButtonsRowCount = 5;
            public const double ButtonFontSize = 16;
            public const double ExpersionFontSize = 20;
            public const double ResultFontSize = 24;
            public const double BinHexResultFontSize = 10;
        }

        #endregion

        #region Constants & Readonly Values

        const int MaxHistoryCount = 50;

        private const string HexLabel = "HEX ";
        private const string BinLabel = "BIN ";
        private const string ResultFormat = "0.#########";

        #endregion

        #region Static Methods

        private static string GetButtonContent(int buttonIndex)
        {
            switch (buttonIndex)
            {
                case Buttons.Sin: return "sin";
                case Buttons.Cos: return "cos";
                case Buttons.Tan: return "tan";
                case Buttons.Log: return "log";
                case Buttons.Abs: return "abs";
                case Buttons.LeftParenthesis: return "(";
                case Buttons.RightParenthesis: return ")";
                case Buttons.Add: return "+";
                case Buttons.Substract: return "-";
                case Buttons.Multiply: return "*";
                case Buttons.Divide: return "/";
                case Buttons.Modulo: return "%";
                case Buttons.Sqrt: return "√";
                case Buttons.Pow: return "^";
                case Buttons.Equal: return "=";
                case Buttons.Comma: return ",";
                case Buttons.Seven: return "7";
                case Buttons.Eight: return "8";
                case Buttons.Nine: return "9";
                case Buttons.Four: return "4";
                case Buttons.Five: return "5";
                case Buttons.Six: return "6";
                case Buttons.One: return "1";
                case Buttons.Two: return "2";
                case Buttons.Three: return "3";
                case Buttons.Zero: return "0";
                case Buttons.ClearExpresion: return "CE";
                case Buttons.ClearHistory: return "CH";
                case Buttons.HistoryUp: return "↑";
                case Buttons.HistoryDown: return "↓";
                default: return "?";
            }
        }

        #endregion

        #region Components & Fields

        private Canvas content;
        private TextBlock expressionTextBlock;
        private TextBlock resultTextBlock;
        private TextBlock hexResultTextBlock;
        private TextBlock binResultTextBlock;
        private Button[] buttons;

        private List<string> operationsHistory;

        #endregion

        #region Constructor

        public MainWindow()
        {
            Width = DesignConstants.WindowWidth;
            Height = DesignConstants.WindowHeight;
            Margin = new Thickness(DesignConstants.MarginThickness);
            Title = DesignConstants.WindowTitle;
            this.ResizeMode = ResizeMode.CanMinimize;

            content = new Canvas { Width = DesignConstants.ContentWidth, Height = DesignConstants.ContentHeight };

            expressionTextBlock = new TextBlock() {
                Width = DesignConstants.OperationTextBlockWidth,
                Height = DesignConstants.OperationTextBlockHeight,
                FontSize = DesignConstants.ExpersionFontSize,
                Foreground = new SolidColorBrush { Color = Colors.Black },
                FontFamily = new FontFamily("Segoe UI"),
                FontWeight = FontWeights.Thin,
                TextAlignment = TextAlignment.Right
            };
            Canvas.SetLeft(expressionTextBlock, 0);
            Canvas.SetTop(expressionTextBlock, 0);
            content.Children.Add(expressionTextBlock);

            resultTextBlock = new TextBlock() {
                Width = DesignConstants.ResultsTextBlockWidth,
                Height = DesignConstants.ResultTextBlockHeight,
                FontFamily = new FontFamily("Segoe UI"),
                FontWeight = FontWeights.SemiBold,
                TextAlignment = TextAlignment.Right,
                FontSize = DesignConstants.ResultFontSize,
            };
            Canvas.SetLeft(resultTextBlock, DesignConstants.ResultsTextBlockWidth);
            Canvas.SetTop(resultTextBlock, DesignConstants.OperationTextBlockHeight);
            content.Children.Add(resultTextBlock);

            hexResultTextBlock = new TextBlock() {
                Width = DesignConstants.ResultsTextBlockWidth,
                Height = DesignConstants.HexResultTextBlockHeight,
                FontFamily = new FontFamily("Segoe UI"),
                FontWeight = FontWeights.Bold,
                Text = HexLabel
            };
            Canvas.SetLeft(hexResultTextBlock, 0);
            Canvas.SetTop(hexResultTextBlock, DesignConstants.OperationTextBlockHeight);
            content.Children.Add(hexResultTextBlock);

            binResultTextBlock = new TextBlock {
                Width = DesignConstants.ResultsTextBlockWidth,
                Height = DesignConstants.BinResultTextBlockHeight,
                Background = new SolidColorBrush { Color = Colors.White }
            };
            Canvas.SetLeft(binResultTextBlock, 0);
            Canvas.SetTop(binResultTextBlock, DesignConstants.OperationTextBlockHeight + DesignConstants.HexResultTextBlockHeight);
            content.Children.Add(binResultTextBlock);

            binResultTextBlock = new TextBlock() {
                Width = DesignConstants.ResultsTextBlockWidth,
                Height = DesignConstants.HexResultTextBlockHeight,
                FontFamily = new FontFamily("Segoe UI"),
                FontWeight = FontWeights.Bold,
                Text = BinLabel
            };
            Canvas.SetLeft(binResultTextBlock, 0);
            Canvas.SetTop(binResultTextBlock, DesignConstants.OperationTextBlockHeight + DesignConstants.HexResultTextBlockHeight);
            content.Children.Add(binResultTextBlock);

            double top = DesignConstants.OperationTextBlockHeight + DesignConstants.ResultTextBlockHeight;
            buttons = new Button[Buttons.Count];
            for (int index = 0; index < Buttons.Count; index++)
            {
                buttons[index] = new Button { Width = DesignConstants.ButtonWidth, Height = DesignConstants.ButtonHeight };
                buttons[index].Content = new TextBlock {
                    Text = GetButtonContent(index),
                    FontSize = DesignConstants.ButtonFontSize,
                    FontWeight = FontWeights.DemiBold
                };
                buttons[index].Click += OnButtonClicked;
                Canvas.SetLeft(buttons[index], (index % DesignConstants.ButtonsRowCount) * DesignConstants.ButtonWidth);
                Canvas.SetTop(buttons[index], top + (index / DesignConstants.ButtonsRowCount) * DesignConstants.ButtonHeight);
                content.Children.Add(buttons[index]);
            }

            Content = content;
            
            operationsHistory = new List<string>();
        }

        #endregion

        #region Event Handlers

        private void OnButtonClicked(object sender, RoutedEventArgs args)
        {
            TextBlock buttonContent = (sender as Button).Content as TextBlock;
            string symbol = buttonContent.Text;

            if (symbol == "CE")
            {
                expressionTextBlock.Text = "";
            }
            else if (symbol == "CH")
            {
                operationsHistory.Clear();
            }
            else if (IsFunction(symbol))
            {
                expressionTextBlock.Text += symbol + "(";
            }
            else if (symbol == "=")
            {
                DoComputation();
                expressionTextBlock.Text = "";
            }
            else if (symbol == "↑")
            {
                string previousExpresion = PreviousInHistory(expressionTextBlock.Text);
                if (previousExpresion != null)
                    expressionTextBlock.Text = previousExpresion;
            }
            else if (symbol == "↓")
            {
                string nextExpresion = NextInHistory(expressionTextBlock.Text);
                if (nextExpresion != null)
                    expressionTextBlock.Text = nextExpresion;
                else
                    expressionTextBlock.Text = "";
            }
            else
            {
                expressionTextBlock.Text += symbol;
            }
        }

        #endregion

        #region Methods

        private bool IsFunction(string operation)
        {
            if (operation == "sin" || operation == "cos" || operation == "tan" || operation == "log" || operation == "abs")
                return true;
            else
                return false;
        }

        private string PreviousInHistory(string curentOperation)
        {
            if (operationsHistory.Count == 0)
                return null;
            
            int index = operationsHistory.IndexOf(curentOperation);

            if (index == -1)
                return operationsHistory[operationsHistory.Count - 1];
            else if (index == 0)
                return null;
            else
                return operationsHistory[index - 1];
        }

        private string NextInHistory(string curentOperation)
        {
            if (operationsHistory.Count == 0)
                return null;

            int index = operationsHistory.IndexOf(curentOperation);

            if (index == -1 || index == operationsHistory.Count - 1)
                return null;
            else
                return operationsHistory[index + 1];
        }

        private void SetResult(string result)
        {
            long resultAsInteger;

            resultTextBlock.Text = result;
            if (long.TryParse(result, out resultAsInteger))
            {
                hexResultTextBlock.Text = HexLabel + resultAsInteger.ToString("X");
                binResultTextBlock.Text = BinLabel + Convert.ToString(resultAsInteger, 2);
            }
            else
            {
                hexResultTextBlock.Text = HexLabel + "- ";
                binResultTextBlock.Text = BinLabel + "- ";
            }
        }

        private void DoComputation()
        {
            string expression = expressionTextBlock.Text;

            if (expression == "" || expression == null)
            {
                resultTextBlock.Text = "";
                hexResultTextBlock.Text = HexLabel;
                binResultTextBlock.Text = BinLabel;
                return;
            }

            double result = Engine.Compute(expression);

            if (operationsHistory.Count < MaxHistoryCount)
            {
                operationsHistory.Add(expression);
            }
            else
            {
                operationsHistory.RemoveAt(0);
                operationsHistory.Add(expression);
            }

            SetResult(result.ToString(ResultFormat));
        }

        #endregion
    }
}
