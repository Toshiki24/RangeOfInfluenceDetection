using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeOfInfluenceDetection
{
    public class SourceInfo
    {
        #region メンバー

        /// <summary>
        /// ファイル名
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 変更前本文
        /// </summary>
        public string codeBefore { get; set; }

        /// <summary>
        /// 変更後本文
        /// </summary>
        public string codeAfter { get; set; }

        /// <summary>
        /// コードの変更有無
        /// </summary>
        public bool codeChange { get; set; }

        /// <summary>
        /// 変更前ソースツリー
        /// </summary>
        public SyntaxTree treeBefore { get; set; }

        /// <summary>
        /// 変更後ソースツリー
        /// </summary>
        public SyntaxTree treeAfter { get; set; }

        /// <summary>
        /// 変更前ルートノード
        /// </summary>
        public CompilationUnitSyntax rootBefore { get; set; }

        /// <summary>
        /// 変更後ルートノード
        /// </summary>
        public CompilationUnitSyntax rootAfter { get; set; }

        /// <summary>
        /// 変更前メソッドコレクター
        /// </summary>
        public Roslyn.MethodCollector methodBefore { get; set; } = new Roslyn.MethodCollector();


        /// <summary>
        /// 変更前メソッドコレクター
        /// </summary>
        public Roslyn.MethodCollector methodAfter { get; set; } = new Roslyn.MethodCollector();

        /// <summary>
        /// 変更前ユージングコレクター
        /// </summary>
        public Roslyn.UsingCollector usingBefore { get; set; }

        /// <summary>
        /// 変更後ユージングコレクター
        /// </summary>
        public Roslyn.UsingCollector usingAfter { get; set; }

        /// <summary>
        /// マーキング情報
        /// </summary>
        public List<MarkInfo> MarkInfos { get; set; } = new List<MarkInfo>();

        /// <summary>
        /// 削除メソッド検索用リスト
        /// </summary>
        public List<string> DeleteList { get; set; } = new List<string>();



        #endregion

        #region コンストラクタ

        /// <summary>
        /// 初期実行時のコンストラクタ
        /// </summary>
        /// <param name="name"></param>
        public SourceInfo(string name)
        {
            this.fileName = name;
        }

        //　テスト用
        public bool TestMethod()
        {
            return this.fileName != null ? true : false;
        }
        #endregion
    }
}
