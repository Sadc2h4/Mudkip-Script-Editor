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
using System.Reflection.PortableExecutable;
using System.Windows.Forms.Design;
using static System.Net.Mime.MediaTypeNames;
using System;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Security.Policy;
using Image = System.Drawing.Image;
using System.Xml.Linq;
using static OfficeOpenXml.ExcelErrorValue;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Mudkip_Script_Editor
{
    public partial class Form1 : Form
    {
        //-------------------------------------------------------------------------------
        //GetPrivateProfileInt関数の宣言(iniファイルの読込)
        //-------------------------------------------------------------------------------
        //iniから値を読込
        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileInt(
            string lpAppName,   //セクション名
            string lpKeyName,   //キー名
            int nDefault,       //ini読込失敗時に返す値
            string lpFileName); //iniファイルのパス

        //iniからテキストを読込
        //テキストの変換はCharSet.Autoに変換が必要
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileString(
        string lpAppName_string,
        string lpKeyName_string,
        string lpDefault_string,
        StringBuilder lpReturnedString,
        uint nSize,
        string lpFileName_string);

        //テキストをiniに書き込み
        [DllImport("KERNEL32.DLL", SetLastError = true)]
        private static extern int WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //PC設定毎に表示サイズが変化することを防ぐ
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            //iniフォルダ選択一覧を作成しておく
            Get_ini_FolderName();
            //StatusStripのプログレスバーｎ表示が縦長にならないようにするにはLayout styleを[Flow]にする

            //最後に読み込んだファイルのパスとiniを読込んでおく
            Get_LastReadFile();

        }
        private void mnu_Save_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //全部纏めてセーブする
            //-------------------------------------------------------------------------------
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("全ての項目の変更を反映しますか？",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {

                Move_effect_Save();      //技効果書き込み
                Move_animation_Save();   //技効果書き込み
                Move_info_Save();        //技アニメ書き込み
                Move_textPattern_Save(); //技表示パターン書き込み
                Chart_Tab_Save();        //タイプチャート書き込み
                MessageBox.Show("全件更新が正常に完了しました.", "System message");
            }
            else if (result == DialogResult.Cancel)
            {
                //キャンセルの場合は何もしない
            }

        }
        private void mnu_Exit_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //ツールを終了する
            //-------------------------------------------------------------------------------
            this.Close();

        }

        private void mnu_FileOpen_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //ダイアログを呼び出して対象のファイルのパスを取得する
            //-------------------------------------------------------------------------------

            //参照先のパス名
            var path = "";

            //ダイアログ呼び出し
            //ファイルダイアログを生成する
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "ファイルを開く";
            op.InitialDirectory = @"C:\";
            op.Filter = "rom files(*.rom; *.gba)| *.rom; *.gba|data files (*.txt;*.bin;*.dat)|*.txt;*.bin;*.dat";
            op.FilterIndex = 1;

            //オープンファイルダイアログを表示する
            DialogResult dialog = op.ShowDialog();

            //「開く」ボタンが選択された際の処理
            if (dialog == DialogResult.OK)
            {
                path = op.FileName;
            }
            //「キャンセル」時の処理
            else if (dialog == DialogResult.Cancel)
            {
                return;
            }

            //toolStripStatusLabel__Fullpathにパスを記載する
            toolStripStatusLabel__Fullpath.Text = path;
            OpenFile(path);        //ファイルを開く処理に進む

        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //メインフォームにファイルがドラッグ&ドロップされた際にも開く
            //-------------------------------------------------------------------------------
            e.Effect = DragDropEffects.All; // ドラッグドロップ時にカーソルの形状を変更
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {

            // ファイルが渡されていなければ、何もしない
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            // 渡されたファイルに対して処理を行う
            foreach (var path in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                OpenFile(path);//ファイルを開く処理に進む
            }
        }

        private void OpenFile(string path)
        {
            //-------------------------------------------------------------------------------
            //ファイルを呼び出して
            //-------------------------------------------------------------------------------
            mnu_Save.Enabled = true;    //全セーブメニューを選択可能にする

            string Ext = Path.GetExtension(path);
            string Base = Path.GetFileNameWithoutExtension(path);

            switch (Ext)
            {
                case ".gba" or ".rom":
                    //romを読み込む場合

                    //toolStripStatusLabel__Fullpathにパスを記載する
                    toolStripStatusLabel__Fullpath.Text = path;

                    //textBox_Move_Numberに反映する値を設定
                    button1.Enabled = true;     //スクリプト効果ビルダーボタン
                    button2.Enabled = true;     //スクリプト情報書き込みボタン
                    button3.Enabled = true;     //リロードボタン
                    button4.Enabled = true;     //効果スクリプト読み込み先表示更新ボタン
                    button5.Enabled = true;     //技情報書き込みボタン
                    button6.Enabled = true;     //技表示パターン書き込みボタン
                    button7.Enabled = true;     //スクリプト詳細ウィンドウ表示ボタン
                    button11.Enabled = true;    //技一覧リスト表示ボタン
                    button12.Enabled = true;    //技エフェクト書き込みボタン
                    button13.Enabled = true;    //アニメスクリプト読み込み先表示更新ボタン
                    button14.Enabled = true;    //技エフェクト詳細ウィンドウ表示ボタン
                    button15.Enabled = true;    //タイプ相性チャートウィンドウ表示ボタン
                    button16.Enabled = true;    //Otherタブ書き込みボタン
                    button17.Enabled = true;    //スクリプトアニメーションビルダーボタン
                    button18.Enabled = true;    //チャート読込先更新ボタン
                    button19.Enabled = true;    //アドレス一覧画面（Form3を再利用）を表示
                    button20.Enabled = true;    //テキスト一覧画面（Form3を再利用）を表示
                    button21.Enabled = true;    //定型文情報書き込みボタン
                    button22.Enabled = true;    //定型文 スクリプト → 日本語
                    button23.Enabled = true;    //定型文 日本語 → スクリプト
                    button_Extract_Updata.Enabled = true;

                    numericUpDown_Move_Number.Enabled = true; //技ナンバー切り替えを押せるようになる
                    numericUpDown_TypeNum.Enabled = true;     //タイプ総数button

                    string LoadSection = "DataBase";                                //Iniファイルのセクション
                    string Loadkey = "Load_MoveNumber";                              //Iniファイルのキー
                    int Load_Num = MyGetPrivateProfileInt(LoadSection, Loadkey);     //専用関数からiniファイル内のデータを取得する
                    numericUpDown_Move_Number.Text = Load_Num.ToString(); //読込内容を反映させる
                    label_Move_Number.Text = Load_Num.ToString();

                    Extract_tools_setting(); //外部ツールのURLを反映
                    Output_LastReadFile(); //最近読込んだファイルの更新を行う
                    Main_Load_fanction();   //URLの読み込みが完了したらメインの処理を呼び出す

                    label_Change_announce.Visible = false; //反映未完了テキストをリセット
                    toolStripComboBox_Useinidata.Enabled = false; //ini選択が固定されるようにする
                    mnu_Movedata_import.Enabled = true; //インポートが選択できるようにする
                    mnu_Movedata_export.Enabled = true;
                    break;

                case ".mse":

                    if (textBox_moveinfo.Text == "")
                    {
                        MessageBox.Show("ファイルの読込に失敗しました。\nromデータ読込前にmseファイルを読み込むことはできません。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    mseFile_Open(path);

                    break;

                default:
                    //bin Fileなどを読み込んだ場合の処理

                    //toolStripStatusLabel__Fullpathにパスを記載する
                    toolStripStatusLabel__Fullpath.Text = path;

                    //読込ファイルの大きさを取得
                    FileInfo file = new FileInfo(path);
                    string size = file.Length.ToString();

                    toolStripTextBox_version.Text = "Read data : " + Base;

                    //スクリプト単体を読む場合
                    //Effect
                    button2.Enabled = true;
                    numericUpDown_Effect_Address_length.Enabled = true;
                    button7.Enabled = true;

                    textBox_Effect_address.Text = "00000000";
                    numericUpDown_Effect_Address_length.Value = int.Parse(size);

                    string LoadText = Read_binary_data(path, 1, int.Parse(size));
                    label_Read_Script.Text = LoadText; //初回反映がないため直接指定
                    richTextBox_Effect_Script.Text = LoadText;

                    //animation
                    button12.Enabled = true;
                    numericUpDown_Animation_Address_length.Enabled = true;
                    button14.Enabled = true;

                    textBox_Animation_address.Text = "00000000";
                    numericUpDown_Animation_Address_length.Value = int.Parse(size);
                    label_Animation_Script.Text = LoadText;//初回反映がないため直接指定
                    richTextBox_Animation_Script.Text = LoadText;

                    //2番目のタブを選択
                    main_tag.SelectedIndex = 1;

                    //Animation,effectの情報ボックスにスクリプトパターンを記載
                    Extract_tools_setting(); //外部ツールのURLを反映
                                             //Output_LastReadFile();   //最近読込んだファイルの更新を行う
                    Apply_Script_pattern();

                    break;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //リロードボタンを押した際に再読み込みする
            //-------------------------------------------------------------------------------
            Main_Load_fanction();   //URLの読み込みが完了したらメインの処理を呼び出す
            label_Change_announce.Visible = false; //反映未完了テキストをリセット
        }


        private void Display_label_Change(string Cartridge_Data)
        {
            //https://codelikes.com/csharp-list/
            //https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/file-system/how-to-read-a-text-file-one-line-at-a-time
            string inifolder = toolStripComboBox_Useinidata.Text;

            int counter = 0;
            if (Cartridge_Data == "BPRJ")                                                       //日本語ロムの場合はテキストボックスを日本語化
            {
                string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\label_name.ini"; //テキストを読み込む
                List<string> get_label_value = new List<string>();                                  //格納用リストを作成
                foreach (string line in System.IO.File.ReadLines(iniFileName))                      //リスト変数にテキストの記載内容を１行ずつ書き込み
                {
                    get_label_value.Add(line);
                    counter++;
                }

                //Move Dataタブ
                label_Move_Number_sub.Text = get_label_value[1];   //技No.
                label_Move_Name_sub.Text = get_label_value[2];   //技名
                label_moveinfo_sub.Text = get_label_value[3];   //技情報

                label_moveID_sub.Text = get_label_value[5];   //技ID
                label_Movepower_sub.Text = get_label_value[6];   //威力
                label_PP_sub.Text = get_label_value[7];   //技PP
                label_Accuracy_sub.Text = get_label_value[8];   //命中率
                label_Prob_sub.Text = get_label_value[9];   //追加効果

                label_moveType_sub.Text = get_label_value[10];　//タイプ
                label_Move_Category.Text = get_label_value[11];  //分類
                label_Attack_range_sub.Text = get_label_value[12];  //範囲
                label_Priority_sub.Text = get_label_value[13];  //優先度

                checkBox_Moveflag_1.Text = get_label_value[15];
                checkBox_Moveflag_2.Text = get_label_value[16];
                checkBox_Moveflag_3.Text = get_label_value[17];
                checkBox_Moveflag_4.Text = get_label_value[18];
                checkBox_Moveflag_5.Text = get_label_value[19];
                checkBox_Moveflag_6.Text = get_label_value[20];
                checkBox_Moveflag_7.Text = get_label_value[21];
                checkBox_Moveflag_8.Text = get_label_value[22];

                checkBox_Abilityflg1_1.Text = get_label_value[24];
                checkBox_Abilityflg1_2.Text = get_label_value[25];
                checkBox_Abilityflg1_3.Text = get_label_value[26];
                checkBox_Abilityflg1_4.Text = get_label_value[27];
                checkBox_Abilityflg1_5.Text = get_label_value[28];
                checkBox_Abilityflg1_6.Text = get_label_value[29];
                checkBox_Abilityflg1_7.Text = get_label_value[30];
                checkBox_Abilityflg1_8.Text = get_label_value[31];

                checkBox_Abilityflg2_1.Text = get_label_value[33];
                checkBox_Abilityflg2_2.Text = get_label_value[34];
                checkBox_Abilityflg2_3.Text = get_label_value[35];
                checkBox_Abilityflg2_4.Text = get_label_value[36];
                checkBox_Abilityflg2_5.Text = get_label_value[37];
                checkBox_Abilityflg2_6.Text = get_label_value[38];
                checkBox_Abilityflg2_7.Text = get_label_value[39];
                checkBox_Abilityflg2_8.Text = get_label_value[40];

                label_Move_text_sub.Text = get_label_value[42];    //図鑑説明文
                label_Move_text_remain_sub.Text = get_label_value[43];

                //Script Infoタブ
                labell_moveID_sub2.Text = get_label_value[5];
                labell_moveID_sub3.Text = get_label_value[5];
                label_Read_address_sub.Text = get_label_value[45] + "オフセット"; //技効果スクリプトアドレス
                label_Move_Address_length_sub.Text = get_label_value[46];

                //Anime Infoタブ
                label_Effect_address_sub.Text = get_label_value[48] + "オフセット";
                label_Move_Address_length_sub2.Text = get_label_value[49];

                //Move text Patternタブ
                label_movetxt_pattern_sub.Text = get_label_value[51] + "オフセット";
                label_movetxt_mode_sub.Text = get_label_value[52];
                label_Select_Pattern_sub.Text = get_label_value[53];
                label_Movetext_sub1.Text = get_label_value[54];
                label_Movetext_sub2.Text = get_label_value[55];
                label_Movetext_sub3.Text = get_label_value[56];
                label_Movetext_sub4.Text = get_label_value[57];

                //Type Chartタブ
                label_TypeChart_Address.Text = get_label_value[59] + "オフセット";
                label_TypeChart_Mode.Text = get_label_value[60];
                label_Type_num_sub.Text = get_label_value[61];

                //Battle textタブ
                label_Max_Battle_Text_Count_sub.Text = get_label_value[63];
                label_Battle_Text_Num_sub.Text = get_label_value[64];
                label_Battle_Text_sub.Text = get_label_value[65];
                label_Battle_Text_info_sub.Text = get_label_value[66];
                label_Convert_Text_info_sub.Text = get_label_value[67];

            }

        }
        private int MyGetPrivateProfileInt(string Loadsection, string Loadkey)
        {
            //-------------------------------------------------------------------------------
            //iniファイルからデータを取得する(int)
            //-------------------------------------------------------------------------------

            // https://se-naruhodo.com/paramini/
            // iniファイルを取得する（実行ファイル＋iniファイル名で作成する）
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Config.ini";

            //変数宣言             
            int intDefault = 0;                　//値が取得できなかった場合の初期値
            string strFilePath = iniFileName;  　//iniファイルのパス
            int StartLen;                        //このメソッドの戻り値用

            //iniファイルの内容をキャストして変数に格納
            StartLen = (int)GetPrivateProfileInt(Loadsection, Loadkey, intDefault, strFilePath);

            return StartLen;
        }
        private void Main_Load_fanction()
        {
            //-------------------------------------------------------------------------------
            //iniファイルから変数に値を渡す
            //-------------------------------------------------------------------------------
            try
            {
                toolStripProgressBar1.Value = 0;//プログレスバーを記載

                //最後に読み込んだファイルのパスとiniを読込んでおく
                Get_LastReadFile();

                //変数を指定
                string path = "";
                string LoadSection = "DataBase";
                string Loadkey = "";
                string LoadText = "";
                int decValue = 0;
                string binValue = "";
                int get_flgnum = 0;
                bool get_lslnum;

                //label_Fullpathからパスを取得する
                path = toolStripStatusLabel__Fullpath.Text;

                int Load_Num = int.Parse(label_Move_Number.Text);                      //初回読み込み時にiniから読んだテキストを参照する

                int Cartridge_Data = 0;
                Loadkey = "Cartridge_Data";
                Cartridge_Data = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int Cartridge_Length = 0;
                Loadkey = "Cartridge_Length";
                Cartridge_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int Effect_Table_Length = 0;
                Loadkey = "Effect_Table_Length";
                Effect_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int Effect_Table = 0;
                Loadkey = "Effect_Table";
                Effect_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
                //技Scriptは技情報の技効果IDから計算するのでここではテーブルは求めない

                int MoveInfo_Table_Length = 0;
                Loadkey = "MoveInfo_Table_Length";
                MoveInfo_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int MoveInfo_Table = 0;
                Loadkey = "MoveInfo_Table";
                MoveInfo_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
                MoveInfo_Table = MoveInfo_Table + (Load_Num * MoveInfo_Table_Length);      //読み込みテーブル位置を該当番号のものに再計算する

                int MoveName_Table_Length = 0;
                Loadkey = "MoveName_Table_Length";
                MoveName_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int MoveName_Table = 0;
                Loadkey = "MoveName_Table";
                MoveName_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
                label_Move_Name_Address.Text = MoveName_Table.ToString();
                MoveName_Table = MoveName_Table + (Load_Num * MoveName_Table_Length);

                int Animation_Table_Length = 0;
                Loadkey = "Animation_Table_Length";
                Animation_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int Animation_Table = 0;
                Loadkey = "Animation_Table";
                Animation_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
                Animation_Table = Animation_Table + (Load_Num * Animation_Table_Length);

                int MoveText_Table_Length = 0;
                Loadkey = "MoveText_Table_Length";
                MoveText_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);
                textBox_Move_text_code.MaxLength = MoveText_Table_Length * 3;

                int MoveText_Table = 0;
                Loadkey = "MoveText_Table";
                MoveText_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
                MoveText_Table = MoveText_Table + (Load_Num * MoveText_Table_Length);

                int TextPattern_Select = 0;
                Loadkey = "TextPattern_Select";
                TextPattern_Select = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int TextPattern_Number = 0;
                Loadkey = "TextPattern_Number";
                TextPattern_Number = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int Max_BattleText_Number = 0;
                Loadkey = "Max_BattleText_Number";
                Max_BattleText_Number = MyGetPrivateProfileInt(LoadSection, Loadkey);
                numericUpDown_Battle_Text_Num.Maximum = Max_BattleText_Number;

                int BattleText_Table1 = 0;
                Loadkey = "BattleText_Table1";
                BattleText_Table1 = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int BattleText_Table2 = 0;
                Loadkey = "BattleText_Table2";
                BattleText_Table2 = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int Type_MaxNum = 0;
                Loadkey = "Max_TypeNumber";
                Type_MaxNum = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int TypeChart_Offset = 0;
                Loadkey = "TypeChart_Offset1";
                TypeChart_Offset = MyGetPrivateProfileInt(LoadSection, Loadkey);

                toolStripProgressBar1.Value = 10;//プログレスバーを記載

                //-------------------------------------------------------------------------------
                //読込データをロードして記載する【テキストボックス・ラベル】
                //-------------------------------------------------------------------------------

                //【1ページ目】
                //label_versionに反映する値を設定
                LoadText = Read_binary_data(path, Cartridge_Data, Cartridge_Length);   //読み込んだ値をテキストボックスに反映させる処理
                LoadText = binary_exchange(LoadText);                                  //バイナリ文字列をASCIIに変換
                Display_label_Change(LoadText);                                        //日本語ロムの場合はラベル書き換えを実行
                toolStripTextBox_version.Text = "Rom version : " + LoadText;           //読込内容を反映させる

                //textBox_moveinfoに反映する値を設定
                LoadText = Read_binary_data(path, MoveInfo_Table, MoveInfo_Table_Length);
                textBox_moveinfo.Text = LoadText;
                label_before_Script_MoveInfo.Text = LoadText;
                label_Updatabefore_MoveInfo.Text = MoveInfo_Table.ToString("X").PadLeft(8, '0'); //変更前アドレスを16進数に変換して登録

                //技IDの処理はbutton1押下直後の処理に移動してます

                //textBox_label_Move_Nameに反映する値を設定
                textBox_Move_Name.Enabled = true;
                LoadText = Read_binary_data(path, MoveName_Table, MoveName_Table_Length);
                label_Move_Name.Text = LoadText;
                LoadText = FontData_exchange(LoadText);
                textBox_Move_Name.Text = LoadText;
                label_before_Script_MoveName.Text = LoadText; //変更前名前文字列を記載
                //技名の読み込み文字数をラベルに記載
                label_Name_length.Text = MoveName_Table_Length.ToString();

                //textBox_Movepowerに反映する値を設定
                textBox_Movepower.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table + 1, 1);
                label_data_02.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                textBox_Movepower.Text = decValue.ToString();

                //textBox_Accuracyに反映する値を設定
                textBox_Accuracy.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table + 3, 1);
                label_data_04.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                textBox_Accuracy.Text = decValue.ToString();

                //textBox_PPに反映する値を設定
                textBox_PP.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table + 4, 1);
                label_data_05.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                textBox_PP.Text = decValue.ToString();

                //textBox_Probに反映する値を設定
                textBox_Prob.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table + 5, 1);
                label_data_06.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                textBox_Prob.Text = decValue.ToString();

                //説明文の読み込み文字数をラベルに記載
                label_Move_text_length.Text = MoveText_Table_Length.ToString();
                //textBox_Move_textに反映する値を設定
                textBox_Move_text.Enabled = true;
                textBox_Move_text_code.Enabled = true;
                LoadText = Read_binary_data(path, MoveText_Table, MoveText_Table_Length);
                LoadText = FontData_exchange(LoadText);
                textBox_Move_text.Text = LoadText;
                label_before_Script_MoveText.Text = LoadText; //更新前説明文を設定

                //textBox_label_moveIDに反映する値を設定
                textBox_label_moveID.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table, 1);                     //iniから　1byte目を取得
                label_data_01.Text = LoadText;                                           //数値保持ラベルに値を記載  
                decValue = Convert.ToInt32(LoadText, 16);                                //16進数を10進数に変換して表示したいので
                textBox_label_moveID.Text = decValue.ToString();                         //d2と記載することで標準の数値書式指定ができる 
                labell_moveID2.Text = decValue.ToString();                               //タブ2ページ目のID表示をみるための反映
                labell_moveID3.Text = decValue.ToString();                               //タブ3ページ目のID表示をみるための反映

                string Loadini = toolStripComboBox_Useinidata.Text;
                string iniName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + Loadini + "\\Effect_ID_info.ini";
                StringBuilder sb = new StringBuilder(1024);
                int IDinfo = GetPrivateProfileString("Effect", LoadText, "読込エラー", sb, (uint)sb.Capacity, iniName); //専用のテキストiniからデータを呼び出す
                string Load_ID = sb.ToString().ToUpper(); //文字列の英語文字を大文字に変更
                label_ID_info_text.Text = Load_ID;

                //【2ページ目】
                //textBox_Read_addressに反映する値を設定
                textBox_Effect_address.Enabled = true;
                Effect_Table = Effect_Table + (decValue * Effect_Table_Length);             //効果IDで掛け算する必要が有るため、ここでtable値を計算
                LoadText = Read_binary_data(path, Effect_Table, Effect_Table_Length);
                label_Read_address.Text = LoadText;
                LoadText = Read_binary_data(path, Effect_Table + 3, 1) + Read_binary_data(path, Effect_Table + 2, 1) +
                           Read_binary_data(path, Effect_Table + 1, 1) + Read_binary_data(path, Effect_Table, 1); //無理やりビッグエンディアンにして表示

                decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                LoadText = decValue.ToString("X").PadLeft(8, '0');
                textBox_Effect_address.Text = LoadText;
                label_Updatabefore_Effect.Text = LoadText; //変更前アドレスを格納

                //richtextBox_Read_Scriptに反映する値の設定
                numericUpDown_Effect_Address_length.Enabled = true;
                decValue = Convert.ToInt32(LoadText, 16);                                               //読込位置がテキストデータのため、16進数から10進数に再計算
                int Address_length = int.Parse(numericUpDown_Effect_Address_length.Value.ToString());   //読み込みbyte数値を取得（デフォルトでは100）
                LoadText = Read_binary_data(path, decValue, Address_length);
                label_Read_Script.Text = LoadText;
                richTextBox_Effect_Script.Text = LoadText;                                              //記載用Scriptデータをラベルに記載
                label_before_Script_Effect.Text = LoadText;                                             //変更前のスクリプト内容を格納


                //【3ページ目】
                //textBox_Effect_addressに反映する値を設定
                textBox_Animation_address.Enabled = true;
                LoadText = Read_binary_data(path, Animation_Table, Animation_Table_Length);             //アニメはID毎のためtable作成は↑で行う
                label_Animation_address.Text = LoadText;
                LoadText = Read_binary_data(path, Animation_Table + 3, 1) + Read_binary_data(path, Animation_Table + 2, 1) +
                           Read_binary_data(path, Animation_Table + 1, 1) + Read_binary_data(path, Animation_Table, 1); //無理やりビッグエンディアンにして表示

                decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                LoadText = decValue.ToString("X").PadLeft(8, '0');
                textBox_Animation_address.Text = LoadText;
                label_Updatabefore_Animation.Text = LoadText; //変更前アドレスを格納

                //richtextBox_Read_Scriptに反映する値の設定
                numericUpDown_Animation_Address_length.Enabled = true;
                decValue = Convert.ToInt32(LoadText, 16);                                               //読込位置がテキストデータのため、16進数から10進数に再計算
                int Effect_length = int.Parse(numericUpDown_Animation_Address_length.Value.ToString()); //読み込みbyte数値を取得（デフォルトでは100）
                LoadText = Read_binary_data(path, decValue, Effect_length);
                label_Animation_Script.Text = LoadText;
                richTextBox_Animation_Script.Text = LoadText;                                           //記載用Scriptデータをラベルに記載
                label_before_Script_Animation.Text = LoadText;                                          //変更前のスクリプト内容を格納


                //【4ページ目】
                //textBox__movetxt_patternに反映する値を設定
                LoadText = Read_binary_data(path, TextPattern_Select, 4);
                LoadText = Read_binary_data(path, TextPattern_Select + 3, 1) + Read_binary_data(path, TextPattern_Select + 2, 1) +
                           Read_binary_data(path, TextPattern_Select + 1, 1) + Read_binary_data(path, TextPattern_Select, 1); //無理やりビッグエンディアンにして表示

                decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                LoadText = decValue.ToString("X").PadLeft(8, '0');
                textBox__movetxt_pattern.Text = LoadText;
                label_Updatabefore_Pattern.Text = LoadText; //変更前アドレスを格納

                Read_Move_binary_2byte(path, decValue); //リストボックスに値を記載させる

                //【5ページ目】
                //textBox_TypeChart_Addressに反映する値を設定
                LoadText = Read_binary_data(path, TypeChart_Offset, 4);
                LoadText = Read_binary_data(path, TypeChart_Offset + 3, 1) + Read_binary_data(path, TypeChart_Offset + 2, 1) +
                           Read_binary_data(path, TypeChart_Offset + 1, 1) + Read_binary_data(path, TypeChart_Offset, 1); //無理やりビッグエンディアンにして表示

                decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                LoadText = decValue.ToString("X").PadLeft(8, '0');
                textBox_TypeChart_Address.Text = LoadText;
                label_Updatabefore_Chart.Text = LoadText;      //変更前アドレスを格納

                //richTextBox_type_Chartに反映する値を設定
                LoadText = Read_binary_toFF(decValue);         //直前のAddress値からffまでのbinaryデータを取得する
                label_before_Script_TypeChart.Text = LoadText; //変更前のスクリプト内容を格納
                richTextBox_type_Chart.Text = LoadText;

                //numericUpDown_TypeNumに反映する値を設定
                numericUpDown_TypeNum.Text = Type_MaxNum.ToString();

                //【8ページ目】
                numericUpDown_Battle_Text_Num.Enabled = true;
                textBox_Battle_Text.Enabled = true;

                //予め値を加算して表示させる
                numericUpDown_Battle_Text_Num.Value += 1;

                //Max_Battle_Text_Countに反映する値の設定
                label_Max_Battle_Text_Count.Text = Max_BattleText_Number.ToString();

                label_Battle_Text_Hex_address1.Text = BattleText_Table1.ToString();
                label_Battle_Text_Hex_address2.Text = BattleText_Table2.ToString();


                toolStripProgressBar1.Value = 30;//プログレスバーを記載

                //-------------------------------------------------------------------------------
                //読込データをロードして記載する【コンボボックス】
                //-------------------------------------------------------------------------------
                string inifolder = toolStripComboBox_Useinidata.Text;
                string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Base_data.ini";
                StringBuilder db = new StringBuilder(2048);
                int res;

                //comboBox_moveTypeに反映する値を設定
                comboBox_moveType.Enabled = true;                   //初期値の時は操作不可なので操作可能にする
                comboBox_moveType.Items.Clear();
                for (int i = 0; i < Type_MaxNum; i++)
                {
                    string x = (i.ToString("X").PadLeft(2, '0'));   //int.Parse("123"),2桁にしないとバグるので
                    res = GetPrivateProfileString("type", x, "読み込みエラー", db, (uint)db.Capacity, iniFileName);
                    comboBox_moveType.Items.Add(db); //comboBox_moveTypeに値を追加
                }
                LoadText = Read_binary_data(path, MoveInfo_Table + 2, 1);
                label_data_03.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);                      //読み込んだ1byteを10進数に変換
                comboBox_moveType.SelectedIndex = decValue;                        //comboBox_moveTypeに反映

                //comboBox_Attack_rangeに反映する値を設定
                comboBox_Attack_range.Enabled = true;
                Dictionary<string, string> Attack_range_list = new Dictionary<string, string>()
                {
                    { "通常" , "00" },
                    { "対象不定" , "01" },
                    { "× 未使用" , "02" },
                    { "複数ターン技" , "04" },
                    { "相手2体" , "08" },
                    { "自分 or 場全体" , "10" },
                    { "自分以外" , "20" },
                    { "設置技" , "40" },
                };
                LoadText = Read_binary_data(path, MoveInfo_Table + 6, 1);
                label_data_07.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                binValue = Convert.ToString(decValue, 2).PadLeft(8, '0');               //16進数を一度10進数に直してから8桁の2進数に変換

                get_flgnum = bin_Getflg(binValue);

                comboBox_Attack_range.DisplayMember = "Key";
                comboBox_Attack_range.ValueMember = "Value";
                comboBox_Attack_range.DataSource = Attack_range_list.ToList();
                comboBox_Attack_range.SelectedIndex = get_flgnum;

                //comboBox_Priorityに反映する値を設定
                comboBox_Priority.Enabled = true;
                Dictionary<string, string> Priority_list = new Dictionary<string, string>()
                {
                    { "優先度 ±0" , "00" },
                    { "優先度 +1"  , "01" },
                    { "優先度 +2"  , "02" },
                    { "優先度 +3"  , "03" },
                    { "優先度 +4"  , "04" },
                    { "優先度 +5"  , "05" },
                    { "優先度 +6"  , "06" },
                    { "優先度 -1"  , "FF" },
                    { "優先度 -2"  , "FE" },
                    { "優先度 -3"  , "FD" },
                    { "優先度 -4"  , "FC" },
                    { "優先度 -5"  , "FB" },
                    { "優先度 -6"  , "FA" },
                };
                LoadText = Read_binary_data(path, MoveInfo_Table + 7, 1);
                label_data_08.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);

                switch (decValue)
                {
                    case 0:
                        get_flgnum = 0;
                        break;

                    case 1:
                        get_flgnum = 1;
                        break;

                    case 2:
                        get_flgnum = 2;
                        break;

                    case 3:
                        get_flgnum = 3;
                        break;

                    case 4:
                        get_flgnum = 4;
                        break;

                    case 5:
                        get_flgnum = 5;
                        break;

                    case 6:
                        get_flgnum = 6;
                        break;

                    case 255:
                        get_flgnum = 7;
                        break;

                    case 254:
                        get_flgnum = 8;
                        break;

                    case 253:
                        get_flgnum = 9;
                        break;

                    case 252:
                        get_flgnum = 10;
                        break;

                    case 251:
                        get_flgnum = 11;
                        break;

                    default:
                        get_flgnum = 12;
                        break;
                }   //ごり押しでフラグ設定

                comboBox_Priority.DisplayMember = "Key";
                comboBox_Priority.ValueMember = "Value";
                comboBox_Priority.DataSource = Priority_list.ToList();
                comboBox_Priority.SelectedIndex = get_flgnum;

                //comboBox_Move_Categoryに反映する値を設定
                comboBox_Move_Category.Enabled = true;
                Dictionary<string, string> Category_list = new Dictionary<string, string>()
                {
                    { "物理⇒防御" , "00" },
                    { "物理⇒特防"  , "01" },
                    { "特殊⇒防御"  , "02" },
                    { "特殊⇒特防"  , "03" },
                    { "変化技"  , "04" },
                    { "未使用1"  , "05" },
                    { "未使用2"  , "06" },
                    { "未使用3"  , "07" },
                    { "のろい"  , "08" },
                };
                LoadText = Read_binary_data(path, MoveInfo_Table + 10, 1);
                label_data_11.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                get_flgnum = decValue;

                comboBox_Move_Category.DisplayMember = "Key";
                comboBox_Move_Category.ValueMember = "Value";
                comboBox_Move_Category.DataSource = Category_list.ToList();
                comboBox_Move_Category.SelectedIndex = get_flgnum;
                toolStripProgressBar1.Value = 50;//プログレスバーを記載    

                //textBox_movetxt_lengthに反映する値を設定
                comboBox_movetxt_mode.Enabled = true;
                Dictionary<string, string> Move_text_list = new Dictionary<string, string>()
                {
                    { "拡張なし" , "B1" },
                    { "拡張あり" , "FF" },
                };
                LoadText = Read_binary_data(path, TextPattern_Number, 1);
                decValue = Convert.ToInt32(LoadText, 16);
                switch (decValue)
                {
                    case 177:
                        get_flgnum = 0;
                        break;

                    default:
                        textBox__movetxt_pattern.Enabled = true;    //拡張がオンになってない場所で操作させないようにするための分岐
                        comboBox_Select_Pattern.Enabled = true;
                        button8.Enabled = true;                     //表示パターン表記反映ボタン
                        button9.Enabled = true;                     //技表示パターン追加ボタン
                        button10.Enabled = true;                    //技表示パターン削除ボタン
                        get_flgnum = 1;
                        break;
                }
                comboBox_movetxt_mode.DisplayMember = "Key";
                comboBox_movetxt_mode.ValueMember = "Value";
                comboBox_movetxt_mode.DataSource = Move_text_list.ToList();
                comboBox_movetxt_mode.SelectedIndex = get_flgnum;

                //comboBox_Select_Patternに反映する値を設定                
                Dictionary<string, string> Text_pattern_list = new Dictionary<string, string>()
                {
                    { "パターン1" , "1" },
                    { "パターン2" , "2" },
                    { "パターン3" , "3" },
                    { "パターン4" , "4" },
                    { "該当なし" , "5" },
                };
                get_flgnum = int.Parse(label_Select_Pattern_result.Text);
                comboBox_Select_Pattern.DisplayMember = "Key";
                comboBox_Select_Pattern.ValueMember = "Value";
                comboBox_Select_Pattern.DataSource = Text_pattern_list.ToList();
                comboBox_Select_Pattern.SelectedIndex = get_flgnum - 1;

                //comboBox_SelectBGに反映する値を設定
                comboBox_SelectBG.Enabled = true;
                Dictionary<string, string> Select_BG_list = new Dictionary<string, string>()
                {
                    { "未指定" , "00" },
                    { "シングル" , "01" },
                    { "ダブル" , "02" },
                    };
                comboBox_SelectBG.DisplayMember = "Key";
                comboBox_SelectBG.ValueMember = "Value";
                comboBox_SelectBG.DataSource = Select_BG_list.ToList();
                comboBox_SelectBG.SelectedIndex = 0;

                //comboBox_Type_Chartに反映する値を設定
                comboBox_Type_Chart.Enabled = true;
                Dictionary<string, string> Type_Chart_list = new Dictionary<string, string>()
                    {
                        { "拡張なし" , "0820BF24" },
                        { "拡張あり" , "FFFFFFFF" },
                    };
                LoadText = textBox_TypeChart_Address.Text;

                switch (LoadText)
                {
                    case "0020BF24":
                        get_flgnum = 0;
                        break;

                    default:
                        textBox_TypeChart_Address.Enabled = true;    //拡張がオンになってない場所で操作させないようにするための分岐
                        get_flgnum = 1;
                        break;
                }
                comboBox_Type_Chart.DisplayMember = "Key";
                comboBox_Type_Chart.ValueMember = "Value";
                comboBox_Type_Chart.DataSource = Type_Chart_list.ToList();
                comboBox_Type_Chart.SelectedIndex = get_flgnum;

                toolStripProgressBar1.Value = 70;//プログレスバーを記載

                //-------------------------------------------------------------------------------
                //読込データをロードして記載する【チェックボックス】
                //-------------------------------------------------------------------------------
                //Move_flagに反映する値を設定
                LoadText = Read_binary_data(path, MoveInfo_Table + 8, 1);
                label_data_09.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);

                (bool, bool, bool, bool, bool, bool, bool, bool) get_flag_tuple = get_flag(decValue);

                foreach (Control item in groupBox_Move_flag.Controls)
                {
                    if (item.GetType().Equals(typeof(System.Windows.Forms.CheckBox)))   //纏めて全てのチェックボックスの操作を実行する
                    {
                        ((System.Windows.Forms.CheckBox)item).Enabled = true;
                    }
                }
                checkBox_Moveflag_1.Checked = get_flag_tuple.Item1;
                checkBox_Moveflag_2.Checked = get_flag_tuple.Item2;
                checkBox_Moveflag_3.Checked = get_flag_tuple.Item3;
                checkBox_Moveflag_4.Checked = get_flag_tuple.Item4;
                checkBox_Moveflag_5.Checked = get_flag_tuple.Item5;
                checkBox_Moveflag_6.Checked = get_flag_tuple.Item6;
                checkBox_Moveflag_7.Checked = get_flag_tuple.Item7;
                checkBox_Moveflag_8.Checked = get_flag_tuple.Item8;

                //Ability_flag_1に反映する値を設定
                LoadText = Read_binary_data(path, MoveInfo_Table + 9, 1);
                label_data_10.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);

                get_flag_tuple = get_flag(decValue);

                foreach (Control item in groupBox_Ability_flag1.Controls)
                {
                    if (item.GetType().Equals(typeof(System.Windows.Forms.CheckBox)))   //纏めて全てのチェックボックスの操作を実行する
                    {
                        ((System.Windows.Forms.CheckBox)item).Enabled = true;
                    }
                }
                checkBox_Abilityflg1_1.Checked = get_flag_tuple.Item1;
                checkBox_Abilityflg1_2.Checked = get_flag_tuple.Item2;
                checkBox_Abilityflg1_3.Checked = get_flag_tuple.Item3;
                checkBox_Abilityflg1_4.Checked = get_flag_tuple.Item4;
                checkBox_Abilityflg1_5.Checked = get_flag_tuple.Item5;
                checkBox_Abilityflg1_6.Checked = get_flag_tuple.Item6;
                checkBox_Abilityflg1_7.Checked = get_flag_tuple.Item7;
                checkBox_Abilityflg1_8.Checked = get_flag_tuple.Item8;

                //Ability_flag_2に反映する値を設定
                LoadText = Read_binary_data(path, MoveInfo_Table + 11, 1);
                label_data_12.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);

                get_flag_tuple = get_flag(decValue);

                foreach (Control item in groupBox_Ability_flag2.Controls)
                {
                    if (item.GetType().Equals(typeof(System.Windows.Forms.CheckBox)))   //纏めて全てのチェックボックスの操作を実行する
                    {
                        ((System.Windows.Forms.CheckBox)item).Enabled = true;
                    }
                }
                checkBox_Abilityflg2_1.Checked = get_flag_tuple.Item1;
                checkBox_Abilityflg2_2.Checked = get_flag_tuple.Item2;
                checkBox_Abilityflg2_3.Checked = get_flag_tuple.Item3;
                checkBox_Abilityflg2_4.Checked = get_flag_tuple.Item4;
                checkBox_Abilityflg2_5.Checked = get_flag_tuple.Item5;
                checkBox_Abilityflg2_6.Checked = get_flag_tuple.Item6;
                checkBox_Abilityflg2_7.Checked = get_flag_tuple.Item7;
                checkBox_Abilityflg2_8.Checked = get_flag_tuple.Item8;

                toolStripProgressBar1.Value = 90;//プログレスバーを記載

                Apply_Script_pattern(); //Animation,effectの情報ボックスにスクリプトパターンを記載

                //-------------------------------------------------------------------------------
                //ログ出力用一覧の表示【データグリッドビュー】
                //-------------------------------------------------------------------------------

                dataGridView_Change_log.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //行の高さの自動調整
                dataGridView_Change_log.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                //-------------------------------------------------------------------------------
                //座標取得画像の大きさを指定【ピクチャーボックス】
                //-------------------------------------------------------------------------------
                //pictureBox_Coordinate.Size = new Size(360, 240);



                //-------------------------------------------------------------------------------
                //読み込みデータの情報をメインラベルに記載
                //-------------------------------------------------------------------------------

                //MoveInfo_build();
                toolStripProgressBar1.Value = 100;//プログレスバーを記載
            }
            catch
            {
                //Errorが発生した場合はメッセージボックスを表示して終了
                MessageBox.Show("入力された情報に問題があるようです．\nアプリを再起動してください．\n編集途中のデータがある場合はツールバーの\n[MSE_File_export]から内容を保存で可能です．", "参照値エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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
        private string Read_binary_toFF(int StartLen)
        {
            //-------------------------------------------------------------------------------
            //読み込んだ開始位置アドレスからFFが読み込まれるまでの値をテキストで返す
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;
            var code_data = "";
            var Apply_data = "";

            var reader = new FileStream(path, FileMode.Open);
            byte[] data = new byte[StartLen + 1000];    //終端が決まっていない為取り合えず1000byte読み込む
            reader.Read(data, 0, data.Length);
            reader.Close();

            for (int i = StartLen; i < data.Length; i++)
            {
                code_data = string.Format("{0:x2} ", data[i]);
                Apply_data += code_data;

                if (code_data == "ff ")
                {
                    goto Read_End;
                }
            }

        Read_End:
            string result = Apply_data.TrimEnd(); //最後に空欄が入るので消す処理
            return result;
        }
        private string binary_exchange(string LoadText)
        {
            //-------------------------------------------------------------------------------
            //読込バイナリデータを通常文字に変換
            //-------------------------------------------------------------------------------
            string input = LoadText;
            string[] bytestr = input.Split(' ');
            byte[] data = new byte[bytestr.Length];

            for (int i = 0; i < bytestr.Length; i++)
            {
                data[i] = Convert.ToByte(bytestr[i], 16);
            }

            //string text = Encoding.GetEncoding("shift_jis").GetString(data);
            string text = System.Text.Encoding.ASCII.GetString(data);

            return text;

        }
        private string FontData_exchange(string LoadText)
        {
            //-------------------------------------------------------------------------------
            //読み込みbyteデータを日本語に変換
            //-------------------------------------------------------------------------------
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";
            string[] data = LoadText.Split(' ');
            //byte[] data = new byte[bytestr.Length];
            string get_nametext = "";
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
                    case "fa" or "FA":
                        Apply_data = Apply_data + sb.ToString();
                        break;

                    case "fc" or "FC":
                        code_data = string.Format("{0:x2}", data[i + 1]) + " " + string.Format("{0:x2}", data[i + 2]);
                        res = GetPrivateProfileString("BM_FC_kanji", code_data, "[未調査]", sb, (uint)sb.Capacity, iniFileName);//変数の一覧をiniから取得
                        Apply_data = Apply_data + sb.ToString();
                        i = i + 2; //2byte余計に進ませる
                        break;

                    case "fd" or "FD":
                        code_data = string.Format("{0:x2}", data[i + 1]); //1byte先の値を取得する
                        res = GetPrivateProfileString("MB_FD_BattleText", code_data, "[未調査]", sb, (uint)sb.Capacity, iniFileName);//変数の一覧をiniから取得
                        Apply_data = Apply_data + sb.ToString();
                        i = i + 1; //1byte余計に進ませる
                        break;

                    case "fe" or "FE":
                        Apply_data = Apply_data + sb.ToString() + "\r\n";
                        break;

                    case "ff" or "FF":
                        //テキストの終端に終了文字を記載するかどうか
                        //Apply_data = Apply_data + sb.ToString();
                        return Apply_data; //終了文字列を見つけたら表記を終了させる

                    default:
                        Apply_data = Apply_data + sb.ToString();
                        break;
                }
            }
            return Apply_data;
        }
        private string ByteData_exchange(string LoadText, int name_length)
        {
            //-------------------------------------------------------------------------------
            //読み込み日本語データをバイナリに変換
            //-------------------------------------------------------------------------------          
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";
            string bytestr = LoadText;
            string get_nametext = "";
            string end_flg = "";

            int remain_length = 0;

            //終端文字が入っていない場合は追加する
            if (LoadText.Contains("Ω")) { end_flg = "□"; } else { end_flg = "Ω"; }

            //文字数が足りない場合は後ろに空白を追加する
            bytestr = bytestr + end_flg;
            end_flg = "";

            for (int i = 2; i <= name_length; i++)
            {
                end_flg += "□";
            }
            bytestr = bytestr + end_flg;

            for (int i = 0; i < name_length; i++)
            {
                //iniファイルの内容をキャストして変数に格納
                StringBuilder sb = new StringBuilder(2048);

                string str_font = bytestr.Substring(i, 1);
                if (str_font == "$")
                {
                    str_font = bytestr.Substring(i, 3);
                    i += 2; //余計に2byte進めておく
                }


                int res = GetPrivateProfileString("MB", str_font, "[?]", sb, (uint)sb.Capacity, iniFileName); //専用のテキストiniからデータを呼び出す

                //残り文字数を計算するsection
                switch (sb.ToString())
                {
                    case "ff" or "FF":
                        //終端を見つけたら残り文字数を計算して表記
                        label_Move_text_remain.Text = (name_length - remain_length).ToString();
                        break;

                    default:
                        if (sb.ToString().Contains("fc"))
                        {
                            remain_length = remain_length + 3;
                        }
                        else
                        {
                            remain_length = remain_length + 1;
                        }
                        break;
                }

                if ((i == 0) || (res == 0)) //行の開始もしくは改行コードが入っている場合は空白追加をスキップする
                {
                    get_nametext = get_nametext + sb.ToString();
                }
                else
                {
                    get_nametext = get_nametext + " " + sb.ToString();
                }
            }

            return get_nametext;
        }
        private void Read_Move_binary_2byte(string path, int StartLen)
        {
            //-------------------------------------------------------------------------------
            //パスに記載されたアドレスから2byteずつ読み込んで表示文字列を判定
            //-------------------------------------------------------------------------------
            var reader = new FileStream(path, FileMode.Open);
            var Apply_data = "";
            int move_num = 0;
            int Load_Address = 0;
            int Count_flg = 1;
            string LoadText = "";
            string get_num = "5";

            string All_Data = "";

            byte[] data = new byte[StartLen + 1000];
            reader.Read(data, 0, data.Length);
            reader.Close();

            listBox_Movetext_pattern1.Items.Clear();
            listBox_Movetext_pattern2.Items.Clear();
            listBox_Movetext_pattern3.Items.Clear();
            listBox_Movetext_pattern4.Items.Clear();

            int i = StartLen;
            while (Count_flg < 5)　//５回目の00 00 を読み込んだ時点で処理を終了させる
            {
                //技IDを作成
                Apply_data = string.Format("{0:x2}", data[i + 1]) + string.Format("{0:x2}", data[i]);
                move_num = Convert.ToInt32(Apply_data, 16); //10進数に変換して技IDを作成

                //技名を日本語データで取得
                int MoveName_Table = int.Parse(label_Move_Name_Address.Text);
                Load_Address = MoveName_Table + (move_num * int.Parse(label_Name_length.Text));       //選択中の技名のアドレスを取得
                LoadText = Read_binary_data(path, Load_Address, int.Parse(label_Name_length.Text));  //該当Addressから技名のデータを取得
                LoadText = FontData_exchange(LoadText);

                //現在表示している技IDとの照合用データを作る
                int Select_move_num = int.Parse(numericUpDown_Move_Number.Value.ToString());    //テキスト取得では一つ前の値になるため);

                //2Byte分を読み込んで変数に成形
                Apply_data = string.Format("{0:x2}", data[i]) + " " + string.Format("{0:x2}", data[i + 1]);

                All_Data += Apply_data + " "; //全量Scriptを格納する

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
                switch (Count_flg)
                {
                    case 1:
                        listBox_Movetext_pattern1.Items.Add(Apply_data + "," + LoadText);
                        break;

                    case 2:
                        listBox_Movetext_pattern2.Items.Add(Apply_data + "," + LoadText);
                        break;

                    case 3:
                        listBox_Movetext_pattern3.Items.Add(Apply_data + "," + LoadText);
                        break;

                    case 4:
                        listBox_Movetext_pattern4.Items.Add(Apply_data + "," + LoadText);
                        break;

                    default:

                        break;
                }
            Read_Move_binary_2byte_skip:
                if (Select_move_num == move_num)
                {
                    get_num = Count_flg.ToString();
                }
                label_Select_Pattern_result.Text = get_num;

                i = i + 1;
                i++;
            }

        Read_Move_binary_2byte_end:
            i = 0;

            label_before_Script_Pattern.Text = All_Data;
        }
        private int bin_Getflg(string binValue)
        {
            //-------------------------------------------------------------------------------
            //2進数からフラグの値を計算
            //-------------------------------------------------------------------------------
            int retrun_flg = 0;
            string getflg = "";

            for (int i = 1; i <= 7; i++)
            {
                getflg = binValue.Substring(i, 1);  //2進数の中で1がある場所でフラグを判定させる

                if (getflg == "1")
                {
                    retrun_flg = 8 - i;              //リストは逆順なので7から減産する．0の場合は読まれないので多分大丈夫
                }
            }
            return retrun_flg;
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
        private void MoveInfo_build()
        {
            //-------------------------------------------------------------------------------
            //textBox_moveinfoに値を記載する
            //-------------------------------------------------------------------------------
            string Data_01 = label_data_01.Text;
            string Data_02 = label_data_02.Text;
            string Data_03 = label_data_03.Text;
            string Data_04 = label_data_04.Text;
            string Data_05 = label_data_05.Text;
            string Data_06 = label_data_06.Text;
            string Data_07 = label_data_07.Text;
            string Data_08 = label_data_08.Text;
            string Data_09 = label_data_09.Text;
            string Data_10 = label_data_10.Text;
            string Data_11 = label_data_11.Text;
            string Data_12 = label_data_12.Text;

            textBox_moveinfo.Text = Data_01 + " " + Data_02 + " " + Data_03 + " " + Data_04 + " " + Data_05 + " " + Data_06 + " "
                                  + Data_07 + " " + Data_08 + " " + Data_09 + " " + Data_10 + " " + Data_11 + " " + Data_12;

        }
        private string Value_over_0To255(string dec)
        {
            //-------------------------------------------------------------------------------
            //入力値が0以下、または255以上になることを防ぐ処理
            //-------------------------------------------------------------------------------
            switch (dec)
            {
                case "":
                    dec = "0";                                      //0以下の場合は強制的に0に変換
                    break;
                default:
                    if (int.Parse(dec) > 255) { dec = "255"; }      //255以上の場合は強制的に255に変換
                    break;
            }
            dec = (int.Parse(dec)).ToString("X").PadLeft(2, '0');   //int.Parse("123"),2桁にしないとバグるので 

            return dec;
        }
        private void Apply_Script_pattern()
        {
            //-------------------------------------------------------------------------------
            //エフェクト・アニメーションのパターンをリストボックスに反映【リストボックス】
            //-------------------------------------------------------------------------------
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\code.ini"; //テキストを読み込む

            //文字コードを変更しないと文字化けする
            //https://qiita.com/pi2ji79/items/4adf09400608f023bf9a
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //sr = new System.IO.StreamReader(iniFileName, System.Text.Encoding.GetEncoding("Shift_JIS"));

            listBox_Effect_select.Items.Clear();
            listBox_Animation_select.Items.Clear();

            StringBuilder db_Effect = new StringBuilder(1024);
            StringBuilder db_Animation = new StringBuilder(1024);
            int res;
            for (int i = 0; i < 256; i++)
            {
                string Loadkey = (i.ToString("X").PadLeft(2, '0'));   //int.Parse("123"),2桁にしないとバグるので
                res = GetPrivateProfileString("Effect", Loadkey, "読み込みエラー", db_Effect, (uint)db_Effect.Capacity, iniFileName);
                res = GetPrivateProfileString("Animation", Loadkey, "読み込みエラー", db_Animation, (uint)db_Animation.Capacity, iniFileName);

                string[] comment_info1 = db_Effect.ToString().Split(',');
                string[] comment_info2 = db_Animation.ToString().Split(',');

                listBox_Effect_select.Items.Add(Loadkey + ":" + comment_info1[9].ToString());       //リスト変数にテキストの記載内容を１行ずつ書き込み
                listBox_Animation_select.Items.Add(Loadkey + ":" + comment_info2[9].ToString()); //リスト変数にテキストの記載内容を１行ずつ書き込み

            }

            listBox_FD_text.Items.Clear();

            iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini"; //テキストを読み込む
            StringBuilder db_FDcode = new StringBuilder(1024);
            for (int i = 0; i < 47; i++)
            {
                string Loadkey = (i.ToString("X").PadLeft(2, '0'));   //int.Parse("123"),2桁にしないとバグるので
                res = GetPrivateProfileString("MB_FD_BattleText", Loadkey, "読み込みエラー", db_FDcode, (uint)db_Effect.Capacity, iniFileName);

                listBox_FD_text.Items.Add(db_FDcode.ToString());       //リスト変数にテキストの記載内容を１行ずつ書き込み

            }


        }

        //--------------------------------------------------------------------------------------------------
        //値入力時の反映処理
        //--------------------------------------------------------------------------------------------------
        private void numericUpDown_Move_Number_ValueChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //番号カウントボックスに指定データを書き込み
            //-------------------------------------------------------------------------------
            label_Move_Number.Text = numericUpDown_Move_Number.Value.ToString();    //テキスト取得では一つ前の値になるため
            button3_Click(sender, e);
        }
        private void textBox_Move_Name_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //名テキストボックスに指定データを書き込み
            //-------------------------------------------------------------------------------
            int move_name_length = int.Parse(label_Name_length.Text);
            string str = textBox_Move_Name.Text;
            str = ByteData_exchange(str, move_name_length);  //文字数拡張に対応するため
            label_Move_Name.Text = str;

            label_Change_announce.Visible = true; //更新チェックテキストを表示
        }
        private void textBox_Move_text_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技説明テキストボックスに指定データを書き込み
            //-------------------------------------------------------------------------------
            int Move_Text_length = int.Parse(label_Move_text_length.Text);
            string str = textBox_Move_text.Text;

            string str_text = str.Replace("\n", "").Replace("\r", "").Replace("\t", "");

            str = ByteData_exchange(str_text, Move_Text_length);
            label_Move_text.Text = str;
            // 最大文字数を超えた場合は、超えた部分を削除して最後の文字が空白の場合は削除する
            if (label_Move_text.Text.Length > textBox_Move_text_code.MaxLength)
            {
                label_Move_text.Text = label_Move_text.Text.Substring(0, textBox_Move_text_code.MaxLength).Trim();
            }
            textBox_Move_text_code.Text = label_Move_text.Text.ToUpper();

            label_Change_announce.Visible = true; //更新チェックテキストを表示
        }
        private void textBox_label_moveID_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技IDテキストボックスに指定データを書き込み
            //-------------------------------------------------------------------------------
            string dec = textBox_label_moveID.Text;
            dec = Value_over_0To255(dec);
            label_data_01.Text = dec;
            MoveInfo_build();
        }
        private void textBox_Movepower_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技威力テキストボックスに指定データを書き込み
            //-------------------------------------------------------------------------------
            string dec = textBox_Movepower.Text;
            dec = Value_over_0To255(dec);
            label_data_02.Text = dec;
            MoveInfo_build();
        }
        private void comboBox_moveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技タイプコンボボックスを変えた際の処理
            //-------------------------------------------------------------------------------
            int sql = comboBox_moveType.SelectedIndex;
            string str_dec = sql.ToString("X").PadLeft(2, '0');
            label_data_03.Text = str_dec;
            MoveInfo_build();
        }
        private void textBox_Accuracy_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //命中率テキストボックスに指定データを書き込み
            //-------------------------------------------------------------------------------
            string dec = textBox_Accuracy.Text;
            dec = Value_over_0To255(dec);
            label_data_04.Text = dec;
            MoveInfo_build();
        }
        private void textBox_PP_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //PPテキストボックスに指定データを書き込み
            //-------------------------------------------------------------------------------
            string dec = textBox_PP.Text;
            dec = Value_over_0To255(dec);
            label_data_05.Text = dec;
            MoveInfo_build();
        }
        private void textBox_Prob_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //追加効果テキストボックスを変えた際の処理
            //-------------------------------------------------------------------------------
            string dec = textBox_Prob.Text;
            dec = Value_over_0To255(dec);
            label_data_06.Text = dec;
            MoveInfo_build();
        }
        private void comboBox_Attack_range_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技範囲コンボボックスを変えた際の処理
            //-------------------------------------------------------------------------------
            string select_value = comboBox_Attack_range.SelectedValue.ToString();
            label_data_07.Text = select_value;
            MoveInfo_build();
        }

        private void comboBox_Priority_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //優先度コンボボックスを変えた際の処理
            //-------------------------------------------------------------------------------
            string select_value = comboBox_Priority.SelectedValue.ToString();
            label_data_08.Text = select_value;
            MoveInfo_build();
        }
        private void comboBox_Move_Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //分類コンボボックスを変えた際の処理
            //-------------------------------------------------------------------------------
            int sql = comboBox_Move_Category.SelectedIndex;
            string str_dec = sql.ToString("X").PadLeft(2, '0');
            label_data_11.Text = str_dec;
            MoveInfo_build();
        }
        private void comboBox_movetxt_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            int select_num = comboBox_movetxt_mode.SelectedIndex;
            switch (select_num)
            {
                case 0:
                    label_movetxt_mode_info.Text = "デフォルトから変更なし，118枠まで対応";
                    break;

                default:
                    textBox__movetxt_pattern.Enabled = true;
                    label_movetxt_mode_info.Text = "拡張済み，上限なし" + "\r\n" + "必ずテーブル開始オフセットを空き領域に指定後に変更すること";
                    break;
            }

        }
        private void comboBox_Type_Chart_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //タイプチャート参照先を変えた際の処理
            //-------------------------------------------------------------------------------
            int select_num = comboBox_Type_Chart.SelectedIndex;
            switch (select_num)
            {
                case 0:
                    label_TypeChart_Mode_txt.Text = "デフォルトから変更なし";
                    break;

                default:
                    textBox_TypeChart_Address.Enabled = true;
                    label_TypeChart_Mode_txt.Text = "拡張済み，上限なし" + "\r\n" + "必ずテーブル開始オフセットを空き領域に指定後に変更すること";
                    break;
            }

        }
        private void groupBox_Move_flag_math(string label_name, string group_name)
        {
            //-------------------------------------------------------------------------------
            //技フラグのチェックを変更した際の共通処理
            //-------------------------------------------------------------------------------
            //https://teratail.com/questions/217302
            //https://ktts.hatenablog.com/entry/2019/09/09/232127
            try
            {
                int flag_num = 0;
                var checkBoxes = main_tag.TabPages["tabPage1"].Controls[group_name].Controls
                        .OfType<System.Windows.Forms.CheckBox>()
                        .ToList();
                foreach (var checkBox in checkBoxes)
                {
                    var Check_tag = ($"{checkBox.Tag}");
                    var Check_flg = ($"{checkBox.Checked}");
                    if (Check_flg == "True")
                    {
                        flag_num = flag_num + int.Parse(Check_tag);
                    }
                }
                string str_dec = flag_num.ToString("X").PadLeft(2, '0');     //2桁の16進数に変換

                main_tag.TabPages["tabPage1"].Controls[label_name].Text = str_dec;
                MoveInfo_build();
            }
            catch
            {
                return;
            }
        }
        private void checkBox_Moveflag_1_CheckedChanged(object sender, EventArgs e)
        {
            string label_name = "label_data_09";
            string group_name = "groupBox_Move_flag";
            groupBox_Move_flag_math(label_name, group_name);
        }
        private void checkBox_Abilityflg1_1_CheckedChanged(object sender, EventArgs e)
        {
            string label_name = "label_data_10";
            string group_name = "groupBox_Ability_flag1";
            groupBox_Move_flag_math(label_name, group_name);
        }
        private void checkBox_Abilityflg2_1_CheckedChanged(object sender, EventArgs e)
        {
            string label_name = "label_data_12";
            string group_name = "groupBox_Ability_flag2";
            groupBox_Move_flag_math(label_name, group_name);
        }
        private void textBox_Read_address_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //スプリプトテキストボックスを変えた際の処理
            //-------------------------------------------------------------------------------
            string LoadText = textBox_Effect_address.Text;
            textBox_Effect_address.Text = LoadText;

            int decValue = 0;

            if (LoadText == "")
            {
                LoadText = "0";
            }

            decValue = Convert.ToInt32(LoadText, 16) + 134217728;    //読込値を10進数に変換してからROM領域番地を足す 
            LoadText = decValue.ToString("X").PadLeft(8, '0');

            LoadText = LoadText + "00000000";                        //空欄があるとErrorになるので00で埋めておく

            LoadText = LoadText.Substring(6, 2) + " " + LoadText.Substring(4, 2) + " " +
                           LoadText.Substring(2, 2) + " " + LoadText.Substring(0, 2);

            label_Read_address.Text = LoadText;
            label_Change_announce.Visible = true; //更新チェックテキストを表示

        }
        private void textBox_Effect_address_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //エフェクトテキストボックスを変えた際の処理
            //-------------------------------------------------------------------------------
            string LoadText = textBox_Animation_address.Text;
            textBox_Animation_address.Text = LoadText;

            int decValue = 0;

            if (LoadText == "")
            {
                LoadText = "0";
            }

            decValue = Convert.ToInt32(LoadText, 16) + 134217728;    //読込値を10進数に変換してからROM領域番地を足す 
            LoadText = decValue.ToString("X").PadLeft(8, '0');

            LoadText = LoadText + "00000000";                        //空欄があるとErrorになるので00で埋めておく

            LoadText = LoadText.Substring(6, 2) + " " + LoadText.Substring(4, 2) + " " +
                           LoadText.Substring(2, 2) + " " + LoadText.Substring(0, 2);

            label_Animation_address.Text = LoadText;
            label_Change_announce.Visible = true; //更新チェックテキストを表示
        }
        private void richTextBox_Read_Script_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //スクリプトデータを記録用ラベルに記載
            //-------------------------------------------------------------------------------
            string script_data = richTextBox_Effect_Script.Text;
            string get_data = "";
            script_data = script_data.Replace(" ", "");
            script_data = script_data.Replace("[", "");
            script_data = script_data.Replace("]", "");
            script_data = script_data.Replace("X", "");
            script_data = script_data.Replace("\n", "");
            script_data = script_data.Replace("\r\n", "");

            //2文字ずつ分割して読み込んで最後に空白がある場合は消す
            for (int i = 0; i < script_data.Length - 1; i += 2)
            {
                get_data += script_data.Substring(i, 2) + " ";
            }

            label_Read_Script.Text = get_data.TrimEnd(); // s.TrimEnd()で最後の余計な空白を消去できる
            label_Change_announce.Visible = true; //更新チェックテキストを表示
        }
        private void richTextBox_Effect_Script_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //エフェクトデータを記録用ラベルに記載
            //-------------------------------------------------------------------------------
            string script_data = richTextBox_Animation_Script.Text;
            string get_data = "";
            script_data = script_data.Replace(" ", "");
            script_data = script_data.Replace("[", "");
            script_data = script_data.Replace("]", "");
            script_data = script_data.Replace("X", "");
            script_data = script_data.Replace("～", "");
            script_data = script_data.Replace("\n", "");
            script_data = script_data.Replace("\r\n", "");

            //2文字ずつ分割して読み込んで最後に空白がある場合は消す
            for (int i = 0; i < script_data.Length - 1; i += 2)
            {
                get_data += script_data.Substring(i, 2) + " ";
            }

            label_Animation_Script.Text = get_data.TrimEnd(); // s.TrimEnd()で最後の余計な空白を消去できる
            label_Change_announce.Visible = true; //更新チェックテキストを表示
        }
        private void listBox_Script_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //スクリプトリストボックスの選択を変えた際の処理
            //-------------------------------------------------------------------------------
            int Select_list_num = 0;
            int res;

            Select_list_num = listBox_Effect_select.SelectedIndex;         //選択中の場所をリストを

            string code_data = Select_list_num.ToString().PadLeft(2, '0'); //2桁の文字列数字に変換
            code_data = Select_list_num.ToString("X").PadLeft(2, '0');     //2桁の16進数に変換する

            string inifolder = toolStripComboBox_Useinidata.Text;
            string code_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\code.ini";    //iniからコードの型を取得 
            StringBuilder db = new StringBuilder(2048);

            //Scriptコードの内訳を読み込み
            res = GetPrivateProfileString("Effect", code_data, "読み込みエラー", db, (uint)db.Capacity, code_iniFileName);
            string[] code_info = db.ToString().Split(',');

            var Script_body = "";

            for (int j = 0; j < 7; j++)
            {
                //読み込みiniから記載文字列を作成
                switch (code_info[j])
                {
                    case "code"://先頭コード                                                
                        Script_body += code_data + " ";
                        break;

                    case "b" or "t" or "c" or "p"://1byte
                        Script_body += "[XX] ";
                        break;

                    case "h" or "r" or "d"://2byte
                        Script_body += "[XX XX] ";
                        break;

                    case "w" or "o" or "m"://4byte
                        Script_body += "[XX XX XX XX] ";
                        break;

                    case "x"://1byte可変長、2byte
                        Script_body += "[NN] [XX XX]～ ";
                        break;

                    case "y"://1byte可変長、1byte
                        Script_body += "[NN] [XX]～ ";
                        break;

                    default://読み込みなし
                        break;
                }
            }

            richTextBox_Effect_Script.Text += "\r\n" + Script_body;

        }
        private void listBox_Effect_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技スクリプトリストボックスの選択を変えた際の処理
            //-------------------------------------------------------------------------------
            int Select_list_num = 0;
            int res;

            Select_list_num = listBox_Animation_select.SelectedIndex;      //選択中の場所をリスト

            string code_data = Select_list_num.ToString().PadLeft(2, '0'); //2桁の文字列数字に変換
            code_data = Select_list_num.ToString("X").PadLeft(2, '0');     //2桁の16進数に変換する

            string inifolder = toolStripComboBox_Useinidata.Text;
            string code_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\code.ini";    //iniからコードの型を取得 
            StringBuilder db = new StringBuilder(2048);

            //Scriptコードの内訳を読み込み
            res = GetPrivateProfileString("Animation", code_data, "読み込みエラー", db, (uint)db.Capacity, code_iniFileName);
            string[] code_info = db.ToString().Split(',');

            var Script_body = "";

            for (int j = 0; j < 7; j++)
            {
                //読み込みiniから記載文字列を作成
                switch (code_info[j])
                {
                    case "code"://先頭コード
                        Script_body += code_data + " ";
                        break;

                    case "b" or "t" or "c" or "p"://1byte
                        Script_body += "[XX] ";
                        break;

                    case "h" or "r" or "d"://2byte
                        Script_body += "[XX XX] ";
                        break;

                    case "w" or "o" or "m"://4byte
                        Script_body += "[XX XX XX XX] ";
                        break;

                    case "x"://1byte可変長、2byte
                        Script_body += "[NN] [XX XX]～ ";
                        break;

                    case "y"://1byte可変長、1byte
                        Script_body += "[NN] [XX]～ ";
                        break;

                    default://読み込みなし
                        break;
                }
            }

            richTextBox_Animation_Script.Text += Script_body + "\r\n";


        }

        private void textBox_label_moveID_KeyPress(object sender, KeyPressEventArgs e)
        {
            //--------------------------------------------------------------------------------------------------
            //テキストボックスの入力規制（数字のみ）
            //--------------------------------------------------------------------------------------------------
            //https://www.itlab51.com/?p=2738
            //https://stellacreate.com/entry/cs-textbox-number-only

            if ((e.KeyChar < 48) || (e.KeyChar > 57))   //入力可能文字列を数字のみに規制
            {
                if (e.KeyChar != '\b')
                {
                    e.Handled = true;
                }
            }
        }
        private void textBox_moveinfo_KeyPress(object sender, KeyPressEventArgs e)
        {
            //--------------------------------------------------------------------------------------------------
            //テキストボックスの入力規制（16進数のみ）
            //--------------------------------------------------------------------------------------------------
            //http://faq.creasus.net/04/0131/CharCode.html
            if (((e.KeyChar < 48) || (e.KeyChar > 57)) &&
                ((e.KeyChar < 65) || (e.KeyChar > 70)) &&
                ((e.KeyChar < 97) || (e.KeyChar > 102))
                )   //入力可能文字列を数字のみに規制
            {
                if (e.KeyChar != '\b')
                {
                    e.Handled = true;
                }
            }

        }
        private void AnyData_Changedcheck(object sender, EventArgs e)
        {
            label_Change_announce.Visible = true; //テキストを見えるように変更
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //効果指定書き込みボタン押下時
            //-------------------------------------------------------------------------------
            string apply_Address = textBox_Effect_address.Text;
            DialogResult result = MessageBox.Show(
                "参照テーブルを " + apply_Address + " に更新し\r\n設定アドレスに変更内容を上書きしますか？",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK)
            {
                try
                {
                    Move_effect_Save();
                    MessageBox.Show("技効果の上書きが完了しました.", "System message");

                    //更新ログに記載
                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "Effect",
                                                     label_Updatabefore_Effect.Text,
                                                     textBox_Effect_address.Text,
                                                     DateTime.Now,
                                                     "詳細情報",
                                                     label_before_Script_Effect.Text,
                                                     richTextBox_Effect_Script.Text
                                                     );
                    label_Updatabefore_Effect.Text = textBox_Effect_address.Text;     //変更前アドレスの内容を更新
                    label_before_Script_Effect.Text = richTextBox_Effect_Script.Text; //変更前スクリプトの内容を更新
                    label_Change_announce.Visible = false;
                }
                catch
                {
                    //Errorが発生した場合はメッセージボックスを表示して終了
                    MessageBox.Show("保存に失敗しました。\n入力できない文字列が記載されていないか確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //キャンセルの場合は何もしない
            }
        }
        private void Move_effect_Save()
        {
            string path = toolStripStatusLabel__Fullpath.Text;   //toolStripStatusLabel__Fullpathにパスを記載する
            int Load_Num = int.Parse(label_Move_Number.Text);    //初回読み込み時にiniから読んだテキストを参照する

            //選択したiniのデータベースを取得する
            string LoadSection = "DataBase";

            //技情報の長さを取得
            int MoveInfo_Table_Length = 0;
            var Loadkey = "MoveInfo_Table_Length";
            MoveInfo_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            //技情報テーブルの開始位置を取得して場所を計算
            int MoveInfo_Table = 0;
            Loadkey = "MoveInfo_Table";
            MoveInfo_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            MoveInfo_Table = MoveInfo_Table + (Load_Num * MoveInfo_Table_Length);      //読み込みテーブル位置を該当番号のものに再計算する

            //---------------------------------------------------------------------
            //技スクリプト内容を反映
            var LoadText = String_exchange(label_Read_Script.Text);

            int MoveScript_Length = 0;
            MoveScript_Length = LoadText.Length;

            int MoveScript = 0;
            var decValue = Convert.ToInt32(textBox_Effect_address.Text, 16); //記載されたAddressはテキストなので10進数に戻す必要がある
            MoveScript = decValue;

            Write_binary_data(path, MoveScript, MoveScript_Length, LoadText);

            //---------------------------------------------------------------------
            //スクリプト設定位置反映
            string effect_num = Read_binary_data(path, MoveInfo_Table, 1);
            decValue = Convert.ToInt32(effect_num, 16);

            int Effect_Table_Length = 0;
            Loadkey = "Effect_Table_Length";
            Effect_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            int Effect_Table = 0;
            Loadkey = "Effect_Table";
            Effect_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            Effect_Table = Effect_Table + (decValue * Effect_Table_Length);

            LoadText = String_exchange(label_Read_address.Text);
            Write_binary_data(path, Effect_Table, Effect_Table_Length, LoadText);

        }
        private void button12_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //アニメ指定書き込みボタン押下時
            //-------------------------------------------------------------------------------
            string apply_Address = textBox_Animation_address.Text;

            DialogResult result = MessageBox.Show(
                "参照テーブルを " + apply_Address + " に更新し\r\n設定アドレスに変更内容を上書きしますか？",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK)
            {
                try
                {
                    Move_animation_Save();
                    MessageBox.Show("技アニメの上書きが完了しました.", "System message");

                    //更新ログに記載
                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "Animation",
                                                     label_Updatabefore_Animation.Text,
                                                     textBox_Animation_address.Text,
                                                     DateTime.Now,
                                                     "詳細情報",
                                                     label_before_Script_Animation.Text,
                                                     richTextBox_Animation_Script.Text
                                                     );
                    label_Updatabefore_Animation.Text = textBox_Animation_address.Text;     //変更前アドレスの内容を更新
                    label_before_Script_Animation.Text = richTextBox_Animation_Script.Text; //変更前スクリプトの内容を更新
                    label_Change_announce.Visible = false;
                }
                catch
                {
                    //Errorが発生した場合はメッセージボックスを表示して終了
                    MessageBox.Show("保存に失敗しました。\n入力できない文字列が記載されていないか確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //キャンセルの場合は何もしない
            }
        }
        private void Move_animation_Save()
        {
            string path = toolStripStatusLabel__Fullpath.Text;           //toolStripStatusLabel__Fullpathにパスを記載する
            int Load_Num = int.Parse(numericUpDown_Move_Number.Text);    //現在のIDを参照

            //選択したiniのデータベースを取得する
            string LoadSection = "DataBase";

            //技情報の長さを取得
            int Animation_Table_Length = 0;
            var Loadkey = "Animation_Table_Length";
            Animation_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            //開始位置を取得して場所を計算
            int Animation_Table = 0;
            Loadkey = "Animation_Table";
            Animation_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            Animation_Table = Animation_Table + (Load_Num * Animation_Table_Length);      //読み込みテーブル位置を該当番号のものに再計算する

            //---------------------------------------------------------------------
            //スクリプト内容を反映
            var LoadText = String_exchange(label_Animation_Script.Text);

            int MoveScript_Length = 0;
            MoveScript_Length = LoadText.Length;

            int MoveScript = 0;
            var decValue = Convert.ToInt32(textBox_Animation_address.Text, 16); //記載されたAddressはテキストなので10進数に戻す必要がある
            MoveScript = decValue;

            Write_binary_data(path, MoveScript, MoveScript_Length, LoadText);

            //---------------------------------------------------------------------
            //スクリプト設定位置反映
            LoadText = String_exchange(label_Animation_address.Text);
            Write_binary_data(path, Animation_Table, Animation_Table_Length, LoadText);
        }
        private void Write_binary_data(string path, int StartLen, int LoadLen, byte[] LoadText)
        {
            //-------------------------------------------------------------------------------
            //バイナリファイルに指定データを書き込み
            //-------------------------------------------------------------------------------
            var reader = new BinaryWriter(new FileStream(path, FileMode.Open));

            // byte配列の作成
            foreach (var item in LoadText)
            {
                Console.Write(string.Format("{0:X2} ", item));
            }
            Console.WriteLine();

            // 指定したアドレスに書き込み位置を移動
            reader.Seek(StartLen, SeekOrigin.Begin);

            reader.Write(LoadText);
            reader.Close();
        }
        private static byte[] String_exchange(string LoadText)
        {
            //-------------------------------------------------------------------------------
            //読込データをバイナリ配列に変換
            //-------------------------------------------------------------------------------
            string input = LoadText;
            string[] bytestr = input.Split(' ');
            byte[] data = new byte[bytestr.Length];

            for (int i = 0; i < bytestr.Length; i++)
            {
                data[i] = Convert.ToByte(bytestr[i], 16);
            }
            return data;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //スクリプトアドレス読み込み先変更ボタン
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                                    //toolStripStatusLabel__Fullpathにパスを記載する

            int decValue = Convert.ToInt32(textBox_Effect_address.Text, 16);                      //読込位置がテキストデータのため、16進数から10進数に再計算                           
            int Address_length = int.Parse(numericUpDown_Effect_Address_length.Value.ToString()); //読み込みbyte数値を取得（デフォルトでは100）

            string LoadText = Read_binary_data(path, decValue, Address_length);                   //読込Address位置からScriptデータを抽出

            label_Read_Script.Text = LoadText;                                                    //記載用Scriptデータをラベルに記載
            richTextBox_Effect_Script.Text = LoadText;                                            //テキストボックスに記載
        }
        private void button13_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技アニメアドレス読み込み先変更ボタン
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                                       //toolStripStatusLabel__Fullpathにパスを記載する

            int decValue = Convert.ToInt32(textBox_Animation_address.Text, 16);                      //読込位置がテキストデータのため、16進数から10進数に再計算                           
            int Address_length = int.Parse(numericUpDown_Animation_Address_length.Value.ToString()); //読み込みbyte数値を取得（デフォルトでは100）

            string LoadText = Read_binary_data(path, decValue, Address_length);                      //読込Address位置からScriptデータを抽出

            label_Animation_Script.Text = LoadText;                                                  //記載用Scriptデータをラベルに記載
            richTextBox_Animation_Script.Text = LoadText;                                            //テキストボックスに記載
        }
        private void button8_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //Animationリスト読み込み先変更ボタン
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;

            int decValue = Convert.ToInt32(textBox__movetxt_pattern.Text, 16);
            Read_Move_binary_2byte(path, decValue); //技表示テキストのリストボックスに値を記載させる

        }
        private void button5_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技情報書き込みボタン押下時
            //-------------------------------------------------------------------------------
            DialogResult result = MessageBox.Show(
                "変更内容を上書きしますか？",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK)
            {
                try
                {
                    Move_info_Save();
                    label_Change_announce.Visible = false;
                    MessageBox.Show("技情報の上書きが完了しました.", "System message");

                    //更新ログに記載
                    string Text_log_before = "Name：\n" +
                                            label_before_Script_MoveName.Text +
                                            "\n" +
                                            "\nInfo：\n" +
                                            label_before_Script_MoveInfo.Text +
                                            "\n" +
                                            "\nText：\n" +
                                            label_before_Script_MoveText.Text;

                    string Text_log_after = "Name：\n" +
                                            textBox_Move_Name.Text +
                                            "\n" +
                                            "\nInfo：\n" +
                                            textBox_moveinfo.Text +
                                            "\n" +
                                            "\nText：\n" +
                                            textBox_Move_text.Text;

                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "Move Data",
                                                     label_Updatabefore_MoveInfo.Text,
                                                     label_Updatabefore_MoveInfo.Text,
                                                     DateTime.Now,
                                                     "詳細情報",
                                                     Text_log_before,
                                                     Text_log_after
                                                     );
                    label_before_Script_MoveName.Text = textBox_Move_Name.Text;
                    label_before_Script_MoveInfo.Text = textBox_moveinfo.Text;
                    label_before_Script_MoveText.Text = textBox_Move_text.Text;
                }
                catch
                {
                    //Errorが発生した場合はメッセージボックスを表示して終了
                    MessageBox.Show("保存に失敗しました。\n入力できない文字列が記載されていないか確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //キャンセルの場合は何もしない
            }
        }
        private void Move_info_Save()
        {
            string path = toolStripStatusLabel__Fullpath.Text;   //toolStripStatusLabel__Fullpathにパスを記載する
            int Load_Num = int.Parse(label_Move_Number.Text);    //初回読み込み時にiniから読んだテキストを参照する

            //Table_Dataの値を反映させる
            //選択したiniのデータベースを取得する
            string LoadSection = "DataBase";

            //技情報の長さを取得
            int MoveInfo_Table_Length = 0;
            var Loadkey = "MoveInfo_Table_Length";
            MoveInfo_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            //技情報テーブルの開始位置を取得して場所を計算
            int MoveInfo_Table = 0;
            Loadkey = "MoveInfo_Table";
            MoveInfo_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            MoveInfo_Table = MoveInfo_Table + (Load_Num * MoveInfo_Table_Length);      //読み込みテーブル位置を該当番号のものに再計算する

            //技情報の本文を指定
            var LoadText = String_exchange(textBox_moveinfo.Text);
            Write_binary_data(path, MoveInfo_Table, MoveInfo_Table_Length, LoadText);

            //---------------------------------------------------------------------
            //技名データを反映
            int MoveName_Table_Length = 0;
            Loadkey = "MoveName_Table_Length";
            MoveName_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            int MoveName_Table = 0;
            Loadkey = "MoveName_Table";
            MoveName_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            MoveName_Table = MoveName_Table + (Load_Num * MoveName_Table_Length);

            LoadText = String_exchange(label_Move_Name.Text);
            Write_binary_data(path, MoveName_Table, MoveName_Table_Length, LoadText);

            //---------------------------------------------------------------------
            //技説明を反映
            int MoveText_Table_Length = 0;
            Loadkey = "MoveText_Table_Length";
            MoveText_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            int MoveText_Table = 0;
            Loadkey = "MoveText_Table";
            MoveText_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            MoveText_Table = MoveText_Table + (Load_Num * MoveText_Table_Length);

            LoadText = String_exchange(label_Move_text.Text);
            Write_binary_data(path, MoveText_Table, MoveText_Table_Length, LoadText);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技表示書き込みボタン押下時
            //-------------------------------------------------------------------------------
            string apply_Address = textBox__movetxt_pattern.Text;

            DialogResult result = MessageBox.Show(
                "参照テーブルを " + apply_Address + " に更新し\r\n設定アドレスに変更内容を上書きしますか？",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK)
            {

                try
                {
                    Move_textPattern_Save();
                    MessageBox.Show("技表示パターンの上書きが完了しました.", "System message");

                    //更新ログに記載
                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "TextPattern",
                                                     label_Updatabefore_Pattern.Text,
                                                     textBox__movetxt_pattern.Text,
                                                     DateTime.Now,
                                                     "詳細情報",
                                                     label_before_Script_Pattern.Text,
                                                     label_after_Script_Pattern.Text    //セーブ処理内にあるのでテストでは確認不可
                                                     );
                    label_Updatabefore_Pattern.Text = textBox__movetxt_pattern.Text;    //変更前アドレスの内容を更新
                    label_before_Script_Pattern.Text = label_after_Script_Pattern.Text; //変更前スクリプトの内容を更新
                    label_Change_announce.Visible = false;

                }
                catch
                {
                    //Errorが発生した場合はメッセージボックスを表示して終了
                    MessageBox.Show("保存に失敗しました。\n入力できない文字列が記載されていないか確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //キャンセルの場合は何もしない
            }

        }
        private void Move_textPattern_Save()
        {
            string path = toolStripStatusLabel__Fullpath.Text;      //toolStripStatusLabel__Fullpathにパスを記載する
            int Load_Num = int.Parse(label_Move_Number.Text);       //初回読み込み時にiniから読んだテキストを参照する

            string LoadSection = "DataBase"; //選択したiniのデータベースを取得する

            //textBox__movetxt_pattenの更新
            int TextPatternSelect1 = 0;
            int TextPatternSelect2 = 0;
            var Loadkey = "TextPattern_Select";
            TextPatternSelect1 = MyGetPrivateProfileInt(LoadSection, Loadkey);
            TextPatternSelect2 = TextPatternSelect1 + 0x60;

            var LoadText = textBox__movetxt_pattern.Text;
            int decValue = 0;

            if (LoadText == "")
            {
                LoadText = "0";
            }

            decValue = Convert.ToInt32(LoadText, 16) + 134217728;    //読込値を10進数に変換してからROM領域番地を足す 
            LoadText = decValue.ToString("X").PadLeft(8, '0');

            LoadText = LoadText + "00000000";                        //空欄があるとErrorになるので00で埋めておく


            LoadText = LoadText.Substring(6, 2) + " " + LoadText.Substring(4, 2) + " " +
                        LoadText.Substring(2, 2) + " " + LoadText.Substring(0, 2);

            var convert_val = String_exchange(LoadText);

            Write_binary_data(path, TextPatternSelect1, 4, convert_val);
            Write_binary_data(path, TextPatternSelect2, 4, convert_val);

            //textBox_movetxt_pattenの更新
            int TextPattern_Number = 0;
            Loadkey = "TextPattern_Number";
            TextPattern_Number = MyGetPrivateProfileInt(LoadSection, Loadkey);

            int select_num = comboBox_movetxt_mode.SelectedIndex;
            switch (select_num)
            {
                case 0:
                    LoadText = "B1";
                    break;

                default:
                    LoadText = "FF";
                    break;
            }
            convert_val = String_exchange(LoadText);
            Write_binary_data(path, TextPattern_Number, 1, convert_val);

            //label_Move_Pattern_Alltext（更新したリストの反映）
            string all_movetext_id = "";
            int movetext_length = 0;

            for (int j = 1; j < 5; j++)
            {
                for (int i = 0; i < ((ListBox)main_tag.TabPages["tabPage4"].Controls["listBox_Movetext_pattern" + j]).Items.Count; i++)
                {
                    string Select_item = (string)((ListBox)main_tag.TabPages["tabPage4"].Controls["listBox_Movetext_pattern" + j]).Items[i];
                    string[] get_load_num = Select_item.Split(",");
                    string get_List_num = get_load_num[0].ToString(); //char型をstring型に変更
                    all_movetext_id += get_List_num + " ";            //技IDだけを集積する
                    movetext_length += movetext_length + 2;           //読み込みbyte数を算出
                }
                all_movetext_id += "00 00 ";
            }
            all_movetext_id = all_movetext_id.TrimEnd();                        //s.TrimEnd()で最後の余計な空白を消去できる;
            label_after_Script_Pattern.Text = all_movetext_id;                  //変更後のスクリプト内容を専用のテキストに記録

            convert_val = String_exchange(all_movetext_id);
            LoadText = textBox__movetxt_pattern.Text;
            decValue = Convert.ToInt32(LoadText, 16);
            Write_binary_data(path, decValue, movetext_length, convert_val);    //値を記載Address位置に書き込み

        }
        private void button7_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技スクリプト詳細サブフォーム表示処理（効果）
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                             //toolStripStatusLabel__Fullpathにパスを記載する
            int decValue = Convert.ToInt32(textBox_Effect_address.Text, 16);                //読込位置がテキストデータのため、16進数から10進数に再計算                           
            int Address_length = int.Parse(numericUpDown_Effect_Address_length.Value.ToString()); //読み込みbyte数値を取得（デフォルトでは100）
            bool endFlag = checkBox_endFlag_check1.Checked;                                 //終了スクリプトがある場合終了させるチェック
            bool jumpFlag = checkBox_jump_check1.Checked;
            string Mode_flg = "Effect";                                                        //読込タイプを設定
            string secret_flag = "";                                                              //初回は特殊フラグはなし
            string VBcheck = "Viewer";
            string LoadPattern = toolStripComboBox_Useinidata.Text; //どのini読んでるかの判定
            string StirlingPath = textBox_StilringURL.Text;
            string Move_Name = textBox_Move_Name.Text;

            string First_data = label_Read_Script.Text;
            string First_flg = "0";

            Form2 form2 = new Form2(path, decValue, Address_length, Mode_flg, endFlag, jumpFlag, secret_flag, First_data, First_flg, VBcheck, LoadPattern, StirlingPath, Move_Name);
            form2.Show();

        }
        private void button14_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技スクリプト詳細サブフォーム表示処理（エフェクト）
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                                //label_Fullpathにパスを記載する
            int decValue = Convert.ToInt32(textBox_Animation_address.Text, 16);                //読込位置がテキストデータのため、16進数から10進数に再計算                           
            int Address_length = int.Parse(numericUpDown_Animation_Address_length.Value.ToString()); //読み込みbyte数値を取得（デフォルトでは100）
            bool endFlag = checkBox_endFlag_check2.Checked;                                    //終了スクリプトがある場合終了させるチェック
            bool jumpFlag = checkBox_jump_check2.Checked;
            string Mode_flg = "Animation";                                                        //読込タイプを設定
            string secret_flag = "";                                                                 //初回は特殊フラグはなし
            string VBcheck = "Viewer";
            string LoadPattern = toolStripComboBox_Useinidata.Text;
            string StirlingPath = textBox_StilringURL.Text;
            string Move_Name = textBox_Move_Name.Text;

            string First_data = label_Animation_Script.Text;
            string First_flg = "0";

            Form2 form2 = new Form2(path, decValue, Address_length, Mode_flg, endFlag, jumpFlag, secret_flag, First_data, First_flg, VBcheck, LoadPattern, StirlingPath, Move_Name);
            form2.Show();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技スクリプトビルダーサブフォーム表示処理（効果）
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                             //toolStripStatusLabel__Fullpathにパスを記載する
            int decValue = Convert.ToInt32(textBox_Effect_address.Text, 16);                //読込位置がテキストデータのため、16進数から10進数に再計算                           
            int Address_length = int.Parse(numericUpDown_Effect_Address_length.Value.ToString()); //読み込みbyte数値を取得（デフォルトでは100）
            bool endFlag = checkBox_endFlag_check1.Checked;                                 //終了スクリプトがある場合終了させるチェック
            bool jumpFlag = checkBox_jump_check1.Checked;
            string Mode_flg = "Effect";                                                        //読込タイプを設定
            string secret_flag = "";                                                              //初回は特殊フラグはなし
            string VBcheck = "Builder";
            string LoadPattern = toolStripComboBox_Useinidata.Text;
            string StirlingPath = textBox_StilringURL.Text;
            string Move_Name = textBox_Move_Name.Text;

            string First_data = label_Read_Script.Text;
            string First_flg = "0";

            Form2 form2 = new Form2(path, decValue, Address_length, Mode_flg, endFlag, jumpFlag, secret_flag, First_data, First_flg, VBcheck, LoadPattern, StirlingPath, Move_Name);
            form2.Show();

        }
        private void button17_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技スクリプトビルダーサブフォーム表示処理（アニメ）
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                              　//toolStripStatusLabel__Fullpathにパスを記載する
            int decValue = Convert.ToInt32(textBox_Animation_address.Text, 16);                //読込位置がテキストデータのため、16進数から10進数に再計算                           
            int Address_length = int.Parse(numericUpDown_Animation_Address_length.Value.ToString()); //読み込みbyte数値を取得（デフォルトでは100）
            bool endFlag = checkBox_endFlag_check2.Checked;                                    //終了スクリプトがある場合終了させるチェック
            bool jumpFlag = checkBox_jump_check2.Checked;
            string Mode_flg = "Animation";                                                        //読込タイプを設定
            string secret_flag = "";                                                                 //初回は特殊フラグはなし
            string VBcheck = "Builder";
            string LoadPattern = toolStripComboBox_Useinidata.Text;
            string StirlingPath = textBox_StilringURL.Text;
            string Move_Name = textBox_Move_Name.Text;

            string First_data = label_Read_Script.Text;
            string First_flg = "0";

            Form2 form2 = new Form2(path, decValue, Address_length, Mode_flg, endFlag, jumpFlag, secret_flag, First_data, First_flg, VBcheck, LoadPattern, StirlingPath, Move_Name);
            form2.Show();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //技テキスト表示リストの更新
            //-------------------------------------------------------------------------------
            //現在指定されているパターンを取得
            string selected_value = label_Select_Pattern_result.Text.ToString();

            //どのパターンを選択したか取得する
            string select_value = comboBox_Select_Pattern.SelectedValue.ToString();

            //現在表示している技IDとの照合用データを作る
            int Select_move_num = int.Parse(numericUpDown_Move_Number.Value.ToString());    //テキスト取得では一つ前の値になるため);
            string Move_id_2byte = Select_move_num.ToString("X").PadLeft(4, '0');  //4桁の16進数に変換する
            Move_id_2byte = Move_id_2byte.Substring(2, 2) + " " + Move_id_2byte.Substring(0, 2);
            Move_id_2byte += "," + textBox_Move_Name.Text;  //技名を日本語データで取得

            //重複する値がある場合は予めリストから削除してしまう
            int iFind;
            switch (selected_value)
            {
                case "1":
                    iFind = listBox_Movetext_pattern1.FindString(Move_id_2byte);
                    if (iFind >= 0) { listBox_Movetext_pattern1.Items.RemoveAt(iFind); }
                    break;

                case "2":
                    iFind = listBox_Movetext_pattern2.FindString(Move_id_2byte);
                    if (iFind >= 0) { listBox_Movetext_pattern2.Items.RemoveAt(iFind); }

                    break;

                case "3":
                    iFind = listBox_Movetext_pattern3.FindString(Move_id_2byte);
                    if (iFind >= 0) { listBox_Movetext_pattern3.Items.RemoveAt(iFind); }
                    break;

                case "4":
                    iFind = listBox_Movetext_pattern4.FindString(Move_id_2byte);
                    if (iFind >= 0) { listBox_Movetext_pattern4.Items.RemoveAt(iFind); }
                    break;

                default:
                    break;
            }

            //追加したいリストに値を記載する
            switch (select_value)
            {
                case "1":
                    listBox_Movetext_pattern1.Items.Add(Move_id_2byte);
                    label_Select_Pattern_result.Text = "1";
                    break;

                case "2":
                    listBox_Movetext_pattern2.Items.Add(Move_id_2byte);
                    label_Select_Pattern_result.Text = "2";
                    break;

                case "3":
                    listBox_Movetext_pattern3.Items.Add(Move_id_2byte);
                    label_Select_Pattern_result.Text = "3";
                    break;

                case "4":
                    listBox_Movetext_pattern4.Items.Add(Move_id_2byte);
                    label_Select_Pattern_result.Text = "4";
                    break;

                default:
                    label_Select_Pattern_result.Text = "5";
                    break;
            }
            label_Change_announce.Visible = true; //反映未完了テキストをリセット

        }
        private void listBox_Movetext_pattern1_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //現在選択中の技テキスト表示リストインデックスを記載
            //-------------------------------------------------------------------------------
            //選択中のリスト値を記載
            label_get_select_listbox.Text = "1," + listBox_Movetext_pattern1.SelectedIndex.ToString();
            //リストボックス1以外の全ての選択を解除
            listBox_Movetext_pattern2.SelectedIndex = -1;
            listBox_Movetext_pattern3.SelectedIndex = -1;
            listBox_Movetext_pattern4.SelectedIndex = -1;
        }
        private void listBox_Movetext_pattern2_Click(object sender, EventArgs e)
        {
            //選択中のリスト値を記載
            label_get_select_listbox.Text = "2," + listBox_Movetext_pattern2.SelectedIndex.ToString();
            //リストボックス2以外の全ての選択を解除
            listBox_Movetext_pattern1.SelectedIndex = -1;
            listBox_Movetext_pattern3.SelectedIndex = -1;
            listBox_Movetext_pattern4.SelectedIndex = -1;
        }
        private void listBox_Movetext_pattern3_Click(object sender, EventArgs e)
        {
            //選択中のリスト値を記載
            label_get_select_listbox.Text = "3," + listBox_Movetext_pattern3.SelectedIndex.ToString();
            //リストボックス3以外の全ての選択を解除
            listBox_Movetext_pattern1.SelectedIndex = -1;
            listBox_Movetext_pattern2.SelectedIndex = -1;
            listBox_Movetext_pattern4.SelectedIndex = -1;
        }
        private void listBox_Movetext_pattern4_Click(object sender, EventArgs e)
        {
            //選択中のリスト値を記載
            label_get_select_listbox.Text = "4," + listBox_Movetext_pattern4.SelectedIndex.ToString();
            //リストボックス4以外の全ての選択を解除
            listBox_Movetext_pattern1.SelectedIndex = -1;
            listBox_Movetext_pattern2.SelectedIndex = -1;
            listBox_Movetext_pattern3.SelectedIndex = -1;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //選択技を技テキスト表示リストから削除
            //-------------------------------------------------------------------------------
            string[] get_load_num = label_get_select_listbox.Text.Split(",");
            string get_List_num = get_load_num[0].ToString(); //char型をstring型に変更
            int get_index_num = int.Parse(get_load_num[1].ToString());

            switch (get_List_num)
            {
                case "1":
                    listBox_Movetext_pattern1.Items.RemoveAt(get_index_num);
                    break;

                case "2":
                    listBox_Movetext_pattern2.Items.RemoveAt(get_index_num);
                    break;

                case "3":
                    listBox_Movetext_pattern3.Items.RemoveAt(get_index_num);
                    break;

                case "4":
                    listBox_Movetext_pattern4.Items.RemoveAt(get_index_num);
                    break;

                default:
                    break;
            }

            label_Change_announce.Visible = true;
        }
        private void button11_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //検索ウィンドウを表示させる
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;
            string LoadSection = toolStripComboBox_Useinidata.Text;
            string Search_word = textBox_Move_Name.Text;
            Form3 form3 = new Form3(path, LoadSection, Search_word, "move", this);
            form3.Show();

        }
        private void button19_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //アドレス一覧ウィンドウを表示
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;
            string LoadSection = toolStripComboBox_Useinidata.Text;
            Form3 form3 = new Form3(path, LoadSection, "", "address", this);
            form3.Show();
        }
        private void button20_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //テキスト一覧ウィンドウを表示
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;
            string LoadSection = toolStripComboBox_Useinidata.Text;
            Form3 form3 = new Form3(path, LoadSection, "", "BattleText", this);
            form3.Show();
        }
        private void pictureBox_Coordinate_MouseClick(object sender, MouseEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //Pictureboxクリック時に座標を取得する
            //-------------------------------------------------------------------------------

            //既に記載されたPointを消去する--------
            pictureBox_Coordinate.Refresh();

            //click地点に点を描画------------------
            Graphics g = pictureBox_Coordinate.CreateGraphics();  // PictureBoxのGraphicsオブジェクトを取得          
            int x = e.X;
            int y = e.Y;

            g.FillEllipse(Brushes.Red, x - 7, y - 7, 15, 15); // 点を描画

            //座標数値を取得-----------------------
            //double x_math = Math.Round (x / 1.5);　//画面倍率を1.5倍しているので四捨五入で計算しなおす
            //double y_math = Math.Round (y / 1.5);
            x = x / 3;
            y = y / 3;

            //中央座標から計算した相対座標を記載
            int Center_X = Convert.ToInt32(x) - int.Parse(label_Center_num1.Text);
            if (Math.Sign(Center_X) == -1)
            {
                Center_X = 65536 + Center_X; //値が負数の場合は値を逆転させる
            }

            int Center_Y = Convert.ToInt32(y) - int.Parse(label_Center_num2.Text);
            if (Math.Sign(Center_Y) == -1)
            {
                Center_Y = 65536 + Center_Y; //値が負数の場合は値を逆転させる
            }

            string x_val = (x.ToString("X").PadLeft(4, '0'));   //int.Parse("123"),2桁にしないとバグるので
            x_val = x_val.Substring(2, 2) + " " + x_val.Substring(0, 2);
            string y_val = (y.ToString("X").PadLeft(4, '0'));
            y_val = y_val.Substring(2, 2) + " " + y_val.Substring(0, 2);
            textBox_pointaA_X.Text = x_val.ToString();
            textBox_pointaA_Y.Text = y_val.ToString();

            x_val = (Center_X.ToString("X").PadLeft(4, '0'));   //int.Parse("123"),2桁にしないとバグるので
            x_val = x_val.Substring(2, 2) + " " + x_val.Substring(0, 2);
            y_val = (Center_Y.ToString("X").PadLeft(4, '0'));
            y_val = y_val.Substring(2, 2) + " " + y_val.Substring(0, 2);
            textBox_pointaB_X.Text = x_val.ToString();
            textBox_pointaB_Y.Text = y_val.ToString();

        }

        private void pictureBox_Coordinate_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //Pictureboxをダブルクリック時に座標を中心点として取得し、線を交差させる
            //-------------------------------------------------------------------------------
            Point center = e.Location;
            // 線の長さを定義-----------------------
            int length = Math.Min(pictureBox_Coordinate.Width, pictureBox_Coordinate.Height) * 2;

            // リセットで線が消えるため、画像参照呼び出したイメージに直接線を記載する
            pictureBox_Coordinate.Image = new Bitmap(pictureBox_Coordinate.Width, pictureBox_Coordinate.Height);
            Graphics g = Graphics.FromImage(pictureBox_Coordinate.Image);

            g.DrawLine(Pens.Red, center.X - length, center.Y, center.X + length, center.Y);
            g.DrawLine(Pens.Red, center.X, center.Y - length, center.X, center.Y + length);

            //bbackground背景に画像をZoom形式で反映させる
            string BG_num = label_SelectBG_num.Text;
            string inifolder = toolStripComboBox_Useinidata.Text;
            string image_file = AppDomain.CurrentDomain.BaseDirectory + "image\\" + "Base_" + BG_num + ".png";
            Image backgroundImage = Image.FromFile(image_file);
            pictureBox_Coordinate.BackgroundImage = backgroundImage;
            pictureBox_Coordinate.BackgroundImageLayout = ImageLayout.Zoom;

            //中心点の座標を記録-----------------------
            int x = e.X;
            int y = e.Y;

            //double x_math = Math.Round(x / 1.5); //画面倍率を1.5倍しているので四捨五入で計算しなおす
            //double y_math = Math.Round(y / 1.5);

            x = x / 3;
            y = y / 3;

            label_Center_num1.Text = x.ToString(); //計算した中央座標を記載
            label_Center_num2.Text = y.ToString();

            string x_val = (x.ToString("X").PadLeft(4, '0'));   //int.Parse("123"),2桁にしないとバグるので
            x_val = x_val.Substring(2, 2) + " " + x_val.Substring(0, 2);

            string y_val = (y.ToString("X").PadLeft(4, '0'));
            y_val = y_val.Substring(2, 2) + " " + y_val.Substring(0, 2);

            textBox_pointaB_X.Text = x_val;
            textBox_pointaB_Y.Text = y_val;

        }

        private void comboBox_SelectBG_SelectedIndexChanged(object sender, EventArgs e)
        {
            //どのパターンを選択したか取得する
            string select_value = comboBox_SelectBG.SelectedValue.ToString();
            label_SelectBG_num.Text = select_value;

            string BG_num = label_SelectBG_num.Text; //背景画像の番号を取得
            string inifolder = toolStripComboBox_Useinidata.Text;
            string image_file = AppDomain.CurrentDomain.BaseDirectory + "image\\" + "Base_" + BG_num + ".png";
            Image backgroundImage = Image.FromFile(image_file);
            pictureBox_Coordinate.BackgroundImage = backgroundImage;
            pictureBox_Coordinate.BackgroundImageLayout = ImageLayout.Zoom;


        }

        private void button15_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //タイプ相性チャート画面を表示
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                       //toolStripStatusLabel__Fullpathにパスを記載する
            int decValue = Convert.ToInt32(textBox_TypeChart_Address.Text, 16);      //読込位置がテキストデータのため、16進数から10進数に再計算                           
            string inifolder = toolStripComboBox_Useinidata.Text;                    //iniの種類を把握
            string Max_typeNum = numericUpDown_TypeNum.Value.ToString();             //タイプ最大数

            Form4 form4 = new Form4(path, decValue, inifolder, Max_typeNum, this);
            form4.Show();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //チャートセーブボタン押下時
            //-------------------------------------------------------------------------------
            string apply_Address = textBox_TypeChart_Address.Text;

            DialogResult result = MessageBox.Show(
                "参照テーブルを " + apply_Address + " に更新し\r\n設定アドレスに変更内容を上書きしますか？",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK)
            {

                try
                {
                    Chart_Tab_Save();
                    MessageBox.Show("タイプチャートの上書きが完了しました.", "System message");

                    //更新ログに記載
                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "TypeChart",
                                                     label_Updatabefore_Chart.Text,
                                                     textBox_TypeChart_Address.Text,
                                                     DateTime.Now,
                                                     "詳細情報",
                                                     label_before_Script_TypeChart.Text,
                                                     richTextBox_type_Chart.Text
                                                     );
                    label_Updatabefore_Chart.Text = textBox_TypeChart_Address.Text;   //変更前アドレスの内容を更新
                    label_before_Script_TypeChart.Text = richTextBox_type_Chart.Text; //変更前スクリプトの内容を更新
                    label_Change_announce.Visible = false;
                }
                catch
                {
                    //Errorが発生した場合はメッセージボックスを表示して終了
                    MessageBox.Show("保存に失敗しました。\n入力できない文字列が記載されていないか確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //キャンセルの場合は何もしない
            }

        }

        private void Chart_Tab_Save()
        {
            //-------------------------------------------------------------------------------
            //チャートタブのセーブ処理
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;   //toolStripStatusLabel__Fullpathにパスを記載する
            string LoadSection = "DataBase"; //選択したiniのデータベースを取得する
            int TypeChartSelect = 0;

            string LoadText = "";
            int decValue = 0;

            for (int j = 1; j < 10; j++)
            {
                //9箇所あるTypeChartOffsetを全て変更させる処理
                var Loadkey = "TypeChart_Offset" + j;
                TypeChartSelect = MyGetPrivateProfileInt(LoadSection, Loadkey);
                LoadText = textBox_TypeChart_Address.Text;
                decValue = 0;

                if (LoadText == "")
                {
                    LoadText = "0";
                }

                decValue = Convert.ToInt32(LoadText, 16) + 134217728;    //読込値を10進数に変換してからROM領域番地を足す 
                LoadText = decValue.ToString("X").PadLeft(8, '0');       //16進数に直して8文字詰めに変換
                LoadText = LoadText + "00000000";                        //空欄があるとErrorになるので00で埋めておく


                LoadText = LoadText.Substring(6, 2) + " " + LoadText.Substring(4, 2) + " " +
                            LoadText.Substring(2, 2) + " " + LoadText.Substring(0, 2);

                var convert_val = String_exchange(LoadText);
                Write_binary_data(path, TypeChartSelect, 4, convert_val); //該当アドレスに4byte書き込み
            }

            //---------------------------------------------------------------------
            //チャート内容を反映
            var ChartText = String_exchange(richTextBox_type_Chart.Text);

            int ChartScript_Length = 0;
            ChartScript_Length = ChartText.Length;

            int ChartScript = 0;
            decValue = Convert.ToInt32(textBox_TypeChart_Address.Text, 16); //記載されたAddressはテキストなので10進数に戻す必要がある
            ChartScript = decValue;

            Write_binary_data(path, ChartScript, ChartScript_Length, ChartText);

        }
        private void Get_ini_FolderName()
        {
            toolStripComboBox_Useinidata.Items.Clear(); //過去に入っているdataがあったら困るので全消ししておく

            //setting以下の全てのフォルダ名を取得する
            string[] iniFileFolder = System.IO.Directory.GetDirectories(
                                     AppDomain.CurrentDomain.BaseDirectory + "Setting",
                                     "*", System.IO.SearchOption.AllDirectories);

            foreach (string name in iniFileFolder)
            {
                //Settingフォルダ内の全てのファイル名を取得して追加する
                string[] ininame = name.Split("\\");           //分割する
                string lastWord = ininame[ininame.Length - 1]; //最後の要素を取得する
                toolStripComboBox_Useinidata.Items.Add(lastWord);
            }
            toolStripComboBox_Useinidata.SelectedIndex = 0; //初期表示で最初のiniを読み込む
        }

        private void Get_LastReadFile()
        {
            //-------------------------------------------------------------------------------
            //最近使ったファイルリストを作成する処理
            //-------------------------------------------------------------------------------
            mun_RecentlyUsedFiles.DropDownItems.Clear(); //過去に入っているdataがあったら困るので全消ししておく

            for (int i = 1; i < 6; i++)
            {
                //iniファイルの内容をキャストして変数に格納
                string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\Referent_File.txt";    //iniからコードの型を取得 
                StringBuilder sb = new StringBuilder(1024);
                int res = GetPrivateProfileString("Last_read_path", "File_Full_Path" + i, "-", sb, (uint)sb.Capacity, iniFileName); //専用のテキストiniからデータを呼び出す
                string Get_LastReadFile_Pass = sb.ToString();

                if (Get_LastReadFile_Pass == "")
                {
                    //空欄の場合は追加しない
                    mun_RecentlyUsedFiles.DropDownItems.Add(i + ".");
                }
                else
                {
                    //フルパスからファイル名を取得
                    string GetFileName = Path.GetFileName(Get_LastReadFile_Pass);
                    mun_RecentlyUsedFiles.DropDownItems.Add(i + "." + GetFileName);
                }

                //読込んだパスをラベルに指定させる
                main_tag.TabPages["tabPage7"].Controls["label_Recently_ReadPath_" + i].Text = Get_LastReadFile_Pass;

            }
        }
        private void Output_LastReadFile()
        {
            //-------------------------------------------------------------------------------
            //読込んだパスを最近使ったファイルリスト用のiniに書き込む処理
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;
            string fp = Environment.CurrentDirectory.ToString();
            fp += "\\" + "Setting\\Referent_File.txt";
            int res;

            for (int j = 5; j > 0; j--)
            {
                string path_log = main_tag.TabPages["tabPage7"].Controls["label_Recently_ReadPath_" + j].Text;
                if (path == path_log)
                {
                    goto InfoSkip;

                }
            }

            //メイン書き込み処理
            for (int i = 5; i > 0; i--)
            {
                if (i == 1)
                {
                    res = WritePrivateProfileString("Last_read_path", "File_Full_Path" + i, path, fp);
                }
                else
                {
                    //5から遡って指定する,
                    string path_log = main_tag.TabPages["tabPage7"].Controls["label_Recently_ReadPath_" + (i - 1)].Text;
                    res = WritePrivateProfileString("Last_read_path", "File_Full_Path" + i, path_log, fp);
                }
            }

        InfoSkip:
            path = "";

        }
        private void mun_RecentlyUsedFiles_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //最近使用したファイルプルダウンを押した際の処理
            //-------------------------------------------------------------------------------
            var selectedIndex = e.ClickedItem.Text.Split(".");
            var selected_value = selectedIndex[0];
            var selected_path = selectedIndex[1];

            if (selected_path != "-")
            {
                //選択した値が無効の値じゃないことを確認して開く
                string path = main_tag.TabPages["tabPage7"].Controls["label_Recently_ReadPath_" + selected_value].Text;
                toolStripStatusLabel__Fullpath.Text = path;

                OpenFile(path); //ファイルを開く処理に進む
            }
        }

        private void Extract_tools_setting()
        {
            //-------------------------------------------------------------------------------
            //外部ツールのURLをiniから反映させる処理
            //-------------------------------------------------------------------------------
            //iniファイルの内容をキャストして変数に格納
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\Referent_File.txt";    //iniからコードの型を取得 
            StringBuilder sb = new StringBuilder(1024);
            int res = GetPrivateProfileString("Extract_tool_path", "Stirling_Path", "-", sb, (uint)sb.Capacity, iniFileName); //専用のテキストiniからデータを呼び出す
            string Get_ExtractTool_Pass = sb.ToString();

            textBox_StilringURL.Text = Get_ExtractTool_Pass;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //チャートアドレス読み込み先変更ボタン
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                                         //toolStripStatusLabel__Fullpathにパスを記載する
            int decValue = Convert.ToInt32(textBox_TypeChart_Address.Text, 16);                        //読込位置がテキストデータのため、16進数から10進数に再計算                           

            richTextBox_type_Chart.Text = Read_binary_toFF(decValue); //直前のAddress値からffまでのbinaryデータを取得する

        }

        private void dataGridView_Change_log_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //ログ詳細表示ボタン押下時の処理
            //-------------------------------------------------------------------------------
            if (e.ColumnIndex == 6)
            {
                try
                {
                    string path = toolStripStatusLabel__Fullpath.Text;                       //toolStripStatusLabel__Fullpathにパスを記載する                          
                    string inifolder = toolStripComboBox_Useinidata.Text;                    //iniの種類を把握
                    string Max_typeNum = numericUpDown_TypeNum.Value.ToString();             //タイプ最大数

                    int rowIndex = e.RowIndex;  //現在列を取得

                    string MoveID = dataGridView_Change_log.Rows[rowIndex].Cells[0].Value.ToString();
                    string MoveName = dataGridView_Change_log.Rows[rowIndex].Cells[1].Value.ToString();
                    string Tab = dataGridView_Change_log.Rows[rowIndex].Cells[2].Value.ToString();
                    string Before_address = dataGridView_Change_log.Rows[rowIndex].Cells[3].Value.ToString();
                    string After_address = dataGridView_Change_log.Rows[rowIndex].Cells[4].Value.ToString();

                    string Before_data = dataGridView_Change_log.Rows[rowIndex].Cells[7].Value.ToString();
                    string After_data = dataGridView_Change_log.Rows[rowIndex].Cells[8].Value.ToString();

                    Form5 form5 = new Form5(MoveID, MoveName, Tab, Before_address, After_address, Before_data, After_data);
                    form5.Show();
                }
                catch
                {

                }

            }

        }

        private void button_Extract_Updata_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //外部ツール参照・更新書き込み
            //-------------------------------------------------------------------------------
            string path = textBox_StilringURL.Text;
            //カレントディレクトリの取得は下記の文章で行う（リリース環境で参照がnullになることがある）
            string fp = Environment.CurrentDirectory.ToString();
            fp += "\\" + "Setting\\Referent_File.txt";
            int res;

            res = WritePrivateProfileString("Extract_tool_path", "Stirling_Path", path, fp);
        }

        private string Open_dialog(string beforePath)
        {
            //-------------------------------------------------------------------------------
            //ダイアログを呼び出して対象のファイルのパスを取得する
            //-------------------------------------------------------------------------------

            //参照先のパス名
            string path = "";

            //ダイアログ呼び出し
            //ファイルダイアログを生成する
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "ファイルを開く";
            op.InitialDirectory = @"C:\";
            //op.Filter = "rom files(*.rom; *.gba)| *.rom; *.gba|data files (*.txt;*.bin;*.dat)|*.txt;*.bin;*.dat";
            op.FilterIndex = 1;

            //オープンファイルダイアログを表示する
            DialogResult dialog = op.ShowDialog();

            //「開く」ボタンが選択された際の処理
            if (dialog == DialogResult.OK)
            {
                path = op.FileName;
            }
            //「キャンセル」時の処理
            else if (dialog == DialogResult.Cancel)
            {
                return (beforePath);
            }

            return (path);
        }

        private void button_URL_Search1_Click(global::System.Object sender, global::System.EventArgs e)
        {
            string beforePath = textBox_StilringURL.Text;
            //dialogからパスを取得する
            string GetPath = Open_dialog(beforePath);
            textBox_StilringURL.Text = GetPath;
        }

        private void mnu_Movedata_import_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //表示データのインポート
            //-------------------------------------------------------------------------------

            //参照先のパス名
            string path = "";

            //ダイアログ呼び出し
            //ファイルダイアログを生成する
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "ファイルを開く";
            op.InitialDirectory = @"C:\";
            op.Filter = "mse files(*.mse)| *.mse;";
            op.FilterIndex = 1;

            //オープンファイルダイアログを表示する
            DialogResult dialog = op.ShowDialog();

            //「開く」ボタンが選択された際の処理
            if (dialog == DialogResult.OK)
            {
                path = op.FileName;
            }
            //「キャンセル」時の処理
            else if (dialog == DialogResult.Cancel)
            {
                return;
            }

            mseFile_Open(path);

        }

        private void mseFile_Open(string path)
        {
            //-------------------------------------------------------------------------------
            //インポート時の部分だけの処理
            //-------------------------------------------------------------------------------
            toolStripProgressBar1.Value = 0;//プログレスバーを記載

            //選択したパスのデータを開く
            System.IO.StreamReader sr = new System.IO.StreamReader(@path, System.Text.Encoding.GetEncoding("UTF-8"));
            //内容をすべて読み込む
            string Get_mse = sr.ReadToEnd();
            //閉じる
            sr.Close();

            string[] mse_Data = Get_mse.Split(",");

            string MoveInfo = mse_Data[0];
            string[] info_data = MoveInfo.Split(" ");

            //テキストボックス項目更新
            //textBox_label_moveIDに反映する値を設定
            label_data_01.Text = info_data[0];
            textBox_label_moveID.Text = Convert.ToInt32(info_data[0], 16).ToString();

            //textBox_Movepowerに反映する値を設定
            label_data_02.Text = info_data[1];
            textBox_Movepower.Text = Convert.ToInt32(info_data[1], 16).ToString();

            //textBox_Accuracyに反映する値を設定
            label_data_04.Text = info_data[3];
            textBox_Accuracy.Text = Convert.ToInt32(info_data[3], 16).ToString();

            //textBox_PPに反映する値を設定
            label_data_05.Text = info_data[4];
            textBox_PP.Text = Convert.ToInt32(info_data[4], 16).ToString();

            //textBox_Probに反映する値を設定
            label_data_06.Text = info_data[5];
            textBox_Prob.Text = Convert.ToInt32(info_data[5], 16).ToString();

            //----------------------------------------------------------------
            //コンボボックス項目更新
            //comboBox_moveTypeに反映する値を設定
            label_data_03.Text = info_data[2];
            comboBox_moveType.SelectedIndex = Convert.ToInt32(info_data[2], 16);//comboBox_moveTypeに反映

            //comboBox_Attack_rangeに反映する値を設定
            label_data_07.Text = info_data[6];
            comboBox_Attack_range.SelectedIndex = bin_Getflg(Convert.ToString(Convert.ToInt32(info_data[6], 16), 2).PadLeft(8, '0'));

            //comboBox_Priorityに反映する値を設定
            label_data_08.Text = info_data[7];
            int get_flgnum = 0;
            switch (Convert.ToInt32(info_data[7], 16))
            {
                case 0:
                    get_flgnum = 0;
                    break;

                case 1:
                    get_flgnum = 1;
                    break;

                case 2:
                    get_flgnum = 2;
                    break;

                case 3:
                    get_flgnum = 3;
                    break;

                case 4:
                    get_flgnum = 4;
                    break;

                case 5:
                    get_flgnum = 5;
                    break;

                case 6:
                    get_flgnum = 6;
                    break;

                case 255:
                    get_flgnum = 7;
                    break;

                case 254:
                    get_flgnum = 8;
                    break;

                case 253:
                    get_flgnum = 9;
                    break;

                case 252:
                    get_flgnum = 10;
                    break;

                case 251:
                    get_flgnum = 11;
                    break;

                default:
                    get_flgnum = 12;
                    break;
            }   //ごり押しでフラグ設定
            comboBox_Priority.SelectedIndex = get_flgnum;

            //comboBox_Move_Categoryに反映する値を設定
            label_data_11.Text = info_data[10];
            comboBox_Move_Category.SelectedIndex = Convert.ToInt32(info_data[10], 16);

            //----------------------------------------------------------------
            //チェックボックス項目更新
            //Move_flagに反映する値を設定
            label_data_09.Text = info_data[8];
            (bool, bool, bool, bool, bool, bool, bool, bool) get_flag_tuple = get_flag(Convert.ToInt32(info_data[8], 16));
            checkBox_Moveflag_1.Checked = get_flag_tuple.Item1;
            checkBox_Moveflag_2.Checked = get_flag_tuple.Item2;
            checkBox_Moveflag_3.Checked = get_flag_tuple.Item3;
            checkBox_Moveflag_4.Checked = get_flag_tuple.Item4;
            checkBox_Moveflag_5.Checked = get_flag_tuple.Item5;
            checkBox_Moveflag_6.Checked = get_flag_tuple.Item6;
            checkBox_Moveflag_7.Checked = get_flag_tuple.Item7;
            checkBox_Moveflag_8.Checked = get_flag_tuple.Item8;

            //Ability_flag_1に反映する値を設定
            label_data_10.Text = info_data[9];
            get_flag_tuple = get_flag(Convert.ToInt32(info_data[9], 16));
            checkBox_Abilityflg1_1.Checked = get_flag_tuple.Item1;
            checkBox_Abilityflg1_2.Checked = get_flag_tuple.Item2;
            checkBox_Abilityflg1_3.Checked = get_flag_tuple.Item3;
            checkBox_Abilityflg1_4.Checked = get_flag_tuple.Item4;
            checkBox_Abilityflg1_5.Checked = get_flag_tuple.Item5;
            checkBox_Abilityflg1_6.Checked = get_flag_tuple.Item6;
            checkBox_Abilityflg1_7.Checked = get_flag_tuple.Item7;
            checkBox_Abilityflg1_8.Checked = get_flag_tuple.Item8;

            //Ability_flag_2に反映する値を設定
            label_data_12.Text = info_data[11];
            get_flag_tuple = get_flag(Convert.ToInt32(info_data[11], 16));
            checkBox_Abilityflg2_1.Checked = get_flag_tuple.Item1;
            checkBox_Abilityflg2_2.Checked = get_flag_tuple.Item2;
            checkBox_Abilityflg2_3.Checked = get_flag_tuple.Item3;
            checkBox_Abilityflg2_4.Checked = get_flag_tuple.Item4;
            checkBox_Abilityflg2_5.Checked = get_flag_tuple.Item5;
            checkBox_Abilityflg2_6.Checked = get_flag_tuple.Item6;
            checkBox_Abilityflg2_7.Checked = get_flag_tuple.Item7;
            checkBox_Abilityflg2_8.Checked = get_flag_tuple.Item8;

            toolStripProgressBar1.Value = 50;//プログレスバーを記載

            //----------------------------------------------------------------
            textBox_Move_Name.Text = mse_Data[1];
            textBox_Move_text.Text = mse_Data[2];

            label_Read_Script.Text = mse_Data[3];
            richTextBox_Effect_Script.Text = mse_Data[3];

            label_Animation_Script.Text = mse_Data[4];
            richTextBox_Animation_Script.Text = mse_Data[4];

            toolStripProgressBar1.Value = 100;//プログレスバーを記載
        }

        private void mnu_Movedata_export_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //表示データのエクスポート処理
            //-------------------------------------------------------------------------------
            MessageBox.Show("MSEファイルのエクスポートが完了しました.", "System message");
            try
            {
                string Export_data = "";
                Export_data += textBox_moveinfo.Text + ",";
                Export_data += textBox_Move_Name.Text + ",";
                Export_data += textBox_Move_text.Text + ",";
                Export_data += label_Read_Script.Text + ",";
                Export_data += label_Animation_Script.Text + ",";

                string FileName = AppDomain.CurrentDomain.BaseDirectory + textBox_Move_Name.Text + ".mse";

                //書き込むファイルが既に存在している場合は、上書きする
                System.IO.StreamWriter sw = new System.IO.StreamWriter(@FileName, false, System.Text.Encoding.GetEncoding("UTF-8"));

                sw.Write(Export_data);
                //閉じる
                sw.Close();

            }
            catch
            {
                //Errorが発生した場合はメッセージボックスを表示して終了
                MessageBox.Show("保存に失敗しました。\n入力できない文字列が記載されていないか確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void numericUpDown_Battle_Text_Num_ValueChanged(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //番号カウントボックスに指定データを書き込み
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;

            int decValue = int.Parse(numericUpDown_Battle_Text_Num.Text);
            int BattleText_Table = 0;
            string LoadText = "";

            string Text_Offset1 = label_Battle_Text_Hex_address1.Text;
            string Text_Offset2 = label_Battle_Text_Hex_address2.Text;


            string LoadSection = "DataBase";
            string inifolder = toolStripComboBox_Useinidata.Text;

            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Config.ini";

            try
            {
                //定型文最大数を読込
                int Max_BattleText_Number = 0;
                string Loadkey = "Max_BattleText_Number";
                Max_BattleText_Number = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

                //定型文Addressの開始位置を読み込み

                int minus_pointa = 0;
                if (decValue < 385) { Loadkey = "BattleText_Table1"; minus_pointa = 12; } else { Loadkey = "BattleText_Table2"; minus_pointa = 385; }
                BattleText_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

                //開始IDが12からなため減算する
                decValue = decValue - minus_pointa;

                //info_dataから読込む定型文オフセットを指定して読込先として組みなおす
                BattleText_Table = BattleText_Table + (4 * decValue);
                LoadText = Read_binary_data(path, BattleText_Table + 3, 1) + Read_binary_data(path, BattleText_Table + 2, 1) +
                           Read_binary_data(path, BattleText_Table + 1, 1) + Read_binary_data(path, BattleText_Table, 1); //無理やりビッグエンディアンにして表示
                textBox_Battle_Text.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16) - 134217728;

                //------------------------------------------------------------------------------------------

                Battle_Text_Convert(decValue);

            }
            catch
            {

            }




        }

        private void Battle_Text_Convert(int decValue)
        {
            //読み込んだ開始位置アドレスからFFが読み込まれるまでの値をテキストで返す        
            var code_data = "";
            var mass_code_data = "";
            var Apply_data = "";

            string path = toolStripStatusLabel__Fullpath.Text;
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";

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
                        mass_code_data += code_data + " ";
                        break;

                    case "fc":
                        code_data = string.Format("{0:x2}", data[i + 1]) + " " + string.Format("{0:x2}", data[i + 2]);
                        res = GetPrivateProfileString("MB_FC_kanji", code_data, "[未調査]", sb, (uint)sb.Capacity, iniFileName);//変数の一覧をiniから取得
                        Apply_data = Apply_data + sb.ToString();
                        mass_code_data += "fc " + code_data + " ";
                        i = i + 2; //2byte余計に進ませる
                        break;

                    case "fd":
                        code_data = string.Format("{0:x2}", data[i + 1]); //1byte先の値を取得する
                        res = GetPrivateProfileString("MB_FD_BattleText", code_data, "[未調査]", sb, (uint)sb.Capacity, iniFileName);//変数の一覧をiniから取得
                        Apply_data = Apply_data + sb.ToString();
                        mass_code_data += "fd " + code_data + " ";
                        i = i + 1; //1byte余計に進ませる
                        break;

                    case "fe":
                        Apply_data = Apply_data + sb.ToString() + "\r\n";
                        mass_code_data += code_data + " ";
                        break;

                    case "ff":
                        Apply_data = Apply_data + sb.ToString();
                        mass_code_data += code_data;
                        richTextBox_Convert_Text_info.Text = Apply_data;
                        richTextBox_Battle_Text.Text = mass_code_data.ToUpper().TrimEnd();
                        return; //終了文字列を見つけたら表記を終了させる

                    default:
                        Apply_data = Apply_data + sb.ToString();
                        mass_code_data += code_data + " ";
                        break;
                }


            }

        }

        private void listBox_FD_text_SelectedIndexChanged(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //FD変数リストボックスの選択を変えた際の処理
            //-------------------------------------------------------------------------------
            int Select_list_num = 0;
            int res;

            Select_list_num = listBox_FD_text.SelectedIndex;         //選択中の場所をリストを

            string code_data = Select_list_num.ToString().PadLeft(2, '0'); //2桁の文字列数字に変換
            code_data = Select_list_num.ToString("X").PadLeft(2, '0');     //2桁の16進数に変換する

            string inifolder = toolStripComboBox_Useinidata.Text;
            string code_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";    //iniからコードの型を取得 
            StringBuilder db = new StringBuilder(64);

            //Scriptコードの内訳を読み込み
            res = GetPrivateProfileString("MB_FD_BattleText", code_data, "読み込みエラー", db, (uint)db.Capacity, code_iniFileName);

            richTextBox_Convert_Text_info.Text += db + " ";

        }

        private void button22_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //定型文　スクリプトを日本語に変換
            //-------------------------------------------------------------------------------
            string LoadText = richTextBox_Battle_Text.Text.TrimEnd();
            LoadText = LoadText.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            LoadText = FontData_exchange(LoadText);
            richTextBox_Convert_Text_info.Text = LoadText + "Ω";

        }

        private void button23_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //定型文　日本語をスクリプトに変換
            //-------------------------------------------------------------------------------
            int Move_Text_length = richTextBox_Convert_Text_info.Text.Length;
            string str = richTextBox_Convert_Text_info.Text;
            string str_text = str.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            string str_FD = "";

            bool FD_flg = false;
            string FDPoint = "";

            foreach (char c in str)
            {
                switch (c)
                {
                    case '[':
                        //FDPoint += c;
                        FD_flg = true;
                        break;

                    case ']':
                        //FDPoint += c;

                        switch (FDPoint)
                        {
                            case "行送り":
                                str_FD += "FA";
                                break;

                            case "更新":
                                str_FD += "FE";
                                break;

                            default:
                                string[] FD_code = FDPoint.ToString().Split(':');
                                string number = FD_code[0];

                                str_FD += "FD " + number + " ";
                                break;

                        }

                        FD_flg = false;
                        FDPoint = "";
                        break;

                    default:

                        if (FD_flg == true)
                        {
                            FDPoint += c;
                        }
                        else
                        {
                            string get_font = c.ToString().Trim();
                            str = ByteData_exchange(get_font, 1) + " ";
                            str_FD += str;
                        }

                        break;
                }

            }
            richTextBox_Battle_Text.Text = str_FD.ToUpper();
        }

        private void mun_ini_maker_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //ini製作画面を表示させる
            //-------------------------------------------------------------------------------
            Form6 form6 = new Form6();
            form6.Show();

        }
    }
}
