namespace WinFormsCalculator;

/// <summary>
/// Minimal designer file — all layout is done programmatically in Form1.cs.
/// </summary>
partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    // InitializeComponent is intentionally empty — all controls are built in BuildUI().
    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.Name = "Form1";
        this.ResumeLayout(false);
    }
}
