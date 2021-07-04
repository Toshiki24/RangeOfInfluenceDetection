using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeOfInfluenceDetection
{
    public static class AppInfomation
    {
        #region プロパティ

        /// <summary>
        /// frmPathSelect(ソリューション選択画面)のインスタンス
        /// </summary>
        public static frmPathSelect PathSelectForm { get; set; }

        /// <summary>
        /// frmAllFileList(ファイル選択画面)
        /// </summary>
        public static frmAllFileList ListForm { get; set; }

        /// <summary>
        /// ファイル検索用パス
        /// </summary>        
        public static string SearchDirectory { get; set; }

        /// <summary>
        /// 選択された拡張子
        /// </summary>        
        public static string Extension { get; set; }

        /// <summary>
        /// 画面表示用拡張子文字列
        /// </summary>        
        public static string Display { get; set; }

        /// <summary>
        /// ファイル名リスト
        /// </summary>
        public static List<string> FileNameList { get; set; }

        /// <summary>
        /// ファイル一覧画面へのマーキング有無 
        /// </summary>
        public static bool Marking { get; set; }

        /// <summary>
        /// 派生クラス一覧
        /// </summary>
        public static Dictionary<string, List<string>> Derived {get; set;}
        #endregion

    }
}
