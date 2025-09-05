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
        //GetPrivateProfileInt�֐��̐錾(ini�t�@�C���̓Ǎ�)
        //-------------------------------------------------------------------------------
        //ini����l��Ǎ�
        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileInt(
            string lpAppName,   //�Z�N�V������
            string lpKeyName,   //�L�[��
            int nDefault,       //ini�Ǎ����s���ɕԂ��l
            string lpFileName); //ini�t�@�C���̃p�X

        //ini����e�L�X�g��Ǎ�
        //�e�L�X�g�̕ϊ���CharSet.Auto�ɕϊ����K�v
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileString(
        string lpAppName_string,
        string lpKeyName_string,
        string lpDefault_string,
        StringBuilder lpReturnedString,
        uint nSize,
        string lpFileName_string);

        //�e�L�X�g��ini�ɏ�������
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
            //PC�ݒ薈�ɕ\���T�C�Y���ω����邱�Ƃ�h��
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            //ini�t�H���_�I���ꗗ���쐬���Ă���
            Get_ini_FolderName();
            //StatusStrip�̃v���O���X�o�[���\�����c���ɂȂ�Ȃ��悤�ɂ���ɂ�Layout style��[Flow]�ɂ���

            //�Ō�ɓǂݍ��񂾃t�@�C���̃p�X��ini��Ǎ���ł���
            Get_LastReadFile();

        }
        private void mnu_Save_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�S���Z�߂ăZ�[�u����
            //-------------------------------------------------------------------------------
            //���b�Z�[�W�{�b�N�X��\������
            DialogResult result = MessageBox.Show("�S�Ă̍��ڂ̕ύX�𔽉f���܂����H",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //�����I�����ꂽ�����ׂ�
            if (result == DialogResult.Yes)
            {

                Move_effect_Save();      //�Z���ʏ�������
                Move_animation_Save();   //�Z���ʏ�������
                Move_info_Save();        //�Z�A�j����������
                Move_textPattern_Save(); //�Z�\���p�^�[����������
                Chart_Tab_Save();        //�^�C�v�`���[�g��������
                MessageBox.Show("�S���X�V������Ɋ������܂���.", "System message");
            }
            else if (result == DialogResult.Cancel)
            {
                //�L�����Z���̏ꍇ�͉������Ȃ�
            }

        }
        private void mnu_Exit_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�c�[�����I������
            //-------------------------------------------------------------------------------
            this.Close();

        }

        private void mnu_FileOpen_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�_�C�A���O���Ăяo���đΏۂ̃t�@�C���̃p�X���擾����
            //-------------------------------------------------------------------------------

            //�Q�Ɛ�̃p�X��
            var path = "";

            //�_�C�A���O�Ăяo��
            //�t�@�C���_�C�A���O�𐶐�����
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "�t�@�C�����J��";
            op.InitialDirectory = @"C:\";
            op.Filter = "rom files(*.rom; *.gba)| *.rom; *.gba|data files (*.txt;*.bin;*.dat)|*.txt;*.bin;*.dat";
            op.FilterIndex = 1;

            //�I�[�v���t�@�C���_�C�A���O��\������
            DialogResult dialog = op.ShowDialog();

            //�u�J���v�{�^�����I�����ꂽ�ۂ̏���
            if (dialog == DialogResult.OK)
            {
                path = op.FileName;
            }
            //�u�L�����Z���v���̏���
            else if (dialog == DialogResult.Cancel)
            {
                return;
            }

            //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            toolStripStatusLabel__Fullpath.Text = path;
            OpenFile(path);        //�t�@�C�����J�������ɐi��

        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //���C���t�H�[���Ƀt�@�C�����h���b�O&�h���b�v���ꂽ�ۂɂ��J��
            //-------------------------------------------------------------------------------
            e.Effect = DragDropEffects.All; // �h���b�O�h���b�v���ɃJ�[�\���̌`���ύX
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {

            // �t�@�C�����n����Ă��Ȃ���΁A�������Ȃ�
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            // �n���ꂽ�t�@�C���ɑ΂��ď������s��
            foreach (var path in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                OpenFile(path);//�t�@�C�����J�������ɐi��
            }
        }

        private void OpenFile(string path)
        {
            //-------------------------------------------------------------------------------
            //�t�@�C�����Ăяo����
            //-------------------------------------------------------------------------------
            mnu_Save.Enabled = true;    //�S�Z�[�u���j���[��I���\�ɂ���

            string Ext = Path.GetExtension(path);
            string Base = Path.GetFileNameWithoutExtension(path);

            switch (Ext)
            {
                case ".gba" or ".rom":
                    //rom��ǂݍ��ޏꍇ

                    //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
                    toolStripStatusLabel__Fullpath.Text = path;

                    //textBox_Move_Number�ɔ��f����l��ݒ�
                    button1.Enabled = true;     //�X�N���v�g���ʃr���_�[�{�^��
                    button2.Enabled = true;     //�X�N���v�g��񏑂����݃{�^��
                    button3.Enabled = true;     //�����[�h�{�^��
                    button4.Enabled = true;     //���ʃX�N���v�g�ǂݍ��ݐ�\���X�V�{�^��
                    button5.Enabled = true;     //�Z��񏑂����݃{�^��
                    button6.Enabled = true;     //�Z�\���p�^�[���������݃{�^��
                    button7.Enabled = true;     //�X�N���v�g�ڍ׃E�B���h�E�\���{�^��
                    button11.Enabled = true;    //�Z�ꗗ���X�g�\���{�^��
                    button12.Enabled = true;    //�Z�G�t�F�N�g�������݃{�^��
                    button13.Enabled = true;    //�A�j���X�N���v�g�ǂݍ��ݐ�\���X�V�{�^��
                    button14.Enabled = true;    //�Z�G�t�F�N�g�ڍ׃E�B���h�E�\���{�^��
                    button15.Enabled = true;    //�^�C�v�����`���[�g�E�B���h�E�\���{�^��
                    button16.Enabled = true;    //Other�^�u�������݃{�^��
                    button17.Enabled = true;    //�X�N���v�g�A�j���[�V�����r���_�[�{�^��
                    button18.Enabled = true;    //�`���[�g�Ǎ���X�V�{�^��
                    button19.Enabled = true;    //�A�h���X�ꗗ��ʁiForm3���ė��p�j��\��
                    button20.Enabled = true;    //�e�L�X�g�ꗗ��ʁiForm3���ė��p�j��\��
                    button21.Enabled = true;    //��^����񏑂����݃{�^��
                    button22.Enabled = true;    //��^�� �X�N���v�g �� ���{��
                    button23.Enabled = true;    //��^�� ���{�� �� �X�N���v�g
                    button_Extract_Updata.Enabled = true;

                    numericUpDown_Move_Number.Enabled = true; //�Z�i���o�[�؂�ւ���������悤�ɂȂ�
                    numericUpDown_TypeNum.Enabled = true;     //�^�C�v����button

                    string LoadSection = "DataBase";                                //Ini�t�@�C���̃Z�N�V����
                    string Loadkey = "Load_MoveNumber";                              //Ini�t�@�C���̃L�[
                    int Load_Num = MyGetPrivateProfileInt(LoadSection, Loadkey);     //��p�֐�����ini�t�@�C�����̃f�[�^���擾����
                    numericUpDown_Move_Number.Text = Load_Num.ToString(); //�Ǎ����e�𔽉f������
                    label_Move_Number.Text = Load_Num.ToString();

                    Extract_tools_setting(); //�O���c�[����URL�𔽉f
                    Output_LastReadFile(); //�ŋߓǍ��񂾃t�@�C���̍X�V���s��
                    Main_Load_fanction();   //URL�̓ǂݍ��݂����������烁�C���̏������Ăяo��

                    label_Change_announce.Visible = false; //���f�������e�L�X�g�����Z�b�g
                    toolStripComboBox_Useinidata.Enabled = false; //ini�I�����Œ肳���悤�ɂ���
                    mnu_Movedata_import.Enabled = true; //�C���|�[�g���I���ł���悤�ɂ���
                    mnu_Movedata_export.Enabled = true;
                    break;

                case ".mse":

                    if (textBox_moveinfo.Text == "")
                    {
                        MessageBox.Show("�t�@�C���̓Ǎ��Ɏ��s���܂����B\nrom�f�[�^�Ǎ��O��mse�t�@�C����ǂݍ��ނ��Ƃ͂ł��܂���B", "���̓G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    mseFile_Open(path);

                    break;

                default:
                    //bin File�Ȃǂ�ǂݍ��񂾏ꍇ�̏���

                    //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
                    toolStripStatusLabel__Fullpath.Text = path;

                    //�Ǎ��t�@�C���̑傫�����擾
                    FileInfo file = new FileInfo(path);
                    string size = file.Length.ToString();

                    toolStripTextBox_version.Text = "Read data : " + Base;

                    //�X�N���v�g�P�̂�ǂޏꍇ
                    //Effect
                    button2.Enabled = true;
                    numericUpDown_Effect_Address_length.Enabled = true;
                    button7.Enabled = true;

                    textBox_Effect_address.Text = "00000000";
                    numericUpDown_Effect_Address_length.Value = int.Parse(size);

                    string LoadText = Read_binary_data(path, 1, int.Parse(size));
                    label_Read_Script.Text = LoadText; //���񔽉f���Ȃ����ߒ��ڎw��
                    richTextBox_Effect_Script.Text = LoadText;

                    //animation
                    button12.Enabled = true;
                    numericUpDown_Animation_Address_length.Enabled = true;
                    button14.Enabled = true;

                    textBox_Animation_address.Text = "00000000";
                    numericUpDown_Animation_Address_length.Value = int.Parse(size);
                    label_Animation_Script.Text = LoadText;//���񔽉f���Ȃ����ߒ��ڎw��
                    richTextBox_Animation_Script.Text = LoadText;

                    //2�Ԗڂ̃^�u��I��
                    main_tag.SelectedIndex = 1;

                    //Animation,effect�̏��{�b�N�X�ɃX�N���v�g�p�^�[�����L��
                    Extract_tools_setting(); //�O���c�[����URL�𔽉f
                                             //Output_LastReadFile();   //�ŋߓǍ��񂾃t�@�C���̍X�V���s��
                    Apply_Script_pattern();

                    break;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�����[�h�{�^�����������ۂɍēǂݍ��݂���
            //-------------------------------------------------------------------------------
            Main_Load_fanction();   //URL�̓ǂݍ��݂����������烁�C���̏������Ăяo��
            label_Change_announce.Visible = false; //���f�������e�L�X�g�����Z�b�g
        }


        private void Display_label_Change(string Cartridge_Data)
        {
            //https://codelikes.com/csharp-list/
            //https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/file-system/how-to-read-a-text-file-one-line-at-a-time
            string inifolder = toolStripComboBox_Useinidata.Text;

            int counter = 0;
            if (Cartridge_Data == "BPRJ")                                                       //���{�ꃍ���̏ꍇ�̓e�L�X�g�{�b�N�X����{�ꉻ
            {
                string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\label_name.ini"; //�e�L�X�g��ǂݍ���
                List<string> get_label_value = new List<string>();                                  //�i�[�p���X�g���쐬
                foreach (string line in System.IO.File.ReadLines(iniFileName))                      //���X�g�ϐ��Ƀe�L�X�g�̋L�ړ��e���P�s����������
                {
                    get_label_value.Add(line);
                    counter++;
                }

                //Move Data�^�u
                label_Move_Number_sub.Text = get_label_value[1];   //�ZNo.
                label_Move_Name_sub.Text = get_label_value[2];   //�Z��
                label_moveinfo_sub.Text = get_label_value[3];   //�Z���

                label_moveID_sub.Text = get_label_value[5];   //�ZID
                label_Movepower_sub.Text = get_label_value[6];   //�З�
                label_PP_sub.Text = get_label_value[7];   //�ZPP
                label_Accuracy_sub.Text = get_label_value[8];   //������
                label_Prob_sub.Text = get_label_value[9];   //�ǉ�����

                label_moveType_sub.Text = get_label_value[10];�@//�^�C�v
                label_Move_Category.Text = get_label_value[11];  //����
                label_Attack_range_sub.Text = get_label_value[12];  //�͈�
                label_Priority_sub.Text = get_label_value[13];  //�D��x

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

                label_Move_text_sub.Text = get_label_value[42];    //�}�Ӑ�����
                label_Move_text_remain_sub.Text = get_label_value[43];

                //Script Info�^�u
                labell_moveID_sub2.Text = get_label_value[5];
                labell_moveID_sub3.Text = get_label_value[5];
                label_Read_address_sub.Text = get_label_value[45] + "�I�t�Z�b�g"; //�Z���ʃX�N���v�g�A�h���X
                label_Move_Address_length_sub.Text = get_label_value[46];

                //Anime Info�^�u
                label_Effect_address_sub.Text = get_label_value[48] + "�I�t�Z�b�g";
                label_Move_Address_length_sub2.Text = get_label_value[49];

                //Move text Pattern�^�u
                label_movetxt_pattern_sub.Text = get_label_value[51] + "�I�t�Z�b�g";
                label_movetxt_mode_sub.Text = get_label_value[52];
                label_Select_Pattern_sub.Text = get_label_value[53];
                label_Movetext_sub1.Text = get_label_value[54];
                label_Movetext_sub2.Text = get_label_value[55];
                label_Movetext_sub3.Text = get_label_value[56];
                label_Movetext_sub4.Text = get_label_value[57];

                //Type Chart�^�u
                label_TypeChart_Address.Text = get_label_value[59] + "�I�t�Z�b�g";
                label_TypeChart_Mode.Text = get_label_value[60];
                label_Type_num_sub.Text = get_label_value[61];

                //Battle text�^�u
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
            //ini�t�@�C������f�[�^���擾����(int)
            //-------------------------------------------------------------------------------

            // https://se-naruhodo.com/paramini/
            // ini�t�@�C�����擾����i���s�t�@�C���{ini�t�@�C�����ō쐬����j
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Config.ini";

            //�ϐ��錾             
            int intDefault = 0;                �@//�l���擾�ł��Ȃ������ꍇ�̏����l
            string strFilePath = iniFileName;  �@//ini�t�@�C���̃p�X
            int StartLen;                        //���̃��\�b�h�̖߂�l�p

            //ini�t�@�C���̓��e���L���X�g���ĕϐ��Ɋi�[
            StartLen = (int)GetPrivateProfileInt(Loadsection, Loadkey, intDefault, strFilePath);

            return StartLen;
        }
        private void Main_Load_fanction()
        {
            //-------------------------------------------------------------------------------
            //ini�t�@�C������ϐ��ɒl��n��
            //-------------------------------------------------------------------------------
            try
            {
                toolStripProgressBar1.Value = 0;//�v���O���X�o�[���L��

                //�Ō�ɓǂݍ��񂾃t�@�C���̃p�X��ini��Ǎ���ł���
                Get_LastReadFile();

                //�ϐ����w��
                string path = "";
                string LoadSection = "DataBase";
                string Loadkey = "";
                string LoadText = "";
                int decValue = 0;
                string binValue = "";
                int get_flgnum = 0;
                bool get_lslnum;

                //label_Fullpath����p�X���擾����
                path = toolStripStatusLabel__Fullpath.Text;

                int Load_Num = int.Parse(label_Move_Number.Text);                      //����ǂݍ��ݎ���ini����ǂ񂾃e�L�X�g���Q�Ƃ���

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
                //�ZScript�͋Z���̋Z����ID����v�Z����̂ł����ł̓e�[�u���͋��߂Ȃ�

                int MoveInfo_Table_Length = 0;
                Loadkey = "MoveInfo_Table_Length";
                MoveInfo_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

                int MoveInfo_Table = 0;
                Loadkey = "MoveInfo_Table";
                MoveInfo_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
                MoveInfo_Table = MoveInfo_Table + (Load_Num * MoveInfo_Table_Length);      //�ǂݍ��݃e�[�u���ʒu���Y���ԍ��̂��̂ɍČv�Z����

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

                toolStripProgressBar1.Value = 10;//�v���O���X�o�[���L��

                //-------------------------------------------------------------------------------
                //�Ǎ��f�[�^�����[�h���ċL�ڂ���y�e�L�X�g�{�b�N�X�E���x���z
                //-------------------------------------------------------------------------------

                //�y1�y�[�W�ځz
                //label_version�ɔ��f����l��ݒ�
                LoadText = Read_binary_data(path, Cartridge_Data, Cartridge_Length);   //�ǂݍ��񂾒l���e�L�X�g�{�b�N�X�ɔ��f�����鏈��
                LoadText = binary_exchange(LoadText);                                  //�o�C�i���������ASCII�ɕϊ�
                Display_label_Change(LoadText);                                        //���{�ꃍ���̏ꍇ�̓��x���������������s
                toolStripTextBox_version.Text = "Rom version : " + LoadText;           //�Ǎ����e�𔽉f������

                //textBox_moveinfo�ɔ��f����l��ݒ�
                LoadText = Read_binary_data(path, MoveInfo_Table, MoveInfo_Table_Length);
                textBox_moveinfo.Text = LoadText;
                label_before_Script_MoveInfo.Text = LoadText;
                label_Updatabefore_MoveInfo.Text = MoveInfo_Table.ToString("X").PadLeft(8, '0'); //�ύX�O�A�h���X��16�i���ɕϊ����ēo�^

                //�ZID�̏�����button1��������̏����Ɉړ����Ă܂�

                //textBox_label_Move_Name�ɔ��f����l��ݒ�
                textBox_Move_Name.Enabled = true;
                LoadText = Read_binary_data(path, MoveName_Table, MoveName_Table_Length);
                label_Move_Name.Text = LoadText;
                LoadText = FontData_exchange(LoadText);
                textBox_Move_Name.Text = LoadText;
                label_before_Script_MoveName.Text = LoadText; //�ύX�O���O��������L��
                //�Z���̓ǂݍ��ݕ����������x���ɋL��
                label_Name_length.Text = MoveName_Table_Length.ToString();

                //textBox_Movepower�ɔ��f����l��ݒ�
                textBox_Movepower.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table + 1, 1);
                label_data_02.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                textBox_Movepower.Text = decValue.ToString();

                //textBox_Accuracy�ɔ��f����l��ݒ�
                textBox_Accuracy.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table + 3, 1);
                label_data_04.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                textBox_Accuracy.Text = decValue.ToString();

                //textBox_PP�ɔ��f����l��ݒ�
                textBox_PP.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table + 4, 1);
                label_data_05.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                textBox_PP.Text = decValue.ToString();

                //textBox_Prob�ɔ��f����l��ݒ�
                textBox_Prob.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table + 5, 1);
                label_data_06.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                textBox_Prob.Text = decValue.ToString();

                //�������̓ǂݍ��ݕ����������x���ɋL��
                label_Move_text_length.Text = MoveText_Table_Length.ToString();
                //textBox_Move_text�ɔ��f����l��ݒ�
                textBox_Move_text.Enabled = true;
                textBox_Move_text_code.Enabled = true;
                LoadText = Read_binary_data(path, MoveText_Table, MoveText_Table_Length);
                LoadText = FontData_exchange(LoadText);
                textBox_Move_text.Text = LoadText;
                label_before_Script_MoveText.Text = LoadText; //�X�V�O��������ݒ�

                //textBox_label_moveID�ɔ��f����l��ݒ�
                textBox_label_moveID.Enabled = true;
                LoadText = Read_binary_data(path, MoveInfo_Table, 1);                     //ini����@1byte�ڂ��擾
                label_data_01.Text = LoadText;                                           //���l�ێ����x���ɒl���L��  
                decValue = Convert.ToInt32(LoadText, 16);                                //16�i����10�i���ɕϊ����ĕ\���������̂�
                textBox_label_moveID.Text = decValue.ToString();                         //d2�ƋL�ڂ��邱�ƂŕW���̐��l�����w�肪�ł��� 
                labell_moveID2.Text = decValue.ToString();                               //�^�u2�y�[�W�ڂ�ID�\�����݂邽�߂̔��f
                labell_moveID3.Text = decValue.ToString();                               //�^�u3�y�[�W�ڂ�ID�\�����݂邽�߂̔��f

                string Loadini = toolStripComboBox_Useinidata.Text;
                string iniName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + Loadini + "\\Effect_ID_info.ini";
                StringBuilder sb = new StringBuilder(1024);
                int IDinfo = GetPrivateProfileString("Effect", LoadText, "�Ǎ��G���[", sb, (uint)sb.Capacity, iniName); //��p�̃e�L�X�gini����f�[�^���Ăяo��
                string Load_ID = sb.ToString().ToUpper(); //������̉p�ꕶ����啶���ɕύX
                label_ID_info_text.Text = Load_ID;

                //�y2�y�[�W�ځz
                //textBox_Read_address�ɔ��f����l��ݒ�
                textBox_Effect_address.Enabled = true;
                Effect_Table = Effect_Table + (decValue * Effect_Table_Length);             //����ID�Ŋ|���Z����K�v���L�邽�߁A������table�l���v�Z
                LoadText = Read_binary_data(path, Effect_Table, Effect_Table_Length);
                label_Read_address.Text = LoadText;
                LoadText = Read_binary_data(path, Effect_Table + 3, 1) + Read_binary_data(path, Effect_Table + 2, 1) +
                           Read_binary_data(path, Effect_Table + 1, 1) + Read_binary_data(path, Effect_Table, 1); //�������r�b�O�G���f�B�A���ɂ��ĕ\��

                decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                LoadText = decValue.ToString("X").PadLeft(8, '0');
                textBox_Effect_address.Text = LoadText;
                label_Updatabefore_Effect.Text = LoadText; //�ύX�O�A�h���X���i�[

                //richtextBox_Read_Script�ɔ��f����l�̐ݒ�
                numericUpDown_Effect_Address_length.Enabled = true;
                decValue = Convert.ToInt32(LoadText, 16);                                               //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z
                int Address_length = int.Parse(numericUpDown_Effect_Address_length.Value.ToString());   //�ǂݍ���byte���l���擾�i�f�t�H���g�ł�100�j
                LoadText = Read_binary_data(path, decValue, Address_length);
                label_Read_Script.Text = LoadText;
                richTextBox_Effect_Script.Text = LoadText;                                              //�L�ڗpScript�f�[�^�����x���ɋL��
                label_before_Script_Effect.Text = LoadText;                                             //�ύX�O�̃X�N���v�g���e���i�[


                //�y3�y�[�W�ځz
                //textBox_Effect_address�ɔ��f����l��ݒ�
                textBox_Animation_address.Enabled = true;
                LoadText = Read_binary_data(path, Animation_Table, Animation_Table_Length);             //�A�j����ID���̂���table�쐬�́��ōs��
                label_Animation_address.Text = LoadText;
                LoadText = Read_binary_data(path, Animation_Table + 3, 1) + Read_binary_data(path, Animation_Table + 2, 1) +
                           Read_binary_data(path, Animation_Table + 1, 1) + Read_binary_data(path, Animation_Table, 1); //�������r�b�O�G���f�B�A���ɂ��ĕ\��

                decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                LoadText = decValue.ToString("X").PadLeft(8, '0');
                textBox_Animation_address.Text = LoadText;
                label_Updatabefore_Animation.Text = LoadText; //�ύX�O�A�h���X���i�[

                //richtextBox_Read_Script�ɔ��f����l�̐ݒ�
                numericUpDown_Animation_Address_length.Enabled = true;
                decValue = Convert.ToInt32(LoadText, 16);                                               //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z
                int Effect_length = int.Parse(numericUpDown_Animation_Address_length.Value.ToString()); //�ǂݍ���byte���l���擾�i�f�t�H���g�ł�100�j
                LoadText = Read_binary_data(path, decValue, Effect_length);
                label_Animation_Script.Text = LoadText;
                richTextBox_Animation_Script.Text = LoadText;                                           //�L�ڗpScript�f�[�^�����x���ɋL��
                label_before_Script_Animation.Text = LoadText;                                          //�ύX�O�̃X�N���v�g���e���i�[


                //�y4�y�[�W�ځz
                //textBox__movetxt_pattern�ɔ��f����l��ݒ�
                LoadText = Read_binary_data(path, TextPattern_Select, 4);
                LoadText = Read_binary_data(path, TextPattern_Select + 3, 1) + Read_binary_data(path, TextPattern_Select + 2, 1) +
                           Read_binary_data(path, TextPattern_Select + 1, 1) + Read_binary_data(path, TextPattern_Select, 1); //�������r�b�O�G���f�B�A���ɂ��ĕ\��

                decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                LoadText = decValue.ToString("X").PadLeft(8, '0');
                textBox__movetxt_pattern.Text = LoadText;
                label_Updatabefore_Pattern.Text = LoadText; //�ύX�O�A�h���X���i�[

                Read_Move_binary_2byte(path, decValue); //���X�g�{�b�N�X�ɒl���L�ڂ�����

                //�y5�y�[�W�ځz
                //textBox_TypeChart_Address�ɔ��f����l��ݒ�
                LoadText = Read_binary_data(path, TypeChart_Offset, 4);
                LoadText = Read_binary_data(path, TypeChart_Offset + 3, 1) + Read_binary_data(path, TypeChart_Offset + 2, 1) +
                           Read_binary_data(path, TypeChart_Offset + 1, 1) + Read_binary_data(path, TypeChart_Offset, 1); //�������r�b�O�G���f�B�A���ɂ��ĕ\��

                decValue = Convert.ToInt32(LoadText, 16) - 134217728;
                LoadText = decValue.ToString("X").PadLeft(8, '0');
                textBox_TypeChart_Address.Text = LoadText;
                label_Updatabefore_Chart.Text = LoadText;      //�ύX�O�A�h���X���i�[

                //richTextBox_type_Chart�ɔ��f����l��ݒ�
                LoadText = Read_binary_toFF(decValue);         //���O��Address�l����ff�܂ł�binary�f�[�^���擾����
                label_before_Script_TypeChart.Text = LoadText; //�ύX�O�̃X�N���v�g���e���i�[
                richTextBox_type_Chart.Text = LoadText;

                //numericUpDown_TypeNum�ɔ��f����l��ݒ�
                numericUpDown_TypeNum.Text = Type_MaxNum.ToString();

                //�y8�y�[�W�ځz
                numericUpDown_Battle_Text_Num.Enabled = true;
                textBox_Battle_Text.Enabled = true;

                //�\�ߒl�����Z���ĕ\��������
                numericUpDown_Battle_Text_Num.Value += 1;

                //Max_Battle_Text_Count�ɔ��f����l�̐ݒ�
                label_Max_Battle_Text_Count.Text = Max_BattleText_Number.ToString();

                label_Battle_Text_Hex_address1.Text = BattleText_Table1.ToString();
                label_Battle_Text_Hex_address2.Text = BattleText_Table2.ToString();


                toolStripProgressBar1.Value = 30;//�v���O���X�o�[���L��

                //-------------------------------------------------------------------------------
                //�Ǎ��f�[�^�����[�h���ċL�ڂ���y�R���{�{�b�N�X�z
                //-------------------------------------------------------------------------------
                string inifolder = toolStripComboBox_Useinidata.Text;
                string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\Base_data.ini";
                StringBuilder db = new StringBuilder(2048);
                int res;

                //comboBox_moveType�ɔ��f����l��ݒ�
                comboBox_moveType.Enabled = true;                   //�����l�̎��͑���s�Ȃ̂ő���\�ɂ���
                comboBox_moveType.Items.Clear();
                for (int i = 0; i < Type_MaxNum; i++)
                {
                    string x = (i.ToString("X").PadLeft(2, '0'));   //int.Parse("123"),2���ɂ��Ȃ��ƃo�O��̂�
                    res = GetPrivateProfileString("type", x, "�ǂݍ��݃G���[", db, (uint)db.Capacity, iniFileName);
                    comboBox_moveType.Items.Add(db); //comboBox_moveType�ɒl��ǉ�
                }
                LoadText = Read_binary_data(path, MoveInfo_Table + 2, 1);
                label_data_03.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);                      //�ǂݍ���1byte��10�i���ɕϊ�
                comboBox_moveType.SelectedIndex = decValue;                        //comboBox_moveType�ɔ��f

                //comboBox_Attack_range�ɔ��f����l��ݒ�
                comboBox_Attack_range.Enabled = true;
                Dictionary<string, string> Attack_range_list = new Dictionary<string, string>()
                {
                    { "�ʏ�" , "00" },
                    { "�Ώەs��" , "01" },
                    { "�~ ���g�p" , "02" },
                    { "�����^�[���Z" , "04" },
                    { "����2��" , "08" },
                    { "���� or ��S��" , "10" },
                    { "�����ȊO" , "20" },
                    { "�ݒu�Z" , "40" },
                };
                LoadText = Read_binary_data(path, MoveInfo_Table + 6, 1);
                label_data_07.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                binValue = Convert.ToString(decValue, 2).PadLeft(8, '0');               //16�i������x10�i���ɒ����Ă���8����2�i���ɕϊ�

                get_flgnum = bin_Getflg(binValue);

                comboBox_Attack_range.DisplayMember = "Key";
                comboBox_Attack_range.ValueMember = "Value";
                comboBox_Attack_range.DataSource = Attack_range_list.ToList();
                comboBox_Attack_range.SelectedIndex = get_flgnum;

                //comboBox_Priority�ɔ��f����l��ݒ�
                comboBox_Priority.Enabled = true;
                Dictionary<string, string> Priority_list = new Dictionary<string, string>()
                {
                    { "�D��x �}0" , "00" },
                    { "�D��x +1"  , "01" },
                    { "�D��x +2"  , "02" },
                    { "�D��x +3"  , "03" },
                    { "�D��x +4"  , "04" },
                    { "�D��x +5"  , "05" },
                    { "�D��x +6"  , "06" },
                    { "�D��x -1"  , "FF" },
                    { "�D��x -2"  , "FE" },
                    { "�D��x -3"  , "FD" },
                    { "�D��x -4"  , "FC" },
                    { "�D��x -5"  , "FB" },
                    { "�D��x -6"  , "FA" },
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
                }   //���艟���Ńt���O�ݒ�

                comboBox_Priority.DisplayMember = "Key";
                comboBox_Priority.ValueMember = "Value";
                comboBox_Priority.DataSource = Priority_list.ToList();
                comboBox_Priority.SelectedIndex = get_flgnum;

                //comboBox_Move_Category�ɔ��f����l��ݒ�
                comboBox_Move_Category.Enabled = true;
                Dictionary<string, string> Category_list = new Dictionary<string, string>()
                {
                    { "�����˖h��" , "00" },
                    { "�����˓��h"  , "01" },
                    { "����˖h��"  , "02" },
                    { "����˓��h"  , "03" },
                    { "�ω��Z"  , "04" },
                    { "���g�p1"  , "05" },
                    { "���g�p2"  , "06" },
                    { "���g�p3"  , "07" },
                    { "�̂낢"  , "08" },
                };
                LoadText = Read_binary_data(path, MoveInfo_Table + 10, 1);
                label_data_11.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);
                get_flgnum = decValue;

                comboBox_Move_Category.DisplayMember = "Key";
                comboBox_Move_Category.ValueMember = "Value";
                comboBox_Move_Category.DataSource = Category_list.ToList();
                comboBox_Move_Category.SelectedIndex = get_flgnum;
                toolStripProgressBar1.Value = 50;//�v���O���X�o�[���L��    

                //textBox_movetxt_length�ɔ��f����l��ݒ�
                comboBox_movetxt_mode.Enabled = true;
                Dictionary<string, string> Move_text_list = new Dictionary<string, string>()
                {
                    { "�g���Ȃ�" , "B1" },
                    { "�g������" , "FF" },
                };
                LoadText = Read_binary_data(path, TextPattern_Number, 1);
                decValue = Convert.ToInt32(LoadText, 16);
                switch (decValue)
                {
                    case 177:
                        get_flgnum = 0;
                        break;

                    default:
                        textBox__movetxt_pattern.Enabled = true;    //�g�����I���ɂȂ��ĂȂ��ꏊ�ő��삳���Ȃ��悤�ɂ��邽�߂̕���
                        comboBox_Select_Pattern.Enabled = true;
                        button8.Enabled = true;                     //�\���p�^�[���\�L���f�{�^��
                        button9.Enabled = true;                     //�Z�\���p�^�[���ǉ��{�^��
                        button10.Enabled = true;                    //�Z�\���p�^�[���폜�{�^��
                        get_flgnum = 1;
                        break;
                }
                comboBox_movetxt_mode.DisplayMember = "Key";
                comboBox_movetxt_mode.ValueMember = "Value";
                comboBox_movetxt_mode.DataSource = Move_text_list.ToList();
                comboBox_movetxt_mode.SelectedIndex = get_flgnum;

                //comboBox_Select_Pattern�ɔ��f����l��ݒ�                
                Dictionary<string, string> Text_pattern_list = new Dictionary<string, string>()
                {
                    { "�p�^�[��1" , "1" },
                    { "�p�^�[��2" , "2" },
                    { "�p�^�[��3" , "3" },
                    { "�p�^�[��4" , "4" },
                    { "�Y���Ȃ�" , "5" },
                };
                get_flgnum = int.Parse(label_Select_Pattern_result.Text);
                comboBox_Select_Pattern.DisplayMember = "Key";
                comboBox_Select_Pattern.ValueMember = "Value";
                comboBox_Select_Pattern.DataSource = Text_pattern_list.ToList();
                comboBox_Select_Pattern.SelectedIndex = get_flgnum - 1;

                //comboBox_SelectBG�ɔ��f����l��ݒ�
                comboBox_SelectBG.Enabled = true;
                Dictionary<string, string> Select_BG_list = new Dictionary<string, string>()
                {
                    { "���w��" , "00" },
                    { "�V���O��" , "01" },
                    { "�_�u��" , "02" },
                    };
                comboBox_SelectBG.DisplayMember = "Key";
                comboBox_SelectBG.ValueMember = "Value";
                comboBox_SelectBG.DataSource = Select_BG_list.ToList();
                comboBox_SelectBG.SelectedIndex = 0;

                //comboBox_Type_Chart�ɔ��f����l��ݒ�
                comboBox_Type_Chart.Enabled = true;
                Dictionary<string, string> Type_Chart_list = new Dictionary<string, string>()
                    {
                        { "�g���Ȃ�" , "0820BF24" },
                        { "�g������" , "FFFFFFFF" },
                    };
                LoadText = textBox_TypeChart_Address.Text;

                switch (LoadText)
                {
                    case "0020BF24":
                        get_flgnum = 0;
                        break;

                    default:
                        textBox_TypeChart_Address.Enabled = true;    //�g�����I���ɂȂ��ĂȂ��ꏊ�ő��삳���Ȃ��悤�ɂ��邽�߂̕���
                        get_flgnum = 1;
                        break;
                }
                comboBox_Type_Chart.DisplayMember = "Key";
                comboBox_Type_Chart.ValueMember = "Value";
                comboBox_Type_Chart.DataSource = Type_Chart_list.ToList();
                comboBox_Type_Chart.SelectedIndex = get_flgnum;

                toolStripProgressBar1.Value = 70;//�v���O���X�o�[���L��

                //-------------------------------------------------------------------------------
                //�Ǎ��f�[�^�����[�h���ċL�ڂ���y�`�F�b�N�{�b�N�X�z
                //-------------------------------------------------------------------------------
                //Move_flag�ɔ��f����l��ݒ�
                LoadText = Read_binary_data(path, MoveInfo_Table + 8, 1);
                label_data_09.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);

                (bool, bool, bool, bool, bool, bool, bool, bool) get_flag_tuple = get_flag(decValue);

                foreach (Control item in groupBox_Move_flag.Controls)
                {
                    if (item.GetType().Equals(typeof(System.Windows.Forms.CheckBox)))   //�Z�߂đS�Ẵ`�F�b�N�{�b�N�X�̑�������s����
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

                //Ability_flag_1�ɔ��f����l��ݒ�
                LoadText = Read_binary_data(path, MoveInfo_Table + 9, 1);
                label_data_10.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);

                get_flag_tuple = get_flag(decValue);

                foreach (Control item in groupBox_Ability_flag1.Controls)
                {
                    if (item.GetType().Equals(typeof(System.Windows.Forms.CheckBox)))   //�Z�߂đS�Ẵ`�F�b�N�{�b�N�X�̑�������s����
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

                //Ability_flag_2�ɔ��f����l��ݒ�
                LoadText = Read_binary_data(path, MoveInfo_Table + 11, 1);
                label_data_12.Text = LoadText;
                decValue = Convert.ToInt32(LoadText, 16);

                get_flag_tuple = get_flag(decValue);

                foreach (Control item in groupBox_Ability_flag2.Controls)
                {
                    if (item.GetType().Equals(typeof(System.Windows.Forms.CheckBox)))   //�Z�߂đS�Ẵ`�F�b�N�{�b�N�X�̑�������s����
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

                toolStripProgressBar1.Value = 90;//�v���O���X�o�[���L��

                Apply_Script_pattern(); //Animation,effect�̏��{�b�N�X�ɃX�N���v�g�p�^�[�����L��

                //-------------------------------------------------------------------------------
                //���O�o�͗p�ꗗ�̕\���y�f�[�^�O���b�h�r���[�z
                //-------------------------------------------------------------------------------

                dataGridView_Change_log.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //�s�̍����̎�������
                dataGridView_Change_log.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                //-------------------------------------------------------------------------------
                //���W�擾�摜�̑傫�����w��y�s�N�`���[�{�b�N�X�z
                //-------------------------------------------------------------------------------
                //pictureBox_Coordinate.Size = new Size(360, 240);



                //-------------------------------------------------------------------------------
                //�ǂݍ��݃f�[�^�̏������C�����x���ɋL��
                //-------------------------------------------------------------------------------

                //MoveInfo_build();
                toolStripProgressBar1.Value = 100;//�v���O���X�o�[���L��
            }
            catch
            {
                //Error�����������ꍇ�̓��b�Z�[�W�{�b�N�X��\�����ďI��
                MessageBox.Show("���͂��ꂽ���ɖ�肪����悤�ł��D\n�A�v�����ċN�����Ă��������D\n�ҏW�r���̃f�[�^������ꍇ�̓c�[���o�[��\n[MSE_File_export]������e��ۑ��ŉ\�ł��D", "�Q�ƒl�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private string Read_binary_data(string path, int StartLen, int LoadLen)
        {
            //-------------------------------------------------------------------------------
            //�p�X����ړ��Ẵo�C�i���f�[�^��ǂݍ��ޏ���
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

            string result = Apply_data.TrimEnd(); //�Ō�ɋ󗓂�����̂ŏ�������
            return result;
        }
        private string Read_binary_toFF(int StartLen)
        {
            //-------------------------------------------------------------------------------
            //�ǂݍ��񂾊J�n�ʒu�A�h���X����FF���ǂݍ��܂��܂ł̒l���e�L�X�g�ŕԂ�
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;
            var code_data = "";
            var Apply_data = "";

            var reader = new FileStream(path, FileMode.Open);
            byte[] data = new byte[StartLen + 1000];    //�I�[�����܂��Ă��Ȃ��׎�荇����1000byte�ǂݍ���
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
            string result = Apply_data.TrimEnd(); //�Ō�ɋ󗓂�����̂ŏ�������
            return result;
        }
        private string binary_exchange(string LoadText)
        {
            //-------------------------------------------------------------------------------
            //�Ǎ��o�C�i���f�[�^��ʏ핶���ɕϊ�
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
            //�ǂݍ���byte�f�[�^����{��ɕϊ�
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

                //ini�t�@�C���̓��e���L���X�g���ĕϐ��Ɋi�[
                StringBuilder sb = new StringBuilder(1024);
                int res = GetPrivateProfileString("BM", data[i], "00", sb, (uint)sb.Capacity, iniFileName); //��p�̃e�L�X�gini����f�[�^���Ăяo��

                switch (code_data)
                {
                    case "fa" or "FA":
                        Apply_data = Apply_data + sb.ToString();
                        break;

                    case "fc" or "FC":
                        code_data = string.Format("{0:x2}", data[i + 1]) + " " + string.Format("{0:x2}", data[i + 2]);
                        res = GetPrivateProfileString("BM_FC_kanji", code_data, "[������]", sb, (uint)sb.Capacity, iniFileName);//�ϐ��̈ꗗ��ini����擾
                        Apply_data = Apply_data + sb.ToString();
                        i = i + 2; //2byte�]�v�ɐi�܂���
                        break;

                    case "fd" or "FD":
                        code_data = string.Format("{0:x2}", data[i + 1]); //1byte��̒l���擾����
                        res = GetPrivateProfileString("MB_FD_BattleText", code_data, "[������]", sb, (uint)sb.Capacity, iniFileName);//�ϐ��̈ꗗ��ini����擾
                        Apply_data = Apply_data + sb.ToString();
                        i = i + 1; //1byte�]�v�ɐi�܂���
                        break;

                    case "fe" or "FE":
                        Apply_data = Apply_data + sb.ToString() + "\r\n";
                        break;

                    case "ff" or "FF":
                        //�e�L�X�g�̏I�[�ɏI���������L�ڂ��邩�ǂ���
                        //Apply_data = Apply_data + sb.ToString();
                        return Apply_data; //�I�����������������\�L���I��������

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
            //�ǂݍ��ݓ��{��f�[�^���o�C�i���ɕϊ�
            //-------------------------------------------------------------------------------          
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";
            string bytestr = LoadText;
            string get_nametext = "";
            string end_flg = "";

            int remain_length = 0;

            //�I�[�����������Ă��Ȃ��ꍇ�͒ǉ�����
            if (LoadText.Contains("��")) { end_flg = "��"; } else { end_flg = "��"; }

            //������������Ȃ��ꍇ�͌��ɋ󔒂�ǉ�����
            bytestr = bytestr + end_flg;
            end_flg = "";

            for (int i = 2; i <= name_length; i++)
            {
                end_flg += "��";
            }
            bytestr = bytestr + end_flg;

            for (int i = 0; i < name_length; i++)
            {
                //ini�t�@�C���̓��e���L���X�g���ĕϐ��Ɋi�[
                StringBuilder sb = new StringBuilder(2048);

                string str_font = bytestr.Substring(i, 1);
                if (str_font == "$")
                {
                    str_font = bytestr.Substring(i, 3);
                    i += 2; //�]�v��2byte�i�߂Ă���
                }


                int res = GetPrivateProfileString("MB", str_font, "[?]", sb, (uint)sb.Capacity, iniFileName); //��p�̃e�L�X�gini����f�[�^���Ăяo��

                //�c�蕶�������v�Z����section
                switch (sb.ToString())
                {
                    case "ff" or "FF":
                        //�I�[����������c�蕶�������v�Z���ĕ\�L
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

                if ((i == 0) || (res == 0)) //�s�̊J�n�������͉��s�R�[�h�������Ă���ꍇ�͋󔒒ǉ����X�L�b�v����
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
            //�p�X�ɋL�ڂ��ꂽ�A�h���X����2byte���ǂݍ���ŕ\��������𔻒�
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
            while (Count_flg < 5)�@//�T��ڂ�00 00 ��ǂݍ��񂾎��_�ŏ������I��������
            {
                //�ZID���쐬
                Apply_data = string.Format("{0:x2}", data[i + 1]) + string.Format("{0:x2}", data[i]);
                move_num = Convert.ToInt32(Apply_data, 16); //10�i���ɕϊ����ċZID���쐬

                //�Z������{��f�[�^�Ŏ擾
                int MoveName_Table = int.Parse(label_Move_Name_Address.Text);
                Load_Address = MoveName_Table + (move_num * int.Parse(label_Name_length.Text));       //�I�𒆂̋Z���̃A�h���X���擾
                LoadText = Read_binary_data(path, Load_Address, int.Parse(label_Name_length.Text));  //�Y��Address����Z���̃f�[�^���擾
                LoadText = FontData_exchange(LoadText);

                //���ݕ\�����Ă���ZID�Ƃ̏ƍ��p�f�[�^�����
                int Select_move_num = int.Parse(numericUpDown_Move_Number.Value.ToString());    //�e�L�X�g�擾�ł͈�O�̒l�ɂȂ邽��);

                //2Byte����ǂݍ���ŕϐ��ɐ��`
                Apply_data = string.Format("{0:x2}", data[i]) + " " + string.Format("{0:x2}", data[i + 1]);

                All_Data += Apply_data + " "; //�S��Script���i�[����

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
            //2�i������t���O�̒l���v�Z
            //-------------------------------------------------------------------------------
            int retrun_flg = 0;
            string getflg = "";

            for (int i = 1; i <= 7; i++)
            {
                getflg = binValue.Substring(i, 1);  //2�i���̒���1������ꏊ�Ńt���O�𔻒肳����

                if (getflg == "1")
                {
                    retrun_flg = 8 - i;              //���X�g�͋t���Ȃ̂�7���猸�Y����D0�̏ꍇ�͓ǂ܂�Ȃ��̂ő������v
                }
            }
            return retrun_flg;
        }
        private (bool a, bool b, bool c, bool d, bool e, bool f, bool g, bool h) get_flag(int decValue)
        {
            //-------------------------------------------------------------------------------
            //�l����t���O���m�F
            //-------------------------------------------------------------------------------

            bool flag1 = (decValue & (1 << 0)) != 0;
            bool flag2 = (decValue & (1 << 1)) != 0;
            bool flag3 = (decValue & (1 << 2)) != 0;
            bool flag4 = (decValue & (1 << 3)) != 0;
            bool flag5 = (decValue & (1 << 4)) != 0;
            bool flag6 = (decValue & (1 << 5)) != 0;
            bool flag7 = (decValue & (1 << 6)) != 0;
            bool flag8 = (decValue & (1 << 7)) != 0;

            //���O�t���^�v���œZ�߂ĕϐ��Ɋi�[����

            return (flag1, flag2, flag3, flag4, flag5, flag6, flag7, flag8);

        }
        private void MoveInfo_build()
        {
            //-------------------------------------------------------------------------------
            //textBox_moveinfo�ɒl���L�ڂ���
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
            //���͒l��0�ȉ��A�܂���255�ȏ�ɂȂ邱�Ƃ�h������
            //-------------------------------------------------------------------------------
            switch (dec)
            {
                case "":
                    dec = "0";                                      //0�ȉ��̏ꍇ�͋����I��0�ɕϊ�
                    break;
                default:
                    if (int.Parse(dec) > 255) { dec = "255"; }      //255�ȏ�̏ꍇ�͋����I��255�ɕϊ�
                    break;
            }
            dec = (int.Parse(dec)).ToString("X").PadLeft(2, '0');   //int.Parse("123"),2���ɂ��Ȃ��ƃo�O��̂� 

            return dec;
        }
        private void Apply_Script_pattern()
        {
            //-------------------------------------------------------------------------------
            //�G�t�F�N�g�E�A�j���[�V�����̃p�^�[�������X�g�{�b�N�X�ɔ��f�y���X�g�{�b�N�X�z
            //-------------------------------------------------------------------------------
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\code.ini"; //�e�L�X�g��ǂݍ���

            //�����R�[�h��ύX���Ȃ��ƕ�����������
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
                string Loadkey = (i.ToString("X").PadLeft(2, '0'));   //int.Parse("123"),2���ɂ��Ȃ��ƃo�O��̂�
                res = GetPrivateProfileString("Effect", Loadkey, "�ǂݍ��݃G���[", db_Effect, (uint)db_Effect.Capacity, iniFileName);
                res = GetPrivateProfileString("Animation", Loadkey, "�ǂݍ��݃G���[", db_Animation, (uint)db_Animation.Capacity, iniFileName);

                string[] comment_info1 = db_Effect.ToString().Split(',');
                string[] comment_info2 = db_Animation.ToString().Split(',');

                listBox_Effect_select.Items.Add(Loadkey + ":" + comment_info1[9].ToString());       //���X�g�ϐ��Ƀe�L�X�g�̋L�ړ��e���P�s����������
                listBox_Animation_select.Items.Add(Loadkey + ":" + comment_info2[9].ToString()); //���X�g�ϐ��Ƀe�L�X�g�̋L�ړ��e���P�s����������

            }

            listBox_FD_text.Items.Clear();

            iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini"; //�e�L�X�g��ǂݍ���
            StringBuilder db_FDcode = new StringBuilder(1024);
            for (int i = 0; i < 47; i++)
            {
                string Loadkey = (i.ToString("X").PadLeft(2, '0'));   //int.Parse("123"),2���ɂ��Ȃ��ƃo�O��̂�
                res = GetPrivateProfileString("MB_FD_BattleText", Loadkey, "�ǂݍ��݃G���[", db_FDcode, (uint)db_Effect.Capacity, iniFileName);

                listBox_FD_text.Items.Add(db_FDcode.ToString());       //���X�g�ϐ��Ƀe�L�X�g�̋L�ړ��e���P�s����������

            }


        }

        //--------------------------------------------------------------------------------------------------
        //�l���͎��̔��f����
        //--------------------------------------------------------------------------------------------------
        private void numericUpDown_Move_Number_ValueChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�ԍ��J�E���g�{�b�N�X�Ɏw��f�[�^����������
            //-------------------------------------------------------------------------------
            label_Move_Number.Text = numericUpDown_Move_Number.Value.ToString();    //�e�L�X�g�擾�ł͈�O�̒l�ɂȂ邽��
            button3_Click(sender, e);
        }
        private void textBox_Move_Name_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //���e�L�X�g�{�b�N�X�Ɏw��f�[�^����������
            //-------------------------------------------------------------------------------
            int move_name_length = int.Parse(label_Name_length.Text);
            string str = textBox_Move_Name.Text;
            str = ByteData_exchange(str, move_name_length);  //�������g���ɑΉ����邽��
            label_Move_Name.Text = str;

            label_Change_announce.Visible = true; //�X�V�`�F�b�N�e�L�X�g��\��
        }
        private void textBox_Move_text_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�Z�����e�L�X�g�{�b�N�X�Ɏw��f�[�^����������
            //-------------------------------------------------------------------------------
            int Move_Text_length = int.Parse(label_Move_text_length.Text);
            string str = textBox_Move_text.Text;

            string str_text = str.Replace("\n", "").Replace("\r", "").Replace("\t", "");

            str = ByteData_exchange(str_text, Move_Text_length);
            label_Move_text.Text = str;
            // �ő啶�����𒴂����ꍇ�́A�������������폜���čŌ�̕������󔒂̏ꍇ�͍폜����
            if (label_Move_text.Text.Length > textBox_Move_text_code.MaxLength)
            {
                label_Move_text.Text = label_Move_text.Text.Substring(0, textBox_Move_text_code.MaxLength).Trim();
            }
            textBox_Move_text_code.Text = label_Move_text.Text.ToUpper();

            label_Change_announce.Visible = true; //�X�V�`�F�b�N�e�L�X�g��\��
        }
        private void textBox_label_moveID_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�ZID�e�L�X�g�{�b�N�X�Ɏw��f�[�^����������
            //-------------------------------------------------------------------------------
            string dec = textBox_label_moveID.Text;
            dec = Value_over_0To255(dec);
            label_data_01.Text = dec;
            MoveInfo_build();
        }
        private void textBox_Movepower_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�Z�З̓e�L�X�g�{�b�N�X�Ɏw��f�[�^����������
            //-------------------------------------------------------------------------------
            string dec = textBox_Movepower.Text;
            dec = Value_over_0To255(dec);
            label_data_02.Text = dec;
            MoveInfo_build();
        }
        private void comboBox_moveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�Z�^�C�v�R���{�{�b�N�X��ς����ۂ̏���
            //-------------------------------------------------------------------------------
            int sql = comboBox_moveType.SelectedIndex;
            string str_dec = sql.ToString("X").PadLeft(2, '0');
            label_data_03.Text = str_dec;
            MoveInfo_build();
        }
        private void textBox_Accuracy_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�������e�L�X�g�{�b�N�X�Ɏw��f�[�^����������
            //-------------------------------------------------------------------------------
            string dec = textBox_Accuracy.Text;
            dec = Value_over_0To255(dec);
            label_data_04.Text = dec;
            MoveInfo_build();
        }
        private void textBox_PP_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //PP�e�L�X�g�{�b�N�X�Ɏw��f�[�^����������
            //-------------------------------------------------------------------------------
            string dec = textBox_PP.Text;
            dec = Value_over_0To255(dec);
            label_data_05.Text = dec;
            MoveInfo_build();
        }
        private void textBox_Prob_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�ǉ����ʃe�L�X�g�{�b�N�X��ς����ۂ̏���
            //-------------------------------------------------------------------------------
            string dec = textBox_Prob.Text;
            dec = Value_over_0To255(dec);
            label_data_06.Text = dec;
            MoveInfo_build();
        }
        private void comboBox_Attack_range_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�Z�͈̓R���{�{�b�N�X��ς����ۂ̏���
            //-------------------------------------------------------------------------------
            string select_value = comboBox_Attack_range.SelectedValue.ToString();
            label_data_07.Text = select_value;
            MoveInfo_build();
        }

        private void comboBox_Priority_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�D��x�R���{�{�b�N�X��ς����ۂ̏���
            //-------------------------------------------------------------------------------
            string select_value = comboBox_Priority.SelectedValue.ToString();
            label_data_08.Text = select_value;
            MoveInfo_build();
        }
        private void comboBox_Move_Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //���ރR���{�{�b�N�X��ς����ۂ̏���
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
                    label_movetxt_mode_info.Text = "�f�t�H���g����ύX�Ȃ��C118�g�܂őΉ�";
                    break;

                default:
                    textBox__movetxt_pattern.Enabled = true;
                    label_movetxt_mode_info.Text = "�g���ς݁C����Ȃ�" + "\r\n" + "�K���e�[�u���J�n�I�t�Z�b�g���󂫗̈�Ɏw���ɕύX���邱��";
                    break;
            }

        }
        private void comboBox_Type_Chart_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�^�C�v�`���[�g�Q�Ɛ��ς����ۂ̏���
            //-------------------------------------------------------------------------------
            int select_num = comboBox_Type_Chart.SelectedIndex;
            switch (select_num)
            {
                case 0:
                    label_TypeChart_Mode_txt.Text = "�f�t�H���g����ύX�Ȃ�";
                    break;

                default:
                    textBox_TypeChart_Address.Enabled = true;
                    label_TypeChart_Mode_txt.Text = "�g���ς݁C����Ȃ�" + "\r\n" + "�K���e�[�u���J�n�I�t�Z�b�g���󂫗̈�Ɏw���ɕύX���邱��";
                    break;
            }

        }
        private void groupBox_Move_flag_math(string label_name, string group_name)
        {
            //-------------------------------------------------------------------------------
            //�Z�t���O�̃`�F�b�N��ύX�����ۂ̋��ʏ���
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
                string str_dec = flag_num.ToString("X").PadLeft(2, '0');     //2����16�i���ɕϊ�

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
            //�X�v���v�g�e�L�X�g�{�b�N�X��ς����ۂ̏���
            //-------------------------------------------------------------------------------
            string LoadText = textBox_Effect_address.Text;
            textBox_Effect_address.Text = LoadText;

            int decValue = 0;

            if (LoadText == "")
            {
                LoadText = "0";
            }

            decValue = Convert.ToInt32(LoadText, 16) + 134217728;    //�Ǎ��l��10�i���ɕϊ����Ă���ROM�̈�Ԓn�𑫂� 
            LoadText = decValue.ToString("X").PadLeft(8, '0');

            LoadText = LoadText + "00000000";                        //�󗓂������Error�ɂȂ�̂�00�Ŗ��߂Ă���

            LoadText = LoadText.Substring(6, 2) + " " + LoadText.Substring(4, 2) + " " +
                           LoadText.Substring(2, 2) + " " + LoadText.Substring(0, 2);

            label_Read_address.Text = LoadText;
            label_Change_announce.Visible = true; //�X�V�`�F�b�N�e�L�X�g��\��

        }
        private void textBox_Effect_address_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�G�t�F�N�g�e�L�X�g�{�b�N�X��ς����ۂ̏���
            //-------------------------------------------------------------------------------
            string LoadText = textBox_Animation_address.Text;
            textBox_Animation_address.Text = LoadText;

            int decValue = 0;

            if (LoadText == "")
            {
                LoadText = "0";
            }

            decValue = Convert.ToInt32(LoadText, 16) + 134217728;    //�Ǎ��l��10�i���ɕϊ����Ă���ROM�̈�Ԓn�𑫂� 
            LoadText = decValue.ToString("X").PadLeft(8, '0');

            LoadText = LoadText + "00000000";                        //�󗓂������Error�ɂȂ�̂�00�Ŗ��߂Ă���

            LoadText = LoadText.Substring(6, 2) + " " + LoadText.Substring(4, 2) + " " +
                           LoadText.Substring(2, 2) + " " + LoadText.Substring(0, 2);

            label_Animation_address.Text = LoadText;
            label_Change_announce.Visible = true; //�X�V�`�F�b�N�e�L�X�g��\��
        }
        private void richTextBox_Read_Script_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�X�N���v�g�f�[�^���L�^�p���x���ɋL��
            //-------------------------------------------------------------------------------
            string script_data = richTextBox_Effect_Script.Text;
            string get_data = "";
            script_data = script_data.Replace(" ", "");
            script_data = script_data.Replace("[", "");
            script_data = script_data.Replace("]", "");
            script_data = script_data.Replace("X", "");
            script_data = script_data.Replace("\n", "");
            script_data = script_data.Replace("\r\n", "");

            //2�������������ēǂݍ���ōŌ�ɋ󔒂�����ꍇ�͏���
            for (int i = 0; i < script_data.Length - 1; i += 2)
            {
                get_data += script_data.Substring(i, 2) + " ";
            }

            label_Read_Script.Text = get_data.TrimEnd(); // s.TrimEnd()�ōŌ�̗]�v�ȋ󔒂������ł���
            label_Change_announce.Visible = true; //�X�V�`�F�b�N�e�L�X�g��\��
        }
        private void richTextBox_Effect_Script_TextChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�G�t�F�N�g�f�[�^���L�^�p���x���ɋL��
            //-------------------------------------------------------------------------------
            string script_data = richTextBox_Animation_Script.Text;
            string get_data = "";
            script_data = script_data.Replace(" ", "");
            script_data = script_data.Replace("[", "");
            script_data = script_data.Replace("]", "");
            script_data = script_data.Replace("X", "");
            script_data = script_data.Replace("�`", "");
            script_data = script_data.Replace("\n", "");
            script_data = script_data.Replace("\r\n", "");

            //2�������������ēǂݍ���ōŌ�ɋ󔒂�����ꍇ�͏���
            for (int i = 0; i < script_data.Length - 1; i += 2)
            {
                get_data += script_data.Substring(i, 2) + " ";
            }

            label_Animation_Script.Text = get_data.TrimEnd(); // s.TrimEnd()�ōŌ�̗]�v�ȋ󔒂������ł���
            label_Change_announce.Visible = true; //�X�V�`�F�b�N�e�L�X�g��\��
        }
        private void listBox_Script_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�X�N���v�g���X�g�{�b�N�X�̑I����ς����ۂ̏���
            //-------------------------------------------------------------------------------
            int Select_list_num = 0;
            int res;

            Select_list_num = listBox_Effect_select.SelectedIndex;         //�I�𒆂̏ꏊ�����X�g��

            string code_data = Select_list_num.ToString().PadLeft(2, '0'); //2���̕����񐔎��ɕϊ�
            code_data = Select_list_num.ToString("X").PadLeft(2, '0');     //2����16�i���ɕϊ�����

            string inifolder = toolStripComboBox_Useinidata.Text;
            string code_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\code.ini";    //ini����R�[�h�̌^���擾 
            StringBuilder db = new StringBuilder(2048);

            //Script�R�[�h�̓����ǂݍ���
            res = GetPrivateProfileString("Effect", code_data, "�ǂݍ��݃G���[", db, (uint)db.Capacity, code_iniFileName);
            string[] code_info = db.ToString().Split(',');

            var Script_body = "";

            for (int j = 0; j < 7; j++)
            {
                //�ǂݍ���ini����L�ڕ�������쐬
                switch (code_info[j])
                {
                    case "code"://�擪�R�[�h                                                
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

                    case "x"://1byte�ϒ��A2byte
                        Script_body += "[NN] [XX XX]�` ";
                        break;

                    case "y"://1byte�ϒ��A1byte
                        Script_body += "[NN] [XX]�` ";
                        break;

                    default://�ǂݍ��݂Ȃ�
                        break;
                }
            }

            richTextBox_Effect_Script.Text += "\r\n" + Script_body;

        }
        private void listBox_Effect_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�Z�X�N���v�g���X�g�{�b�N�X�̑I����ς����ۂ̏���
            //-------------------------------------------------------------------------------
            int Select_list_num = 0;
            int res;

            Select_list_num = listBox_Animation_select.SelectedIndex;      //�I�𒆂̏ꏊ�����X�g

            string code_data = Select_list_num.ToString().PadLeft(2, '0'); //2���̕����񐔎��ɕϊ�
            code_data = Select_list_num.ToString("X").PadLeft(2, '0');     //2����16�i���ɕϊ�����

            string inifolder = toolStripComboBox_Useinidata.Text;
            string code_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\code.ini";    //ini����R�[�h�̌^���擾 
            StringBuilder db = new StringBuilder(2048);

            //Script�R�[�h�̓����ǂݍ���
            res = GetPrivateProfileString("Animation", code_data, "�ǂݍ��݃G���[", db, (uint)db.Capacity, code_iniFileName);
            string[] code_info = db.ToString().Split(',');

            var Script_body = "";

            for (int j = 0; j < 7; j++)
            {
                //�ǂݍ���ini����L�ڕ�������쐬
                switch (code_info[j])
                {
                    case "code"://�擪�R�[�h
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

                    case "x"://1byte�ϒ��A2byte
                        Script_body += "[NN] [XX XX]�` ";
                        break;

                    case "y"://1byte�ϒ��A1byte
                        Script_body += "[NN] [XX]�` ";
                        break;

                    default://�ǂݍ��݂Ȃ�
                        break;
                }
            }

            richTextBox_Animation_Script.Text += Script_body + "\r\n";


        }

        private void textBox_label_moveID_KeyPress(object sender, KeyPressEventArgs e)
        {
            //--------------------------------------------------------------------------------------------------
            //�e�L�X�g�{�b�N�X�̓��͋K���i�����̂݁j
            //--------------------------------------------------------------------------------------------------
            //https://www.itlab51.com/?p=2738
            //https://stellacreate.com/entry/cs-textbox-number-only

            if ((e.KeyChar < 48) || (e.KeyChar > 57))   //���͉\������𐔎��݂̂ɋK��
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
            //�e�L�X�g�{�b�N�X�̓��͋K���i16�i���̂݁j
            //--------------------------------------------------------------------------------------------------
            //http://faq.creasus.net/04/0131/CharCode.html
            if (((e.KeyChar < 48) || (e.KeyChar > 57)) &&
                ((e.KeyChar < 65) || (e.KeyChar > 70)) &&
                ((e.KeyChar < 97) || (e.KeyChar > 102))
                )   //���͉\������𐔎��݂̂ɋK��
            {
                if (e.KeyChar != '\b')
                {
                    e.Handled = true;
                }
            }

        }
        private void AnyData_Changedcheck(object sender, EventArgs e)
        {
            label_Change_announce.Visible = true; //�e�L�X�g��������悤�ɕύX
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //���ʎw�菑�����݃{�^��������
            //-------------------------------------------------------------------------------
            string apply_Address = textBox_Effect_address.Text;
            DialogResult result = MessageBox.Show(
                "�Q�ƃe�[�u���� " + apply_Address + " �ɍX�V��\r\n�ݒ�A�h���X�ɕύX���e���㏑�����܂����H",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //�����I�����ꂽ�����ׂ�
            if (result == DialogResult.OK)
            {
                try
                {
                    Move_effect_Save();
                    MessageBox.Show("�Z���ʂ̏㏑�����������܂���.", "System message");

                    //�X�V���O�ɋL��
                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "Effect",
                                                     label_Updatabefore_Effect.Text,
                                                     textBox_Effect_address.Text,
                                                     DateTime.Now,
                                                     "�ڍ׏��",
                                                     label_before_Script_Effect.Text,
                                                     richTextBox_Effect_Script.Text
                                                     );
                    label_Updatabefore_Effect.Text = textBox_Effect_address.Text;     //�ύX�O�A�h���X�̓��e���X�V
                    label_before_Script_Effect.Text = richTextBox_Effect_Script.Text; //�ύX�O�X�N���v�g�̓��e���X�V
                    label_Change_announce.Visible = false;
                }
                catch
                {
                    //Error�����������ꍇ�̓��b�Z�[�W�{�b�N�X��\�����ďI��
                    MessageBox.Show("�ۑ��Ɏ��s���܂����B\n���͂ł��Ȃ������񂪋L�ڂ���Ă��Ȃ����m�F���Ă��������B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //�L�����Z���̏ꍇ�͉������Ȃ�
            }
        }
        private void Move_effect_Save()
        {
            string path = toolStripStatusLabel__Fullpath.Text;   //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int Load_Num = int.Parse(label_Move_Number.Text);    //����ǂݍ��ݎ���ini����ǂ񂾃e�L�X�g���Q�Ƃ���

            //�I������ini�̃f�[�^�x�[�X���擾����
            string LoadSection = "DataBase";

            //�Z���̒������擾
            int MoveInfo_Table_Length = 0;
            var Loadkey = "MoveInfo_Table_Length";
            MoveInfo_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            //�Z���e�[�u���̊J�n�ʒu���擾���ďꏊ���v�Z
            int MoveInfo_Table = 0;
            Loadkey = "MoveInfo_Table";
            MoveInfo_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            MoveInfo_Table = MoveInfo_Table + (Load_Num * MoveInfo_Table_Length);      //�ǂݍ��݃e�[�u���ʒu���Y���ԍ��̂��̂ɍČv�Z����

            //---------------------------------------------------------------------
            //�Z�X�N���v�g���e�𔽉f
            var LoadText = String_exchange(label_Read_Script.Text);

            int MoveScript_Length = 0;
            MoveScript_Length = LoadText.Length;

            int MoveScript = 0;
            var decValue = Convert.ToInt32(textBox_Effect_address.Text, 16); //�L�ڂ��ꂽAddress�̓e�L�X�g�Ȃ̂�10�i���ɖ߂��K�v������
            MoveScript = decValue;

            Write_binary_data(path, MoveScript, MoveScript_Length, LoadText);

            //---------------------------------------------------------------------
            //�X�N���v�g�ݒ�ʒu���f
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
            //�A�j���w�菑�����݃{�^��������
            //-------------------------------------------------------------------------------
            string apply_Address = textBox_Animation_address.Text;

            DialogResult result = MessageBox.Show(
                "�Q�ƃe�[�u���� " + apply_Address + " �ɍX�V��\r\n�ݒ�A�h���X�ɕύX���e���㏑�����܂����H",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //�����I�����ꂽ�����ׂ�
            if (result == DialogResult.OK)
            {
                try
                {
                    Move_animation_Save();
                    MessageBox.Show("�Z�A�j���̏㏑�����������܂���.", "System message");

                    //�X�V���O�ɋL��
                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "Animation",
                                                     label_Updatabefore_Animation.Text,
                                                     textBox_Animation_address.Text,
                                                     DateTime.Now,
                                                     "�ڍ׏��",
                                                     label_before_Script_Animation.Text,
                                                     richTextBox_Animation_Script.Text
                                                     );
                    label_Updatabefore_Animation.Text = textBox_Animation_address.Text;     //�ύX�O�A�h���X�̓��e���X�V
                    label_before_Script_Animation.Text = richTextBox_Animation_Script.Text; //�ύX�O�X�N���v�g�̓��e���X�V
                    label_Change_announce.Visible = false;
                }
                catch
                {
                    //Error�����������ꍇ�̓��b�Z�[�W�{�b�N�X��\�����ďI��
                    MessageBox.Show("�ۑ��Ɏ��s���܂����B\n���͂ł��Ȃ������񂪋L�ڂ���Ă��Ȃ����m�F���Ă��������B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //�L�����Z���̏ꍇ�͉������Ȃ�
            }
        }
        private void Move_animation_Save()
        {
            string path = toolStripStatusLabel__Fullpath.Text;           //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int Load_Num = int.Parse(numericUpDown_Move_Number.Text);    //���݂�ID���Q��

            //�I������ini�̃f�[�^�x�[�X���擾����
            string LoadSection = "DataBase";

            //�Z���̒������擾
            int Animation_Table_Length = 0;
            var Loadkey = "Animation_Table_Length";
            Animation_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            //�J�n�ʒu���擾���ďꏊ���v�Z
            int Animation_Table = 0;
            Loadkey = "Animation_Table";
            Animation_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            Animation_Table = Animation_Table + (Load_Num * Animation_Table_Length);      //�ǂݍ��݃e�[�u���ʒu���Y���ԍ��̂��̂ɍČv�Z����

            //---------------------------------------------------------------------
            //�X�N���v�g���e�𔽉f
            var LoadText = String_exchange(label_Animation_Script.Text);

            int MoveScript_Length = 0;
            MoveScript_Length = LoadText.Length;

            int MoveScript = 0;
            var decValue = Convert.ToInt32(textBox_Animation_address.Text, 16); //�L�ڂ��ꂽAddress�̓e�L�X�g�Ȃ̂�10�i���ɖ߂��K�v������
            MoveScript = decValue;

            Write_binary_data(path, MoveScript, MoveScript_Length, LoadText);

            //---------------------------------------------------------------------
            //�X�N���v�g�ݒ�ʒu���f
            LoadText = String_exchange(label_Animation_address.Text);
            Write_binary_data(path, Animation_Table, Animation_Table_Length, LoadText);
        }
        private void Write_binary_data(string path, int StartLen, int LoadLen, byte[] LoadText)
        {
            //-------------------------------------------------------------------------------
            //�o�C�i���t�@�C���Ɏw��f�[�^����������
            //-------------------------------------------------------------------------------
            var reader = new BinaryWriter(new FileStream(path, FileMode.Open));

            // byte�z��̍쐬
            foreach (var item in LoadText)
            {
                Console.Write(string.Format("{0:X2} ", item));
            }
            Console.WriteLine();

            // �w�肵���A�h���X�ɏ������݈ʒu���ړ�
            reader.Seek(StartLen, SeekOrigin.Begin);

            reader.Write(LoadText);
            reader.Close();
        }
        private static byte[] String_exchange(string LoadText)
        {
            //-------------------------------------------------------------------------------
            //�Ǎ��f�[�^���o�C�i���z��ɕϊ�
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
            //�X�N���v�g�A�h���X�ǂݍ��ݐ�ύX�{�^��
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                                    //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���

            int decValue = Convert.ToInt32(textBox_Effect_address.Text, 16);                      //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z                           
            int Address_length = int.Parse(numericUpDown_Effect_Address_length.Value.ToString()); //�ǂݍ���byte���l���擾�i�f�t�H���g�ł�100�j

            string LoadText = Read_binary_data(path, decValue, Address_length);                   //�Ǎ�Address�ʒu����Script�f�[�^�𒊏o

            label_Read_Script.Text = LoadText;                                                    //�L�ڗpScript�f�[�^�����x���ɋL��
            richTextBox_Effect_Script.Text = LoadText;                                            //�e�L�X�g�{�b�N�X�ɋL��
        }
        private void button13_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�Z�A�j���A�h���X�ǂݍ��ݐ�ύX�{�^��
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                                       //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���

            int decValue = Convert.ToInt32(textBox_Animation_address.Text, 16);                      //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z                           
            int Address_length = int.Parse(numericUpDown_Animation_Address_length.Value.ToString()); //�ǂݍ���byte���l���擾�i�f�t�H���g�ł�100�j

            string LoadText = Read_binary_data(path, decValue, Address_length);                      //�Ǎ�Address�ʒu����Script�f�[�^�𒊏o

            label_Animation_Script.Text = LoadText;                                                  //�L�ڗpScript�f�[�^�����x���ɋL��
            richTextBox_Animation_Script.Text = LoadText;                                            //�e�L�X�g�{�b�N�X�ɋL��
        }
        private void button8_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //Animation���X�g�ǂݍ��ݐ�ύX�{�^��
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;

            int decValue = Convert.ToInt32(textBox__movetxt_pattern.Text, 16);
            Read_Move_binary_2byte(path, decValue); //�Z�\���e�L�X�g�̃��X�g�{�b�N�X�ɒl���L�ڂ�����

        }
        private void button5_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�Z��񏑂����݃{�^��������
            //-------------------------------------------------------------------------------
            DialogResult result = MessageBox.Show(
                "�ύX���e���㏑�����܂����H",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //�����I�����ꂽ�����ׂ�
            if (result == DialogResult.OK)
            {
                try
                {
                    Move_info_Save();
                    label_Change_announce.Visible = false;
                    MessageBox.Show("�Z���̏㏑�����������܂���.", "System message");

                    //�X�V���O�ɋL��
                    string Text_log_before = "Name�F\n" +
                                            label_before_Script_MoveName.Text +
                                            "\n" +
                                            "\nInfo�F\n" +
                                            label_before_Script_MoveInfo.Text +
                                            "\n" +
                                            "\nText�F\n" +
                                            label_before_Script_MoveText.Text;

                    string Text_log_after = "Name�F\n" +
                                            textBox_Move_Name.Text +
                                            "\n" +
                                            "\nInfo�F\n" +
                                            textBox_moveinfo.Text +
                                            "\n" +
                                            "\nText�F\n" +
                                            textBox_Move_text.Text;

                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "Move Data",
                                                     label_Updatabefore_MoveInfo.Text,
                                                     label_Updatabefore_MoveInfo.Text,
                                                     DateTime.Now,
                                                     "�ڍ׏��",
                                                     Text_log_before,
                                                     Text_log_after
                                                     );
                    label_before_Script_MoveName.Text = textBox_Move_Name.Text;
                    label_before_Script_MoveInfo.Text = textBox_moveinfo.Text;
                    label_before_Script_MoveText.Text = textBox_Move_text.Text;
                }
                catch
                {
                    //Error�����������ꍇ�̓��b�Z�[�W�{�b�N�X��\�����ďI��
                    MessageBox.Show("�ۑ��Ɏ��s���܂����B\n���͂ł��Ȃ������񂪋L�ڂ���Ă��Ȃ����m�F���Ă��������B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //�L�����Z���̏ꍇ�͉������Ȃ�
            }
        }
        private void Move_info_Save()
        {
            string path = toolStripStatusLabel__Fullpath.Text;   //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int Load_Num = int.Parse(label_Move_Number.Text);    //����ǂݍ��ݎ���ini����ǂ񂾃e�L�X�g���Q�Ƃ���

            //Table_Data�̒l�𔽉f������
            //�I������ini�̃f�[�^�x�[�X���擾����
            string LoadSection = "DataBase";

            //�Z���̒������擾
            int MoveInfo_Table_Length = 0;
            var Loadkey = "MoveInfo_Table_Length";
            MoveInfo_Table_Length = MyGetPrivateProfileInt(LoadSection, Loadkey);

            //�Z���e�[�u���̊J�n�ʒu���擾���ďꏊ���v�Z
            int MoveInfo_Table = 0;
            Loadkey = "MoveInfo_Table";
            MoveInfo_Table = MyGetPrivateProfileInt(LoadSection, Loadkey);
            MoveInfo_Table = MoveInfo_Table + (Load_Num * MoveInfo_Table_Length);      //�ǂݍ��݃e�[�u���ʒu���Y���ԍ��̂��̂ɍČv�Z����

            //�Z���̖{�����w��
            var LoadText = String_exchange(textBox_moveinfo.Text);
            Write_binary_data(path, MoveInfo_Table, MoveInfo_Table_Length, LoadText);

            //---------------------------------------------------------------------
            //�Z���f�[�^�𔽉f
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
            //�Z�����𔽉f
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
            //�Z�\���������݃{�^��������
            //-------------------------------------------------------------------------------
            string apply_Address = textBox__movetxt_pattern.Text;

            DialogResult result = MessageBox.Show(
                "�Q�ƃe�[�u���� " + apply_Address + " �ɍX�V��\r\n�ݒ�A�h���X�ɕύX���e���㏑�����܂����H",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //�����I�����ꂽ�����ׂ�
            if (result == DialogResult.OK)
            {

                try
                {
                    Move_textPattern_Save();
                    MessageBox.Show("�Z�\���p�^�[���̏㏑�����������܂���.", "System message");

                    //�X�V���O�ɋL��
                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "TextPattern",
                                                     label_Updatabefore_Pattern.Text,
                                                     textBox__movetxt_pattern.Text,
                                                     DateTime.Now,
                                                     "�ڍ׏��",
                                                     label_before_Script_Pattern.Text,
                                                     label_after_Script_Pattern.Text    //�Z�[�u�������ɂ���̂Ńe�X�g�ł͊m�F�s��
                                                     );
                    label_Updatabefore_Pattern.Text = textBox__movetxt_pattern.Text;    //�ύX�O�A�h���X�̓��e���X�V
                    label_before_Script_Pattern.Text = label_after_Script_Pattern.Text; //�ύX�O�X�N���v�g�̓��e���X�V
                    label_Change_announce.Visible = false;

                }
                catch
                {
                    //Error�����������ꍇ�̓��b�Z�[�W�{�b�N�X��\�����ďI��
                    MessageBox.Show("�ۑ��Ɏ��s���܂����B\n���͂ł��Ȃ������񂪋L�ڂ���Ă��Ȃ����m�F���Ă��������B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //�L�����Z���̏ꍇ�͉������Ȃ�
            }

        }
        private void Move_textPattern_Save()
        {
            string path = toolStripStatusLabel__Fullpath.Text;      //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int Load_Num = int.Parse(label_Move_Number.Text);       //����ǂݍ��ݎ���ini����ǂ񂾃e�L�X�g���Q�Ƃ���

            string LoadSection = "DataBase"; //�I������ini�̃f�[�^�x�[�X���擾����

            //textBox__movetxt_patten�̍X�V
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

            decValue = Convert.ToInt32(LoadText, 16) + 134217728;    //�Ǎ��l��10�i���ɕϊ����Ă���ROM�̈�Ԓn�𑫂� 
            LoadText = decValue.ToString("X").PadLeft(8, '0');

            LoadText = LoadText + "00000000";                        //�󗓂������Error�ɂȂ�̂�00�Ŗ��߂Ă���


            LoadText = LoadText.Substring(6, 2) + " " + LoadText.Substring(4, 2) + " " +
                        LoadText.Substring(2, 2) + " " + LoadText.Substring(0, 2);

            var convert_val = String_exchange(LoadText);

            Write_binary_data(path, TextPatternSelect1, 4, convert_val);
            Write_binary_data(path, TextPatternSelect2, 4, convert_val);

            //textBox_movetxt_patten�̍X�V
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

            //label_Move_Pattern_Alltext�i�X�V�������X�g�̔��f�j
            string all_movetext_id = "";
            int movetext_length = 0;

            for (int j = 1; j < 5; j++)
            {
                for (int i = 0; i < ((ListBox)main_tag.TabPages["tabPage4"].Controls["listBox_Movetext_pattern" + j]).Items.Count; i++)
                {
                    string Select_item = (string)((ListBox)main_tag.TabPages["tabPage4"].Controls["listBox_Movetext_pattern" + j]).Items[i];
                    string[] get_load_num = Select_item.Split(",");
                    string get_List_num = get_load_num[0].ToString(); //char�^��string�^�ɕύX
                    all_movetext_id += get_List_num + " ";            //�ZID�������W�ς���
                    movetext_length += movetext_length + 2;           //�ǂݍ���byte�����Z�o
                }
                all_movetext_id += "00 00 ";
            }
            all_movetext_id = all_movetext_id.TrimEnd();                        //s.TrimEnd()�ōŌ�̗]�v�ȋ󔒂������ł���;
            label_after_Script_Pattern.Text = all_movetext_id;                  //�ύX��̃X�N���v�g���e���p�̃e�L�X�g�ɋL�^

            convert_val = String_exchange(all_movetext_id);
            LoadText = textBox__movetxt_pattern.Text;
            decValue = Convert.ToInt32(LoadText, 16);
            Write_binary_data(path, decValue, movetext_length, convert_val);    //�l���L��Address�ʒu�ɏ�������

        }
        private void button7_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�Z�X�N���v�g�ڍ׃T�u�t�H�[���\�������i���ʁj
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                             //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int decValue = Convert.ToInt32(textBox_Effect_address.Text, 16);                //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z                           
            int Address_length = int.Parse(numericUpDown_Effect_Address_length.Value.ToString()); //�ǂݍ���byte���l���擾�i�f�t�H���g�ł�100�j
            bool endFlag = checkBox_endFlag_check1.Checked;                                 //�I���X�N���v�g������ꍇ�I��������`�F�b�N
            bool jumpFlag = checkBox_jump_check1.Checked;
            string Mode_flg = "Effect";                                                        //�Ǎ��^�C�v��ݒ�
            string secret_flag = "";                                                              //����͓���t���O�͂Ȃ�
            string VBcheck = "Viewer";
            string LoadPattern = toolStripComboBox_Useinidata.Text; //�ǂ�ini�ǂ�ł邩�̔���
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
            //�Z�X�N���v�g�ڍ׃T�u�t�H�[���\�������i�G�t�F�N�g�j
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                                //label_Fullpath�Ƀp�X���L�ڂ���
            int decValue = Convert.ToInt32(textBox_Animation_address.Text, 16);                //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z                           
            int Address_length = int.Parse(numericUpDown_Animation_Address_length.Value.ToString()); //�ǂݍ���byte���l���擾�i�f�t�H���g�ł�100�j
            bool endFlag = checkBox_endFlag_check2.Checked;                                    //�I���X�N���v�g������ꍇ�I��������`�F�b�N
            bool jumpFlag = checkBox_jump_check2.Checked;
            string Mode_flg = "Animation";                                                        //�Ǎ��^�C�v��ݒ�
            string secret_flag = "";                                                                 //����͓���t���O�͂Ȃ�
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
            //�Z�X�N���v�g�r���_�[�T�u�t�H�[���\�������i���ʁj
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                             //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int decValue = Convert.ToInt32(textBox_Effect_address.Text, 16);                //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z                           
            int Address_length = int.Parse(numericUpDown_Effect_Address_length.Value.ToString()); //�ǂݍ���byte���l���擾�i�f�t�H���g�ł�100�j
            bool endFlag = checkBox_endFlag_check1.Checked;                                 //�I���X�N���v�g������ꍇ�I��������`�F�b�N
            bool jumpFlag = checkBox_jump_check1.Checked;
            string Mode_flg = "Effect";                                                        //�Ǎ��^�C�v��ݒ�
            string secret_flag = "";                                                              //����͓���t���O�͂Ȃ�
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
            //�Z�X�N���v�g�r���_�[�T�u�t�H�[���\�������i�A�j���j
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                              �@//toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int decValue = Convert.ToInt32(textBox_Animation_address.Text, 16);                //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z                           
            int Address_length = int.Parse(numericUpDown_Animation_Address_length.Value.ToString()); //�ǂݍ���byte���l���擾�i�f�t�H���g�ł�100�j
            bool endFlag = checkBox_endFlag_check2.Checked;                                    //�I���X�N���v�g������ꍇ�I��������`�F�b�N
            bool jumpFlag = checkBox_jump_check2.Checked;
            string Mode_flg = "Animation";                                                        //�Ǎ��^�C�v��ݒ�
            string secret_flag = "";                                                                 //����͓���t���O�͂Ȃ�
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
            //�Z�e�L�X�g�\�����X�g�̍X�V
            //-------------------------------------------------------------------------------
            //���ݎw�肳��Ă���p�^�[�����擾
            string selected_value = label_Select_Pattern_result.Text.ToString();

            //�ǂ̃p�^�[����I���������擾����
            string select_value = comboBox_Select_Pattern.SelectedValue.ToString();

            //���ݕ\�����Ă���ZID�Ƃ̏ƍ��p�f�[�^�����
            int Select_move_num = int.Parse(numericUpDown_Move_Number.Value.ToString());    //�e�L�X�g�擾�ł͈�O�̒l�ɂȂ邽��);
            string Move_id_2byte = Select_move_num.ToString("X").PadLeft(4, '0');  //4����16�i���ɕϊ�����
            Move_id_2byte = Move_id_2byte.Substring(2, 2) + " " + Move_id_2byte.Substring(0, 2);
            Move_id_2byte += "," + textBox_Move_Name.Text;  //�Z������{��f�[�^�Ŏ擾

            //�d������l������ꍇ�͗\�߃��X�g����폜���Ă��܂�
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

            //�ǉ����������X�g�ɒl���L�ڂ���
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
            label_Change_announce.Visible = true; //���f�������e�L�X�g�����Z�b�g

        }
        private void listBox_Movetext_pattern1_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //���ݑI�𒆂̋Z�e�L�X�g�\�����X�g�C���f�b�N�X���L��
            //-------------------------------------------------------------------------------
            //�I�𒆂̃��X�g�l���L��
            label_get_select_listbox.Text = "1," + listBox_Movetext_pattern1.SelectedIndex.ToString();
            //���X�g�{�b�N�X1�ȊO�̑S�Ă̑I��������
            listBox_Movetext_pattern2.SelectedIndex = -1;
            listBox_Movetext_pattern3.SelectedIndex = -1;
            listBox_Movetext_pattern4.SelectedIndex = -1;
        }
        private void listBox_Movetext_pattern2_Click(object sender, EventArgs e)
        {
            //�I�𒆂̃��X�g�l���L��
            label_get_select_listbox.Text = "2," + listBox_Movetext_pattern2.SelectedIndex.ToString();
            //���X�g�{�b�N�X2�ȊO�̑S�Ă̑I��������
            listBox_Movetext_pattern1.SelectedIndex = -1;
            listBox_Movetext_pattern3.SelectedIndex = -1;
            listBox_Movetext_pattern4.SelectedIndex = -1;
        }
        private void listBox_Movetext_pattern3_Click(object sender, EventArgs e)
        {
            //�I�𒆂̃��X�g�l���L��
            label_get_select_listbox.Text = "3," + listBox_Movetext_pattern3.SelectedIndex.ToString();
            //���X�g�{�b�N�X3�ȊO�̑S�Ă̑I��������
            listBox_Movetext_pattern1.SelectedIndex = -1;
            listBox_Movetext_pattern2.SelectedIndex = -1;
            listBox_Movetext_pattern4.SelectedIndex = -1;
        }
        private void listBox_Movetext_pattern4_Click(object sender, EventArgs e)
        {
            //�I�𒆂̃��X�g�l���L��
            label_get_select_listbox.Text = "4," + listBox_Movetext_pattern4.SelectedIndex.ToString();
            //���X�g�{�b�N�X4�ȊO�̑S�Ă̑I��������
            listBox_Movetext_pattern1.SelectedIndex = -1;
            listBox_Movetext_pattern2.SelectedIndex = -1;
            listBox_Movetext_pattern3.SelectedIndex = -1;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�I���Z���Z�e�L�X�g�\�����X�g����폜
            //-------------------------------------------------------------------------------
            string[] get_load_num = label_get_select_listbox.Text.Split(",");
            string get_List_num = get_load_num[0].ToString(); //char�^��string�^�ɕύX
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
            //�����E�B���h�E��\��������
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
            //�A�h���X�ꗗ�E�B���h�E��\��
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;
            string LoadSection = toolStripComboBox_Useinidata.Text;
            Form3 form3 = new Form3(path, LoadSection, "", "address", this);
            form3.Show();
        }
        private void button20_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�e�L�X�g�ꗗ�E�B���h�E��\��
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;
            string LoadSection = toolStripComboBox_Useinidata.Text;
            Form3 form3 = new Form3(path, LoadSection, "", "BattleText", this);
            form3.Show();
        }
        private void pictureBox_Coordinate_MouseClick(object sender, MouseEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //Picturebox�N���b�N���ɍ��W���擾����
            //-------------------------------------------------------------------------------

            //���ɋL�ڂ��ꂽPoint����������--------
            pictureBox_Coordinate.Refresh();

            //click�n�_�ɓ_��`��------------------
            Graphics g = pictureBox_Coordinate.CreateGraphics();  // PictureBox��Graphics�I�u�W�F�N�g���擾          
            int x = e.X;
            int y = e.Y;

            g.FillEllipse(Brushes.Red, x - 7, y - 7, 15, 15); // �_��`��

            //���W���l���擾-----------------------
            //double x_math = Math.Round (x / 1.5);�@//��ʔ{����1.5�{���Ă���̂Ŏl�̌ܓ��Ōv�Z���Ȃ���
            //double y_math = Math.Round (y / 1.5);
            x = x / 3;
            y = y / 3;

            //�������W����v�Z�������΍��W���L��
            int Center_X = Convert.ToInt32(x) - int.Parse(label_Center_num1.Text);
            if (Math.Sign(Center_X) == -1)
            {
                Center_X = 65536 + Center_X; //�l�������̏ꍇ�͒l���t�]������
            }

            int Center_Y = Convert.ToInt32(y) - int.Parse(label_Center_num2.Text);
            if (Math.Sign(Center_Y) == -1)
            {
                Center_Y = 65536 + Center_Y; //�l�������̏ꍇ�͒l���t�]������
            }

            string x_val = (x.ToString("X").PadLeft(4, '0'));   //int.Parse("123"),2���ɂ��Ȃ��ƃo�O��̂�
            x_val = x_val.Substring(2, 2) + " " + x_val.Substring(0, 2);
            string y_val = (y.ToString("X").PadLeft(4, '0'));
            y_val = y_val.Substring(2, 2) + " " + y_val.Substring(0, 2);
            textBox_pointaA_X.Text = x_val.ToString();
            textBox_pointaA_Y.Text = y_val.ToString();

            x_val = (Center_X.ToString("X").PadLeft(4, '0'));   //int.Parse("123"),2���ɂ��Ȃ��ƃo�O��̂�
            x_val = x_val.Substring(2, 2) + " " + x_val.Substring(0, 2);
            y_val = (Center_Y.ToString("X").PadLeft(4, '0'));
            y_val = y_val.Substring(2, 2) + " " + y_val.Substring(0, 2);
            textBox_pointaB_X.Text = x_val.ToString();
            textBox_pointaB_Y.Text = y_val.ToString();

        }

        private void pictureBox_Coordinate_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //Picturebox���_�u���N���b�N���ɍ��W�𒆐S�_�Ƃ��Ď擾���A��������������
            //-------------------------------------------------------------------------------
            Point center = e.Location;
            // ���̒������`-----------------------
            int length = Math.Min(pictureBox_Coordinate.Width, pictureBox_Coordinate.Height) * 2;

            // ���Z�b�g�Ő��������邽�߁A�摜�Q�ƌĂяo�����C���[�W�ɒ��ڐ����L�ڂ���
            pictureBox_Coordinate.Image = new Bitmap(pictureBox_Coordinate.Width, pictureBox_Coordinate.Height);
            Graphics g = Graphics.FromImage(pictureBox_Coordinate.Image);

            g.DrawLine(Pens.Red, center.X - length, center.Y, center.X + length, center.Y);
            g.DrawLine(Pens.Red, center.X, center.Y - length, center.X, center.Y + length);

            //bbackground�w�i�ɉ摜��Zoom�`���Ŕ��f������
            string BG_num = label_SelectBG_num.Text;
            string inifolder = toolStripComboBox_Useinidata.Text;
            string image_file = AppDomain.CurrentDomain.BaseDirectory + "image\\" + "Base_" + BG_num + ".png";
            Image backgroundImage = Image.FromFile(image_file);
            pictureBox_Coordinate.BackgroundImage = backgroundImage;
            pictureBox_Coordinate.BackgroundImageLayout = ImageLayout.Zoom;

            //���S�_�̍��W���L�^-----------------------
            int x = e.X;
            int y = e.Y;

            //double x_math = Math.Round(x / 1.5); //��ʔ{����1.5�{���Ă���̂Ŏl�̌ܓ��Ōv�Z���Ȃ���
            //double y_math = Math.Round(y / 1.5);

            x = x / 3;
            y = y / 3;

            label_Center_num1.Text = x.ToString(); //�v�Z�����������W���L��
            label_Center_num2.Text = y.ToString();

            string x_val = (x.ToString("X").PadLeft(4, '0'));   //int.Parse("123"),2���ɂ��Ȃ��ƃo�O��̂�
            x_val = x_val.Substring(2, 2) + " " + x_val.Substring(0, 2);

            string y_val = (y.ToString("X").PadLeft(4, '0'));
            y_val = y_val.Substring(2, 2) + " " + y_val.Substring(0, 2);

            textBox_pointaB_X.Text = x_val;
            textBox_pointaB_Y.Text = y_val;

        }

        private void comboBox_SelectBG_SelectedIndexChanged(object sender, EventArgs e)
        {
            //�ǂ̃p�^�[����I���������擾����
            string select_value = comboBox_SelectBG.SelectedValue.ToString();
            label_SelectBG_num.Text = select_value;

            string BG_num = label_SelectBG_num.Text; //�w�i�摜�̔ԍ����擾
            string inifolder = toolStripComboBox_Useinidata.Text;
            string image_file = AppDomain.CurrentDomain.BaseDirectory + "image\\" + "Base_" + BG_num + ".png";
            Image backgroundImage = Image.FromFile(image_file);
            pictureBox_Coordinate.BackgroundImage = backgroundImage;
            pictureBox_Coordinate.BackgroundImageLayout = ImageLayout.Zoom;


        }

        private void button15_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�^�C�v�����`���[�g��ʂ�\��
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                       //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int decValue = Convert.ToInt32(textBox_TypeChart_Address.Text, 16);      //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z                           
            string inifolder = toolStripComboBox_Useinidata.Text;                    //ini�̎�ނ�c��
            string Max_typeNum = numericUpDown_TypeNum.Value.ToString();             //�^�C�v�ő吔

            Form4 form4 = new Form4(path, decValue, inifolder, Max_typeNum, this);
            form4.Show();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�`���[�g�Z�[�u�{�^��������
            //-------------------------------------------------------------------------------
            string apply_Address = textBox_TypeChart_Address.Text;

            DialogResult result = MessageBox.Show(
                "�Q�ƃe�[�u���� " + apply_Address + " �ɍX�V��\r\n�ݒ�A�h���X�ɕύX���e���㏑�����܂����H",
                "System message",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //�����I�����ꂽ�����ׂ�
            if (result == DialogResult.OK)
            {

                try
                {
                    Chart_Tab_Save();
                    MessageBox.Show("�^�C�v�`���[�g�̏㏑�����������܂���.", "System message");

                    //�X�V���O�ɋL��
                    dataGridView_Change_log.Rows.Add(numericUpDown_Move_Number.Text,
                                                     textBox_Move_Name.Text,
                                                     "TypeChart",
                                                     label_Updatabefore_Chart.Text,
                                                     textBox_TypeChart_Address.Text,
                                                     DateTime.Now,
                                                     "�ڍ׏��",
                                                     label_before_Script_TypeChart.Text,
                                                     richTextBox_type_Chart.Text
                                                     );
                    label_Updatabefore_Chart.Text = textBox_TypeChart_Address.Text;   //�ύX�O�A�h���X�̓��e���X�V
                    label_before_Script_TypeChart.Text = richTextBox_type_Chart.Text; //�ύX�O�X�N���v�g�̓��e���X�V
                    label_Change_announce.Visible = false;
                }
                catch
                {
                    //Error�����������ꍇ�̓��b�Z�[�W�{�b�N�X��\�����ďI��
                    MessageBox.Show("�ۑ��Ɏ��s���܂����B\n���͂ł��Ȃ������񂪋L�ڂ���Ă��Ȃ����m�F���Ă��������B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                //�L�����Z���̏ꍇ�͉������Ȃ�
            }

        }

        private void Chart_Tab_Save()
        {
            //-------------------------------------------------------------------------------
            //�`���[�g�^�u�̃Z�[�u����
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;   //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            string LoadSection = "DataBase"; //�I������ini�̃f�[�^�x�[�X���擾����
            int TypeChartSelect = 0;

            string LoadText = "";
            int decValue = 0;

            for (int j = 1; j < 10; j++)
            {
                //9�ӏ�����TypeChartOffset��S�ĕύX�����鏈��
                var Loadkey = "TypeChart_Offset" + j;
                TypeChartSelect = MyGetPrivateProfileInt(LoadSection, Loadkey);
                LoadText = textBox_TypeChart_Address.Text;
                decValue = 0;

                if (LoadText == "")
                {
                    LoadText = "0";
                }

                decValue = Convert.ToInt32(LoadText, 16) + 134217728;    //�Ǎ��l��10�i���ɕϊ����Ă���ROM�̈�Ԓn�𑫂� 
                LoadText = decValue.ToString("X").PadLeft(8, '0');       //16�i���ɒ�����8�����l�߂ɕϊ�
                LoadText = LoadText + "00000000";                        //�󗓂������Error�ɂȂ�̂�00�Ŗ��߂Ă���


                LoadText = LoadText.Substring(6, 2) + " " + LoadText.Substring(4, 2) + " " +
                            LoadText.Substring(2, 2) + " " + LoadText.Substring(0, 2);

                var convert_val = String_exchange(LoadText);
                Write_binary_data(path, TypeChartSelect, 4, convert_val); //�Y���A�h���X��4byte��������
            }

            //---------------------------------------------------------------------
            //�`���[�g���e�𔽉f
            var ChartText = String_exchange(richTextBox_type_Chart.Text);

            int ChartScript_Length = 0;
            ChartScript_Length = ChartText.Length;

            int ChartScript = 0;
            decValue = Convert.ToInt32(textBox_TypeChart_Address.Text, 16); //�L�ڂ��ꂽAddress�̓e�L�X�g�Ȃ̂�10�i���ɖ߂��K�v������
            ChartScript = decValue;

            Write_binary_data(path, ChartScript, ChartScript_Length, ChartText);

        }
        private void Get_ini_FolderName()
        {
            toolStripComboBox_Useinidata.Items.Clear(); //�ߋ��ɓ����Ă���data���������獢��̂őS�������Ă���

            //setting�ȉ��̑S�Ẵt�H���_�����擾����
            string[] iniFileFolder = System.IO.Directory.GetDirectories(
                                     AppDomain.CurrentDomain.BaseDirectory + "Setting",
                                     "*", System.IO.SearchOption.AllDirectories);

            foreach (string name in iniFileFolder)
            {
                //Setting�t�H���_���̑S�Ẵt�@�C�������擾���Ēǉ�����
                string[] ininame = name.Split("\\");           //��������
                string lastWord = ininame[ininame.Length - 1]; //�Ō�̗v�f���擾����
                toolStripComboBox_Useinidata.Items.Add(lastWord);
            }
            toolStripComboBox_Useinidata.SelectedIndex = 0; //�����\���ōŏ���ini��ǂݍ���
        }

        private void Get_LastReadFile()
        {
            //-------------------------------------------------------------------------------
            //�ŋߎg�����t�@�C�����X�g���쐬���鏈��
            //-------------------------------------------------------------------------------
            mun_RecentlyUsedFiles.DropDownItems.Clear(); //�ߋ��ɓ����Ă���data���������獢��̂őS�������Ă���

            for (int i = 1; i < 6; i++)
            {
                //ini�t�@�C���̓��e���L���X�g���ĕϐ��Ɋi�[
                string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\Referent_File.txt";    //ini����R�[�h�̌^���擾 
                StringBuilder sb = new StringBuilder(1024);
                int res = GetPrivateProfileString("Last_read_path", "File_Full_Path" + i, "-", sb, (uint)sb.Capacity, iniFileName); //��p�̃e�L�X�gini����f�[�^���Ăяo��
                string Get_LastReadFile_Pass = sb.ToString();

                if (Get_LastReadFile_Pass == "")
                {
                    //�󗓂̏ꍇ�͒ǉ����Ȃ�
                    mun_RecentlyUsedFiles.DropDownItems.Add(i + ".");
                }
                else
                {
                    //�t���p�X����t�@�C�������擾
                    string GetFileName = Path.GetFileName(Get_LastReadFile_Pass);
                    mun_RecentlyUsedFiles.DropDownItems.Add(i + "." + GetFileName);
                }

                //�Ǎ��񂾃p�X�����x���Ɏw�肳����
                main_tag.TabPages["tabPage7"].Controls["label_Recently_ReadPath_" + i].Text = Get_LastReadFile_Pass;

            }
        }
        private void Output_LastReadFile()
        {
            //-------------------------------------------------------------------------------
            //�Ǎ��񂾃p�X���ŋߎg�����t�@�C�����X�g�p��ini�ɏ������ޏ���
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

            //���C���������ݏ���
            for (int i = 5; i > 0; i--)
            {
                if (i == 1)
                {
                    res = WritePrivateProfileString("Last_read_path", "File_Full_Path" + i, path, fp);
                }
                else
                {
                    //5����k���Ďw�肷��,
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
            //�ŋߎg�p�����t�@�C���v���_�E�����������ۂ̏���
            //-------------------------------------------------------------------------------
            var selectedIndex = e.ClickedItem.Text.Split(".");
            var selected_value = selectedIndex[0];
            var selected_path = selectedIndex[1];

            if (selected_path != "-")
            {
                //�I�������l�������̒l����Ȃ����Ƃ��m�F���ĊJ��
                string path = main_tag.TabPages["tabPage7"].Controls["label_Recently_ReadPath_" + selected_value].Text;
                toolStripStatusLabel__Fullpath.Text = path;

                OpenFile(path); //�t�@�C�����J�������ɐi��
            }
        }

        private void Extract_tools_setting()
        {
            //-------------------------------------------------------------------------------
            //�O���c�[����URL��ini���甽�f�����鏈��
            //-------------------------------------------------------------------------------
            //ini�t�@�C���̓��e���L���X�g���ĕϐ��Ɋi�[
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\Referent_File.txt";    //ini����R�[�h�̌^���擾 
            StringBuilder sb = new StringBuilder(1024);
            int res = GetPrivateProfileString("Extract_tool_path", "Stirling_Path", "-", sb, (uint)sb.Capacity, iniFileName); //��p�̃e�L�X�gini����f�[�^���Ăяo��
            string Get_ExtractTool_Pass = sb.ToString();

            textBox_StilringURL.Text = Get_ExtractTool_Pass;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�`���[�g�A�h���X�ǂݍ��ݐ�ύX�{�^��
            //-------------------------------------------------------------------------------
            string path = toolStripStatusLabel__Fullpath.Text;                                         //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���
            int decValue = Convert.ToInt32(textBox_TypeChart_Address.Text, 16);                        //�Ǎ��ʒu���e�L�X�g�f�[�^�̂��߁A16�i������10�i���ɍČv�Z                           

            richTextBox_type_Chart.Text = Read_binary_toFF(decValue); //���O��Address�l����ff�܂ł�binary�f�[�^���擾����

        }

        private void dataGridView_Change_log_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //-------------------------------------------------------------------------------
            //���O�ڍו\���{�^���������̏���
            //-------------------------------------------------------------------------------
            if (e.ColumnIndex == 6)
            {
                try
                {
                    string path = toolStripStatusLabel__Fullpath.Text;                       //toolStripStatusLabel__Fullpath�Ƀp�X���L�ڂ���                          
                    string inifolder = toolStripComboBox_Useinidata.Text;                    //ini�̎�ނ�c��
                    string Max_typeNum = numericUpDown_TypeNum.Value.ToString();             //�^�C�v�ő吔

                    int rowIndex = e.RowIndex;  //���ݗ���擾

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
            //�O���c�[���Q�ƁE�X�V��������
            //-------------------------------------------------------------------------------
            string path = textBox_StilringURL.Text;
            //�J�����g�f�B���N�g���̎擾�͉��L�̕��͂ōs���i�����[�X���ŎQ�Ƃ�null�ɂȂ邱�Ƃ�����j
            string fp = Environment.CurrentDirectory.ToString();
            fp += "\\" + "Setting\\Referent_File.txt";
            int res;

            res = WritePrivateProfileString("Extract_tool_path", "Stirling_Path", path, fp);
        }

        private string Open_dialog(string beforePath)
        {
            //-------------------------------------------------------------------------------
            //�_�C�A���O���Ăяo���đΏۂ̃t�@�C���̃p�X���擾����
            //-------------------------------------------------------------------------------

            //�Q�Ɛ�̃p�X��
            string path = "";

            //�_�C�A���O�Ăяo��
            //�t�@�C���_�C�A���O�𐶐�����
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "�t�@�C�����J��";
            op.InitialDirectory = @"C:\";
            //op.Filter = "rom files(*.rom; *.gba)| *.rom; *.gba|data files (*.txt;*.bin;*.dat)|*.txt;*.bin;*.dat";
            op.FilterIndex = 1;

            //�I�[�v���t�@�C���_�C�A���O��\������
            DialogResult dialog = op.ShowDialog();

            //�u�J���v�{�^�����I�����ꂽ�ۂ̏���
            if (dialog == DialogResult.OK)
            {
                path = op.FileName;
            }
            //�u�L�����Z���v���̏���
            else if (dialog == DialogResult.Cancel)
            {
                return (beforePath);
            }

            return (path);
        }

        private void button_URL_Search1_Click(global::System.Object sender, global::System.EventArgs e)
        {
            string beforePath = textBox_StilringURL.Text;
            //dialog����p�X���擾����
            string GetPath = Open_dialog(beforePath);
            textBox_StilringURL.Text = GetPath;
        }

        private void mnu_Movedata_import_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�\���f�[�^�̃C���|�[�g
            //-------------------------------------------------------------------------------

            //�Q�Ɛ�̃p�X��
            string path = "";

            //�_�C�A���O�Ăяo��
            //�t�@�C���_�C�A���O�𐶐�����
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "�t�@�C�����J��";
            op.InitialDirectory = @"C:\";
            op.Filter = "mse files(*.mse)| *.mse;";
            op.FilterIndex = 1;

            //�I�[�v���t�@�C���_�C�A���O��\������
            DialogResult dialog = op.ShowDialog();

            //�u�J���v�{�^�����I�����ꂽ�ۂ̏���
            if (dialog == DialogResult.OK)
            {
                path = op.FileName;
            }
            //�u�L�����Z���v���̏���
            else if (dialog == DialogResult.Cancel)
            {
                return;
            }

            mseFile_Open(path);

        }

        private void mseFile_Open(string path)
        {
            //-------------------------------------------------------------------------------
            //�C���|�[�g���̕��������̏���
            //-------------------------------------------------------------------------------
            toolStripProgressBar1.Value = 0;//�v���O���X�o�[���L��

            //�I�������p�X�̃f�[�^���J��
            System.IO.StreamReader sr = new System.IO.StreamReader(@path, System.Text.Encoding.GetEncoding("UTF-8"));
            //���e�����ׂēǂݍ���
            string Get_mse = sr.ReadToEnd();
            //����
            sr.Close();

            string[] mse_Data = Get_mse.Split(",");

            string MoveInfo = mse_Data[0];
            string[] info_data = MoveInfo.Split(" ");

            //�e�L�X�g�{�b�N�X���ڍX�V
            //textBox_label_moveID�ɔ��f����l��ݒ�
            label_data_01.Text = info_data[0];
            textBox_label_moveID.Text = Convert.ToInt32(info_data[0], 16).ToString();

            //textBox_Movepower�ɔ��f����l��ݒ�
            label_data_02.Text = info_data[1];
            textBox_Movepower.Text = Convert.ToInt32(info_data[1], 16).ToString();

            //textBox_Accuracy�ɔ��f����l��ݒ�
            label_data_04.Text = info_data[3];
            textBox_Accuracy.Text = Convert.ToInt32(info_data[3], 16).ToString();

            //textBox_PP�ɔ��f����l��ݒ�
            label_data_05.Text = info_data[4];
            textBox_PP.Text = Convert.ToInt32(info_data[4], 16).ToString();

            //textBox_Prob�ɔ��f����l��ݒ�
            label_data_06.Text = info_data[5];
            textBox_Prob.Text = Convert.ToInt32(info_data[5], 16).ToString();

            //----------------------------------------------------------------
            //�R���{�{�b�N�X���ڍX�V
            //comboBox_moveType�ɔ��f����l��ݒ�
            label_data_03.Text = info_data[2];
            comboBox_moveType.SelectedIndex = Convert.ToInt32(info_data[2], 16);//comboBox_moveType�ɔ��f

            //comboBox_Attack_range�ɔ��f����l��ݒ�
            label_data_07.Text = info_data[6];
            comboBox_Attack_range.SelectedIndex = bin_Getflg(Convert.ToString(Convert.ToInt32(info_data[6], 16), 2).PadLeft(8, '0'));

            //comboBox_Priority�ɔ��f����l��ݒ�
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
            }   //���艟���Ńt���O�ݒ�
            comboBox_Priority.SelectedIndex = get_flgnum;

            //comboBox_Move_Category�ɔ��f����l��ݒ�
            label_data_11.Text = info_data[10];
            comboBox_Move_Category.SelectedIndex = Convert.ToInt32(info_data[10], 16);

            //----------------------------------------------------------------
            //�`�F�b�N�{�b�N�X���ڍX�V
            //Move_flag�ɔ��f����l��ݒ�
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

            //Ability_flag_1�ɔ��f����l��ݒ�
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

            //Ability_flag_2�ɔ��f����l��ݒ�
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

            toolStripProgressBar1.Value = 50;//�v���O���X�o�[���L��

            //----------------------------------------------------------------
            textBox_Move_Name.Text = mse_Data[1];
            textBox_Move_text.Text = mse_Data[2];

            label_Read_Script.Text = mse_Data[3];
            richTextBox_Effect_Script.Text = mse_Data[3];

            label_Animation_Script.Text = mse_Data[4];
            richTextBox_Animation_Script.Text = mse_Data[4];

            toolStripProgressBar1.Value = 100;//�v���O���X�o�[���L��
        }

        private void mnu_Movedata_export_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�\���f�[�^�̃G�N�X�|�[�g����
            //-------------------------------------------------------------------------------
            MessageBox.Show("MSE�t�@�C���̃G�N�X�|�[�g���������܂���.", "System message");
            try
            {
                string Export_data = "";
                Export_data += textBox_moveinfo.Text + ",";
                Export_data += textBox_Move_Name.Text + ",";
                Export_data += textBox_Move_text.Text + ",";
                Export_data += label_Read_Script.Text + ",";
                Export_data += label_Animation_Script.Text + ",";

                string FileName = AppDomain.CurrentDomain.BaseDirectory + textBox_Move_Name.Text + ".mse";

                //�������ރt�@�C�������ɑ��݂��Ă���ꍇ�́A�㏑������
                System.IO.StreamWriter sw = new System.IO.StreamWriter(@FileName, false, System.Text.Encoding.GetEncoding("UTF-8"));

                sw.Write(Export_data);
                //����
                sw.Close();

            }
            catch
            {
                //Error�����������ꍇ�̓��b�Z�[�W�{�b�N�X��\�����ďI��
                MessageBox.Show("�ۑ��Ɏ��s���܂����B\n���͂ł��Ȃ������񂪋L�ڂ���Ă��Ȃ����m�F���Ă��������B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void numericUpDown_Battle_Text_Num_ValueChanged(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //�ԍ��J�E���g�{�b�N�X�Ɏw��f�[�^����������
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
                //��^���ő吔��Ǎ�
                int Max_BattleText_Number = 0;
                string Loadkey = "Max_BattleText_Number";
                Max_BattleText_Number = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

                //��^��Address�̊J�n�ʒu��ǂݍ���

                int minus_pointa = 0;
                if (decValue < 385) { Loadkey = "BattleText_Table1"; minus_pointa = 12; } else { Loadkey = "BattleText_Table2"; minus_pointa = 385; }
                BattleText_Table = (int)GetPrivateProfileInt(LoadSection, Loadkey, 0, iniFileName);

                //�J�nID��12����Ȃ��ߌ��Z����
                decValue = decValue - minus_pointa;

                //info_data����Ǎ��ޒ�^���I�t�Z�b�g���w�肵�ēǍ���Ƃ��đg�݂Ȃ���
                BattleText_Table = BattleText_Table + (4 * decValue);
                LoadText = Read_binary_data(path, BattleText_Table + 3, 1) + Read_binary_data(path, BattleText_Table + 2, 1) +
                           Read_binary_data(path, BattleText_Table + 1, 1) + Read_binary_data(path, BattleText_Table, 1); //�������r�b�O�G���f�B�A���ɂ��ĕ\��
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
            //�ǂݍ��񂾊J�n�ʒu�A�h���X����FF���ǂݍ��܂��܂ł̒l���e�L�X�g�ŕԂ�        
            var code_data = "";
            var mass_code_data = "";
            var Apply_data = "";

            string path = toolStripStatusLabel__Fullpath.Text;
            string inifolder = toolStripComboBox_Useinidata.Text;
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";

            var reader = new FileStream(path, FileMode.Open);
            byte[] data = new byte[decValue + 256];    //�I�[�����܂��Ă��Ȃ��׎�荇����256byte�ǂݍ���
            reader.Read(data, 0, data.Length);
            reader.Close();

            //------------------------------------------------------------------------------------------
            //�J�nAddress����+256byte�܂ł͈̔͂�ff������܂œǍ���ŕ����ɕϊ�����
            for (int i = decValue; i < data.Length; i++)
            {
                code_data = string.Format("{0:x2}", data[i]);

                //ini�t�@�C���̓��e���L���X�g���ĕϐ��Ɋi�[
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
                        res = GetPrivateProfileString("MB_FC_kanji", code_data, "[������]", sb, (uint)sb.Capacity, iniFileName);//�ϐ��̈ꗗ��ini����擾
                        Apply_data = Apply_data + sb.ToString();
                        mass_code_data += "fc " + code_data + " ";
                        i = i + 2; //2byte�]�v�ɐi�܂���
                        break;

                    case "fd":
                        code_data = string.Format("{0:x2}", data[i + 1]); //1byte��̒l���擾����
                        res = GetPrivateProfileString("MB_FD_BattleText", code_data, "[������]", sb, (uint)sb.Capacity, iniFileName);//�ϐ��̈ꗗ��ini����擾
                        Apply_data = Apply_data + sb.ToString();
                        mass_code_data += "fd " + code_data + " ";
                        i = i + 1; //1byte�]�v�ɐi�܂���
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
                        return; //�I�����������������\�L���I��������

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
            //FD�ϐ����X�g�{�b�N�X�̑I����ς����ۂ̏���
            //-------------------------------------------------------------------------------
            int Select_list_num = 0;
            int res;

            Select_list_num = listBox_FD_text.SelectedIndex;         //�I�𒆂̏ꏊ�����X�g��

            string code_data = Select_list_num.ToString().PadLeft(2, '0'); //2���̕����񐔎��ɕϊ�
            code_data = Select_list_num.ToString("X").PadLeft(2, '0');     //2����16�i���ɕϊ�����

            string inifolder = toolStripComboBox_Useinidata.Text;
            string code_iniFileName = AppDomain.CurrentDomain.BaseDirectory + "Setting\\" + inifolder + "\\mozi.ini";    //ini����R�[�h�̌^���擾 
            StringBuilder db = new StringBuilder(64);

            //Script�R�[�h�̓����ǂݍ���
            res = GetPrivateProfileString("MB_FD_BattleText", code_data, "�ǂݍ��݃G���[", db, (uint)db.Capacity, code_iniFileName);

            richTextBox_Convert_Text_info.Text += db + " ";

        }

        private void button22_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //��^���@�X�N���v�g����{��ɕϊ�
            //-------------------------------------------------------------------------------
            string LoadText = richTextBox_Battle_Text.Text.TrimEnd();
            LoadText = LoadText.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            LoadText = FontData_exchange(LoadText);
            richTextBox_Convert_Text_info.Text = LoadText + "��";

        }

        private void button23_Click(global::System.Object sender, global::System.EventArgs e)
        {
            //-------------------------------------------------------------------------------
            //��^���@���{����X�N���v�g�ɕϊ�
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
                            case "�s����":
                                str_FD += "FA";
                                break;

                            case "�X�V":
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
            //ini�����ʂ�\��������
            //-------------------------------------------------------------------------------
            Form6 form6 = new Form6();
            form6.Show();

        }
    }
}
