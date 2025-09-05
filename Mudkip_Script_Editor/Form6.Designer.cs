namespace Mudkip_Script_Editor
{
    partial class Form6
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
            label_iniMaker_txt1 = new Label();
            label_iniMaker_txt2 = new Label();
            label_iniMaker_txt3 = new Label();
            label_iniMaker_txt4 = new Label();
            SuspendLayout();
            // 
            // label_iniMaker_txt1
            // 
            label_iniMaker_txt1.AutoSize = true;
            label_iniMaker_txt1.Location = new Point(36, 36);
            label_iniMaker_txt1.Name = "label_iniMaker_txt1";
            label_iniMaker_txt1.Size = new Size(535, 41);
            label_iniMaker_txt1.TabIndex = 0;
            label_iniMaker_txt1.Text = "新規のiniに登録する情報を指定してください";
            // 
            // label_iniMaker_txt2
            // 
            label_iniMaker_txt2.AutoSize = true;
            label_iniMaker_txt2.Location = new Point(36, 154);
            label_iniMaker_txt2.Name = "label_iniMaker_txt2";
            label_iniMaker_txt2.Size = new Size(242, 41);
            label_iniMaker_txt2.TabIndex = 1;
            label_iniMaker_txt2.Text = "初回で読み込むID";
            // 
            // label_iniMaker_txt3
            // 
            label_iniMaker_txt3.AutoSize = true;
            label_iniMaker_txt3.Location = new Point(36, 222);
            label_iniMaker_txt3.Name = "label_iniMaker_txt3";
            label_iniMaker_txt3.Size = new Size(138, 41);
            label_iniMaker_txt3.TabIndex = 2;
            label_iniMaker_txt3.Text = "登録技数";
            // 
            // label_iniMaker_txt4
            // 
            label_iniMaker_txt4.AutoSize = true;
            label_iniMaker_txt4.Location = new Point(36, 286);
            label_iniMaker_txt4.Name = "label_iniMaker_txt4";
            label_iniMaker_txt4.Size = new Size(138, 41);
            label_iniMaker_txt4.TabIndex = 3;
            label_iniMaker_txt4.Text = "登録技数";
            // 
            // Form6
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1192, 1377);
            Controls.Add(label_iniMaker_txt4);
            Controls.Add(label_iniMaker_txt3);
            Controls.Add(label_iniMaker_txt2);
            Controls.Add(label_iniMaker_txt1);
            Name = "Form6";
            Text = "Form6";
            Load += Form6_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_iniMaker_txt1;
        private Label label_iniMaker_txt2;
        private Label label_iniMaker_txt3;
        private Label label_iniMaker_txt4;
    }
}