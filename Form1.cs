using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace WinFormsCalculator;

public partial class Form1 : Form
{
    private readonly CalculatorEngine _engine = new();
    private bool _isDarkMode = true;

    // Theme colors
    private static readonly Color DarkBgTop = Color.FromArgb(20, 20, 22);
    private static readonly Color DarkBgBottom = Color.FromArgb(40, 42, 48);
    private static readonly Color AppleOrange = Color.FromArgb(255, 159, 10);
    private static readonly Color AppleGray = Color.FromArgb(165, 165, 167);
    private static readonly Color AppleDarkGray = Color.FromArgb(51, 51, 53);

    private Label _displayMain;
    private Label _displayExpr;
    private TableLayoutPanel _buttonGrid;

    public Form1()
    {
        InitializeComponent();
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        this.BackColor = DarkBgTop;
        this.ForeColor = Color.White;
        this.Size = new Size(340, 560);
        this.Text = "Calculator";
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        SetupUI();
        WireEvents();
    }

    private void SetupUI()
    {
        _displayExpr = new Label()
        {
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.BottomRight,
            Font = new Font("Segoe UI Semibold", 12F),
            ForeColor = Color.FromArgb(150, 255, 255, 255),
            Padding = new Padding(0, 0, 20, 0),
            BackColor = Color.Transparent
        };

        _displayMain = new Label()
        {
            Dock = DockStyle.Top,
            Height = 100,
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI Semibold", 48F, FontStyle.Bold),
            ForeColor = Color.White,
            Padding = new Padding(0, 0, 20, 0),
            BackColor = Color.Transparent,
            Text = "0"
        };

        _buttonGrid = new TableLayoutPanel()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 5,
            Padding = new Padding(12),
            BackColor = Color.Transparent
        };

        for (int i = 0; i < 4; i++) _buttonGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        for (int i = 0; i < 5; i++) _buttonGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

        this.Controls.Add(_buttonGrid);
        this.Controls.Add(_displayMain);
        this.Controls.Add(_displayExpr);

        PopulateButtons();
    }

    private void PopulateButtons()
    {
        string[] buttons = {
            "AC", "+/-", "%", "÷",
            "7", "8", "9", "×",
            "4", "5", "6", "-",
            "1", "2", "3", "+",
            "0", ".", "="
        };

        int index = 0;
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (index >= buttons.Length) break;

                string text = buttons[index];
                CalcButton btn = CreateStyledButton(text);

                if (text == "0")
                {
                    _buttonGrid.Controls.Add(btn, col, row);
                    _buttonGrid.SetColumnSpan(btn, 2);
                    col++;
                }
                else
                {
                    _buttonGrid.Controls.Add(btn, col, row);
                }
                index++;
            }
        }
    }

    private CalcButton CreateStyledButton(string text)
    {
        CalcButton btn = new CalcButton()
        {
            Text = text,
            Dock = DockStyle.Fill,
            Margin = new Padding(6),
            Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold)
        };

        if (int.TryParse(text, out _) || text == ".")
        {
            btn.GlassColor = AppleDarkGray;
            btn.ForeColor = Color.White;
            btn.Click += (s, e) => { if (text == ".") _engine.InputDecimal(); else _engine.InputDigit(text[0]); };
        }
        else if ("÷×-+= ".Contains(text))
        {
            btn.GlassColor = AppleOrange;
            btn.ForeColor = Color.White;
            btn.Click += (s, e) => { if (text == "=") _engine.Equals(); else _engine.SetOperator(text); };
        }
        else
        {
            btn.GlassColor = AppleGray;
            btn.ForeColor = Color.Black;
            btn.Click += (s, e) => {
                switch (text) {
                    case "AC": _engine.Clear(); break;
                    case "+/-": _engine.ToggleSign(); break;
                    case "%": _engine.Percentage(); break;
                }
            };
        }
        return btn;
    }

    private void WireEvents()
    {
        _engine.StateChanged += (s, e) => {
            _displayMain.Text = _engine.CurrentInput;
            _displayExpr.Text = _engine.Expression;
            
            // Adjust font size for big numbers
            if (_displayMain.Text.Length > 8) _displayMain.Font = new Font("Segoe UI Semibold", 32F, FontStyle.Bold);
            else _displayMain.Font = new Font("Segoe UI Semibold", 48F, FontStyle.Bold);
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Apple-style vertical gradient
        using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, DarkBgTop, DarkBgBottom, LinearGradientMode.Vertical))
        {
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }
    }
}
