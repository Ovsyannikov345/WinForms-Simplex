using Lab3.Extensions;
using SimplexMethod;

namespace Lab3;

public partial class ConstraintsTable : Form
{
    private Constraint[] _constraints = [];

    private Function? _function = default;

    private bool _isSolved = false;

    public ConstraintsTable()
    {
        InitializeComponent();
    }

    private void RenderConstraintTable(object sender, EventArgs e)
    {
        tableLayoutPanel.SuspendLayout();

        createTablesButton.Text = "Сброс";

        int constraintsCount = (int)ConstraintsCount.Value;

        int variablesCount = (int)VariablesCount.Value;

        // Clear the existing table
        tableLayoutPanel.Controls.Clear();
        tableLayoutPanel.RowStyles.Clear();
        tableLayoutPanel.ColumnStyles.Clear();

        // Add columns
        tableLayoutPanel.RowCount = 0;
        tableLayoutPanel.ColumnCount = variablesCount + 3;

        for (int j = 0; j < tableLayoutPanel.ColumnCount; j++)
        {
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
        }

        // Controlled parameters label
        tableLayoutPanel.AddRow(new RowStyle(SizeType.AutoSize));
        tableLayoutPanel.AddToLastRow(GetTableLabel("Управляемые параметры"), 0, columnSpan: tableLayoutPanel.ColumnCount);

        // Parameters table
        tableLayoutPanel.AddRow();
        for (var i = 0; i < variablesCount; i++)
        {
            tableLayoutPanel.AddToLastRow(GetColumnLabel($"x{i + 1}"), i);
        }

        tableLayoutPanel.AddToLastRow(GetColumnLabel("ЦФ"), variablesCount + 1);
        tableLayoutPanel.AddRow();
        for (var i = 0; i < variablesCount; i++)
        {
            var tb = GetTableTextBox();

            tb.TextChanged += TextChangeHandler;
            tableLayoutPanel.AddToLastRow(tb, i);
        }

        tableLayoutPanel.AddToLastRow(GetColumnLabel("->"), variablesCount);
        tableLayoutPanel.AddToLastRow(GetTableTextBox(disabled: true), variablesCount + 1);

        // Constraint table label
        tableLayoutPanel.AddRow(new RowStyle(SizeType.AutoSize));
        tableLayoutPanel.AddToLastRow(GetTableLabel("Таблица ограничений"), 0, columnSpan: tableLayoutPanel.ColumnCount);

        // Constraint table column names
        tableLayoutPanel.AddRow();

        for (var i = 0; i < variablesCount; i++)
        {
            tableLayoutPanel.AddToLastRow(GetColumnLabel($"a{i + 1}"), i);
        }

        tableLayoutPanel.AddToLastRow(GetColumnLabel("Ресурс"), variablesCount);
        tableLayoutPanel.AddToLastRow(GetColumnLabel("Знак"), variablesCount + 1);

        // Constraint table content
        for (int i = 0; i < constraintsCount; i++)
        {
            tableLayoutPanel.AddRow();

            for (var j = 0; j < variablesCount; j++)
            {
                tableLayoutPanel.AddToLastRow(GetTableTextBox(), j);
            }

            tableLayoutPanel.AddToLastRow(GetTableTextBox(disabled: true), variablesCount);
            tableLayoutPanel.AddToLastRow(GetTableComboBox(["<=", "=", ">="]), variablesCount + 1);
            tableLayoutPanel.AddToLastRow(GetTableTextBox(), variablesCount + 2);
        }

        // Function table label
        tableLayoutPanel.AddRow(new RowStyle(SizeType.AutoSize));
        tableLayoutPanel.AddToLastRow(GetTableLabel("Целевая функция"), 0, columnSpan: tableLayoutPanel.ColumnCount);

        // Function table column names
        tableLayoutPanel.AddRow();

        for (var i = 0; i < variablesCount; i++)
        {
            tableLayoutPanel.AddToLastRow(GetColumnLabel($"c{i + 1}"), i);
        }

        tableLayoutPanel.AddToLastRow(GetColumnLabel("Конст."), variablesCount);
        tableLayoutPanel.AddToLastRow(GetColumnLabel("Экстр."), variablesCount + 1);

        // Function table content
        tableLayoutPanel.AddRow();

        for (var i = 0; i < variablesCount + 1; i++)
        {
            tableLayoutPanel.AddToLastRow(GetTableTextBox(), i);
        }

        tableLayoutPanel.AddToLastRow(GetTableComboBox(["Макс", "Мин"]), variablesCount + 1);

        // Solve button
        var solveButton = new Button()
        {
            Text = "Решить",
            AutoSize = true,
            Dock = DockStyle.Fill,
        };
        solveButton.Click += Solve;

        tableLayoutPanel.AddRow();
        tableLayoutPanel.AddToLastRow(solveButton, 0, columnSpan: tableLayoutPanel.ColumnCount);

        tableLayoutPanel.ResumeLayout();
    }

