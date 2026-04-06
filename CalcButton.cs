using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;

namespace WinFormsCalculator;

public class CalcButton : Button
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color GlassColor { get; set; } = Color.FromArgb(60, 255, 255, 255);

    private float _scale = 1.0f;
    private bool _isHovered = false;
    private bool _isPressed = false;
    private System.Windows.Forms.Timer _animTimer;

    public CalcButton()
    {
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;
        this.BackColor = Color.Transparent;
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        
        _animTimer = new System.Windows.Forms.Timer { Interval = 16 }; // 60 FPS
        _animTimer.Tick += (s, e) => {
            float targetScale = _isPressed ? 0.92f : 1.0f;
            if (Math.Abs(_scale - targetScale) > 0.01f) {
                _scale += (targetScale - _scale) * 0.4f;
                Invalidate();
            } else {
                _scale = targetScale;
                _animTimer.Stop();
                Invalidate();
            }
        };
    }

    protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { _isPressed = true; _animTimer.Start(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; _animTimer.Start(); base.OnMouseUp(e); }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Do NOT call base.OnPaintBackground to avoid rectangular flickering.
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        float w = Width * _scale;
        float h = Height * _scale;
        float x = (Width - w) / 2;
        float y = (Height - h) / 2;
        RectangleF buttonRect = new RectangleF(x, y, w, h);
        float radius = Math.Min(w, h) / 2 - 1;

        using (GraphicsPath path = GetRoundedPath(buttonRect, radius))
        {
            int alpha = _isHovered ? 120 : (_isPressed ? 180 : 80);
            if (GlassColor.A == 255) alpha = 255; 

            Color drawColor = Color.FromArgb(alpha, GlassColor.R, GlassColor.G, GlassColor.B);

            using (SolidBrush brush = new SolidBrush(drawColor))
            {
                g.FillPath(brush, path);
            }

            if (!_isPressed)
            {
                using (LinearGradientBrush highlight = new LinearGradientBrush(buttonRect, Color.FromArgb(40, 255, 255, 255), Color.Transparent, 90F))
                {
                    g.FillPath(highlight, path);
                }
            }

            using (Pen pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1.25f))
            {
                g.DrawPath(pen, path);
            }
        }

        using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
        {
            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
            {
                float fontSize = Font.Size * _scale;
                using (Font scaledFont = new Font(Font.FontFamily, fontSize, Font.Style))
                {
                    g.DrawString(Text, scaledFont, textBrush, buttonRect, sf);
                }
            }
        }
    }

    private GraphicsPath GetRoundedPath(RectangleF rect, float radius)
    {
        GraphicsPath path = new GraphicsPath();
        float d = radius * 2;
        if (d <= 0) d = 1;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _animTimer != null) _animTimer.Dispose();
        base.Dispose(disposing);
    }
}
