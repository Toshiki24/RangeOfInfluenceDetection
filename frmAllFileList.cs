using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RangeOfInfluenceDetection
{
    public partial class frmAllFileList : Form
    {
        #region 宣言
        // 影響範囲情報

        #endregion

        /// <summary>
        /// メイン画面から呼ばれる際のコンストラクタ
        /// </summary>リストボックスに表示されする文言リスト
        public frmAllFileList ()
        {
            InitializeComponent();

            // MarkInfoの初期化
            Program.SourceList.ForEach(x => x.MarkInfos.Clear());

            foreach (SourceInfo info in Program.SourceList)
            {
                // コードを読み込む
                using (StreamReader reader = new StreamReader(Path.Combine(AppInfomation.SearchDirectory,
                    info.fileName)))
                {
                    info.codeBefore = reader.ReadToEnd();
                }

                // ツリーとルートノードを取得する
                info.treeBefore = CSharpSyntaxTree.ParseText(info.codeBefore);
                info.rootBefore = info.treeBefore.GetCompilationUnitRoot();
                info.methodBefore.Visit(info.rootBefore);

                // リストアイテムに追加する
                lstResult.Items.Add(info.fileName);
            }
            btnCodeDisplay.Enabled = false;
        }

        private void btnSln_Click(object sender, EventArgs e)
        {
            AppInfomation.PathSelectForm.Visible = true;
            this.Close();
        }

        /// <summary>
        /// コードを表示をクリック
        /// </summary>
        private void btnCodeDisplay_Click(object sender, EventArgs e)
        {

            // 選択されているファイルを文字列として読み取る
            string selected = lstResult.SelectedItem.ToString().TrimStart(' ','★');

            SourceInfo info = Program.SourceList.Find(x => x.fileName == selected);
            
            Source source = new Source(info);
            source.Show();
            this.Visible = false;
        }

        /// <summary>
        /// 影響範囲があるリストアイテムをマーク
        /// </summary>
        public void RefreshItem()
        {
            lstResult.Items.Clear();
            
            // 影響範囲だったソースファイルはリストボックスに表示する際、目印を付ける
            foreach (SourceInfo info in Program.SourceList)
            {
                if (info.codeChange)
                {
                    // ディクショナリーに登録されている場合は目印を付けてリストに登録する
                    lstResult.Items.Add("★  " + info.fileName);
                }
                else
                {
                    lstResult.Items.Add("     " + info.fileName);
                }
            }
        }

        // RUNしているタスクを終了する
        private void frmAllFileList_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.StackFrame caller = new System.Diagnostics.StackFrame(1);

            // TODO: ×やボタンで閉じたときの制御を見る
            if (caller.GetMethod().ReflectedType.Name != this.GetType().FullName)
                AppInfomation.PathSelectForm.Close();
        }

        /// <summary>
        /// 表示時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAllFileList_Shown(object sender, EventArgs e)
        {
            // 自身をAppInfoに追加
            AppInfomation.ListForm = this;
        }

        /// <summary>
        /// 変更をリセットする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            // MarkInfoの初期化
            Program.SourceList.ForEach(x => x.MarkInfos.Clear());

            // ファイルの読み直し
            foreach (SourceInfo info in Program.SourceList)
            {
                // コードを読み込む
                using (StreamReader reader = new StreamReader(Path.Combine(AppInfomation.SearchDirectory,
                    info.fileName)))
                {
                    info.codeBefore = reader.ReadToEnd();
                }

                info.codeChange = false;

                // ツリーとルートノードを取得する
                info.treeBefore = CSharpSyntaxTree.ParseText(info.codeBefore);
                info.rootBefore = info.treeBefore.GetCompilationUnitRoot();
                info.methodBefore = new Roslyn.MethodCollector();
                info.methodBefore.Visit(info.rootBefore);

                // 変更後情報(After)をリセットする
                info.codeAfter = null;
                info.treeAfter = null;
                info.rootAfter = null;
                info.methodAfter = new Roslyn.MethodCollector();
            }
            // ★をリセットする
            RefreshItem();
        }

        /// <summary>
        ///  グリッドの選択アイテム変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstResult.SelectedIndex != -1)
                // アイテムが選択されている場合
                btnCodeDisplay.Enabled = true;
            else
                btnCodeDisplay.Enabled = false;
        }
    }
}