    private void Solve(object? sender, EventArgs e)
    {
        var constraintsCount = (int)ConstraintsCount.Value;

        var variablesCount = (int)VariablesCount.Value;

        try
        {
            _constraints = ExtractConstraintsData(constraintsCount, variablesCount);
            _function = ExtractFunctionData(constraintsCount, variablesCount);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return;
        }

        var simplex = new Simplex(_function, _constraints);

        var result = simplex.GetResult();

        switch (result.Item2)
        {
            case SimplexResult.Found:
                _isSolved = true;
                SetFunctionValue(result.Item3.Length, _function.isExtrMax ? result.Item1[^1].fValue : -result.Item1[^1].fValue);
                SetParameters(result.Item3, silent: true);
                SetResources(_constraints, result.Item3);

                break;
            case SimplexResult.Unbounded:
                MessageBox.Show("Область допустимых решений не ограничена", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                break;
            case SimplexResult.NotYetFound:
                MessageBox.Show("За 100 итераций алгоритма решение не было найдено", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); ChangeText("Algorithm has made 100 cycles and hasn't found any optimal solution.", 0, tableLayoutPanel.RowCount - 1);

                break;
        }

        void ChangeText(string text, int column, int row)
        {
            if (tableLayoutPanel.GetControlFromPosition(column, row) is Label resultLabel)
            {
                resultLabel.Text = text;
            }
            else
            {
                tableLayoutPanel.AddToLastRow(GetTableLabel(text), 0, columnSpan: tableLayoutPanel.RowCount);
            }
        }
    }

    private static Label GetTableLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        TextAlign = ContentAlignment.MiddleCenter,
    };

    private Constraint[] ExtractConstraintsData(int constraintsCount, int variablesCount)
    {
        double[][] coefficients = new double[constraintsCount][];

        string[] signs = new string[(int)ConstraintsCount.Value];

        double[] rightParts = new double[(int)ConstraintsCount.Value];

        string[] allowedSigns = ["<=", "=", ">="];

        for (int i = 0; i < constraintsCount; i++)
        {
            // Extract coefficients
            coefficients[i] = new double[variablesCount];

            for (int j = 0; j < variablesCount; j++)
            {
                if (tableLayoutPanel.GetControlFromPosition(j, i + 5) is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        coefficients[i][j] = 0;
                        continue;
                    }

                    if (double.TryParse(textBox.Text, out double value))
                    {
                        coefficients[i][j] = value;
                        continue;
                    }

                    throw new ArgumentException($"Ошибка в ограничении {i + 1}, x{j + 1}");
                }
            }

            // Extract signs
            if (tableLayoutPanel.GetControlFromPosition(variablesCount + 1, i + 5) is ComboBox comboBox)
            {
                var sign = comboBox.SelectedItem?.ToString();

                if (sign is null || !allowedSigns.Contains(sign))
                {
                    throw new ArgumentException($"Неверный знак в ограничении {i + 1}");
                }

                signs[i] = sign;
            }

            // Extract right parts of constraints
            if (tableLayoutPanel.GetControlFromPosition(variablesCount + 2, i + 5) is TextBox textBox1)
            {
                if (double.TryParse(textBox1.Text, out double value))
                {
                    rightParts[i] = value;
                }
                else
                {
                    throw new ArgumentException($"Ошибка в правой части ограничения {i + 1}");
                }
            }
        }

