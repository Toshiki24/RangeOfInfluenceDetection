using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace RangeOfInfluenceDetection
{
    class matchingInfo
    {
        // 違っていた場合その値を格納する
        string Indentifier;      // 識別名
        List<string> parmerter;  // パラメーター
        string disclosureRange;  // 公開範囲
        string mainText;         // 本文
        string returntype;       // 戻り値
    }
}
