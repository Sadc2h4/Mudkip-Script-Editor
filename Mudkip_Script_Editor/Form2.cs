using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Diagnostics.Metrics;
using System.Reflection;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Mudkip_Script_Editor
{
    public partial class Form2 : Form
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
        public Form2(string path, int StartLen, int LoadLen, string Mode_flg, bool endFlag, bool jumpFlag, string secret_flag,
                     string First_data, string First_flg, string VBcheck, string LoadPattern, string StirlingPath, string Move_Name)
        {
            //↑メインフォームから必要情報を取得
            InitializeComponent();
            label_path.Text = path;
            toolStripStatusLabel_Script_Category.Text = Mode_flg;
            label_max_length.Text = LoadLen.ToString();
            checkBox_endFlag_check.Checked = endFlag;
            checkBox_jump_check.Checked = jumpFlag;
            textBox_Advanced_Search.Text = StartLen.ToString("X").PadLeft(8, '0').ToLower(); //オフセットを16進数にして記載
            toolStripStatusLabel_SP_flg.Text = secret_flag;                            //読み替えが必要な場合のチェック
            toolStripStatusLabel_Open_mode.Text = VBcheck;
            toolStripStatusLabel_Useinidata.Text = LoadPattern;
            label_StirlingPath.Text = StirlingPath;
            label_Move_Name.Text = Move_Name;

            Script_Viewer(path, StartLen, LoadLen, Mode_flg, secret_flag, First_data, First_flg);   //読込Address位置からScriptデータを抽出

        }

        private void Form2_Load(object sender, EventArgs e)
        {


        }

        public void Script_Viewer(string path, int StartLen, int LoadLen, string Mode_flg, string S_flag, string First_data, string First_flg)
        {
            //-------------------------------------------------------------------------------
            //読み込みScriptを組み立てて表示する
            //-------------------------------------------------------------------------------
            string inifolder = toolStripStatusLabel_Useinidata.Text;
            string Config_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Config.ini";
            string Info_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Base_data.ini";
            string code_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\code.ini";
            string Address_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Address.ini";
            string Moji_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";
            byte[] data = null;

            var all_data = "";
            var code_data = "";
            var info_data = "";
            var Apply_data = "";
            var comment_data = "";
            var secret_flag = "";
            var unique_value = "";

            int Load_addless = StartLen;
            int lb = 0;

            int row_count = -1; //開始が「0」からのため無理矢理負数で調節
            string Marker_flag = "";
            int Marker_colomn = 0;

            if (First_flg == "0")
            {
                //初回読み込み時のみはメインフォームのテキストを読込
                //-------------------------------------------------------------------------------
                //初回読み込みテキストをバイナリ配列に変換
                //-------------------------------------------------------------------------------
                StartLen = 0;
                string f2 = First_data + " ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff";
                string[] bytestr = f2.Split(' ');
                data = new byte[bytestr.Length];

                for (int i = 0; i < bytestr.Length; i++)
                {
                    data[i] = Convert.ToByte(bytestr[i], 16);
                }
            }
            else
            {
                //advancesearchで2回目移行を読む場合、読込テキストをbinaryデータ形式で分割して1byteずつ読み込ませる
                //byte[] data = new byte[reader.Length];
                var reader = new FileStream(path, FileMode.Open);
                data = new byte[StartLen + LoadLen + 16]; //最後に読み込むScriptの長さが最大数を超えた場合エラーになるのでバッファで16byte分余計に読む
                reader.Read(data, 0, data.Length);
                reader.Close();

            }

            dataGridView1.Rows.Clear();
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Yu Gothic UI", 8); //見出しのフォントの種類と大きさ
            dataGridView1.DefaultCellStyle.Font = new Font("Yu Gothic UI", 7);              //本文のフォントの種類と大きさ
            StringBuilder db = new StringBuilder(2048);
            var info_list = new List<string>();
            int res;

            //Buildモード時は設定=====================================================================
            if (toolStripStatusLabel_Open_mode.Text == "Builder")
            {
                dataGridView1.Columns[1].Visible = true;   //Viewモードのためビルドモードの列は表示

                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[2].Visible = false;

                dataGridView1.AllowUserToAddRows = true;　 //行追加をできるようにする

                button_Advanced_Search.Enabled = false;    //ビルドモードは使えなくする

                for (int x = 0; x < 256; x++)
                {
                    string Loadkey = (x.ToString("X").PadLeft(2, '0'));   //int.Parse("123"),2桁にしないとバグるので
                    res = GetPrivateProfileString(Mode_flg, Loadkey, "読み込みエラー", db, (uint)db.Capacity, code_iniFileName);

                    if (dataGridView1.Columns[1] is DataGridViewComboBoxColumn comboBoxColumn)
                    {
                        string[] comment_info = db.ToString().Split(',');

                        // ComboBox列が見つかった場合の処理
                        comboBoxColumn.Items.Add(Loadkey + ":" + comment_info[9].ToString());
                    }
                }

                goto Script_end; //ビルドモードの時はスクリプトを読まない
            }
            //=========================================================================================

            //---------------------------------------------------------------------------------------

            for (int i = StartLen; i < data.Length - 16; i++)
            {
                //コードの1byte目を格納
                code_data = string.Format("{0:x2} ", data[i]);
                Apply_data += code_data;
                lb = 0;

                //Scriptコードの内訳を読み込み
                res = GetPrivateProfileString(Mode_flg, code_data, "読み込みエラー", db, (uint)db.Capacity, code_iniFileName);
                string[] code_info = db.ToString().Split(',');

                string AddProf_Offset = "";
                int decValue = 0;

                //★隠しフラグで呼び出し関数の引数を変える分岐--------------------------------------
                switch (S_flag)
                {
                    case "image":
                        comment_data += "code,h,h,w,w,w,w,w,none,";
                        comment_data += "画像表示のパラメータ設定 \n" +
                                        "情報1：[画像ID]\n" +
                                        "情報2：[パレット]\n" +
                                        "情報3：[画像制御]\n" +
                                        "情報4：[動作]\n" +
                                        "情報5：[未使用]\n" +
                                        "情報6：[動作の一部]\n" +
                                        "情報7：[プログラム領域]";
                        code_info = comment_data.Split(',');

                        Apply_data = "";
                        i = i - 1; //先頭コードが存在しないことで1byteずれるため
                        break;

                    case "text":
                        comment_data += "code,d,-,-,-,-,-,-,none,";
                        comment_data += "定型文番号\nフラグで読込位置がズレる";
                        code_info = comment_data.Split(',');

                        Apply_data = "";
                        i = i - 1; //先頭コードが存在しないことで1byteずれるため
                        break;

                    default://読み込みなし
                        break;
                }
                //----------------------------------------------------------------------------------
                int Extend_num = 0;
                for (int j = 0; j < 8; j++) //7回繰り返す
                {
                    switch (code_info[j])
                    {
                        case "code"://先頭コード                        
                            secret_flag = code_info[8];
                            comment_data = code_info[9];
                            lb += 1;
                            break;

                        //1byte-------------------------------------------------------------------------------------------
                        case "b"://未定1byte
                            info_data = string.Format("{0:x2} ", data[i + 1]);
                            try
                            {
                                //特定のメモリアドレス指定の場合は個別分岐で内容記載
                                AddProf_Offset = string.Format("{0:x2} ", data[i - 0]) + string.Format("{0:x2} ", data[i - 1])
                                               + string.Format("{0:x2} ", data[i - 2]) + string.Format("{0:x2} ", data[i - 3]);
                                AddProf_Offset = AddProf_Offset.Replace(" ", "");

                                switch (AddProf_Offset) //特定のオフセットを読込んでいる場合，内容に反応して内容を記載させる
                                {
                                    case "02023ccb": //実行者フラグ
                                        res = GetPrivateProfileString("Target_Group_ram", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                                        info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                                        break;

                                    case "02023ccc": //対象者フラグ
                                        res = GetPrivateProfileString("Target_Group_ram", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                                        info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                                        break;

                                    case "02023d2c": //命中メッセージフラグ
                                        decValue = Convert.ToInt32(info_data.TrimEnd(), 16);
                                        (bool, bool, bool, bool, bool, bool, bool, bool) get_flag_tuple = get_flag(decValue);
                                        if (get_flag_tuple.Item1 == true) { info_data = info_data + "\n01:回避"; }
                                        if (get_flag_tuple.Item2 == true) { info_data = info_data + "\n02:抜群"; }
                                        if (get_flag_tuple.Item3 == true) { info_data = info_data + "\n04:今一つ"; }
                                        if (get_flag_tuple.Item4 == true) { info_data = info_data + "\n08:効果なし"; }
                                        if (get_flag_tuple.Item5 == true) { info_data = info_data + "\n10:一撃必殺"; }
                                        if (get_flag_tuple.Item6 == true) { info_data = info_data + "\n20:=技失敗"; }
                                        if (get_flag_tuple.Item7 == true) { info_data = info_data + "\n40:こらえた"; }
                                        if (get_flag_tuple.Item8 == true) { info_data = info_data + "\n80:ハチマキ"; }

                                        //res = GetPrivateProfileString("Compatibility_Group", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                                        info_list.Add(info_data);   //データ反映リストの末尾に値を追加
                                        break;

                                    case "02023de5": //追加効果の場合
                                        res = GetPrivateProfileString("Move_SubEffect", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                                        info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                                        break;

                                    case "02023f3e": //補助技の場合
                                        res = GetPrivateProfileString("Status_SubEffect", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                                        info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                                        break;

                                    case "02023f33": //貯めメッセージ
                                        res = GetPrivateProfileString("Multiturn_Message", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                                        info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                                        break;

                                    case "02023e7c": //天候状態
                                        res = GetPrivateProfileString("Weather_Flag", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                                        info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                                        break;

                                    default://特殊読み込みなし
                                        info_list.Add(info_data);   //データ反映リストの末尾に値を追加
                                        break;
                                }
                            }
                            catch
                            {
                                info_list.Add(info_data);   //データ反映リストの末尾に値を追加
                            }

                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]).TrimEnd() + "] ");    //スクリプト反映に現在のScript内容を追加
                            i += 1; //読み込み位置をずらしてダブらないようにする
                            lb += 1;
                            break;

                        case "t"://ターゲット1byte
                            info_data = string.Format("{0:x2} ", data[i + 1]);

                            res = GetPrivateProfileString("Target_Group_flag", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                            info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加

                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]).TrimEnd() + "] ");    //スクリプト反映に現在のScript内容を追加
                            i += 1; //読み込み位置をずらしてダブらないようにする
                            lb += 1;
                            break;

                        case "c"://条件1byte
                            info_data = string.Format("{0:x2} ", data[i + 1]);

                            res = GetPrivateProfileString("Match_Identifier", info_data.TrimEnd(), "未定義", db, (uint)db.Capacity, Info_iniFileName);
                            info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加

                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]).TrimEnd() + "] ");    //スクリプト反映に現在のScript内容を追加
                            i += 1; //読み込み位置をずらしてダブらないようにする
                            lb += 1;
                            break;

                        case "p"://タイプ1byte
                            info_data = string.Format("{0:x2} ", data[i + 1]);
                            res = GetPrivateProfileString("Type", info_data.TrimEnd(), info_data.TrimEnd(), db, (uint)db.Capacity, Info_iniFileName);
                            info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加

                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]).TrimEnd() + "] ");    //スクリプト反映に現在のScript内容を追加
                            i += 1; //読み込み位置をずらしてダブらないようにする
                            lb += 1;
                            break;

                        case "a"://特性1byte
                            info_data = string.Format("{0:x2} ", data[i + 1]);
                            res = GetPrivateProfileString("Type", info_data.TrimEnd(), info_data.TrimEnd(), db, (uint)db.Capacity, Info_iniFileName);
                            info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加

                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]).TrimEnd() + "] ");    //スクリプト反映に現在のScript内容を追加
                            i += 1; //読み込み位置をずらしてダブらないようにする
                            lb += 1;
                            break;

                        //2byte-------------------------------------------------------------------------------------------
                        case "h"://未定2byte
                            info_data = string.Format("{0:x2} ", data[i + 2]) + string.Format("{0:x2} ", data[i + 1]);
                            info_data = info_data.Replace(" ", "");

                            try
                            {
                                //特定のメモリアドレス指定の場合は個別分岐で内容記載
                                AddProf_Offset = string.Format("{0:x2} ", data[i - 0]) + string.Format("{0:x2} ", data[i - 1])
                                               + string.Format("{0:x2} ", data[i - 2]) + string.Format("{0:x2} ", data[i - 3]);
                                AddProf_Offset = AddProf_Offset.Replace(" ", "");

                                switch (AddProf_Offset) //特定のオフセットを読込んでいる場合，内容に反応して内容を記載させる
                                {
                                    case "02023ca8" or "02023caa" or "02023cac" or "02023d24" or "02023d26" or "02023d28" or "02023d2a": //実行者フラグ

                                        string Move_Name = BattleTextData_exchange(info_data, "");

                                        info_list.Add(info_data + "\n" + Move_Name);   //データ反映リストの末尾に値を追加
                                        break;

                                    default://特殊読み込みなし
                                        info_list.Add(info_data);   //データ反映リストの末尾に値を追加
                                        break;
                                }
                            }
                            catch
                            {
                                info_list.Add(info_data);   //データ反映リストの末尾に値を追加
                            }

                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]) + string.Format("{0:x2} ", data[i + 2]).TrimEnd() + "] ");
                            i += 2;
                            lb += 2;
                            break;

                        case "d"://戦闘時定型文
                            Marker_flag = "Battle_Text";
                            Marker_colomn = j + 2; 　//該当の列に色をつける列番号を記録

                            info_data = string.Format("{0:x2} ", data[i + 2]) + string.Format("{0:x2} ", data[i + 1]);
                            info_data = info_data.Replace(" ", "");

                            //定型文の読込と変数の反映を関数にして対応
                            string Battle_text = BattleTextData_exchange(info_data, "BattleText");

                            info_list.Add(info_data + "\n" + Battle_text);   //データ反映リストの末尾に値を追加

                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]) + string.Format("{0:x2} ", data[i + 2]).TrimEnd() + "] ");
                            i += 2;
                            lb += 2;
                            break;

                        case "r"://戦闘時ステータス特殊参照
                            info_data = string.Format("{0:x2} ", data[i + 1]);
                            unique_value = string.Format("{0:x2} ", data[i + 2]);

                            //分岐がヤバすぎたので個別の関数にして対応
                            info_data = Rank_Value_point(path, info_data.TrimEnd(), unique_value.TrimEnd());

                            info_list.Add(info_data);   //データ反映リストの末尾に値を追加
                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]) + string.Format("{0:x2} ", data[i + 2]).TrimEnd() + "] ");
                            i += 2; //読み込み位置をずらしてダブらないようにする
                            lb += 2;
                            break;

                        //4byte-------------------------------------------------------------------------------------------
                        case "w"://未定4byte
                            info_data = string.Format("{0:x2} ", data[i + 4]) + string.Format("{0:x2} ", data[i + 3])
                                      + string.Format("{0:x2} ", data[i + 2]) + string.Format("{0:x2} ", data[i + 1]);

                            info_data = info_data.Replace(" ", "");
                            res = GetPrivateProfileString("Address", info_data, "", db, (uint)db.Capacity, Address_iniFileName);

                            info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                            Apply_data += ("["
                                        + string.Format("{0:x2} ", data[i + 1]) + string.Format("{0:x2} ", data[i + 2])
                                        + string.Format("{0:x2} ", data[i + 3]) + string.Format("{0:x2} ", data[i + 4]).TrimEnd()
                                        + "] ");
                            i += 4;
                            lb += 4;
                            break;

                        case "o"://オフセット4byte
                            Marker_flag = "offset";
                            Marker_colomn = j + 2; 　//該当の列に色をつける列番号を記録

                            info_data = string.Format("{0:x2} ", data[i + 4]) + string.Format("{0:x2} ", data[i + 3])
                                      + string.Format("{0:x2} ", data[i + 2]) + string.Format("{0:x2} ", data[i + 1]);

                            info_data = info_data.Replace(" ", "");
                            res = GetPrivateProfileString("Address", info_data, "", db, (uint)db.Capacity, Address_iniFileName);

                            info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                            Apply_data += ("["
                                        + string.Format("{0:x2} ", data[i + 1]) + string.Format("{0:x2} ", data[i + 2])
                                        + string.Format("{0:x2} ", data[i + 3]) + string.Format("{0:x2} ", data[i + 4]).TrimEnd()
                                        + "] ");
                            i += 4;
                            lb += 4;
                            break;

                        case "m"://メモリ4byte
                            Marker_flag = "memory";

                            info_data = string.Format("{0:x2} ", data[i + 4]) + string.Format("{0:x2} ", data[i + 3])
                                      + string.Format("{0:x2} ", data[i + 2]) + string.Format("{0:x2} ", data[i + 1]);

                            info_data = info_data.Replace(" ", "");
                            res = GetPrivateProfileString("Address", info_data, "", db, (uint)db.Capacity, Address_iniFileName);

                            info_list.Add(info_data + "\n" + db.ToString());   //データ反映リストの末尾に値を追加
                            Apply_data += ("["
                                        + string.Format("{0:x2} ", data[i + 1]) + string.Format("{0:x2} ", data[i + 2])
                                        + string.Format("{0:x2} ", data[i + 3]) + string.Format("{0:x2} ", data[i + 4]).TrimEnd()
                                        + "] ");
                            i += 4;
                            lb += 4;
                            break;

                        //可変長-------------------------------------------------------------------------------------------
                        case "x"://1byte可変長,2byte
                            Extend_num = Convert.ToInt32(string.Format("{0:x2}", data[i + 1]), 16);    //特殊読込のある場所の数値を取得
                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]).TrimEnd() + "] ");
                            info_data = ""; //データへの記載文字列をリセットする
                            i += 1;

                            try
                            {
                                for (int x = 1; x <= Extend_num; x++)    //回数分繰り返し
                                {
                                    Apply_data += ("[" + string.Format("{0:x2} ", data[i + j + x]) + string.Format("{0:x2} ", data[i + j + x + 1]).TrimEnd() + "] ");
                                    info_data += "読込id" + x + "：";
                                    info_data += string.Format("{0:x2} ", data[i + j + x + 1]) + string.Format("{0:x2} ", data[i + j + x]) + "\n";
                                    info_data = info_data.Replace(" ", "");
                                    i += 2;
                                    lb += 2;
                                }
                            }
                            catch
                            {
                                Apply_data += "[終端が読み込めません]";
                                info_data = "読込に失敗しました";

                            }

                            info_list.Add(info_data.TrimEnd('\r', '\n')); //文字列ラストの場合は空白、改行文字を飛ばす
                            break;

                        case "y"://1byte可変長,1byte
                            Extend_num = Convert.ToInt32(string.Format("{0:x2}", data[i + 1]), 16);    //特殊読込のある場所の数値を取得
                            Apply_data += ("[" + string.Format("{0:x2} ", data[i + 1]).TrimEnd() + "] ");
                            info_data = ""; //データへの記載文字列をリセットする
                            i += 1;

                            try
                            {
                                for (int x = 1; x <= Extend_num; x++)    //回数分繰り返し
                                {
                                    Apply_data += ("[" + string.Format("{0:x2} ", data[i + j + x]).TrimEnd() + "] ");
                                    info_data += "読込id" + x + "：";
                                    info_data += string.Format("{0:x2} ", data[i + j + x + 1]) + "\n";
                                    info_data = info_data.Replace(" ", "");
                                    i += 1;
                                    lb += 1;
                                }
                            }
                            catch
                            {
                                Apply_data += "[終端が読み込めません]";
                                info_data = "読込に失敗しました";
                            }
                            info_list.Add(info_data.TrimEnd('\r', '\n'));　//文字列ラストの場合は空白、改行文字を飛ばす
                            break;

                        case "e"://終了コード
                            Marker_flag = "end";
                            info_list.Add("-");
                            lb += 1;
                            break;

                        default://読み込みなし
                            info_list.Add("-");
                            break;
                    }
                }

                //データグリッドビューに内容を記載（繰り返しで一行ずつ）
                dataGridView1.Rows.Add(
                    Load_addless.ToString("X").PadLeft(8, '0'),
                    "",
                    Apply_data.ToUpper(),
                    info_list[0].ToUpper(),
                    info_list[1].ToUpper(),
                    info_list[2].ToUpper(),
                    info_list[3].ToUpper(),
                    info_list[4].ToUpper(),
                    info_list[5].ToUpper(),
                    info_list[6].ToUpper(),
                    comment_data,
                    secret_flag,
                    "Editor");
                row_count = row_count + 1;

                //読込Addressを作成して次の行に記載
                Load_addless = Load_addless + lb;

                //★セルの着色分岐-----------------------------------------------------------------
                switch (Marker_flag)
                {
                    case "end"://終了コードの場合のみ列に色を付ける
                        dataGridView1.Rows[row_count].DefaultCellStyle.BackColor = Color.Pink;
                        Marker_flag = "";
                        break;

                    case "offset":
                        dataGridView1.Rows[row_count].Cells[Marker_colomn].Style.BackColor = Color.Khaki;
                        Marker_flag = "";
                        break;

                    default:
                        break;
                }
                //---------------------------------------------------------------------------------
                //★ユニーク処理分岐---------------------------------------------------------------
                switch (secret_flag)
                {
                    case "end":
                        if (checkBox_endFlag_check.Checked == true)
                        {
                            goto Script_end; //終端で終了させるチェックを入れている場合は読込終了
                        }
                        break;

                    case "jump":
                        if (checkBox_jump_check.Checked == true)
                        {
                            goto Script_end; //終端で終了させるチェックを入れている場合は読込終了
                        }
                        break;

                    case "text":
                        dataGridView1.Rows[row_count].Cells[Marker_colomn].Style.BackColor = Color.LightGreen;
                        Marker_flag = "";
                        break;

                    default:
                        break;
                }

                //---------------------------------------------------------------------------------

                //一つのScriptのデータ内容を入力完了後に一度リセットして次を読み込む
                info_list.Clear();
                all_data += Apply_data;
                Apply_data = "";
                comment_data = "";
                secret_flag = "";
            }

        Script_end:

            dataGridView1.Columns["data1"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns["data2"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns["data3"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns["data4"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns["data5"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns["data6"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns["data7"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //行の高さの自動調整
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //テキストボックスに内容を反映
            //string result = all_data.Substring(0, all_data.Length - 1); //最後に空欄が入るので消す処理
            //label_Read_Script.Text = result;

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
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (toolStripStatusLabel_Open_mode.Text == "Builder")
            {
                goto Script_end; //ビルドモードの時はスクリプトを読まない
            }

            string sStr = "";
            if (dataGridView1.CurrentCell.Value != null)
            {
                //https://yukamemo-ver3.blogspot.com/2016/09/c-datagridviewnull.html
                sStr = dataGridView1.CurrentCell.Value.ToString();
            }

            string secret_flag = dataGridView1.CurrentRow.Cells[11].Value.ToString();

            string[] address = sStr.Split('\n'); //読込情報を改行で分割し、最初の値を取得する

            var LoadText = address[0].Replace("[", "");
            int decValue = 0;

            if (LoadText.Length == 8)
            {
                if (LoadText == "")
                {
                    LoadText = "0";
                }
                string header = LoadText.Substring(0, 2);

                switch (header)
                {
                    case "08" or "09":
                        decValue = Convert.ToInt32(LoadText, 16) - 134217728;        //読込値を10進数に変換してからROM領域番地を足す 
                        LoadText = decValue.ToString("X").PadLeft(8, '0').ToUpper(); //大文字になるので小文字に変換     
                        break;

                    case "00" or "01" or "02 " or "03":
                        break;

                    default:
                        break;
                }

                textBox_Advanced_Search.Text = LoadText;
            }
            toolStripStatusLabel_SP_flg.Text = secret_flag;
        Script_end:
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
        private void button_Advanced_Search_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //追加画面表示の処理（4byteオフセット情報からform2をもう一枚開く）
            //-------------------------------------------------------------------------------
            string path = label_path.Text;
            string Mode_flg = toolStripStatusLabel_Script_Category.Text;
            int Address_length = int.Parse(label_max_length.Text);
            int decValue = Convert.ToInt32(textBox_Advanced_Search.Text, 16);
            bool endFlag = checkBox_endFlag_check.Checked;
            bool jumpFlag = checkBox_jump_check.Checked;
            string secret_flag = toolStripStatusLabel_SP_flg.Text;
            string VBcheck = "Viewer";
            string LoadPattern = toolStripStatusLabel_Useinidata.Text;
            string StirlingPath = label_StirlingPath.Text;
            string Move_Name = label_Move_Name.Text;

            string First_data = ""; //初回専用のフラグのため2回目以降専用のここでは無記載
            string First_flg = "1";  //binaryから読み込む

            if (toolStripStatusLabel_SP_flg.Text == "image")
            {
                Address_length = 24; //特殊読込の場合は24byteロードする
            }
            //追加検索ボタン押下後にフォームを別窓で開く
            Form2 form2 = new Form2(path, decValue, Address_length, Mode_flg, endFlag, jumpFlag, secret_flag, First_data, First_flg, VBcheck, LoadPattern, StirlingPath, Move_Name);
            form2.Show();

        }
        private string Rank_Value_point(string path, string info_data, string unique_value)
        {
            //-------------------------------------------------------------------------------
            //ランク補正テキスト用の文章をiniから参照する
            //-------------------------------------------------------------------------------
            string inifolder = toolStripStatusLabel_Useinidata.Text;
            string Info_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Base_data.ini";

            string get_type = "";
            string Rank_val_text = "";

            StringBuilder sb = new StringBuilder(2048); //テキストも呼び出すため
            int res;

            switch (info_data)
            {
                case "00":
                    Rank_val_text = "ランク補正 体力(未使用)\n";
                    get_type = "Rank";
                    break;
                case "01":
                    Rank_val_text = "ランク補正 攻撃\n";
                    get_type = "Rank";
                    break;
                case "02":
                    Rank_val_text = "ランク補正 防御\n";
                    get_type = "Rank";
                    break;
                case "03":
                    Rank_val_text = "ランク補正 素早さ\n";
                    get_type = "Rank";
                    break;
                case "04":
                    Rank_val_text = "ランク補正 特攻\n";
                    get_type = "Rank";
                    break;
                case "05":
                    Rank_val_text = "ランク補正 特防\n";
                    get_type = "Rank";
                    break;
                case "06":
                    Rank_val_text = "ランク補正 命中\n";
                    get_type = "Rank";
                    break;
                case "07":
                    Rank_val_text = "ランク補正 回避\n";
                    get_type = "Rank";
                    break;

                case "08":
                    Rank_val_text = "特性\n";
                    get_type = "Ability";
                    break;

                case "09":
                    Rank_val_text = "タイプ1\n";
                    get_type = "Type";
                    break;
                case "0A":
                    Rank_val_text = "タイプ2\n";
                    get_type = "Type";
                    break;

                case "0C":
                    Rank_val_text = "技1のPP\n";
                    break;
                case "0D":
                    Rank_val_text = "技2のPP\n";
                    break;
                case "0E":
                    Rank_val_text = "技3のPP\n";
                    break;
                case "0F":
                    Rank_val_text = "技4のPP\n";
                    break;
                case "10":
                    Rank_val_text = "現在HP（1byte目）\n";
                    break;
                case "11":
                    Rank_val_text = "現在HP（2byte目）\n";
                    break;
                case "13":
                    Rank_val_text = "懐き度\n";
                    break;
                case "14":
                    Rank_val_text = "最大HP（1byte目）\n";
                    break;
                case "15":
                    Rank_val_text = "最大HP（2byte目）\n";
                    break;
                case "16":
                    Rank_val_text = "持ち物（1byte目）\n";
                    break;
                case "17":
                    Rank_val_text = "持ち物（2byte目）\n";
                    break;

                default://読み込みなし
                    Rank_val_text = "不明";
                    break;
            }

            //commentデータを記載
            res = GetPrivateProfileString(get_type, unique_value, unique_value, sb, (uint)sb.Capacity, Info_iniFileName);
            Rank_val_text += sb;

            return Rank_val_text;

        }


        private string BattleTextData_exchange(string info_data, string category)
        {
            //-------------------------------------------------------------------------------
            //定型文IDからテーブル取得と文字列取得
            //-------------------------------------------------------------------------------
            string path = label_path.Text;
            string LoadSection = "DataBase";

            string inifolder = toolStripStatusLabel_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Config.ini";

            try
            {
                //info_dataの値を数値に変換する
                int decValue = Convert.ToInt32(info_data, 16);

                if (category == "BattleText")
                {

                    //定型文最大数を読込
                    int Max_BattleText_Number = 0;
                    string Loadkey = "Max_BattleText_Number";
                    Max_BattleText_Number = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

                    //定型文Addressの開始位置を読み込み
                    int BattleText_Table = 0;
                    int minus_pointa = 0;
                    if (decValue < 385) { Loadkey = "BattleText_Table1"; minus_pointa = 12; } else { Loadkey = "BattleText_Table2"; minus_pointa = 385; }
                    BattleText_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

                    //開始IDが12からなため減算する
                    decValue = decValue - minus_pointa;

                    //info_dataから読込む定型文オフセットを指定して読込先として組みなおす
                    BattleText_Table = BattleText_Table + (4 * decValue);
                    string LoadText = Read_binary_data(path, BattleText_Table + 3, 1) + Read_binary_data(path, BattleText_Table + 2, 1) +
                                Read_binary_data(path, BattleText_Table + 1, 1) + Read_binary_data(path, BattleText_Table, 1); //無理やりビッグエンディアンにして表示
                    decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                }
                else
                {
                    //技名を日本語データで取得
                    int MoveName_Table = (int)GetPrivateProfileInt("DataBase", "MoveName_Table", 0, iniFileName);
                    int MoveName_Length = (int)GetPrivateProfileInt("DataBase", "MoveName_Table_Length", 0, iniFileName);

                    decValue = MoveName_Table + (decValue * MoveName_Length);       //選択中の技名のアドレスを取得

                }

                //------------------------------------------------------------------------------------------
                //読み込んだ開始位置アドレスからFFが読み込まれるまでの値をテキストで返す        
                var code_data = "";
                var Apply_data = "";

                iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";

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
                    int res = GetPrivateProfileString("BM", code_data, "00", sb, (uint)sb.Capacity, iniFileName);

                    switch (code_data)
                    {
                        case "fa":
                            Apply_data = Apply_data + sb.ToString();
                            break;

                        case "fc":
                            code_data = string.Format("{0:x2}", data[i + 1]) + " " + string.Format("{0:x2}", data[i + 2]);
                            res = GetPrivateProfileString("MB_FC_kanji", code_data, "[未調査]", sb, (uint)sb.Capacity, iniFileName);//変数の一覧をiniから取得
                            Apply_data = Apply_data + sb.ToString();
                            i = i + 2; //2byte余計に進ませる
                            break;

                        case "fd":
                            code_data = string.Format("{0:x2}", data[i + 1]); //1byte先の値を取得する
                            res = GetPrivateProfileString("MB_FD_BattleText", code_data, "[未調査]", sb, (uint)sb.Capacity, iniFileName);//変数の一覧をiniから取得
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
            catch
            {
                //binから直接読込の際は定型文が定型文の割り出しが不可能なため分岐
                return "";
            }
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
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //セルの変更を感知して実行される処理（buildモードだけ動かす予定）
            //-------------------------------------------------------------------------------

            if (toolStripStatusLabel_Open_mode.Text == "Builder")
            {

                // ユーザーがコンボボックスのセルを編集した場合のみ反応する
                if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && e.RowIndex >= 0)
                {
                    string Mode_flg = toolStripStatusLabel_Script_Category.Text;

                    string inifolder = toolStripStatusLabel_Useinidata.Text;
                    string code_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\code.ini";
                    StringBuilder db = new StringBuilder(2048);
                    int res;

                    // 変更されたコンボボックスの値を取得
                    DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    string selectedValue = comboBoxCell.Value.ToString();
                    string[] value_data = selectedValue.Split(':');

                    //スクリプトの内容を取得
                    res = GetPrivateProfileString(Mode_flg, value_data[0], "読み込みエラー", db, (uint)db.Capacity, code_iniFileName); ;
                    string[] code_info = db.ToString().Split(',');

                    string textdata = "";

                    for (int j = 0; j < 8; j++)
                    {
                        switch (code_info[j])
                        {
                            case "code"://先頭コード                        

                                break;

                            //1byte-------------------------------------------------------------------------------------------
                            case "b"://未定1byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "任意1byte";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "t"://ターゲット1byte
                                textdata = "00:相手\n";
                                textdata += "01:自分\n";
                                textdata += "08:味方2体\n";
                                textdata += "09:相手2体\n";
                                textdata += "【アニメの0x19,1b】\n";
                                textdata += "00:全体\n";
                                textdata += "3f:相手\n";
                                textdata += "c0:味方";

                                dataGridView1.CurrentRow.Cells[j + 2].Value = textdata;
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "c"://条件1byte
                                textdata = "00:一致（＝）\n";
                                textdata += "01:不一致（≠）\n";
                                textdata += "02:超過（＞指定値xx）\n";
                                textdata += "03:未満（＜指定値xx）\n";
                                textdata += "04:1bitでも一致（and）\n";
                                textdata += "05:1bitも一致しない（not and）";

                                dataGridView1.CurrentRow.Cells[j + 2].Value = textdata;
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "p"://タイプ1byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "タイプID";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "a"://特性1byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "特性ID";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "s"://追加効果1byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "追加効果ID";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            //2byte-------------------------------------------------------------------------------------------
                            case "h"://未定2byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "任意2byte";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "r"://戦闘時ステータス特殊参照
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "戦闘時ステータス（1byte`）";
                                dataGridView1.CurrentRow.Cells[j + 3].Value = "設定値（1byte）";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            //4byte-------------------------------------------------------------------------------------------
                            case "w"://未定4byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "任意4byte";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "o"://オフセット4byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "オフセット4byte";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "m"://メモリ4byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "メモリ4byte";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            //可変長-------------------------------------------------------------------------------------------
                            case "x"://1byte可変長,2byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "直前の指定×2byteを記載";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "y"://1byte可変長,1byte
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "直前の指定×1byteを記載";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Empty;
                                break;

                            case "e"://終了コード
                                break;

                            case "-"://読み込みなし
                                dataGridView1.CurrentRow.Cells[j + 2].Value = "-";
                                dataGridView1.CurrentRow.Cells[j + 2].Style.BackColor = Color.Gainsboro;
                                break;

                            default://読み込みなし
                                break;
                        }

                    }
                }
                else
                {

                }


            }

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
                string Move_Name = label_Move_Name.Text; ;
                string GetFileName = Path.GetFileName(label_path.Text);
                string FileName = AppDomain.CurrentDomain.BaseDirectory + GetFileName + "_" + Move_Name + ".xlsx";

                // Excelファイルを作成し、データを書き込む
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(GetFileName);
                    worksheet.Cells["A4"].LoadFromDataTable(dt, true);
                    worksheet.DeleteColumn(13);
                    worksheet.DeleteColumn(12);
                    worksheet.DeleteColumn(2);

                    worksheet.Cells["A1"].Value = "技名：";
                    worksheet.Cells["B1"].Value = Move_Name;
                    worksheet.Cells["A2"].Value = "開始アドレス：";
                    worksheet.Cells["B2"].Value = textBox_Advanced_Search.Text;

                    FileInfo excelFile = new FileInfo(FileName);
                    package.SaveAs(excelFile);
                    MessageBox.Show("Excelファイルが保存されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("エラー: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //エディタ呼出ボタンを押した際の処理
            //-------------------------------------------------------------------------------
            if (e.ColumnIndex == 12)
            {
                try
                {
                    int rowIndex = e.RowIndex; // 行のインデックス
                    int columnIndex = 0; // 列のインデックス

                    object cellValue = dataGridView1.Rows[rowIndex].Cells[columnIndex].Value;


                    //ナイナリエディタを呼出
                    string OpenFile = '"' + label_path.Text + '"';

                    ProcessStartInfo pInfo = new ProcessStartInfo();

                    pInfo.UseShellExecute = true;
                    pInfo.FileName = label_StirlingPath.Text;
                    pInfo.Arguments = @OpenFile;
                    Process.Start(pInfo);
                    //https://www.sejuku.net/blog/101552

                    //Microsoft.VisualBasic.Interaction.AppActivate(label_StirlingPath.Text);
                    //キー操作
                    System.Threading.Thread.Sleep(200);
                    SendKeys.Send("%s"); // % は ALT キーを表します
                    SendKeys.Send("j");
                    System.Threading.Thread.Sleep(500);
                    SendKeys.Send(cellValue.ToString());
                    SendKeys.SendWait("{ENTER}");
                    System.Threading.Thread.Sleep(200);
                    SendKeys.Send("%s"); // % は ALT キーを表します
                    SendKeys.Send("k");


                }
                catch
                {

                }

            }




        }





    }
}
