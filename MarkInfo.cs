using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeOfInfluenceDetection
{
    public class MarkInfo
    {
        public string fileName;
        public int markStart;
        public int markEnd;
        public int markLength;

        // コンストラクタ
        public MarkInfo()
        {

        }

        // コンストラクタ
        public MarkInfo(string fname, int start, int end)
        {
            this.fileName = fname;
            this.markStart = start -1;
            this.markEnd = end;
            this.markLength = this.markEnd - this.markStart;

            Program.SourceList.Find(x => x.fileName == this.fileName).codeChange = true;
        }
    }
}
