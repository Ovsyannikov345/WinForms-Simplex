namespace Lab3.Extensions;

public static class TableLayoutPanelExtensions
{
    public static void AddRow(this TableLayoutPanel panel, RowStyle? rowStyle = default)
    {
        panel.RowCount++;

        if (rowStyle is null)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            return;
        }

        panel.RowStyles.Add(rowStyle);
    }

    public static void AddToLastRow(this TableLayoutPanel panel, Control control, int column, int columnSpan = 1)
    {
        panel.Controls.Add(control, column, panel.RowCount - 1);
        panel.SetColumnSpan(control, columnSpan);
    }
}
