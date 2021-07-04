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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RangeOfInfluenceDetection
{
    public partial class Source : Form
    {
        // 表示中画面のソース情報
        private SourceInfo sourceInfo;
        // テスト用コード
        private Roslyn roslynTest = new Roslyn();

        public Source(SourceInfo info)
        {
            InitializeComponent();

            sourceInfo = info;

            // ディスプレイの高さ
            int h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            
            // ディスプレイの幅
            int w = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            
            // ディスプレイのサイズに合わせてフォームのサイズを変更
            this.Size = new Size(h, w);

            // 文字列をテキストボックスに表示
            txtSource.Text = sourceInfo.codeAfter ?? sourceInfo.codeBefore;

            // フォーム名を変更
            this.Text = sourceInfo.fileName.TrimStart(' ', '★');

            // 影響範囲があるコードの場合は影響範囲をマークアップ
            sourceInfo.MarkInfos.ForEach(x => codeMarking(x));
        }

        /// <summary>
        /// 影響範囲を検索を押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsSeach_Click(object sender, EventArgs e)
        {
            // ソースの変更があった場合にtreeとrootを取得する
            UpdateSourceInfo();

            // 影響範囲を検索
            roslynTest.Parse();

            // ファイル一覧画面を更新する
            AppInfomation.ListForm.RefreshItem();

            // 変更箇所の影響範囲をマーキングする
            sourceInfo.MarkInfos.ForEach(x => codeMarking(x));
        }

        /// <summary>
        /// ファイル一覧に戻る押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsPre_Click(object sender, EventArgs e)
        {
            AppInfomation.ListForm.Visible = true;
            this.Visible = false;
        }

        private void tsmFileSelect_Click(object sender, EventArgs e)
        {
            // ダイアログを開く
            frmFileDialog fileDialog = new frmFileDialog(this);
            this.Enabled = false;
            fileDialog.Show(this);
        }

        /// <summary>
        /// 対象の文字列をマーキング
        /// </summary>
        /// <param name="start">マーキング開始文字数</param>
        /// <param name="end">マーキング開始文字数</param>
        private void codeMarking(MarkInfo markInfo)
        {
            // 指定行を選択する
            txtSource.Select(markInfo.markStart, markInfo.markEnd);

            // 選択する文字数を取得
            txtSource.SelectionLength = markInfo.markLength;

            // 色を赤にする
            txtSource.SelectionColor = Color.Red;

            // 背景を黄色にする
            txtSource.SelectionBackColor = Color.Yellow;

            if (txtSource.SelectionFont != null)
            {
                // 太字にする
                txtSource.SelectionFont = new Font(txtSource.SelectionFont, FontStyle.Bold);
            }
        }

        /// <summary>
        /// SourceInfoの更新処理
        /// </summary>
        private void UpdateSourceInfo()
        {
            string text = string.Empty;
            
            for(int i = 0; i < txtSource.Lines.Length; i++)
            {
                text += txtSource.Lines[i];

                if (txtSource.Lines.Length -1 != i)
                text += "\r\n";
            }

            if (text != sourceInfo.codeAfter && text != sourceInfo.codeBefore)
            {
                // コードを変更していた場合
                // 変更後のツリーとルートノードを取得する
                sourceInfo.codeAfter = text;
                sourceInfo.treeAfter = CSharpSyntaxTree.ParseText(text);
                sourceInfo.rootAfter = sourceInfo.treeAfter.GetCompilationUnitRoot();
                sourceInfo.methodAfter.Visit(sourceInfo.rootAfter);
                sourceInfo.codeChange = true;
            }
        }

        /// <summary>
        /// AppInfomation.Derivedの更新
        /// </summary>
        private void RefreshDerived()
        {
            // 変更後のルートリストを作成
            List<CompilationUnitSyntax> rootList = new List<CompilationUnitSyntax>();
            Program.SourceList.ForEach(x => rootList.Add(x.rootAfter));

            // クラスリストを作成
            List<ClassDeclarationSyntax> classList = new List<ClassDeclarationSyntax>();
            rootList.ForEach(x => classList.AddRange(x.ChildNodes().ToList().OfType<ClassDeclarationSyntax>()));

            // 継承しているクラスを辞書として登録＜クラス名，継承しているクラス＞
            Dictionary<string, string> map = new Dictionary<string, string>();
            foreach(ClassDeclarationSyntax classSyntax in classList)
            {
                List<BaseListSyntax> bases = classSyntax.ChildNodes().OfType<BaseListSyntax>().ToList();
                bases.ForEach(x => map.Add(x.Types.First().ToString(), classSyntax.Identifier.ToString()));
            }

            // 各クラスに対応した継承クラスを作成する
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

            // ベースクラスを一意にとる
            List<string> classNames = map.Keys.Distinct().ToList();
            foreach(string name in classNames)
            {
                List<string> list = map.Where(x => x.Key.ToString() == name).ToList().ConvertAll(x => x.Value.ToString());
                dic.Add(name,list);
            }

            // 派生クラスを継承している場合も考慮してDictionryを再作成
            foreach(KeyValuePair<string, List<string>> kvp in dic)
            {
                foreach(string str in kvp.Value)
                {
                    KeyValuePair<string, List<string>> pair = dic.Where(x => x.Key.ToString() == str).First();
                    if (pair.Value.Count != 0)
                        kvp.Value.AddRange(pair.Value);                
                }
            }
        }

        /// <summary>
        /// 画面終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Source_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppInfomation.ListForm.Visible = true;

            UpdateSourceInfo();
        }

        // テスト用メソッド
        private void TestTest()
        {
            Program.SourceList.FindAll(x => x.fileName == "aaa").ForEach(x => x.TestMethod());
        }

        public class A
        {

        }
    }
}
