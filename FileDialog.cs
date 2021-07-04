using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RangeOfInfluenceDetection
{
    public partial class frmFileDialog : Form
    {
        // メンバ変数
        Source m_source;
        // 選択されていたファイル名を取得
        string m_fileName = string.Empty;
        //選択されていたファイル拡張子を取得
        string m_extension = string.Empty;

        public frmFileDialog(Source source)
        {
            InitializeComponent();
            // メンバ変数に格納する
            m_source = source;
            m_fileName = m_source.Name;
        }

        /// <summary>
        /// 戻るボタン押下時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturn_Click(object sender, EventArgs e)
        {
            // 呼び出し元を活性状態にする
            m_source.Enabled = true;
            this.Close();
        }

        /// <summary>
        /// 実行ボタン押下時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, EventArgs e)
        {
            // ファイル名チェック
            // 指定ファイルと変更されている箇所を検索する(戻り値変更された箇所の場所を特定できる情報)
            using (FileStream fs = new FileStream(txtComparisonPath.Text, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs);
                string Text = sr.ReadToEnd();
                // 指定されたパスのファイルを読み込んだところまで実装

            }
                // 呼び出し元を活性状態にする
                m_source.Enabled = true;
        }

        /// <summary>
        /// ファイルパス選択ダイアログを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            string fileContent = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // デフォルトは選択したソリューションが存在するディレクトリ
                openFileDialog.InitialDirectory = AppInfomation.SearchDirectory; // "c:\\"
                // フィルタは無し
                //openFileDialog.Filter = $"{pathSelect.M_display()} ({pathSelect.M_extension()})|*.txt|All files (*.*)|*.*";

                // ダイアログでOKボタン押下の場合
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    txtComparisonPath.Text = filePath;
                }
            }
        }
    }
}
