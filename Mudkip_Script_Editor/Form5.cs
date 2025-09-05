using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mudkip_Script_Editor
{
    public partial class Form5 : Form
    {
        public Form5(string MoveID,string MoveName,string Tab,string Before_address,string After_address,string Before_data,string After_data)
        {
            InitializeComponent();
            label_MoveID_sub.Text          = MoveID.TrimEnd();
            label_Move_Name_sub.Text       = MoveName.TrimEnd();
            label_Tab_name.Text            = Tab.TrimEnd();
            textBox_before_address.Text    = Before_address.TrimEnd();
            textBox_aftrt_address.Text     = After_address.TrimEnd();
            richTextBox_Cjange_Before.Text = Before_data.TrimEnd();
            richTextBox_Change_After.Text  = After_data.TrimEnd();

        }

        private void Form5_Load(object sender, EventArgs e)
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            string Before_data = richTextBox_Cjange_Before.Text;
            string After_data = richTextBox_Change_After.Text;

            string Before_address = textBox_before_address.Text;
            string After_address = textBox_aftrt_address.Text;

            label_Information_text.Text = "内容：参照先を " + Before_address + " から " + After_address + " に更新して値を変更";

            if (Before_data == After_data)
            {
                label_Information_text.Text = "内容：" + Before_address + " から " + After_address + " に値をコピー";
            }

            if (Before_address == After_address)
            {
                label_Information_text.Text = "内容：" + After_address + " の値を変更";
            }


        }
    }
}
