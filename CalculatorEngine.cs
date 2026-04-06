using System;
using System.Collections.Generic;

namespace WinFormsCalculator
{
    /// <summary>
    /// Pure calculation engine — no UI dependencies.
    /// Handles basic arithmetic, scientific functions, memory, history, and error states.
    /// </summary>
    public class CalculatorEngine
    {
        // ─── State ───────────────────────────────────────────────────────────
        public string CurrentInput { get; private set; } = "0";
        public string Expression   { get; private set; } = "";
        public string ErrorMessage { get; private set; } = "";
        public bool   HasError     { get; private set; } = false;

        private double  _operand1          = 0;
        private double  _operand2          = 0;
        private string  _pendingOperator   = "";
        private bool    _freshAfterEquals  = false;
        private bool    _freshAfterOp      = false;

        // Memory
        private double _memory = 0;

        // History (most recent first)
        private readonly List<string> _history = new();
        public IReadOnlyList<string> History => _history;

        // ─── Events ──────────────────────────────────────────────────────────
        public event EventHandler? StateChanged;
        private void Notify() => StateChanged?.Invoke(this, EventArgs.Empty);

        // ─── Input ───────────────────────────────────────────────────────────

        /// <summary>Append a digit character (0-9).</summary>
        public void InputDigit(char digit)
        {
            ClearError();
            if (_freshAfterEquals || _freshAfterOp)
            {
                CurrentInput = digit.ToString();
                _freshAfterEquals = false;
                _freshAfterOp     = false;
            }
            else if (CurrentInput == "0")
                CurrentInput = digit.ToString();
            else
                CurrentInput += digit;

            Notify();
        }

        /// <summary>Append decimal point.</summary>
        public void InputDecimal()
        {
            ClearError();
            if (_freshAfterEquals || _freshAfterOp)
            {
                CurrentInput = "0.";
                _freshAfterEquals = false;
                _freshAfterOp     = false;
            }
            else if (!CurrentInput.Contains('.'))
                CurrentInput += '.';

            Notify();
        }

        // ─── Operators ───────────────────────────────────────────────────────

        /// <summary>Apply a binary operator (+, -, ×, ÷, ^).</summary>
        public void SetOperator(string op)
        {
            ClearError();
            if (!_freshAfterOp && _pendingOperator != "")
                Calculate(); // chain: 5 + 3 × → compute 5+3 first

            if (!double.TryParse(CurrentInput, out _operand1))
            {
                SetError("Invalid input");
                return;
            }

            _pendingOperator  = op;
            Expression        = $"{FormatNumber(_operand1)} {op}";
            _freshAfterOp     = true;
            _freshAfterEquals = false;
            Notify();
        }

        /// <summary>Execute the pending calculation (=).</summary>
        public void Equals()
        {
            if (_pendingOperator == "") return;
            ClearError();

            if (!double.TryParse(CurrentInput, out _operand2))
            {
                SetError("Invalid input");
                return;
            }

            string historyEntry = $"{FormatNumber(_operand1)} {_pendingOperator} {FormatNumber(_operand2)} = ";
            double result = Compute(_operand1, _operand2, _pendingOperator);
            historyEntry += FormatNumber(result);

            if (!HasError)
            {
                _history.Insert(0, historyEntry);
                if (_history.Count > 50) _history.RemoveAt(50);
                CurrentInput      = FormatNumber(result);
                Expression        = historyEntry;
                _operand1         = result;
                _pendingOperator  = "";
                _freshAfterEquals = true;
                _freshAfterOp     = false;
            }

            Notify();
        }

        private double Compute(double a, double b, string op)
        {
            return op switch
            {
                "+" => a + b,
                "-" => a - b,
                "×" => a * b,
                "÷" => b == 0 ? SetErrorVal("Cannot divide by zero") : a / b,
                "^" => Math.Pow(a, b),
                _   => b
            };
        }

        // Internal helper for error path inside Compute
        private double SetErrorVal(string msg) { SetError(msg); return 0; }

        private void Calculate()
        {
            if (!double.TryParse(CurrentInput, out _operand2)) return;
            double result = Compute(_operand1, _operand2, _pendingOperator);
            if (!HasError)
            {
                CurrentInput = FormatNumber(result);
                _operand1    = result;
            }
        }

        // ─── Unary / Scientific ──────────────────────────────────────────────

        public void Percentage()
        {
            if (!double.TryParse(CurrentInput, out double val)) return;
            double result = _pendingOperator is "+" or "-"
                ? _operand1 * val / 100.0   // relative %
                : val / 100.0;
            CurrentInput = FormatNumber(result);
            Notify();
        }

        public void ToggleSign()
        {
            if (!double.TryParse(CurrentInput, out double val)) return;
            CurrentInput = FormatNumber(-val);
            Notify();
        }

        public void Backspace()
        {
            ClearError();
            if (CurrentInput.Length <= 1 || CurrentInput == "-0")
                CurrentInput = "0";
            else
                CurrentInput = CurrentInput[..^1];
            Notify();
        }

        public void Clear()
        {
            CurrentInput      = "0";
            Expression        = "";
            _pendingOperator  = "";
            _operand1         = 0;
            _operand2         = 0;
            _freshAfterEquals = false;
            _freshAfterOp     = false;
            ClearError();
            Notify();
        }

        // Scientific functions
        public void ApplyScientific(string func)
        {
            if (!double.TryParse(CurrentInput, out double val)) return;

            double result = func switch
            {
                "sin"  => Math.Sin(ToRad(val)),
                "cos"  => Math.Cos(ToRad(val)),
                "tan"  => Math.Tan(ToRad(val)),
                "log"  => val > 0 ? Math.Log10(val) : SetErrorVal("log undefined"),
                "ln"   => val > 0 ? Math.Log(val)   : SetErrorVal("ln undefined"),
                "√"    => val >= 0 ? Math.Sqrt(val)  : SetErrorVal("√ of negative"),
                "x²"   => Math.Pow(val, 2),
                "1/x"  => val != 0 ? 1.0 / val       : SetErrorVal("Cannot divide by zero"),
                "π"    => Math.PI,
                "e"    => Math.E,
                _      => val
            };

            if (!HasError)
            {
                Expression   = $"{func}({FormatNumber(val)}) =";
                CurrentInput = FormatNumber(result);
            }

            Notify();
        }

        private static double ToRad(double deg) => deg * Math.PI / 180.0;

        // ─── Memory ──────────────────────────────────────────────────────────

        public void MemoryStore()  { if (double.TryParse(CurrentInput, out double v)) _memory = v; }
        public void MemoryRecall() { CurrentInput = FormatNumber(_memory); _freshAfterOp = true; Notify(); }
        public void MemoryAdd()    { if (double.TryParse(CurrentInput, out double v)) _memory += v; }
        public void MemorySubtract() { if (double.TryParse(CurrentInput, out double v)) _memory -= v; }
        public void MemoryClear()  { _memory = 0; }
        public double MemoryValue => _memory;

        // ─── Error ───────────────────────────────────────────────────────────
        private void SetError(string msg) { HasError = true; ErrorMessage = msg; CurrentInput = "Error"; Notify(); }
        private void ClearError()         { if (HasError) { HasError = false; ErrorMessage = ""; CurrentInput = "0"; } }

        // ─── Utility ─────────────────────────────────────────────────────────
        private static string FormatNumber(double n)
        {
            if (double.IsNaN(n) || double.IsInfinity(n)) return "Error";
            // Remove unnecessary trailing decimals
            string s = n.ToString("G15");
            return s;
        }

        public void ClearHistory() { _history.Clear(); Notify(); }
    }
}
