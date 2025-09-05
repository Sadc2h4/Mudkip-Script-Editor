using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Mudkip_Script_Editor
{
    public partial class Form3 : Form
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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
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

        public Form3(string path, string LoadSection, string Search_word, string Select_mode, Form1 f)
        {
            InitializeComponent();//↑メインフォームから必要情報を取得
            f1 = f;               //メインフォームへの参照を保存
            label_path.Text = path;
            toolStripStatusLabel_Useinidata.Text = LoadSection;
            textBox_Search.Text = Search_word;
            toolStripStatusLabel_mode.Text = Select_mode;

        }
        private void Form3_Shown(object sender, EventArgs e)
        {
            //PC設定毎に表示サイズが変化することを防ぐ
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;


            switch (toolStripStatusLabel_mode.Text)
            {
                case "move":
                    Script_builder(label_path.Text); //読込Address位置からScriptデータを抽出
                    break;

                case "address":
                    Address_builder(label_path.Text);
                    break;

                case "BattleText":
                    Text_builder();
                    break;

                default:
                    break;
            }
        }
        private void Script_builder(string path)
        {
            //-------------------------------------------------------------------------------
            //iniから読込文字列を取得
            //-------------------------------------------------------------------------------
            int counter = 0;
            //選択したiniのデータベースを取得する
            string inifolder = toolStripStatusLabel_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\label_name.ini"; //テキストを読み込む
            List<string> get_label_value = new List<string>();                                  //格納用リストを作成
            foreach (string line in System.IO.File.ReadLines(iniFileName))                      //リスト変数にテキストの記載内容を１行ずつ書き込み
            {
                get_label_value.Add(line);
                counter++;
            }
            //-------------------------------------------------------------------------------
            //iniの情報を取得して変数に格納する
            //-------------------------------------------------------------------------------
            iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Config.ini";

            //変数を指定
            string LoadSection = "DataBase";
            string Loadkey = "";
            string LoadText = "";
            int math_value = 0;

            int Max_MoveNumber = 0;
            Loadkey = "Max_MoveNumber";
            Max_MoveNumber = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int Effect_Table_Length = 0;
            Loadkey = "Effect_Table_Length";
            Effect_Table_Length = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int Effect_Table = 0;
            Loadkey = "Effect_Table";
            Effect_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int MoveInfo_Table_Length = 0;
            Loadkey = "MoveInfo_Table_Length";
            MoveInfo_Table_Length = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int MoveInfo_Table = 0;
            Loadkey = "MoveInfo_Table";
            MoveInfo_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int MoveName_Table_Length = 0;
            Loadkey = "MoveName_Table_Length";
            MoveName_Table_Length = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int MoveName_Table = 0;
            Loadkey = "MoveName_Table";
            MoveName_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int MoveText_Table_Length = 0;
            Loadkey = "MoveText_Table_Length";
            MoveText_Table_Length = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int MoveText_Table = 0;
            Loadkey = "MoveText_Table";
            MoveText_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int Animation_Table_Length = 0;
            Loadkey = "Animation_Table_Length";
            Animation_Table_Length = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int Animation_Table = 0;
            Loadkey = "Animation_Table";
            Animation_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

            int TextPattern_Select = 0;
            Loadkey = "TextPattern_Select";
            TextPattern_Select = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);


            iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Effect_ID_info.ini";

            toolStripProgressBar1.Minimum = 0;                    //最小値を設定
            toolStripProgressBar1.Maximum = Max_MoveNumber + 10;  //最大値を設定
            toolStripProgressBar1.Value = 0;                      //プログレスバーを記載

            string Search_info = textBox_Search.Text;             //検索用文字列の記載

            //-------------------------------------------------------------------------------
            //情報をテーブルに記載する処理
            //-------------------------------------------------------------------------------

            for (int i = 0; i < Max_MoveNumber; i++)
            {
                //16進数
                string Hex_num = i.ToString("X");   //16進数

                //名前
                math_value = MoveName_Table + (i * MoveName_Table_Length);　 //技名
                string Load_Move_Name = Read_binary_data(path, math_value, MoveName_Table_Length);
                Load_Move_Name = FontData_exchange(Load_Move_Name);

                //検索文字列が含まれる場合はtrueを返す
                bool Search_Flg = Load_Move_Name.Contains(Search_info);
                //名前から検索を行い、条件に合うものを追加する
                if ((Search_Flg == true) || (Search_info == ""))
                {

                    //技テキスト
                    math_value = MoveText_Table + (i * MoveText_Table_Length); //技説明文
                    string Load_Move_Text = Read_binary_data(path, math_value, MoveText_Table_Length);
                    Load_Move_Text = FontData_exchange(Load_Move_Text);
                    string Load_Description_offset = math_value.ToString("X").PadLeft(8, '0');

                    //情報
                    math_value = MoveInfo_Table + (i * MoveInfo_Table_Length); //技情報
                    string Load_Move_Info = Read_binary_data(path, math_value, MoveInfo_Table_Length);

                    //アニメアドレス
                    math_value = Animation_Table + (i * Animation_Table_Length); //アニメーション
                    string Load_Anime_Info = Read_binary_data(path, math_value, Animation_Table_Length);
                    LoadText = Read_binary_data(path, math_value + 3, 1) + Read_binary_data(path, math_value + 2, 1) +
                               Read_binary_data(path, math_value + 1, 1) + Read_binary_data(path, math_value, 1); //無理やりビッグエンディアン
                    int decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                    string Load_Anime_offset = decValue.ToString("X").PadLeft(8, '0');

                    string[] Move_Info_unit = Load_Move_Info.ToString().Split(' ');

                    //効果スクリプト
                    int j = Convert.ToInt32(Move_Info_unit[0], 16);
                    math_value = Effect_Table + (j * Effect_Table_Length);         //効果スクリプト
                    LoadText = Read_binary_data(path, math_value + 3, 1) + Read_binary_data(path, math_value + 2, 1) +
                               Read_binary_data(path, math_value + 1, 1) + Read_binary_data(path, math_value, 1); //無理やりビッグエンディアン
                    decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                    string Load_Script_offset = decValue.ToString("X").PadLeft(8, '0');

                    //ID
                    Loadkey = Move_Info_unit[0].ToString();
                    StringBuilder sb = new StringBuilder(1024);
                    int res = GetPrivateProfileString("Effect", Loadkey, "読込エラー", sb, (uint)sb.Capacity, iniFileName); //専用のテキストiniからデータを呼び出す
                    string Load_ID = sb.ToString().ToUpper(); //文字列の英語文字を大文字に変更

                    //Pow
                    string Load_Pow = Convert.ToInt32(Move_Info_unit[1], 16).ToString();

                    //PP
                    string Load_PP = Convert.ToInt32(Move_Info_unit[4], 16).ToString();

                    //Accuracy
                    string Load_Accuracy = Convert.ToInt32(Move_Info_unit[3], 16).ToString();

                    //Prob
                    string Load_Prob = Convert.ToInt32(Move_Info_unit[5], 16).ToString();

                    //type
                    string Load_Type = Move_Info_unit[2].ToString();
                    Load_Type = "Type_" + Load_Type;

                    //Category
                    string Load_Category = Move_Info_unit[10].ToString();
                    Load_Category = "Category_" + Load_Category;

                    //Attack_range
                    string Load_Range = Move_Info_unit[6].ToString();
                    switch (Load_Range)
                    {
                        case "00":
                            Load_Range = "通常";
                            break;
                        case "01":
                            Load_Range = "対象不定";
                            break;
                        case "02":
                            Load_Range = "未使用";
                            break;
                        case "04":
                            Load_Range = "複数ターン技";
                            break;
                        case "08":
                            Load_Range = "相手2体";
                            break;
                        case "10":
                            Load_Range = "自分 or 場全体";
                            break;
                        case "20":
                            Load_Range = "自分以外";
                            break;
                        case "40":
                            Load_Range = "設置技";
                            break;
                        default:
                            Load_Range = "未定義";
                            break;
                    }

                    //Priority
                    string Load_Priority = Move_Info_unit[7].ToString();
                    switch (Load_Priority)
                    {
                        case "00":
                            Load_Priority = "優先度 ±0";
                            break;
                        case "01":
                            Load_Priority = "優先度 +1";
                            break;
                        case "02":
                            Load_Priority = "優先度 +2";
                            break;
                        case "03":
                            Load_Priority = "優先度 +3";
                            break;
                        case "04":
                            Load_Priority = "優先度 +4";
                            break;
                        case "05":
                            Load_Priority = "優先度 +5";
                            break;
                        case "06":
                            Load_Priority = "優先度 +6";
                            break;
                        case "fa":
                            Load_Priority = "優先度 -1";
                            break;
                        case "fb":
                            Load_Priority = "優先度 -2";
                            break;
                        case "fc":
                            Load_Priority = "優先度 -3";
                            break;
                        case "fd":
                            Load_Priority = "優先度 -4";
                            break;
                        case "fe":
                            Load_Priority = "優先度 -5";
                            break;
                        case "ff":
                            Load_Priority = "優先度 -6";
                            break;
                        default:
                            Load_Priority = "未定義";
                            break;
                    }

                    //表示Pattern
                    LoadText = Read_binary_data(path, TextPattern_Select + 3, 1) + Read_binary_data(path, TextPattern_Select + 2, 1) +
                               Read_binary_data(path, TextPattern_Select + 1, 1) + Read_binary_data(path, TextPattern_Select, 1); //無理やりビッグエンディアン
                    decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                    string Load_MovePattern = Read_Move_binary_2byte(path, decValue, Hex_num);

                    //Move_Flag
                    string Load_Move_Flag = Convert.ToInt32(Move_Info_unit[8], 16).ToString();
                    (bool, bool, bool, bool, bool, bool, bool, bool) get_flag_tuple = get_flag(int.Parse(Load_Move_Flag));
                    Load_Move_Flag = "";
                    if (get_flag_tuple.Item1 == true) { Load_Move_Flag += get_label_value[15] + "\r\n"; }
                    if (get_flag_tuple.Item2 == true) { Load_Move_Flag += get_label_value[16] + "\r\n"; }
                    if (get_flag_tuple.Item3 == true) { Load_Move_Flag += get_label_value[17] + "\r\n"; }
                    if (get_flag_tuple.Item4 == true) { Load_Move_Flag += get_label_value[18] + "\r\n"; }
                    if (get_flag_tuple.Item5 == true) { Load_Move_Flag += get_label_value[19] + "\r\n"; }
                    if (get_flag_tuple.Item6 == true) { Load_Move_Flag += get_label_value[20] + "\r\n"; }
                    if (get_flag_tuple.Item7 == true) { Load_Move_Flag += get_label_value[21] + "\r\n"; }
                    if (get_flag_tuple.Item8 == true) { Load_Move_Flag += get_label_value[22] + "\r\n"; }
                    Load_Move_Flag.Trim();  //先頭と末尾の空白と改行を削除

                    //Ability_Flag1
                    string Load_Ability_Flag1 = Convert.ToInt32(Move_Info_unit[9], 16).ToString();
                    get_flag_tuple = get_flag(int.Parse(Load_Ability_Flag1));
                    Load_Ability_Flag1 = "";
                    if (get_flag_tuple.Item1 == true) { Load_Ability_Flag1 += get_label_value[24] + "\r\n"; }
                    if (get_flag_tuple.Item2 == true) { Load_Ability_Flag1 += get_label_value[25] + "\r\n"; }
                    if (get_flag_tuple.Item3 == true) { Load_Ability_Flag1 += get_label_value[26] + "\r\n"; }
                    if (get_flag_tuple.Item4 == true) { Load_Ability_Flag1 += get_label_value[27] + "\r\n"; }
                    if (get_flag_tuple.Item5 == true) { Load_Ability_Flag1 += get_label_value[28] + "\r\n"; }
                    if (get_flag_tuple.Item6 == true) { Load_Ability_Flag1 += get_label_value[29] + "\r\n"; }
                    if (get_flag_tuple.Item7 == true) { Load_Ability_Flag1 += get_label_value[30] + "\r\n"; }
                    if (get_flag_tuple.Item8 == true) { Load_Ability_Flag1 += get_label_value[31] + "\r\n"; }
                    Load_Ability_Flag1.Trim();

                    //Ability_Flag2
                    string Load_Ability_Flag2 = Convert.ToInt32(Move_Info_unit[11], 16).ToString();
                    get_flag_tuple = get_flag(int.Parse(Load_Ability_Flag2));
                    Load_Ability_Flag2 = "";
                    if (get_flag_tuple.Item1 == true) { Load_Ability_Flag2 += get_label_value[33] + "\r\n"; }
                    if (get_flag_tuple.Item2 == true) { Load_Ability_Flag2 += get_label_value[34] + "\r\n"; }
                    if (get_flag_tuple.Item3 == true) { Load_Ability_Flag2 += get_label_value[35] + "\r\n"; }
                    if (get_flag_tuple.Item4 == true) { Load_Ability_Flag2 += get_label_value[36] + "\r\n"; }
                    if (get_flag_tuple.Item5 == true) { Load_Ability_Flag2 += get_label_value[37] + "\r\n"; }
                    if (get_flag_tuple.Item6 == true) { Load_Ability_Flag2 += get_label_value[38] + "\r\n"; }
                    if (get_flag_tuple.Item7 == true) { Load_Ability_Flag2 += get_label_value[39] + "\r\n"; }
                    if (get_flag_tuple.Item8 == true) { Load_Ability_Flag2 += get_label_value[40] + "\r\n"; }
                    Load_Ability_Flag2.Trim();

                    //Search条件が満たされる場合のみリストに追加される
                    dataGridView1.Rows.Add
                        (
                        Hex_num,
                        i,
                        Load_Move_Name,
                        Load_MovePattern,
                        Load_Move_Text,
                        Load_ID,
                        Properties.Resources.ResourceManager.GetObject(Load_Type, Properties.Resources.Culture),
                        Properties.Resources.ResourceManager.GetObject(Load_Category, Properties.Resources.Culture),
                        Load_Pow,
                        Load_PP,
                        Load_Accuracy,
                        Load_Prob,
                        Load_Range,
                        Load_Priority,
                        Load_Move_Flag,
                        Load_Ability_Flag1,
                        Load_Ability_Flag2,
                        Load_Script_offset,
                        Load_Anime_offset,
                        Load_Description_offset,
                        Load_Move_Info
                        );

                }
                toolStripProgressBar1.Value += 1;
                //progressBar1.PerformStep(); //プログレスバーを進捗させる

            }
            //-------------------------------------------------------------------------------
            //一覧の表示情報を設定
            //-------------------------------------------------------------------------------

            dataGridView1.Columns["Move_text"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //行の高さの自動調整
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //ヘッダー文字
            label_search_key.Text = get_label_value[2];
            dataGridView1.Columns[2].HeaderText = get_label_value[2];   //技名
            dataGridView1.Columns[3].HeaderText = get_label_value[51];  //表示パターン
            dataGridView1.Columns[4].HeaderText = get_label_value[42];  //説明文
            dataGridView1.Columns[5].HeaderText = get_label_value[5];　 //技ID
            dataGridView1.Columns[6].HeaderText = get_label_value[10];  //タイプ
            dataGridView1.Columns[7].HeaderText = get_label_value[11];  //分類
            dataGridView1.Columns[8].HeaderText = get_label_value[6];   //威力
            dataGridView1.Columns[9].HeaderText = get_label_value[7];   //技PP
            dataGridView1.Columns[10].HeaderText = get_label_value[8];  //命中率
            dataGridView1.Columns[11].HeaderText = get_label_value[9];  //追加効果
            dataGridView1.Columns[12].HeaderText = get_label_value[12]; //効果範囲
            dataGridView1.Columns[13].HeaderText = get_label_value[13]; //優先度
            dataGridView1.Columns[17].HeaderText = get_label_value[45];  //技効果Offset
            dataGridView1.Columns[18].HeaderText = get_label_value[48]; //技アニメOffset
            dataGridView1.Columns[19].HeaderText = get_label_value[42] + "Script";//説明文Offset
            dataGridView1.Columns[20].HeaderText = get_label_value[3];  //技情報

            toolStripProgressBar1.Value += 5;

            //列の色
            dataGridView1.Columns[14].DefaultCellStyle.BackColor = Color.LightBlue;
            dataGridView1.Columns[15].DefaultCellStyle.BackColor = Color.LightBlue;
            dataGridView1.Columns[16].DefaultCellStyle.BackColor = Color.LightBlue;
            dataGridView1.Columns[17].DefaultCellStyle.BackColor = Color.Moccasin;
            dataGridView1.Columns[18].DefaultCellStyle.BackColor = Color.Moccasin;
            dataGridView1.Columns[19].DefaultCellStyle.BackColor = Color.Moccasin;

            toolStripProgressBar1.Value += 5;
        }


        private string Read_binary_data(string path, int StartLen, int LoadLen)
        {
            //-------------------------------------------------------------------------------
            //パスから目当てのバイナリデータを読み込む処理
            //-------------------------------------------------------------------------------

            var reader = new FileStream(path, FileMode.Open);
            var Apply_data = "";

            //byte[] data = new byte[reader.Length];
            byte[] data = new byte[StartLen + LoadLen];
            reader.Read(data, 0, data.Length);
            reader.Close();

            for (int i = StartLen; i < data.Length; i++)
            {
                Apply_data += string.Format("{0:x2} ", data[i]);
            }

            string result = Apply_data.TrimEnd(); //最後に空欄が入るので消す処理
            return result;
        }

        private string FontData_exchange(string LoadText)
        {
            //-------------------------------------------------------------------------------
            //読み込みbyteデータを日本語に変換
            //-------------------------------------------------------------------------------
            string inifolder = toolStripStatusLabel_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";
            string[] data = LoadText.Split(' ');
            //byte[] data = new byte[bytestr.Length];
            var code_data = "";
            var Apply_data = "";

            for (int i = 0; i < data.Length; i++)
            {
                code_data = string.Format("{0:x2}", data[i]);

                //iniファイルの内容をキャストして変数に格納
                StringBuilder sb = new StringBuilder(1024);
                int res = GetPrivateProfileString("BM", data[i], "00", sb, (uint)sb.Capacity, iniFileName); //専用のテキストiniからデータを呼び出す

                switch (code_data)
                {
                    case "fa":
                        Apply_data = Apply_data + sb.ToString();
                        break;

                    case "fc":
                        code_data = string.Format("{0:x2}", data[i + 1]) + " " + string.Format("{0:x2}", data[i + 2]);
                        res = GetPrivateProfileString("BM_FC_kanji", code_data, "[未調査]", sb, (uint)sb.Capacity, iniFileName);//変数の一覧をiniから取得
                        Apply_data = Apply_data + sb.ToString();
                        i = i + 2; //2byte余計に進ませる
                        break;

                    case "fd":
                        code_data = string.Format("{0:x2}", data[i + 1]); //1byte先の値を取得する
                        res = GetPrivateProfileString("BM_FD_BattleText", code_data, "[未調査]", sb, (uint)sb.Capacity, iniFileName);//変数の一覧をiniから取得
                        Apply_data = Apply_data + sb.ToString();
                        i = i + 1; //1byte余計に進ませる
                        break;

                    case "fe":
                        Apply_data = Apply_data + sb.ToString() + "\r\n";
                        break;

                    case "ff":
                        Apply_data = Apply_data + sb.ToString();
                        return Apply_data; //終了文字列を見つけたら表記を終了させる

                    default:
                        Apply_data = Apply_data + sb.ToString();
                        break;
                }
            }
            return Apply_data;
        }
        private string Read_Move_binary_2byte(string path, int StartLen, string Hex_num)
        {
            //-------------------------------------------------------------------------------
            //パスに記載されたアドレスから2byteずつ読み込んで表示文字列を判定
            //-------------------------------------------------------------------------------
            var reader = new FileStream(path, FileMode.Open);
            var Apply_data = "";
            int move_num = 0;
            int load_num = 0;
            string get_text = "";
            int Count_flg = 1;

            byte[] data = new byte[StartLen + 1000];
            reader.Read(data, 0, data.Length);
            reader.Close();

            int i = StartLen;
            while (Count_flg < 5)　//５回目の00 00 を読み込んだ時点で処理を終了させる
            {
                //技IDを作成
                Apply_data = string.Format("{0:x2}", data[i + 1]) + string.Format("{0:x2}", data[i]);
                move_num = Convert.ToInt32(Apply_data, 16); //10進数に変換して技IDを作成
                load_num = Convert.ToInt32(Hex_num, 16);


                //2Byte分を読み込んで変数に成形
                Apply_data = string.Format("{0:x2}", data[i]) + " " + string.Format("{0:x2}", data[i + 1]);
                switch (Apply_data)
                {
                    case "00 00":
                        Count_flg = Count_flg + 1;
                        goto Read_Move_binary_2byte_skip;

                    case "ff ff":
                        goto Read_Move_binary_2byte_end;

                    default:
                        break;
                }
                if (load_num == move_num)
                {
                    switch (Count_flg)
                    {
                        case 1:
                            get_text = "を　つかった！";
                            goto Read_Move_binary_2byte_end;

                        case 2:
                            get_text = "した！";
                            goto Read_Move_binary_2byte_end;

                        case 3:
                            get_text = "を　した！";
                            goto Read_Move_binary_2byte_end;

                        case 4:
                            get_text = "こうげき！";
                            goto Read_Move_binary_2byte_end;

                        default:
                            break;
                    }
                }
                else
                {
                    get_text = "!";
                }

            Read_Move_binary_2byte_skip:
                i = i + 1;
                i++;
            }

        Read_Move_binary_2byte_end:
            i = 0;

            return get_text;
        }

        private (bool a, bool b, bool c, bool d, bool e, bool f, bool g, bool h) get_flag(int decValue)
        {
            //-------------------------------------------------------------------------------
            //値からフラグを確認
            //-------------------------------------------------------------------------------

            bool flag1 = (decValue & (1 << 0)) != 0;
            bool flag2 = (decValue & (1 << 1)) != 0;
            bool flag3 = (decValue & (1 << 2)) != 0;
            bool flag4 = (decValue & (1 << 3)) != 0;
            bool flag5 = (decValue & (1 << 4)) != 0;
            bool flag6 = (decValue & (1 << 5)) != 0;
            bool flag7 = (decValue & (1 << 6)) != 0;
            bool flag8 = (decValue & (1 << 7)) != 0;

            //名前付きタプルで纏めて変数に格納する

            return (flag1, flag2, flag3, flag4, flag5, flag6, flag7, flag8);

        }
        private void toolStripMenuItem_Export_CSV_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show(
                "表示中の検索リストをExcel形式で保存しますか？",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK)
            {

                try
                {
                    ExportCSV();
                }
                catch
                {
                    //Errorが発生した場合はメッセージボックスを表示して終了
                    MessageBox.Show("保存に失敗しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        private void ExportCSV()
        {
            //-------------------------------------------------------------------------------
            //データをExcelで出力する
            //-------------------------------------------------------------------------------

            //非営利証明を入れないと動いてくれない
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string inifolder = toolStripStatusLabel_Useinidata.Text;
            string Info_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Base_data.ini";
            StringBuilder sb = new StringBuilder(2048); //テキストも呼び出すため
            int res;

            try
            {
                // データグリッドビューのデータを取得
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    dt.Columns.Add(col.HeaderText);
                }
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataRow dRow = dt.NewRow();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        dRow[cell.ColumnIndex] = cell.Value;
                    }
                    dt.Rows.Add(dRow);
                }

                //出力ファイル名を作成する                
                string GetFileName = Path.GetFileName(label_path.Text);
                string FileName = AppDomain.CurrentDomain.BaseDirectory + GetFileName + "_技情報リスト.xlsx";
                switch (toolStripStatusLabel_mode.Text)
                {
                    case "move":
                        // Excelファイルを作成し、データを書き込む
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(GetFileName);
                            worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                            int rowCount = worksheet.Dimension.Rows;
                            for (int row = 3; row <= rowCount; row++) // 2から始めてヘッダー行をスキップ
                            {
                                string info_data = Convert.ToString(dataGridView1.Rows[row - 2].Cells[20].Value);
                                string[] move_data = info_data.Split(' ');

                                string info_value = move_data[2].ToString();
                                res = GetPrivateProfileString("Type", info_value, "未定義", sb, (uint)sb.Capacity, Info_iniFileName);
                                worksheet.Cells[row, 7].Value = sb.ToString();

                                info_value = move_data[10].ToString();
                                res = GetPrivateProfileString("Category", info_value, "未定義", sb, (uint)sb.Capacity, Info_iniFileName);
                                worksheet.Cells[row, 8].Value = sb.ToString();
                            }

                            FileInfo excelFile = new FileInfo(FileName);
                            package.SaveAs(excelFile);
                            MessageBox.Show("Excelファイルが保存されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        break;

                    case "address":
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(GetFileName);
                            worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                            FileInfo excelFile = new FileInfo(FileName);
                            package.SaveAs(excelFile);
                            MessageBox.Show("Excelファイルが保存されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        break;

                    case "BattleText":
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(GetFileName);
                            worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                            FileInfo excelFile = new FileInfo(FileName);
                            package.SaveAs(excelFile);
                            MessageBox.Show("Excelファイルが保存されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        break;

                    default:
                        break;
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show("エラー: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void button_Search_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //検索ボタン押下
            //-------------------------------------------------------------------------------
            string Select_mode = toolStripStatusLabel_mode.Text;
            dataGridView1.Rows.Clear();      //既に記載されている列を全消しする    

            if (Select_mode == "move")
            {
                Script_builder(label_path.Text); //読込Address位置からScriptデータを抽出 }
            }
            else
            {
                Address_builder(label_path.Text);//Address一覧を抽出
            }
        }
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //行をクリック時に値を記載する
            //-------------------------------------------------------------------------------
            foreach (DataGridViewCell item in dataGridView1.SelectedCells)
            {
                label_select_num.Text = item.RowIndex.ToString();
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //メインフォームに選択値を渡す
            //-------------------------------------------------------------------------------
            //https://atmarkit.itmedia.co.jp/ait/articles/0806/12/news139.html
            {
                var get_value = dataGridView1.CurrentRow.Cells[1].Value;
                f1.numericUpDown_Move_Number.Text = get_value.ToString();
            }
        }

        private void Address_builder(string path)
        {
            string inifolder = toolStripStatusLabel_Useinidata.Text;
            string Info_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Address.ini";
            string[] lines = System.IO.File.ReadAllLines(Info_iniFileName); //途中の行から開始する
            string Search_info = textBox_Search.Text;             　　　　　//検索用文字列の記載

            foreach (string line in lines)
            {
                try
                {
                    string x = line.Split('=')[1];
                    StringBuilder sb = new StringBuilder(1024);
                    int res = GetPrivateProfileString("Address", line.Split('=')[0], "00", sb, (uint)sb.Capacity, Info_iniFileName); //専用のテキストiniからデータを呼び出す
                    string Search_contents = line.Split('=')[0] + sb.ToString();

                    //検索文字列が含まれる場合はtryeを返す
                    bool Search_Flg = Search_contents.Contains(Search_info);
                    //名前から検索を行い、条件に合うものを追加する
                    if ((Search_Flg == true) || (Search_info == ""))
                    {
                        dataGridView1.Rows.Add(line.Split('=')[0], sb.ToString(), Search_contents);
                    }
                }
                catch
                { }
            }
            dataGridView1.Columns[0].HeaderText = "アドレス";
            dataGridView1.Columns[1].HeaderText = "詳細情報";
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[7].Visible = false;
            dataGridView1.Columns[8].Visible = false;
            dataGridView1.Columns[9].Visible = false;
            dataGridView1.Columns[10].Visible = false;
            dataGridView1.Columns[11].Visible = false;
            dataGridView1.Columns[12].Visible = false;
            dataGridView1.Columns[13].Visible = false;
            dataGridView1.Columns[14].Visible = false;
            dataGridView1.Columns[15].Visible = false;
            dataGridView1.Columns[16].Visible = false;
            dataGridView1.Columns[17].Visible = false;
            dataGridView1.Columns[18].Visible = false;
            dataGridView1.Columns[19].Visible = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //行の高さの自動調整
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        private void Text_builder()
        {
            //-------------------------------------------------------------------------------
            //定型文IDからテーブル取得と文字列取得
            //-------------------------------------------------------------------------------
            string path = label_path.Text;
            string LoadSection = "DataBase";

            string inifolder = toolStripStatusLabel_Useinidata.Text;
            string base_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Config.ini";
            string text_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";

            //定型文最大数を読込
            int Max_BattleText_Number = 0;
            string Loadkey = "Max_BattleText_Number";
            Max_BattleText_Number = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, base_iniFileName);

            for (int text_num = 12; text_num <= Max_BattleText_Number; text_num++) // 13から始めてヘッダー行をスキップ
            {
                var Apply_data = "";
                string LoadText = "";

                try
                {
                    //info_dataの値を数値に変換する
                    int decValue = text_num;

                    //定型文Addressの開始位置を読み込み
                    int BattleText_Table = 0;
                    int minus_pointa = 0;
                    if (decValue < 385) { Loadkey = "BattleText_Table1"; minus_pointa = 12; } else { Loadkey = "BattleText_Table2"; minus_pointa = 385; }
                    BattleText_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, base_iniFileName);

                    //開始IDが12からなため減算する
                    decValue = decValue - minus_pointa;

                    //info_dataから読込む定型文オフセットを指定して読込先として組みなおす
                    BattleText_Table = BattleText_Table + (4 * decValue);
                    LoadText = Read_binary_data(path, BattleText_Table + 3, 1) + Read_binary_data(path, BattleText_Table + 2, 1) +
                                Read_binary_data(path, BattleText_Table + 1, 1) + Read_binary_data(path, BattleText_Table, 1); //無理やりビッグエンディアンにして表示
                    decValue = Convert.ToInt32(LoadText, 16) - 134217728;

                    //------------------------------------------------------------------------------------------
                    //読み込んだ開始位置アドレスからFFが読み込まれるまでの値をテキストで返す        
                    var code_data = "";

                    var reader = new FileStream(path, FileMode.Open);
                    byte[] data = new byte[decValue + 256];    //終端が決まっていない為取り合えず256byte読み込む
                    reader.Read(data, 0, data.Length);
                    reader.Close();

                    //------------------------------------------------------------------------------------------
                    //開始Addressから+256byteまでの範囲をffがあるまで読込んで文字に変換する
                    for (int i = decValue; i < data.Length; i++)
                    {
                        code_data = string.Format("{0:x2}", data[i]);

                        //iniファイルの内容をキャストして変数に格納
                        StringBuilder sb = new StringBuilder(1024);
                        int res = GetPrivateProfileString("BM", code_data, "00", sb, (uint)sb.Capacity, text_iniFileName);

                        switch (code_data)
                        {
                            case "fa":
                                Apply_data = Apply_data + sb.ToString();
                                break;

                            case "fc":
                                code_data = string.Format("{0:x2}", data[i + 1]) + " " + string.Format("{0:x2}", data[i + 2]);
                                res = GetPrivateProfileString("MB_FC_kanji", code_data, "[未調査]", sb, (uint)sb.Capacity, text_iniFileName);//変数の一覧をiniから取得
                                Apply_data = Apply_data + sb.ToString();
                                i = i + 2; //2byte余計に進ませる
                                break;

                            case "fd":
                                code_data = string.Format("{0:x2}", data[i + 1]); //1byte先の値を取得する
                                res = GetPrivateProfileString("MB_FD_BattleText", code_data, "[未調査]", sb, (uint)sb.Capacity, text_iniFileName);//変数の一覧をiniから取得
                                Apply_data = Apply_data + sb.ToString();
                                i = i + 1; //1byte余計に進ませる
                                break;

                            case "fe":
                                Apply_data = Apply_data + sb.ToString() + " ";
                                break;

                            case "ff":
                                Apply_data = Apply_data + sb.ToString();
                                goto Next_test; //終了文字列を見つけたら表記を終了させる

                            default:
                                Apply_data = Apply_data + sb.ToString();
                                break;
                        }

                    }

                }
                catch
                {
                    //binから直接読込の際は定型文が定型文の割り出しが不可能なため分岐
                    Apply_data = "取得失敗";
                    goto Next_test; //終了文字列を見つけたら表記を終了させる
                }

            Next_test:

                string script_num = "10 " + text_num.ToString().Substring(2) + text_num.ToString().Substring(0, 2);
                dataGridView1.Rows.Add(text_num, LoadText, Apply_data);
            }

            dataGridView1.Columns[0].HeaderText = "Dex";
            dataGridView1.Columns[1].HeaderText = "Offset";
            dataGridView1.Columns[2].HeaderText = "本文";
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[7].Visible = false;
            dataGridView1.Columns[8].Visible = false;
            dataGridView1.Columns[9].Visible = false;
            dataGridView1.Columns[10].Visible = false;
            dataGridView1.Columns[11].Visible = false;
            dataGridView1.Columns[12].Visible = false;
            dataGridView1.Columns[13].Visible = false;
            dataGridView1.Columns[14].Visible = false;
            dataGridView1.Columns[15].Visible = false;
            dataGridView1.Columns[16].Visible = false;
            dataGridView1.Columns[17].Visible = false;
            dataGridView1.Columns[18].Visible = false;
            dataGridView1.Columns[19].Visible = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //行の高さの自動調整
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }
    }
}
