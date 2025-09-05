using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mudkip_Script_Editor
{
    public partial class Form4 : Form
    {
        //-------------------------------------------------------------------------------
        //GetPrivateProfileInt関数の宣言(iniファイルの読込)
        //-------------------------------------------------------------------------------
        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileInt(
            string lpAppName,   //セクション名
            string lpKeyName,   //キー名
            int nDefault,       //ini読込失敗時に返す値
            string lpFileName); //iniファイルのパス

        [DllImport("KERNEL32.DLL")]
        private static extern int GetPrivateProfileString(
            string lpAppName_string,
            string lpKeyName_string,
            string lpDefault_string,
            StringBuilder lpReturnedString,
            uint nSize,
            string lpFileName_string);

        //-------------------------------------------------------------------------------
        //メインフォームの参照を設定
        //-------------------------------------------------------------------------------
        Form1 f1;

        public Form4(string path, int decValue, string inifolder, string Max_typeNum, Form1 f)
        {
            InitializeComponent();  //↑メインフォームから必要情報を取得
            f1 = f;                 //メインフォームへの参照を保存
            label_path.Text = path; //メイン処理から引き継いだパスを取得
            label_address.Text = decValue.ToString();
            toolStripStatusLabel_Useinidata.Text = inifolder;
            toolStripStatusLabel_MaxTypeNum.Text = Max_typeNum;

            label_Chart_item1.Text = "○：効果抜群（x2）\n△：いまひとつ（1/2）\n×：こうかなし（x0）";
            label_Chart_item2.Text = "●：効果抜群（AI判定有り）\n▲：いまひとつ（AI判定有り）\n■：こうかなし（AI判定有り）";
        }

        private void Form4_Shown(object sender, EventArgs e)
        {
            //PC設定毎に表示サイズが変化することを防ぐ
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            TypeChart_builder(label_path.Text, int.Parse(label_address.Text)); //読込Address位置からScriptデータを抽出
        }
        private void TypeChart_builder(string path, int decValue)
        {
            try
            {
                int Max_typeNum = int.Parse(toolStripStatusLabel_MaxTypeNum.Text);

                int icon = 0;
                string Load_Type = "";
                // 予め行を11行追加----------------------------------------
                for (int i = 0; i < Max_typeNum + 1; i++)
                {
                    icon = i - 1;
                    Load_Type = "Type_" + icon.ToString("X").PadLeft(2, '0').ToLower();

                    //最初の行のみアイコンの追加をとばす
                    if (i != 0)
                    {
                        //それ以外の行の処理
                        dataGridView1.Rows.Add(
                        Properties.Resources.ResourceManager.GetObject(Load_Type, Properties.Resources.Culture));
                        // Titleと表示名を指定して追加
                        dataGridView1.Columns.Add(Load_Type, icon.ToString("X").PadLeft(2, '0').ToLower());

                        string image_file = AppDomain.CurrentDomain.BaseDirectory + "image\\" + Load_Type + ".png";
                        Bitmap myBitmap = new Bitmap(image_file);
                        Color pixelColor = myBitmap.GetPixel(10, 10);

                        //参照画像から色を取得して1行目に設定
                        dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
                        myBitmap.Dispose(); //使用済みのBitMapオブジェクトを開放する
                    }
                    else
                    {
                        //タイトル行の処理
                        dataGridView1.Rows.Add(
                        Properties.Resources.ResourceManager.GetObject("Blank", Properties.Resources.Culture));

                    }
                    dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                }

                // リストからチャートのdataを3byteずつ読込-----------------
                var code_data = "";
                var Apply_data = "";
                var count_val = 0;

                int Get_row = 0;
                int Get_column = 0;
                int Get_mul = 0;

                string flg_font = "";
                bool SP_flg = false;

                var reader = new FileStream(path, FileMode.Open);

                byte[] data = new byte[decValue + 500]; //全量データの終端が判らないので一旦500byte読み込んで処理する
                reader.Read(data, 0, data.Length);
                reader.Close();

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Yu Gothic UI", 8); //見出しのフォントの種類と大きさ
                dataGridView1.DefaultCellStyle.Font = new Font("Yu Gothic UI", 10, FontStyle.Bold);              //本文のフォントの種類と大きさ
                var info_list = new List<string>();

                for (int i = decValue; i < data.Length; i++)
                {
                start_roll:
                    code_data = string.Format("{0:x2}", data[i]);
                    Apply_data += code_data;
                    count_val = count_val + 1;

                    if (code_data == "fe")
                    {
                        i += 3; //FEを読み込んだらフラグを立てて3byte分とばす
                        SP_flg = true;
                        count_val = 0;
                        goto start_roll;
                    }
                    if (code_data == "ff")
                    {
                        goto chart_End;

                    }

                    //3byteずつの繰り返しで読み込む
                    switch (count_val)
                    {
                        case 1:
                            Get_row = Convert.ToInt32(code_data, 16) + 1; //現在の取得行を10進数に変換
                            break;

                        case 2:
                            Get_column = Convert.ToInt32(code_data, 16) + 1;//現在の取得列を10進数に変換
                            break;

                        case 3:
                            Get_mul = Convert.ToInt32(code_data, 16);


                            switch (Get_mul)
                            {
                                case 0:
                                    if (SP_flg == true) { flg_font = "■"; } else { flg_font = "×"; }
                                    dataGridView1.Rows[Get_row].Cells[Get_column].Value = flg_font;
                                    dataGridView1.Rows[Get_row].Cells[Get_column].Style.BackColor = Color.FromArgb(207, 183, 255);
                                    break;

                                case 5:
                                    if (SP_flg == true) { flg_font = "▲"; } else { flg_font = "△"; }
                                    dataGridView1.Rows[Get_row].Cells[Get_column].Value = flg_font;
                                    dataGridView1.Rows[Get_row].Cells[Get_column].Style.BackColor = Color.FromArgb(221, 235, 247);
                                    break;

                                case 20:
                                    if (SP_flg == true) { flg_font = "●"; } else { flg_font = "○"; }
                                    dataGridView1.Rows[Get_row].Cells[Get_column].Value = flg_font;
                                    dataGridView1.Rows[Get_row].Cells[Get_column].Style.BackColor = Color.FromArgb(255, 225, 225);
                                    break;

                                default://読み込みなし                          
                                    break;

                            }
                            count_val = 0; //三の倍数を確認後にカウンターを戻す
                            break;

                        default://読み込みなし                          
                            break;

                    }

                }
            chart_End:
                count_val = 0;

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable; //全ての行のソートを無効化する
                }

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                toolStripStatusLabell_first_flag.Text = "1"; //初回読込フラグを記載
            }
            catch
            {

            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string editflg = toolStripStatusLabell_first_flag.Text;
            if (editflg == "0") { goto Math_End; } //最初の読込時に反応されたら困るため

            //-------------------------------------------------------------------------------
            //一覧表が編集された際の処理
            //-------------------------------------------------------------------------------
            // 編集されたセルの行と列のインデックスを取得
            int Get_row = e.RowIndex;
            int Get_column = e.ColumnIndex;

            string SP_num = "";

            // 編集されたセルの値を取得
            object editedValue = dataGridView1.Rows[Get_row].Cells[Get_column].Value;

            switch (editedValue)
            {
                //片方の内容が書き換えられたら反対側のチャートも変更する
                case "×" or "■":
                    dataGridView1.Rows[Get_row].Cells[Get_column].Style.BackColor = Color.FromArgb(207, 183, 255);
                    break;

                case "△" or "▲":
                    dataGridView1.Rows[Get_row].Cells[Get_column].Style.BackColor = Color.FromArgb(221, 235, 247);
                    break;

                case "○" or "●":
                    dataGridView1.Rows[Get_row].Cells[Get_column].Style.BackColor = Color.FromArgb(255, 225, 225);
                    break;

                default://読み込みなし
                    dataGridView1.Rows[Get_row].Cells[Get_column].Style.BackColor = Color.FromArgb(255, 255, 255);
                    break;
            }

            //-------------------------------------------------------------------------------
            //メインフォームに選択値を渡す
            //-------------------------------------------------------------------------------

            int lastRow = dataGridView1.Rows.Count - 1;    //最終行
            int lastColumn = dataGridView1.Columns.Count - 1; //最終列

            string Get_Chart_num = "";

            int i = 1;
            int j = 1;
            int st_j = 1;

            for (i = 1; i < lastRow + 1; i++)
            {
                for (j = st_j; j < lastColumn; j++)
                {
                    Get_row = i - 1;
                    Get_column = j - 1;
                    object Loadvalue = dataGridView1.Rows[i].Cells[j].Value;
                    switch (Loadvalue)
                    {
                        case "×":
                            Get_Chart_num += Get_row.ToString("X").PadLeft(2, '0').ToLower() + " "
                                           + Get_column.ToString("X").PadLeft(2, '0').ToLower() + " "
                                           + "00 ";
                            break;
                        case "■":
                            SP_num += Get_row.ToString("X").PadLeft(2, '0').ToLower() + " "
                                    + Get_column.ToString("X").PadLeft(2, '0').ToLower() + " "
                                    + "00 ";
                            break;

                        case "△":
                            Get_Chart_num += Get_row.ToString("X").PadLeft(2, '0').ToLower() + " "
                                           + Get_column.ToString("X").PadLeft(2, '0').ToLower() + " "
                                           + "05 ";
                            break;
                        case "▲":
                            SP_num += Get_row.ToString("X").PadLeft(2, '0').ToLower() + " "
                                    + Get_column.ToString("X").PadLeft(2, '0').ToLower() + " "
                                    + "05 ";
                            break;

                        case "○":
                            Get_Chart_num += Get_row.ToString("X").PadLeft(2, '0').ToLower() + " "
                                           + Get_column.ToString("X").PadLeft(2, '0').ToLower() + " "
                                           + "14 ";
                            break;
                        case "●":
                            SP_num += Get_row.ToString("X").PadLeft(2, '0').ToLower() + " "
                                    + Get_column.ToString("X").PadLeft(2, '0').ToLower() + " "
                                    + "14 ";
                            break;

                        default://読み込みなし

                            break;
                    }
                    //dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Red;
                }

                if (checkBox_optimization_flg .Checked ==true)
                {
                    st_j = st_j + 1;　//最適化摘要時はチャートは片方反映できれば良いので（ダブリ防止）
                }
                
            }

            Get_Chart_num += "fe fe 00 " + SP_num + "ff"; //ラストに特殊パターンを記載
            Get_Chart_num.TrimEnd();

            //対象フォームのデザイナ画面を開き、テキストボックスを選択した状態で、
            //プロパティの「Modifiers」を「Public」か「Interal」に設定すれば他のクラスからアクセスすることができます。
            f1.richTextBox_type_Chart.Text = Get_Chart_num;

        Math_End:

            lastRow = 0;
        }

        private void button_Height_adjustment_Click(object sender, EventArgs e)
        {
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //行の高さの自動調整
        }
    }
}
