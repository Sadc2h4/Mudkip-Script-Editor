namespace Mudkip_Script_Editor
{
    partial class Form3
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
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            Hex = new DataGridViewTextBoxColumn();
            Dec = new DataGridViewTextBoxColumn();
            Move_Name = new DataGridViewTextBoxColumn();
            Text_Pattern = new DataGridViewTextBoxColumn();
            Move_text = new DataGridViewTextBoxColumn();
            ID = new DataGridViewTextBoxColumn();
            Type = new DataGridViewImageColumn();
            Category = new DataGridViewImageColumn();
            Pow = new DataGridViewTextBoxColumn();
            PP = new DataGridViewTextBoxColumn();
            Accuracy = new DataGridViewTextBoxColumn();
            Prob = new DataGridViewTextBoxColumn();
            Attack_range = new DataGridViewTextBoxColumn();
            Priority = new DataGridViewTextBoxColumn();
            Move_flag = new DataGridViewTextBoxColumn();
            Ability_flag_1 = new DataGridViewTextBoxColumn();
            Ability_flag_2 = new DataGridViewTextBoxColumn();
            Effect_offset = new DataGridViewTextBoxColumn();
            Animation_offset = new DataGridViewTextBoxColumn();
            Description_offset = new DataGridViewTextBoxColumn();
            Move_Info = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            Number = new DataGridViewTextBoxColumn();
            label_select_num_sub = new Label();
            label_select_num = new Label();
            label_path = new Label();
            menuStrip1 = new MenuStrip();
            toolStripMenuItem_Export_CSV = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel_Useinidata = new ToolStripStatusLabel();
            toolStripStatusLabel_mode = new ToolStripStatusLabel();
            label_search_key = new Label();
            button_Search = new Button();
            textBox_Search = new TextBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Hex, Dec, Move_Name, Text_Pattern, Move_text, ID, Type, Category, Pow, PP, Accuracy, Prob, Attack_range, Priority, Move_flag, Ability_flag_1, Ability_flag_2, Effect_offset, Animation_offset, Description_offset, Move_Info });
            dataGridView1.Location = new Point(24, 154);
            dataGridView1.Margin = new Padding(7, 5, 7, 5);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(2705, 1085);
            dataGridView1.TabIndex = 0;
            dataGridView1.MouseClick += dataGridView1_MouseClick;
            dataGridView1.MouseDoubleClick += dataGridView1_MouseDoubleClick;
            // 
            // Hex
            // 
            Hex.HeaderText = "Hex";
            Hex.MinimumWidth = 6;
            Hex.Name = "Hex";
            Hex.ReadOnly = true;
            // 
            // Dec
            // 
            Dec.HeaderText = "Dec";
            Dec.MinimumWidth = 6;
            Dec.Name = "Dec";
            // 
            // Move_Name
            // 
            Move_Name.HeaderText = "Move_Name";
            Move_Name.MinimumWidth = 6;
            Move_Name.Name = "Move_Name";
            Move_Name.ReadOnly = true;
            // 
            // Text_Pattern
            // 
            Text_Pattern.HeaderText = "Text_Pattern";
            Text_Pattern.MinimumWidth = 6;
            Text_Pattern.Name = "Text_Pattern";
            Text_Pattern.ReadOnly = true;
            // 
            // Move_text
            // 
            Move_text.HeaderText = "Move_text";
            Move_text.MinimumWidth = 6;
            Move_text.Name = "Move_text";
            Move_text.ReadOnly = true;
            // 
            // ID
            // 
            ID.HeaderText = "ID";
            ID.MinimumWidth = 15;
            ID.Name = "ID";
            ID.ReadOnly = true;
            // 
            // Type
            // 
            Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Type.HeaderText = "Type";
            Type.ImageLayout = DataGridViewImageCellLayout.Zoom;
            Type.MinimumWidth = 6;
            Type.Name = "Type";
            Type.Resizable = DataGridViewTriState.True;
            Type.SortMode = DataGridViewColumnSortMode.Automatic;
            Type.Width = 80;
            // 
            // Category
            // 
            Category.HeaderText = "Category";
            Category.ImageLayout = DataGridViewImageCellLayout.Zoom;
            Category.MinimumWidth = 6;
            Category.Name = "Category";
            // 
            // Pow
            // 
            Pow.HeaderText = "Pow";
            Pow.MinimumWidth = 15;
            Pow.Name = "Pow";
            Pow.ReadOnly = true;
            // 
            // PP
            // 
            PP.HeaderText = "PP";
            PP.MinimumWidth = 15;
            PP.Name = "PP";
            PP.ReadOnly = true;
            // 
            // Accuracy
            // 
            Accuracy.HeaderText = "Accuracy";
            Accuracy.MinimumWidth = 15;
            Accuracy.Name = "Accuracy";
            Accuracy.ReadOnly = true;
            // 
            // Prob
            // 
            Prob.HeaderText = "Prob(%)";
            Prob.MinimumWidth = 15;
            Prob.Name = "Prob";
            Prob.ReadOnly = true;
            // 
            // Attack_range
            // 
            Attack_range.HeaderText = "Attack_range";
            Attack_range.MinimumWidth = 6;
            Attack_range.Name = "Attack_range";
            Attack_range.ReadOnly = true;
            // 
            // Priority
            // 
            Priority.HeaderText = "Priority";
            Priority.MinimumWidth = 6;
            Priority.Name = "Priority";
            Priority.ReadOnly = true;
            // 
            // Move_flag
            // 
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            Move_flag.DefaultCellStyle = dataGridViewCellStyle7;
            Move_flag.HeaderText = "Move_flag";
            Move_flag.MinimumWidth = 6;
            Move_flag.Name = "Move_flag";
            Move_flag.ReadOnly = true;
            // 
            // Ability_flag_1
            // 
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.True;
            Ability_flag_1.DefaultCellStyle = dataGridViewCellStyle8;
            Ability_flag_1.HeaderText = "Ability_flag_1";
            Ability_flag_1.MinimumWidth = 6;
            Ability_flag_1.Name = "Ability_flag_1";
            Ability_flag_1.ReadOnly = true;
            // 
            // Ability_flag_2
            // 
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.True;
            Ability_flag_2.DefaultCellStyle = dataGridViewCellStyle9;
            Ability_flag_2.HeaderText = "Ability_flag_2";
            Ability_flag_2.MinimumWidth = 6;
            Ability_flag_2.Name = "Ability_flag_2";
            Ability_flag_2.ReadOnly = true;
            // 
            // Effect_offset
            // 
            Effect_offset.HeaderText = "Effect_offset";
            Effect_offset.MinimumWidth = 6;
            Effect_offset.Name = "Effect_offset";
            // 
            // Animation_offset
            // 
            Animation_offset.HeaderText = "Animation_offset";
            Animation_offset.MinimumWidth = 15;
            Animation_offset.Name = "Animation_offset";
            // 
            // Description_offset
            // 
            Description_offset.HeaderText = "Description_offset";
            Description_offset.MinimumWidth = 15;
            Description_offset.Name = "Description_offset";
            // 
            // Move_Info
            // 
            Move_Info.HeaderText = "Move_Info";
            Move_Info.MinimumWidth = 6;
            Move_Info.Name = "Move_Info";
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Move Name";
            dataGridViewTextBoxColumn1.MinimumWidth = 6;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 125;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Script Address";
            dataGridViewTextBoxColumn2.MinimumWidth = 6;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 200;
            // 
            // Number
            // 
            Number.HeaderText = "Number";
            Number.MinimumWidth = 6;
            Number.Name = "Number";
            Number.Width = 125;
            // 
            // label_select_num_sub
            // 
            label_select_num_sub.AutoSize = true;
            label_select_num_sub.Location = new Point(670, 85);
            label_select_num_sub.Margin = new Padding(2, 0, 2, 0);
            label_select_num_sub.Name = "label_select_num_sub";
            label_select_num_sub.Size = new Size(170, 41);
            label_select_num_sub.TabIndex = 1;
            label_select_num_sub.Text = "Select Num";
            // 
            // label_select_num
            // 
            label_select_num.AutoSize = true;
            label_select_num.Location = new Point(848, 85);
            label_select_num.Margin = new Padding(2, 0, 2, 0);
            label_select_num.Name = "label_select_num";
            label_select_num.Size = new Size(42, 41);
            label_select_num.TabIndex = 2;
            label_select_num.Text = "--";
            // 
            // label_path
            // 
            label_path.AutoSize = true;
            label_path.Location = new Point(2526, 82);
            label_path.Margin = new Padding(2, 0, 2, 0);
            label_path.Name = "label_path";
            label_path.Size = new Size(209, 41);
            label_path.TabIndex = 3;
            label_path.Text = "get_label_path";
            label_path.Visible = false;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_Export_CSV });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(12, 5, 0, 5);
            menuStrip1.Size = new Size(2759, 55);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem_Export_CSV
            // 
            toolStripMenuItem_Export_CSV.Image = Properties.Resources.Excel_icon;
            toolStripMenuItem_Export_CSV.Name = "toolStripMenuItem_Export_CSV";
            toolStripMenuItem_Export_CSV.Size = new Size(207, 45);
            toolStripMenuItem_Export_CSV.Text = "Export_xlsx";
            toolStripMenuItem_Export_CSV.Click += toolStripMenuItem_Export_CSV_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabel_Useinidata, toolStripStatusLabel_mode });
            statusStrip1.LayoutStyle = ToolStripLayoutStyle.Flow;
            statusStrip1.Location = new Point(0, 1258);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(2, 0, 29, 0);
            statusStrip1.Size = new Size(2759, 60);
            statusStrip1.TabIndex = 7;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(850, 44);
            // 
            // toolStripStatusLabel_Useinidata
            // 
            toolStripStatusLabel_Useinidata.Name = "toolStripStatusLabel_Useinidata";
            toolStripStatusLabel_Useinidata.Size = new Size(177, 41);
            toolStripStatusLabel_Useinidata.Text = "ReadiniData";
            // 
            // toolStripStatusLabel_mode
            // 
            toolStripStatusLabel_mode.Name = "toolStripStatusLabel_mode";
            toolStripStatusLabel_mode.Size = new Size(187, 41);
            toolStripStatusLabel_mode.Text = "Select_mode";
            // 
            // label_search_key
            // 
            label_search_key.AutoSize = true;
            label_search_key.Location = new Point(2477, 79);
            label_search_key.Margin = new Padding(2, 0, 2, 0);
            label_search_key.Name = "label_search_key";
            label_search_key.Size = new Size(42, 41);
            label_search_key.TabIndex = 8;
            label_search_key.Text = "--";
            label_search_key.Visible = false;
            // 
            // button_Search
            // 
            button_Search.Location = new Point(24, 71);
            button_Search.Margin = new Padding(2, 3, 2, 3);
            button_Search.Name = "button_Search";
            button_Search.Size = new Size(170, 66);
            button_Search.TabIndex = 9;
            button_Search.Text = "Search";
            button_Search.UseVisualStyleBackColor = true;
            button_Search.Click += button_Search_Click;
            // 
            // textBox_Search
            // 
            textBox_Search.Location = new Point(209, 74);
            textBox_Search.Margin = new Padding(2, 3, 2, 3);
            textBox_Search.Name = "textBox_Search";
            textBox_Search.Size = new Size(434, 47);
            textBox_Search.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(942, 55);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(805, 82);
            label1.TabIndex = 11;
            label1.Text = "※空欄で検索すると登録された技の全検索結果が表示されます\r\n※選択したい技をダブルクリックするとメインフォーム側に反映されます";
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2759, 1318);
            Controls.Add(label1);
            Controls.Add(textBox_Search);
            Controls.Add(button_Search);
            Controls.Add(label_search_key);
            Controls.Add(statusStrip1);
            Controls.Add(label_path);
            Controls.Add(label_select_num);
            Controls.Add(label_select_num_sub);
            Controls.Add(dataGridView1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(7, 5, 7, 5);
            Name = "Form3";
            Text = "Move Search List";
            Shown += Form3_Shown;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn Number;
        private Label label_select_num_sub;
        private Label label_select_num;
        private Label label_path;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem_Export_CSV;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar toolStripProgressBar1;
        private Label label_search_key;
        private Button button_Search;
        private TextBox textBox_Search;
        private ToolStripStatusLabel toolStripStatusLabel_Useinidata;
        private ToolStripStatusLabel toolStripStatusLabel_mode;
        private Label label1;
        private DataGridViewTextBoxColumn Hex;
        private DataGridViewTextBoxColumn Dec;
        private DataGridViewTextBoxColumn Move_Name;
        private DataGridViewTextBoxColumn Text_Pattern;
        private DataGridViewTextBoxColumn Move_text;
        private DataGridViewTextBoxColumn ID;
        private DataGridViewImageColumn Type;
        private DataGridViewImageColumn Category;
        private DataGridViewTextBoxColumn Pow;
        private DataGridViewTextBoxColumn PP;
        private DataGridViewTextBoxColumn Accuracy;
        private DataGridViewTextBoxColumn Prob;
        private DataGridViewTextBoxColumn Attack_range;
        private DataGridViewTextBoxColumn Priority;
        private DataGridViewTextBoxColumn Move_flag;
        private DataGridViewTextBoxColumn Ability_flag_1;
        private DataGridViewTextBoxColumn Ability_flag_2;
        private DataGridViewTextBoxColumn Effect_offset;
        private DataGridViewTextBoxColumn Animation_offset;
        private DataGridViewTextBoxColumn Description_offset;
        private DataGridViewTextBoxColumn Move_Info;
    }
}