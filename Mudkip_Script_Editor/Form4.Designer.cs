namespace Mudkip_Script_Editor
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            Type_AT = new DataGridViewImageColumn();
            label_path = new Label();
            label_address = new Label();
            label_Chart_item1 = new Label();
            label_Chart_item2 = new Label();
            button_Height_adjustment = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel_Useinidata = new ToolStripStatusLabel();
            toolStripStatusLabel_MaxTypeNum = new ToolStripStatusLabel();
            toolStripStatusLabell_first_flag = new ToolStripStatusLabel();
            checkBox_optimization_flg = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Type_AT });
            dataGridView1.Location = new Point(26, 150);
            dataGridView1.Margin = new Padding(6, 6, 6, 6);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(2350, 1058);
            dataGridView1.TabIndex = 1;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            // 
            // Type_AT
            // 
            Type_AT.HeaderText = "";
            Type_AT.ImageLayout = DataGridViewImageCellLayout.Zoom;
            Type_AT.MinimumWidth = 15;
            Type_AT.Name = "Type_AT";
            Type_AT.Resizable = DataGridViewTriState.True;
            Type_AT.SortMode = DataGridViewColumnSortMode.Automatic;
            Type_AT.Width = 120;
            // 
            // label_path
            // 
            label_path.AutoSize = true;
            label_path.Location = new Point(2146, 49);
            label_path.Margin = new Padding(2, 0, 2, 0);
            label_path.Name = "label_path";
            label_path.Size = new Size(209, 41);
            label_path.TabIndex = 4;
            label_path.Text = "get_label_path";
            label_path.Visible = false;
            // 
            // label_address
            // 
            label_address.AutoSize = true;
            label_address.Location = new Point(1885, 51);
            label_address.Margin = new Padding(2, 0, 2, 0);
            label_address.Name = "label_address";
            label_address.Size = new Size(252, 41);
            label_address.TabIndex = 5;
            label_address.Text = "get_label_address";
            label_address.Visible = false;
            // 
            // label_Chart_item1
            // 
            label_Chart_item1.AutoSize = true;
            label_Chart_item1.Location = new Point(26, 18);
            label_Chart_item1.Margin = new Padding(2, 0, 2, 0);
            label_Chart_item1.Name = "label_Chart_item1";
            label_Chart_item1.Size = new Size(30, 41);
            label_Chart_item1.TabIndex = 7;
            label_Chart_item1.Text = "-";
            // 
            // label_Chart_item2
            // 
            label_Chart_item2.AutoSize = true;
            label_Chart_item2.Location = new Point(338, 18);
            label_Chart_item2.Margin = new Padding(2, 0, 2, 0);
            label_Chart_item2.Name = "label_Chart_item2";
            label_Chart_item2.Size = new Size(30, 41);
            label_Chart_item2.TabIndex = 8;
            label_Chart_item2.Text = "-";
            // 
            // button_Height_adjustment
            // 
            button_Height_adjustment.Location = new Point(748, 43);
            button_Height_adjustment.Margin = new Padding(2, 2, 2, 2);
            button_Height_adjustment.Name = "button_Height_adjustment";
            button_Height_adjustment.Size = new Size(302, 59);
            button_Height_adjustment.TabIndex = 9;
            button_Height_adjustment.Text = "Height adjustment";
            button_Height_adjustment.UseVisualStyleBackColor = true;
            button_Height_adjustment.Click += button_Height_adjustment_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel_Useinidata, toolStripStatusLabel_MaxTypeNum, toolStripStatusLabell_first_flag });
            statusStrip1.Location = new Point(0, 1156);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(2, 0, 30, 0);
            statusStrip1.Size = new Size(2414, 54);
            statusStrip1.TabIndex = 10;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_Useinidata
            // 
            toolStripStatusLabel_Useinidata.Name = "toolStripStatusLabel_Useinidata";
            toolStripStatusLabel_Useinidata.Size = new Size(177, 41);
            toolStripStatusLabel_Useinidata.Text = "ReadiniData";
            // 
            // toolStripStatusLabel_MaxTypeNum
            // 
            toolStripStatusLabel_MaxTypeNum.Name = "toolStripStatusLabel_MaxTypeNum";
            toolStripStatusLabel_MaxTypeNum.Size = new Size(42, 41);
            toolStripStatusLabel_MaxTypeNum.Text = "--";
            // 
            // toolStripStatusLabell_first_flag
            // 
            toolStripStatusLabell_first_flag.Name = "toolStripStatusLabell_first_flag";
            toolStripStatusLabell_first_flag.Size = new Size(34, 41);
            toolStripStatusLabell_first_flag.Text = "0";
            // 
            // checkBox_optimization_flg
            // 
            checkBox_optimization_flg.AutoSize = true;
            checkBox_optimization_flg.Location = new Point(1176, 48);
            checkBox_optimization_flg.Name = "checkBox_optimization_flg";
            checkBox_optimization_flg.Size = new Size(229, 45);
            checkBox_optimization_flg.TabIndex = 11;
            checkBox_optimization_flg.Text = "最適化チェック";
            checkBox_optimization_flg.UseVisualStyleBackColor = true;
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2414, 1210);
            Controls.Add(checkBox_optimization_flg);
            Controls.Add(statusStrip1);
            Controls.Add(button_Height_adjustment);
            Controls.Add(label_Chart_item2);
            Controls.Add(label_Chart_item1);
            Controls.Add(label_address);
            Controls.Add(label_path);
            Controls.Add(dataGridView1);
            Margin = new Padding(6, 6, 6, 6);
            Name = "Form4";
            Text = "Type Chart";
            Shown += Form4_Shown;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Label label_path;
        private Label label_address;
        private Label label_Chart_item1;
        private Label label_Chart_item2;
        private Button button_Height_adjustment;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel_Useinidata;
        private ToolStripStatusLabel toolStripStatusLabel_MaxTypeNum;
        private DataGridViewImageColumn Type_AT;
        private ToolStripStatusLabel toolStripStatusLabell_first_flag;
        private CheckBox checkBox_optimization_flg;
    }
}