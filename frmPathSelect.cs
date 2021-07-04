using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RangeOfInfluenceDetection
{
    public partial class frmPathSelect : Form
    {

        // 比較対象のパスの拡張子
        const string SOLUTION_EXTENSION = ".sln";

        // 対応している言語一覧
        const string C_SHARP_DiSPLAY = "C#";
        const string C_SHARP_EXTENSION = "cs";
        
        public frmPathSelect()
        {
            InitializeComponent();
            // combobox add
            cmbLanguages.Items.Add(C_SHARP_DiSPLAY);
            // default selected
            cmbLanguages.SelectedIndex = 0;

        }

        private void btnRun_Click(object sender, EventArgs e)
        {

            // 入力欄にあるソリューションの解析
            // パスの確認
            string path = solutionPath.Text;
            int pathCount = path.Length;
            int num = SOLUTION_EXTENSION.Length;
            string comparionString = string.Empty;
            if (path.Length > SOLUTION_EXTENSION.Length)
            {
                comparionString = path.Substring(pathCount - num, num);
            }
            // ファイル拡張子が違った場合
            if (String.IsNullOrEmpty(comparionString) || comparionString != SOLUTION_EXTENSION)
            {
                MessageBox.Show("ソリューションパスを入力してください。");
                return;
            }
            // 検索用のディレクトリのパスを取得
            AppInfomation.SearchDirectory = Path.GetDirectoryName(path);

            //コンボボックスで選択している言語の拡張子を取得
            string extension = cmbLanguages.Text;

            switch (extension)
            {
                case C_SHARP_DiSPLAY:   // C#の場合
                    AppInfomation.Extension = "*." + C_SHARP_EXTENSION;
                    AppInfomation.Display = C_SHARP_DiSPLAY;
                    break;

            }
            
            // ファイル一覧を取得
            string[] fileNames = Directory.GetFiles(AppInfomation.SearchDirectory, AppInfomation.Extension, SearchOption.TopDirectoryOnly);

            if (fileNames.Length == 0)
            {
                MessageBox.Show("指定されたパスにソースファイルが存在しません。");
                return;
            } 
           
            // 取得した文字列の加工
            foreach (string str in fileNames)
            {
                Program.SourceList.Add(new SourceInfo(str.Replace(AppInfomation.SearchDirectory + "\\", "")));
            }

            AppInfomation.PathSelectForm = this;
            // ファイル一覧画面を立ち上げる
            frmAllFileList allFileList = new frmAllFileList();

            allFileList.Show();

            this.Visible = false;
        }

        // テスト用コード
        public class A
        {

        }
    }
}
