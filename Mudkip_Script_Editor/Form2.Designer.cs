namespace Mudkip_Script_Editor
{
    partial class Form2
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            label_path = new Label();
            dataGridView1 = new DataGridView();
            button_Advanced_Search = new Button();
            textBox_Advanced_Search = new TextBox();
            checkBox_endFlag_check = new CheckBox();
            label_Advanced_Search_sub = new Label();
            label_max_length_sub = new Label();
            label_max_length = new Label();
            checkBox_jump_check = new CheckBox();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel_Open_mode = new ToolStripStatusLabel();
            toolStripStatusLabel_Script_Category = new ToolStripStatusLabel();
            toolStripStatusLabel_SP_flg = new ToolStripStatusLabel();
            toolStripStatusLabel_blank = new ToolStripStatusLabel();
            toolStripStatusLabel_Useinidata = new ToolStripStatusLabel();
            menuStrip1 = new MenuStrip();
            toolStripMenuItem_Export_CSV = new ToolStripMenuItem();
            label_StirlingPath = new Label();
            label_Move_Name = new Label();
            Count = new DataGridViewTextBoxColumn();
            Edit_Script = new DataGridViewComboBoxColumn();
            Script_Data = new DataGridViewTextBoxColumn();
            data1 = new DataGridViewTextBoxColumn();
            data2 = new DataGridViewTextBoxColumn();
            data3 = new DataGridViewTextBoxColumn();
            data4 = new DataGridViewTextBoxColumn();
            data5 = new DataGridViewTextBoxColumn();
            data6 = new DataGridViewTextBoxColumn();
            data7 = new DataGridViewTextBoxColumn();
            Script_Info = new DataGridViewTextBoxColumn();
            secret_flag = new DataGridViewTextBoxColumn();
            Open_Editor = new DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label_path
            // 
            label_path.AutoSize = true;
            label_path.Location = new Point(1062, 22);
            label_path.Name = "label_path";
            label_path.Size = new Size(83, 15);
            label_path.TabIndex = 73;
            label_path.Text = "get_label_path";
            label_path.Visible = false;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(192, 255, 255);
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Count, Edit_Script, Script_Data, data1, data2, data3, data4, data5, data6, data7, Script_Info, secret_flag, Open_Editor });
            dataGridView1.Location = new Point(12, 56);
            dataGridView1.Margin = new Padding(3, 2, 3, 2);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(1171, 423);
            dataGridView1.TabIndex = 74;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.MouseClick += dataGridView1_MouseClick;
            // 
            // button_Advanced_Search
            // 
            button_Advanced_Search.Location = new Point(10, 32);
            button_Advanced_Search.Margin = new Padding(1);
            button_Advanced_Search.Name = "button_Advanced_Search";
            button_Advanced_Search.Size = new Size(125, 22);
            button_Advanced_Search.TabIndex = 75;
            button_Advanced_Search.Text = "Advanced Search";
            button_Advanced_Search.UseVisualStyleBackColor = true;
            button_Advanced_Search.Click += button_Advanced_Search_Click;
            // 
            // textBox_Advanced_Search
            // 
            textBox_Advanced_Search.Location = new Point(200, 34);
            textBox_Advanced_Search.Margin = new Padding(1);
            textBox_Advanced_Search.Name = "textBox_Advanced_Search";
            textBox_Advanced_Search.Size = new Size(101, 23);
            textBox_Advanced_Search.TabIndex = 76;
            textBox_Advanced_Search.Text = "00000000";
            // 
            // checkBox_endFlag_check
            // 
            checkBox_endFlag_check.AutoSize = true;
            checkBox_endFlag_check.Checked = true;
            checkBox_endFlag_check.CheckState = CheckState.Checked;
            checkBox_endFlag_check.Location = new Point(308, 34);
            checkBox_endFlag_check.Margin = new Padding(1);
            checkBox_endFlag_check.Name = "checkBox_endFlag_check";
            checkBox_endFlag_check.Size = new Size(179, 19);
            checkBox_endFlag_check.TabIndex = 78;
            checkBox_endFlag_check.Text = "終了コードを見つけたら読込終了";
            checkBox_endFlag_check.UseVisualStyleBackColor = true;
            // 
            // label_Advanced_Search_sub
            // 
            label_Advanced_Search_sub.AutoSize = true;
            label_Advanced_Search_sub.Location = new Point(145, 35);
            label_Advanced_Search_sub.Name = "label_Advanced_Search_sub";
            label_Advanced_Search_sub.Size = new Size(50, 15);
            label_Advanced_Search_sub.TabIndex = 79;
            label_Advanced_Search_sub.Text = "Offset：";
            label_Advanced_Search_sub.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label_max_length_sub
            // 
            label_max_length_sub.AutoSize = true;
            label_max_length_sub.Location = new Point(716, 34);
            label_max_length_sub.Name = "label_max_length_sub";
            label_max_length_sub.Size = new Size(80, 15);
            label_max_length_sub.TabIndex = 80;
            label_max_length_sub.Text = "Max_Lengh：";
            // 
            // label_max_length
            // 
            label_max_length.AutoSize = true;
            label_max_length.Location = new Point(794, 34);
            label_max_length.Name = "label_max_length";
            label_max_length.Size = new Size(19, 15);
            label_max_length.TabIndex = 81;
            label_max_length.Text = "00";
            // 
            // checkBox_jump_check
            // 
            checkBox_jump_check.AutoSize = true;
            checkBox_jump_check.Checked = true;
            checkBox_jump_check.CheckState = CheckState.Checked;
            checkBox_jump_check.Location = new Point(505, 34);
            checkBox_jump_check.Margin = new Padding(1);
            checkBox_jump_check.Name = "checkBox_jump_check";
            checkBox_jump_check.Size = new Size(195, 19);
            checkBox_jump_check.TabIndex = 84;
            checkBox_jump_check.Text = "別オフセットへのジャンプで読込終了";
            checkBox_jump_check.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel_Open_mode, toolStripStatusLabel_Script_Category, toolStripStatusLabel_SP_flg, toolStripStatusLabel_blank, toolStripStatusLabel_Useinidata });
            statusStrip1.Location = new Point(0, 481);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 12, 0);
            statusStrip1.Size = new Size(1195, 22);
            statusStrip1.TabIndex = 86;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_Open_mode
            // 
            toolStripStatusLabel_Open_mode.Name = "toolStripStatusLabel_Open_mode";
            toolStripStatusLabel_Open_mode.Size = new Size(72, 17);
            toolStripStatusLabel_Open_mode.Text = "Select Mode";
            // 
            // toolStripStatusLabel_Script_Category
            // 
            toolStripStatusLabel_Script_Category.Name = "toolStripStatusLabel_Script_Category";
            toolStripStatusLabel_Script_Category.Size = new Size(54, 17);
            toolStripStatusLabel_Script_Category.Text = "Category";
            // 
            // toolStripStatusLabel_SP_flg
            // 
            toolStripStatusLabel_SP_flg.Name = "toolStripStatusLabel_SP_flg";
            toolStripStatusLabel_SP_flg.Size = new Size(17, 17);
            toolStripStatusLabel_SP_flg.Text = "--";
            // 
            // toolStripStatusLabel_blank
            // 
            toolStripStatusLabel_blank.Name = "toolStripStatusLabel_blank";
            toolStripStatusLabel_blank.Size = new Size(969, 17);
            toolStripStatusLabel_blank.Spring = true;
            // 
            // toolStripStatusLabel_Useinidata
            // 
            toolStripStatusLabel_Useinidata.Name = "toolStripStatusLabel_Useinidata";
            toolStripStatusLabel_Useinidata.Size = new Size(70, 17);
            toolStripStatusLabel_Useinidata.Text = "ReadiniData";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_Export_CSV });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(1195, 28);
            menuStrip1.TabIndex = 87;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem_Export_CSV
            // 
            toolStripMenuItem_Export_CSV.Image = Properties.Resources.Excel_icon;
            toolStripMenuItem_Export_CSV.Name = "toolStripMenuItem_Export_CSV";
            toolStripMenuItem_Export_CSV.Size = new Size(98, 24);
            toolStripMenuItem_Export_CSV.Text = "Export_xlsx";
            toolStripMenuItem_Export_CSV.Click += toolStripMenuItem_Export_CSV_Click;
            // 
            // label_StirlingPath
            // 
            label_StirlingPath.AutoSize = true;
            label_StirlingPath.Location = new Point(864, 34);
            label_StirlingPath.Name = "label_StirlingPath";
            label_StirlingPath.Size = new Size(19, 15);
            label_StirlingPath.TabIndex = 88;
            label_StirlingPath.Text = "00";
            label_StirlingPath.Visible = false;
            // 
            // label_Move_Name
            // 
            label_Move_Name.AutoSize = true;
            label_Move_Name.Location = new Point(844, 34);
            label_Move_Name.Name = "label_Move_Name";
            label_Move_Name.Size = new Size(17, 15);
            label_Move_Name.TabIndex = 89;
            label_Move_Name.Text = "--";
            label_Move_Name.Visible = false;
            // 
            // Count
            // 
            Count.HeaderText = "Count";
            Count.MinimumWidth = 15;
            Count.Name = "Count";
            Count.SortMode = DataGridViewColumnSortMode.NotSortable;
            Count.Width = 70;
            // 
            // Edit_Script
            // 
            dataGridViewCellStyle2.Font = new Font("MS UI Gothic", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            Edit_Script.DefaultCellStyle = dataGridViewCellStyle2;
            Edit_Script.FlatStyle = FlatStyle.Flat;
            Edit_Script.HeaderText = "Edit_Script";
            Edit_Script.MinimumWidth = 6;
            Edit_Script.Name = "Edit_Script";
            Edit_Script.Visible = false;
            Edit_Script.Width = 300;
            // 
            // Script_Data
            // 
            Script_Data.HeaderText = "Script_Data";
            Script_Data.MinimumWidth = 6;
            Script_Data.Name = "Script_Data";
            Script_Data.SortMode = DataGridViewColumnSortMode.NotSortable;
            Script_Data.Width = 280;
            // 
            // data1
            // 
            data1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            data1.DefaultCellStyle = dataGridViewCellStyle3;
            data1.HeaderText = "data1";
            data1.MinimumWidth = 6;
            data1.Name = "data1";
            data1.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // data2
            // 
            data2.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            data2.DefaultCellStyle = dataGridViewCellStyle4;
            data2.HeaderText = "data2";
            data2.MinimumWidth = 6;
            data2.Name = "data2";
            data2.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // data3
            // 
            data3.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            data3.DefaultCellStyle = dataGridViewCellStyle5;
            data3.HeaderText = "data3";
            data3.MinimumWidth = 6;
            data3.Name = "data3";
            data3.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // data4
            // 
            data4.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.True;
            data4.DefaultCellStyle = dataGridViewCellStyle6;
            data4.HeaderText = "data4";
            data4.MinimumWidth = 6;
            data4.Name = "data4";
            data4.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // data5
            // 
            data5.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            data5.DefaultCellStyle = dataGridViewCellStyle7;
            data5.HeaderText = "data5";
            data5.MinimumWidth = 6;
            data5.Name = "data5";
            data5.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // data6
            // 
            data6.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.True;
            data6.DefaultCellStyle = dataGridViewCellStyle8;
            data6.HeaderText = "data6";
            data6.MinimumWidth = 6;
            data6.Name = "data6";
            data6.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // data7
            // 
            data7.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.True;
            data7.DefaultCellStyle = dataGridViewCellStyle9;
            data7.HeaderText = "data7";
            data7.MinimumWidth = 6;
            data7.Name = "data7";
            data7.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Script_Info
            // 
            Script_Info.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle10.WrapMode = DataGridViewTriState.True;
            Script_Info.DefaultCellStyle = dataGridViewCellStyle10;
            Script_Info.HeaderText = "Script_Info";
            Script_Info.MinimumWidth = 6;
            Script_Info.Name = "Script_Info";
            Script_Info.SortMode = DataGridViewColumnSortMode.NotSortable;
            Script_Info.Width = 500;
            // 
            // secret_flag
            // 
            secret_flag.HeaderText = "secret_flag";
            secret_flag.MinimumWidth = 15;
            secret_flag.Name = "secret_flag";
            secret_flag.ReadOnly = true;
            secret_flag.Visible = false;
            secret_flag.Width = 300;
            // 
            // Open_Editor
            // 
            Open_Editor.HeaderText = "Open_Editor";
            Open_Editor.MinimumWidth = 6;
            Open_Editor.Name = "Open_Editor";
            Open_Editor.Width = 125;
            // 
            // Form2
            // 
            AccessibleRole = AccessibleRole.None;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1195, 503);
            Controls.Add(label_Move_Name);
            Controls.Add(label_StirlingPath);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(checkBox_jump_check);
            Controls.Add(label_max_length);
            Controls.Add(label_max_length_sub);
            Controls.Add(label_Advanced_Search_sub);
            Controls.Add(checkBox_endFlag_check);
            Controls.Add(textBox_Advanced_Search);
            Controls.Add(button_Advanced_Search);
            Controls.Add(dataGridView1);
            Controls.Add(label_path);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(1);
            Name = "Form2";
            Text = "Script Viewer";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_path;
        private DataGridView dataGridView1;
        private Button button_Advanced_Search;
        private TextBox textBox_Advanced_Search;
        private CheckBox checkBox_endFlag_check;
        private Label label_Advanced_Search_sub;
        private Label label_max_length_sub;
        private Label label_max_length;
        private CheckBox checkBox_jump_check;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel_Open_mode;
        private ToolStripStatusLabel toolStripStatusLabel_Script_Category;
        private ToolStripStatusLabel toolStripStatusLabel_SP_flg;
        private ToolStripStatusLabel toolStripStatusLabel_Useinidata;
        private ToolStripStatusLabel toolStripStatusLabel_blank;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem_Export_CSV;
        private Label label_StirlingPath;
        private Label label_Move_Name;
        private DataGridViewTextBoxColumn jump;
        private DataGridViewTextBoxColumn Count;
        private DataGridViewComboBoxColumn Edit_Script;
        private DataGridViewTextBoxColumn Script_Data;
        private DataGridViewTextBoxColumn data1;
        private DataGridViewTextBoxColumn data2;
        private DataGridViewTextBoxColumn data3;
        private DataGridViewTextBoxColumn data4;
        private DataGridViewTextBoxColumn data5;
        private DataGridViewTextBoxColumn data6;
        private DataGridViewTextBoxColumn data7;
        private DataGridViewTextBoxColumn Script_Info;
        private DataGridViewTextBoxColumn secret_flag;
        private DataGridViewButtonColumn Open_Editor;
    }
}