        return coefficients.Select((coefs, constraintIndex) => new Constraint(coefs, rightParts[constraintIndex], signs[constraintIndex])).ToArray();
    }

    private Function ExtractFunctionData(int constraintsCount, int variablesCount)
    {
        double[] coefficients = new double[variablesCount + 1];

        string[] allowedExtrs = ["Макс", "Мин"];

        string selectedExtr = "";

        // Extract coefficients
        for (var i = 0; i < variablesCount + 1; i++)
        {
            if (tableLayoutPanel.GetControlFromPosition(i, constraintsCount + 7) is TextBox textBox)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    coefficients[i] = 0;
                    continue;
                }

                if (double.TryParse(textBox.Text, out double value))
                {
                    coefficients[i] = value;
                    continue;
                }

                throw new ArgumentException($"Ошибка в коэффициенте ЦФ: {(i == variablesCount ? "C" : $"x{i + 1}")}");
            }
        }

        // Extract extr
        if (tableLayoutPanel.GetControlFromPosition(variablesCount + 1, constraintsCount + 7) is ComboBox comboBox)
        {
            var extr = comboBox.SelectedItem?.ToString();

            if (extr is null || !allowedExtrs.Contains(extr))
            {
                throw new ArgumentException($"Ошибка в экстремуме ЦФ");
            }

            selectedExtr = extr;
        }

        return new Function(coefficients[..^1], coefficients[^1], selectedExtr == "Макс");
    }

    private static Label GetColumnLabel(string text) => new()
    {
        Text = text,
        TextAlign = ContentAlignment.MiddleCenter,
        Dock = DockStyle.Fill,
    };

    private static TextBox GetTableTextBox(bool disabled = false) => new()
    {
        TextAlign = HorizontalAlignment.Center,
        Dock = DockStyle.Fill,
        ReadOnly = disabled,
    };

    private static ComboBox GetTableComboBox(object[] items)
    {
        var comboBox = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };
        comboBox.Items.AddRange(items);

        return comboBox;
    }

    private void SetFunctionValue(int variablesCount, double value)
    {
        if (tableLayoutPanel.GetControlFromPosition(variablesCount + 1, 2) is TextBox tb)
        {
            tb.Text = value.ToString();
        }
    }

    private void SetParameters(double[] values, bool silent = false)
    {
        for (var i = 0; i < values.Length; i++)
        {
            if (tableLayoutPanel.GetControlFromPosition(i, 2) is TextBox tb)
            {
                if (!silent)
                {
                    tb.Text = values[i].ToString();
                    continue;
                }

                tb.TextChanged -= TextChangeHandler;
                tb.Text = values[i].ToString();
                tb.TextChanged += TextChangeHandler;
            }
        }
    }

    private void SetResources(Constraint[] constraints, double[] values)
    {
        double limit;

        for (var i = 0; i < constraints.Length; i++)
        {
            if (tableLayoutPanel.GetControlFromPosition(values.Length, 5 + i) is TextBox tb)
            {
                tb.BackColor = Color.White;

                if (constraints[i].variables.Count(v => v != 0) <= 1 && constraints[i].b == 0)
                {
                    continue;
                }

                var resource = constraints[i].variables.Select((v, index) => v * values[index]).Sum();

                tb.Text = resource.ToString();

                limit = constraints[i].b;

                if (constraints[i].sign != "=" && resource == limit)
                {
                    tb.BackColor = Color.Yellow;
                    continue;
                }

                if ((constraints[i].sign == "<=" && resource > limit) ||
                    (constraints[i].sign == "=" && resource != limit) ||
                    (constraints[i].sign == ">=" && resource < limit))
                {
                    tb.BackColor = Color.PaleVioletRed;
                }
            }
        }
    }

    private void TextChangeHandler(object? sender, EventArgs e) => UpdateSolution((int)VariablesCount.Value);

    private void UpdateSolution(int variablesCount)
    {
        if (_function is null || !_isSolved)
        {
            return;
        }

        double[] parameterValues;

        try
        {
            parameterValues = GetValues();
        }
        catch
        {
            return;
        }

        var functionValue = parameterValues.Select((v, index) => v * _function.variables[index]).Sum();

        SetFunctionValue(parameterValues.Length, functionValue);
        SetResources(_constraints, parameterValues);

        double[] GetValues()
        {
            var values = new double[variablesCount];

            for (var i = 0; i < variablesCount; i++)
            {
                if (tableLayoutPanel.GetControlFromPosition(i, 2) is TextBox tb)
                {
                    values[i] = double.Parse(tb.Text.Replace('.', ','));
                }
            }

            return values;
        }
    }
}
