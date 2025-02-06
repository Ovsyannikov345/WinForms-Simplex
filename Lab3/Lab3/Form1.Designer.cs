namespace Lab3
{
    partial class ConstraintsTable
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ConstraintsCount = new NumericUpDown();
            VariablesCount = new NumericUpDown();
            tableLayoutPanel = new TableLayoutPanel();
            createTablesButton = new Button();
            label2 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)ConstraintsCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VariablesCount).BeginInit();
            SuspendLayout();
            // 
            // ConstraintsCount
            // 
            ConstraintsCount.Location = new Point(33, 45);
            ConstraintsCount.Name = "ConstraintsCount";
            ConstraintsCount.Size = new Size(123, 27);
            ConstraintsCount.TabIndex = 0;
            // 
            // VariablesCount
            // 
            VariablesCount.Location = new Point(206, 45);
            VariablesCount.Name = "VariablesCount";
            VariablesCount.Size = new Size(123, 27);
            VariablesCount.TabIndex = 1;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Location = new Point(33, 90);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Size = new Size(0, 0);
            tableLayoutPanel.TabIndex = 2;
            // 
            // createTablesButton
            // 
            createTablesButton.Location = new Point(396, 39);
            createTablesButton.Name = "createTablesButton";
            createTablesButton.Size = new Size(140, 36);
            createTablesButton.TabIndex = 3;
            createTablesButton.Text = "Создать таблицу";
            createTablesButton.UseVisualStyleBackColor = true;
            createTablesButton.Click += RenderConstraintTable;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 22);
            label2.Margin = new Padding(15);
            label2.Name = "label2";
            label2.Size = new Size(155, 20);
            label2.TabIndex = 5;
            label2.Text = "Кол-во ограничений";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(206, 22);
            label3.Name = "label3";
            label3.Size = new Size(175, 20);
            label3.TabIndex = 6;
            label3.Text = "Кол-во упр.параметров";
            // 
            // ConstraintsTable
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(561, 547);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(createTablesButton);
            Controls.Add(tableLayoutPanel);
            Controls.Add(VariablesCount);
            Controls.Add(ConstraintsCount);
            Name = "ConstraintsTable";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)ConstraintsCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)VariablesCount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown ConstraintsCount;
        private NumericUpDown VariablesCount;
        private TableLayoutPanel tableLayoutPanel;
        private Button createTablesButton;
        private Label label2;
        private Label label3;
    }
}
