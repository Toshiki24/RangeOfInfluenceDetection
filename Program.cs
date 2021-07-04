using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RangeOfInfluenceDetection
{
    static class Program
    {
        #region プロパティ
        
        /// <summary>
        /// ソースファイル情報(解析時に使用)
        /// </summary>
        public static List<SourceInfo> SourceList { get; set; } = new List<SourceInfo>();

        #endregion

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()  
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmPathSelect());
        }
    }
}
