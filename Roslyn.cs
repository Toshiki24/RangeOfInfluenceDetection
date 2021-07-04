using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

// Roslynで使用
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RangeOfInfluenceDetection
{
    public class Roslyn
    {
        #region 宣言

        // 改行コード変換毎に発生するギャップ
        private const int GAP_COUNT = 2;

        // TextBoxでの改行コード
        private const string LINE_FEED_CODE = "\n";

        // 終了時のステータス
        //enum endStatus
        //{
        //    Initial,                // 初期値
        //    ConditionalExpression,  // 条件式で終了した場合
        //    AbnormalTermination     // 異常終了した場合
        //}

        #endregion


        #region Collector
        // Usingディレクティブ
        public class UsingCollector : CSharpSyntaxWalker
        {
            // 学習用コード6用のプロパティ
            public ICollection<UsingDirectiveSyntax> Usings { get; } = new List<UsingDirectiveSyntax>();

            public override void VisitUsingDirective(UsingDirectiveSyntax node)
            {
                if (node.Name.ToString() != "System" &&
                    !node.Name.ToString().StartsWith("System."))
                {
                    string name = node.Name.ToString();
                    this.Usings.Add(node);
                    SyntaxNode node1 = node.Parent;
                    string str3 = node1.ToString();
                }
            }
        }

        // メソッド宣言
        public class MethodCollector : CSharpSyntaxWalker
        {
            // 学習用コード6用のプロパティ
            public ICollection<MethodDeclarationSyntax> Methods { get; } = new List<MethodDeclarationSyntax>();

            public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                // メソッドの判定条件
                //if (node.Name.ToString() != "System" &&
                //    !node.Name.ToString().StartsWith("System."))
                //{
                //string name = node..ToString();
                this.Methods.Add(node);
                SyntaxNode node1 = node.Parent;
                string str3 = node1.ToString();
                //}
            }
        }

        #endregion


        /// <summary>
        /// 解析を行うメソッド
        /// </summary>
        /// <returns>影響範囲情報リスト</returns>
        public void Parse()
        {
            foreach (SourceInfo info in Program.SourceList)
            {
                // 変更されていない場合は次のソースを比較する
                if (info.codeAfter == null || info.codeBefore == info.codeAfter)
                    continue;

                // 影響範囲の特定
                ExamineTheChanges(info);
            }
        }

        /// <summary>
        /// 構文チェック
        /// </summary>
        /// <param name="text">解析対象のcode</param>
        /// <returns>構文エラーの場合はtrue</returns>
        public bool SyntaxCheck(string text)
        {
            var tree = CSharpSyntaxTree.ParseText(text);
            foreach (var item in tree.GetDiagnostics())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 変更箇所の特定
        /// </summary>
        /// <returns></returns>
        private void ExamineTheChanges(SourceInfo info)
        {
            // 変更されたメソッド名リスト
            List<string> methodList = info.methodBefore.Methods.ToList().ConvertAll(x => x.Identifier.ToFullString());

            // 変更されたBodyリスト
            List<string> bodyList = info.methodBefore.Methods.ToList().ConvertAll(x => x.Body.ToFullString());

            // 照合したメソッド名リスト(変更後のメソッド名が格納される)
            List<string> CollatedMethodList = new List<string>();

            // 変更語ノードのメソッド宣言に対して処理を行う
            foreach (MethodDeclarationSyntax dec in info.methodAfter.Methods)
            {
                // 本文の終わりまで改行コードの文字列の差を取得する
                string targetBody = info.codeAfter.Substring(0, dec.Body.Span.End);
                int gapCountBody = CountOf(targetBody, LINE_FEED_CODE);

                // 識別子の終わりまで改行コードの文字列の差を取得する
                string targetStart = info.codeAfter.Substring(0, dec.Identifier.Span.Start);
                int gapCountStart = CountOf(targetStart, LINE_FEED_CODE);

                if (!methodList.Contains(dec.Identifier.ToFullString()))
                {
                    // 同じメソッド名が存在しない場合

                    if (!bodyList.Contains(dec.Body.ToFullString()))
                    {
                        // 内容が同じメソッドが存在しない場合はすべてをマーキング
                        info.MarkInfos.Add(new MarkInfo(info.fileName, dec.Span.Start - gapCountStart + 1,
                                                     dec.Span.End - gapCountBody));
                    }
                    else
                    {
                        // 内容が同じメソッドが存在する(メソッド名の変更)

                        // 呼び出し箇所をマーキング

                        info.MarkInfos.Add(new MarkInfo(info.fileName, dec.Identifier.Span.Start - gapCountStart + 1,
                             dec.Identifier.Span.End - gapCountStart));

                        string beforeIdentifier = info.methodBefore.Methods.ToList().
                            Find(x => x.Body.ToString() == dec.Body.ToString()).Identifier.ToFullString();

                        // 呼び出し箇所をリストに追加
                        Parallel.ForEach(Program.SourceList, (source) =>
                        {
                            IdentifierChange(source, beforeIdentifier, beforeIdentifier.Length, dec);
                        });

                        // リスト更新
                        CollatedMethodList.Add(dec.Identifier.ToString());
                    }
                }
                else
                {
                    // 同じメソッド名が存在する場合

                    // リスト更新
                    CollatedMethodList.Add(dec.Identifier.ToString());

                    MethodDeclarationSyntax beforeMethod = info.methodBefore.Methods.ToList().Find(x => x.Identifier.ToString() == dec.Identifier.ToString());
                    if (beforeMethod.Body.ToString() != dec.Body.ToString())
                    {
                        // 内容が同じメソッドが存在しない

                        // 内容すべてをMarkInfoに登録する
                        info.MarkInfos.Add(new MarkInfo(info.fileName, dec.Body.Span.Start - gapCountStart, dec.Body.Span.End - gapCountBody));
                        ContentChanges(info, dec, beforeMethod);
                    }
                }
                // アクセスビリティが変更されているかチェック
                //if (アクセスビリティが変更されている場合)
                //    MethodAccessibilityChanges();　//無効になった呼び出し箇所をマーク
            }
        }

        /// <summary>
        /// 変数のクラス名調査
        /// </summary>
        /// <param name="method">検索対象のMethodNode</param>
        /// <param name="syntax">型のチェック対象</param>
        /// <param name="changedClassName">変更箇所のクラス名</param>
        /// <param name="changeSyntax">変更されたNode</param>
        /// <returns></returns>
        private string CheckClassName(MethodDeclarationSyntax method, SyntaxNode syntax,
            string changedClassName, SyntaxNode changeSyntax)
        {
            string typeName = ValiableToTypeName(method, syntax);

            if (!String.IsNullOrWhiteSpace(typeName))
                return typeName;

            var simples = method.DescendantNodes().OfType<SimpleLambdaExpressionSyntax>()
                .Where(x => x.Span.Start <= syntax.SpanStart && x.Span.End >= syntax.SpanStart);

            // リンク内の変数から呼ばれた場合
            if (SearchSyntax(simples, syntax))
            {
                var accessNode = simples.First();

                // 呼び出し元がリンクの変数だった場合		
                if (syntax.ToString() == accessNode.Parameter.Identifier.ToString())
                {
                    SyntaxNode workNode = accessNode.Parent.Parent.Parent.ChildNodes().First();

                    // 呼び出し先リスト
                    List<IdentifierNameSyntax> workList = new List<IdentifierNameSyntax>();
                    while (true)
                    {
                        // 列挙体の呼び出しを取得
                        var wNode = workNode.ChildNodes();
                        var list = wNode.OfType<IdentifierNameSyntax>().Reverse();
                        workList.AddRange(list);

                        if (wNode.All(x => x is IdentifierNameSyntax))
                            break;

                        workNode = wNode.First();
                    }
                    // クラス名を検索する
                    string className = GetLinkClassName(workList);
                    if (!String.IsNullOrWhiteSpace(className))
                        return className;
                }
            }

            // 検索対象のクラスnode取得
            var targetClass = GetDeclarationNode<ClassDeclarationSyntax>(syntax);

            // 影響範囲候補クラスのフィールドをすべて検索する
            var menbers = targetClass.DescendantNodes().OfType<FieldDeclarationSyntax>();
            List<VariableDeclaratorSyntax> nodes = new List<VariableDeclaratorSyntax>();

            // すべてのフィールドの宣言部分を取得
            foreach (FieldDeclarationSyntax menbersyntax in menbers)
            {
                var variables = menbersyntax.DescendantNodes();
                nodes.AddRange(variables.OfType<VariableDeclaratorSyntax>());
            }

            // メンバ変数として定義されているか確認
            string fieldName =  ValiableFieldCheck(nodes, syntax);
            if (!String.IsNullOrWhiteSpace(fieldName))
                return fieldName;

            //// 他クラスのパブリックメンバーを参照する
            string MenberName = PublicMenberCheck(nodes, syntax, changedClassName);
            if (!String.IsNullOrWhiteSpace(MenberName))
                return MenberName;

            return null;
        }

        /// <summary>
        /// パラメータと戻り値の照合
        /// </summary
        private void ContentChanges(SourceInfo info, MethodDeclarationSyntax syntaxAfter, MethodDeclarationSyntax syntaxBefore)
        {
            // パラメータが違う場合
            List<string> paramList = syntaxBefore.ParameterList.Parameters.ToList().ConvertAll(x => x.ToString());

            foreach (ParameterSyntax parameter in syntaxAfter.ParameterList.Parameters)
            {
                // 識別子の終わりまで改行コードの文字列の差を取得する
                string target = info.codeAfter.Substring(0, parameter.Span.Start);
                int gapCount = CountOf(target, LINE_FEED_CODE);

                if (!paramList.Contains(parameter.ToString()))
                    info.MarkInfos.Add(new MarkInfo(info.fileName, parameter.Span.Start - gapCount + 1, parameter.Span.End - gapCount));
            }

            // 戻り値が違う場合
            if (syntaxBefore.ReturnType.ToString() != syntaxAfter.ReturnType.ToString())
            {
                // 識別子の終わりまで改行コードの文字列の差を取得する
                string target = info.codeAfter.Substring(0, syntaxAfter.Span.Start);
                int gapCount = CountOf(target, LINE_FEED_CODE);
                info.MarkInfos.Add(new MarkInfo(info.fileName, syntaxAfter.ReturnType.Span.Start + 1 - gapCount, syntaxAfter.ReturnType.Span.End - gapCount));
            }
        }

        /// <summary>
        /// メソッド呼び出し先のチェック(クラス、パラメータ)
        /// </summary>
        /// <param name="info">検索するSorceInfo(foreachでSorceListを回している)</param>
        /// <param name="nodes">検索対象の文字列を使用しているNodeリスト(Afterがある場合はAfterから検索、
        /// ない場合Beforeから検索したNode)</param>
        /// <param name="method">現在検索中のメソッド(Afterがある場合はAfterから検索、ない場合Beforeから検索したNode)</param>
        /// <param name="beforeIdentifire">変更前のメソッド名(Before)</param>
        /// <param name="syntaxNode">変更されたメソッドnode(After)</param>
        /// <returns></returns>
        private List<MarkInfo> CallConfirmation(SourceInfo info, List<SyntaxNode> nodes, MethodDeclarationSyntax method,
            string beforeIdentifire, SyntaxNode syntaxNode)
        {
            List<MarkInfo> marks = new List<MarkInfo>();

            // 対象のnode分処理を繰り返す(メソッドの中の１文がリストになっている)
            foreach (SyntaxNode node in nodes)
            {
                // 呼び出しクラスのチェック

                if (!MethodCheck(info, node, method, syntaxNode))
                    continue;

                // テスト用コード
                continue;

                // 引数違いのメソッドではないかのチェック
                // 影響範囲候補のnodeとそのnodeが含まれるメソッドの本文を引数に使用
                if (!OverloadCheck(info, node, method, beforeIdentifire, (MethodDeclarationSyntax)syntaxNode))
                    continue;

                // オフセットの取得
                string code = info.codeAfter ?? info.codeBefore;
                string target = code.Substring(0, node.Span.End);
                int gapCount = CountOf(target, LINE_FEED_CODE);

                // marksへの追加
                marks.Add(new MarkInfo(info.fileName, node.Span.Start - gapCount + 1, node.Span.End - gapCount));
            }

            return marks;
        }
    

        /// <summary>
        /// 呼び出しクラスのチェック
        /// </summary>
        /// <param name="info">現在処理しているSourceInfo</param>
        /// <param name="node">チェック対象の呼び出しnode(Afterがある場合はAfterから検索、ない場合Beforeから検索したNode)</param>
        /// <param name="method">チェック対象の呼び出しMethodNode(Afterがある場合はAfterから検索、ない場合Beforeから検索したNode)</param>
        /// <param name="syntax">変更したnode(After)</param>
        /// <returns></returns>
        private bool MethodCheck(SourceInfo info, SyntaxNode node, MethodDeclarationSyntax method, SyntaxNode syntax)
        {
            // メソッドが変更されたクラス名
            string changeClassName = string.Empty;
            string candidate = string.Empty;

            // 変更箇所のクラス名を取得
            var root = info.rootAfter ?? info.rootBefore;

            changeClassName = GetClassName(syntax);

            // 影響範囲候補のnodeのクラス名を取得する
            candidate = GetClassName(node);

            // 変更されたクラス名が影響範囲候補のクラス名と同じ(静的メソッド、同クラスからの呼び出しの場合)
            if (changeClassName == candidate)
                return true;

            // 宣言(変数)部分を取得する
            SyntaxNode declaration = node.Parent.ChildNodes().ToList().First();
            string className = string.Empty;

            List<MethodDeclarationSyntax> methodList = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            MethodDeclarationSyntax targetMethod = methodList.ToList().Find(x => x.Span.Start <= declaration.SpanStart && x.Span.End >= declaration.Span.End);

            // 上記が代入されているクラス名を探す
            className = CheckClassName(targetMethod, declaration, changeClassName, syntax);

            // TODO: 変更されたメソッドが現在のnodeから参照範囲か確認(アクセシビリティチェック)

            // 上記で取得したnodeの判定
            if (className == changeClassName)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 引数違いのメソッドではないかのチェック
        /// </summary>
        /// <param name="node">チェック対象の呼び出しnode</param>
        /// <returns></returns>
        private bool OverloadCheck(SourceInfo info, SyntaxNode node, MethodDeclarationSyntax method, string beforeIdentifire, MethodDeclarationSyntax methodNode)
        {
            // TODO:エラー発生箇所あり
            // 変更したメソッドのパラメータの型リスト
            List<string> chengedList = new List<string>();
            chengedList = methodNode.ParameterList.Parameters.ToList().ConvertAll(x => x.ToString());

            // nodeのパラメータの型リスト
            List<string> pramList = new List<string>();
            ArgumentListSyntax node1 = (ArgumentListSyntax)node.Parent.Parent.ChildNodes().ToList().Last();
            chengedList = node1.Arguments.ToList().ConvertAll(x => x.ToString());

            if (chengedList.All(x => pramList.Contains(x)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 呼び出し箇所をリストに追加(識別子が変更されている場合)
        /// </summary>
        /// <param name="info">コード情報</param>
        /// <param name="before">変更前のメソッド名(Before)</param>
        /// <param name="AfterCount">identifierの文字列カウント</param>
        /// <param name="syntaxNode">変更されたメソッドnode(After)</param>
        private void IdentifierChange(SourceInfo info, string before, int AfterCount, SyntaxNode syntaxNode)
        {
            List<MarkInfo> marks = new List<MarkInfo>();
            List<SyntaxNode> syntaxNodes = new List<SyntaxNode>();  // 検索対象nodeリスト
            List<SyntaxNode> resultNodes = new List<SyntaxNode>();  // 検索結果nodeリスト

            var methods = info.methodAfter.Methods.Count != 0 ? info.methodAfter.Methods : info.methodBefore.Methods;

            foreach (MethodDeclarationSyntax method in methods)
            {
                string bodyStr = method.Body.ToString();

                if (!bodyStr.Contains(before))
                    continue;

                syntaxNodes.Clear();

                // 対象の文字列を使用しているnodeを取得する
                syntaxNodes = method.ChildNodes().ToList();
                resultNodes = SearchTree(syntaxNodes, before);

                // 探し出したメソッド呼び出し文のExpresstionで変数名を取得する
                if (resultNodes.Count == 0)
                    continue;

                // 呼び出し箇所のチェック、MarkInfoに登録
                List<MarkInfo> markInfo = CallConfirmation(info, resultNodes, method, before, syntaxNode);
                marks.AddRange(markInfo);
            }

            // リストに追加する
            info.MarkInfos.AddRange(marks);
        }

        /// <summary>
        /// メソッド内で定義されている変数の型を取得する
        /// </summary>
        /// <param name="method">検索対象のメソッド</param>
        /// <param name="syntax">検索する変数node</param>
        /// <returns></returns>
        private string ValiableToTypeName(MethodDeclarationSyntax method, SyntaxNode syntax)
        {
            // パラメータに存在するか確認する
            var param = method.ParameterList.Parameters;
            foreach (ParameterSyntax syntax1 in param)
            {
                if (syntax1.Identifier.ToString() == syntax.ToString())
                    return syntax1.Type.ToString();
            }

            // インスタンス化しているクラス名を取得するパターン
            List<VariableDeclaratorSyntax> syntaxNodes = method.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList();
            List<VariableDeclaratorSyntax> serchNodes = syntaxNodes.FindAll(x => x.Identifier.ValueText == syntax.ToString());

            // 呼び出しているクラス名が取得できた場合
            if (serchNodes.Count == 0)
                return string.Empty;

            // 検索用の変数を定義
            int count = serchNodes.Where(x => x.SpanStart < syntax.SpanStart).Select(x => x.SpanStart).Max();
            VariableDeclaratorSyntax variableSyntax = serchNodes.Find(x => x.SpanStart == count);
            string variable = syntax.ToString();
            bool AbnormalFlag = false;

            // 変数の定義位置を検索する
            while (variableSyntax.Initializer.Value.ToString() != variable)
            {
                // 代入している変数名を取得する
                variableSyntax = syntaxNodes.Find(x => x.SpanStart == count);
                variable = variableSyntax.Initializer.Value.ToString();

                // 次のループのために変数を更新する
                serchNodes = syntaxNodes.FindAll(x => x.Identifier.ValueText == variable);

                if (serchNodes.Count == 0)
                {
                    AbnormalFlag = true;
                    break;
                }

                count = serchNodes.Where(x => x.SpanStart < variableSyntax.SpanStart).Select(x => x.SpanStart).Max();
            }

            if (!AbnormalFlag)
            {
                variableSyntax = syntaxNodes.Find(x => x.SpanStart == count);
                return variableSyntax.Initializer.Value.ChildNodes().ToList().OfType<IdentifierNameSyntax>().First().Identifier.ValueText;
            }

            return string.Empty;
        }

        /// <summary>
        /// フィールドリストから変数nodeの定義を検索して型取得
        /// </summary>
        /// <param name="nodes">フィールドリスト</param>
        /// <param name="syntax">検索する変数node</param>
        /// <returns></returns>
        private string ValiableFieldCheck(List<VariableDeclaratorSyntax> nodes,SyntaxNode syntax)
        {
            // メンバ変数として定義されているか確認
            if (!nodes.ConvertAll(x => x.Identifier.ToString()).Contains(syntax.ToString()))
                return string.Empty;

            // 左辺の型を取得する
            var initializer = nodes.Find(x => x.Identifier.ToString() == syntax.ToString()).Initializer;
            var identifier = initializer.Parent.Parent.ChildNodes().OfType<IdentifierNameSyntax>().ToList().First();
            if (identifier != null)
            {
                // 以降のコードをデバッグするため一時的にコメントアウト
                return identifier.Identifier.ToString();
            }
         
            return string.Empty;
        }

        /// <summary>
        /// 全クラスから変数が定義されている箇所を検索
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="syntax"></param>
        /// <returns></returns>
        private string PublicMenberCheck(List<VariableDeclaratorSyntax> nodes, SyntaxNode syntax, string changedClassName)
        {
            List<ClassDeclarationSyntax> Classes = new List<ClassDeclarationSyntax>();
            
            foreach (SourceInfo info in Program.SourceList)
            {
                var root = info.rootAfter ?? info.rootBefore;
                Classes.AddRange(root.DescendantNodes().OfType<ClassDeclarationSyntax>());
            }

            // 他クラスのパブリックメンバーを参照する
            foreach (ClassDeclarationSyntax classDeclaration in Classes)
            {
                // 影響範囲候補クラスのフィールドをすべて検索する
                var fields = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>();
                List<VariableDeclaratorSyntax> nodes2 = new List<VariableDeclaratorSyntax>();
                foreach (FieldDeclarationSyntax fieldsyntax in fields)
                {
                    // すべてのフィールドの宣言部分を取得
                    nodes.AddRange(fieldsyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>());
                }

                // メンバ変数として定義されているか確認
                if (!nodes.ConvertAll(x => x.Identifier.ToString()).Contains(syntax.ToString()))
                    continue;

                // 左辺の型を取得する
                VariableDeclaratorSyntax variableNode = nodes.Find(x => x.Identifier.ToString() == syntax.ToString());
                var initializer = variableNode.Initializer;
                var identifier = initializer.Parent.Parent.ChildNodes().OfType<IdentifierNameSyntax>().First();
                var str = identifier.Parent.Parent.ToString();
                

                if (identifier == null)
                    continue;

                if (str.Contains("public") && str.Contains("internal"))
                {
                    // アクセス可能なクラスの場合
                    return identifier.Identifier.ToString();
                }

                // 派生クラスを確認する
                // 検索対象のクラス名を取得する
                string className = GetClassName(syntax);

                // 検索対象のクラスが継承しているクラスリストを取得
                List<string> derivedList = AppInfomation.Derived[changedClassName];

                // 派生クラスだった場合は影響ありと判断
                if (derivedList.Contains(className))
                    return identifier.Identifier.ToString();
            }

            return string.Empty;
        }

        #region 検索メソッド

        /// <summary>
        /// 指定文字列がどれだけ含まれるか検索を行う
        /// </summary>
        /// <param name="code">検索コード</param>
        /// <param name="target">検索する文字列</param>
        /// <returns></returns>
        private int CountOf(string code, string target)
        {
            int index = 0;
            int count = 0;

            while (index != -1)
            {
                count++;
                index = code.IndexOf(LINE_FEED_CODE, index + target.Length);
            }
            return count - 1;
        }

        /// <summary>
        /// 指定した文字を含むnodeを最小nodeまで検索する
        /// </summary>
        /// <param name="syntaxNodes">検索するNode</param>
        /// <param name="str">検索する文字列</param>
        /// <returns></returns>
        private List<SyntaxNode> SearchTree(List<SyntaxNode> syntaxNodes, string str)
        {
            // 取得したnode格納用リスト
            List<SyntaxNode> getNodes = new List<SyntaxNode>();

            // 取得した結果取得用リスト
            List<SyntaxNode> returnNodes = new List<SyntaxNode>();

            foreach (SyntaxNode node in syntaxNodes)
            {
                getNodes = node.ChildNodes().ToList();

                //if (getNodes.Count == 1)
                //{
                //    if (node.Kind().ToString() == "VariableDeclarator")
                //        returnNodes.Add(node);
                //}

                if (getNodes.Count == 0)
                {
                    if (node.ToString() == str)
                        returnNodes.Add(node);
                }
                else
                    returnNodes.AddRange(SearchTree(getNodes, str));
            }
            return returnNodes;
        }

        /// <summary>
        /// リスト内に対象のnodeが存在するかチェックする
        /// </summary>
        /// <param name="syntaxNodes">検索対象のnode</param>
        /// <param name="syntax">検索するnode</param>
        /// <returns>リスト内に検索するnodeが存在する場合はtrue</returns>
        private bool SearchSyntax<T>(T syntaxNodes, SyntaxNode syntax)
            where T : IEnumerable<SyntaxNode>
        {
            List<SyntaxNode> getNodes = new List<SyntaxNode>();

            foreach (var node in syntaxNodes)
            {
                SyntaxNode castNode = node as SyntaxNode;

                getNodes = castNode.ChildNodes().ToList();

                if (getNodes.Count == 0)
                {
                    if (castNode == syntax)
                        return true;
                }
                else
                {
                    if (SearchSyntax(getNodes, syntax))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// クラス名を調査する
        /// </summary>
        /// <param name="syntaxNode">クラス名を調べたいNode</param>
        /// <returns>nodeが存在するクラス名</returns>
        private string GetClassName(SyntaxNode syntaxNodes)
        {
            // ファイルのルートnodeを格納するリスト
            List<CompilationUnitSyntax> sourcesNodes = new List<CompilationUnitSyntax>();

            //全クラス一覧を格納するリスト
            List<ClassDeclarationSyntax> classNodes = new List<ClassDeclarationSyntax>();

            // 一時保存用ディクショナリー
            Dictionary<int, string> workdDic = new Dictionary<int, string>();

            Program.SourceList.ForEach(x => sourcesNodes.Add(x.rootAfter ?? x.rootBefore));

            // 取得したnode格納用リスト
            foreach (CompilationUnitSyntax node in sourcesNodes)
            {
                var classes = node.DescendantNodes().ToList().OfType<ClassDeclarationSyntax>();
                classNodes.AddRange(classes);
            }

            // クラス単位で取得する
            foreach (ClassDeclarationSyntax node in classNodes)
            {
                if (SearchSyntax(new List<SyntaxNode> { node }, syntaxNodes))
                    workdDic.Add(node.ToString().Length, node.ToString());
            }

            if (workdDic.Count == 0)
                return string.Empty;

            int max = workdDic.Keys.Max();

            return workdDic[max];
        }

        /// <summary>
        /// 宣言nodeを取得する
        /// </summary>
        /// <param name="name">検索するクラス名</param>
        /// <returns></returns>
        private T GetDeclarationNode <T>(string name)
            where T : TypeDeclarationSyntax
        {
            // 文字列と同じ識別子のクラスnodeを取得する
            List<CompilationUnitSyntax> unitSyntaxes = new List<CompilationUnitSyntax>();
            List<T> declaration = new List<T>();

            Program.SourceList.ForEach(x => unitSyntaxes.Add(x.rootAfter ?? x.rootBefore));

            unitSyntaxes.ForEach(delegate (CompilationUnitSyntax syntax)
            {
                declaration.AddRange(syntax.DescendantNodes().OfType<T>());
            });

            return declaration.Find(x => x.Identifier.ToString() == name) ?? null;
        }

        /// <summary>
        /// メソッド宣言nodeを取得する
        /// </summary>
        /// <param name="name">検索する識別子</param>
        /// <returns></returns>
        private FieldDeclarationSyntax GetFieldDeclarationNode(SyntaxNode serchDeclarationNode, string name)
        {
            var field = serchDeclarationNode.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            return field.Find(x => x.Declaration.ToString() == name);
        }

        /// <summary>
        /// 宣言nodeを取得する
        /// </summary>
        /// <param name="name">検索する識別子</param>
        /// <returns></returns>
        private List<T> GetMenberDeclarationNode<T>(string name)
            where T : MemberDeclarationSyntax
        {
            // 文字列と同じ識別子のnodeを取得する
            List<CompilationUnitSyntax> unitSyntaxes = new List<CompilationUnitSyntax>();
            List<T> declaration = new List<T>();

            Program.SourceList.ForEach(x => unitSyntaxes.Add(x.rootAfter ?? x.rootBefore));

            unitSyntaxes.ForEach(delegate (CompilationUnitSyntax syntax)
            {
                declaration.AddRange(syntax.DescendantNodes().OfType<T>());
            });

            return declaration.FindAll(x => x.ToString().Contains(name));
        }

        /// <summary>
        /// メソッド宣言nodeを取得する
        /// </summary>
        /// <param name="name">検索する識別子</param>
        /// <returns></returns>
        private MethodDeclarationSyntax GetMethodDeclarationNode(SyntaxNode serchDeclarationNode, string name)
        {
            var methods = serchDeclarationNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            return methods.Find(x => x.Identifier.ToString() == name);
        }

        /// <summary>
        /// 宣言nodeを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        private T GetDeclarationNode<T>(SyntaxNode node)
            where T : MemberDeclarationSyntax
        {
            // 文字列と同じ識別子のクラスnodeを取得する
            List<CompilationUnitSyntax> unitSyntaxes = new List<CompilationUnitSyntax>();
            List<T> declaration = new List<T>();

            Program.SourceList.ForEach(x => unitSyntaxes.Add(x.rootAfter ?? x.rootBefore));

            unitSyntaxes.ForEach(delegate (CompilationUnitSyntax syntax)
            {
                declaration.AddRange(syntax.DescendantNodes().OfType<T>());
            });
            
            return declaration.Find(x => SearchSyntax(new List<SyntaxNode>{ x },node ));
        }

        /// <summary>
        /// 宣言nodeを取得する
        /// </summary>
        /// <param name="node">検索対象node</param>
        private SyntaxNode GetDeclarationNode(SyntaxNode node)
        {
            string str = node.ToFullString();
            string[] accessArray = str.Split('.');
            
            var firstClassNode = node.ChildNodes().First();

            // nodeと同じ識別子のクラスnodeリストを取得
            List<ClassDeclarationSyntax> classes = GetMenberDeclarationNode<ClassDeclarationSyntax>(firstClassNode.ToString());

            if (classes.Count >= 2)
                return Search(accessArray);

            if (classes.Count == 1)
                return classes.First();

            List<StructDeclarationSyntax> structs = GetMenberDeclarationNode<StructDeclarationSyntax>(firstClassNode.ToString());

            if (structs.Count >= 2)
                return Search(accessArray);

            if (structs.Count == 1)
                return structs.First();

            return null;

            // 匿名メソッド
            SyntaxNode Search(string[] array)
            {
                SyntaxNode syntaxNode = null;
                ClassDeclarationSyntax classDeclaration = null;
                StructDeclarationSyntax structDeclaration = null;

                foreach (string name in array)
                {
                    if (syntaxNode == null)
                    {
                        classDeclaration = GetDeclarationNode<ClassDeclarationSyntax>(name);
                        structDeclaration = GetDeclarationNode<StructDeclarationSyntax>(name);
                    }
                    else
                    {
                        classDeclaration = syntaxNode.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList().Find(x => x.Identifier.ToString() == name);
                        structDeclaration = syntaxNode.DescendantNodes().OfType<StructDeclarationSyntax>().ToList().Find(x => x.Identifier.ToString() == name);
                    }

                    syntaxNode = (SyntaxNode)classDeclaration ?? structDeclaration;
                    classDeclaration = null;
                    structDeclaration = null;
                }
                return syntaxNode;
            }
        }

        /// <summary>
        /// 引数指定したnodeの型nodeを取得する(検索対象のフィールドかどうか)
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private SyntaxNode ValiableToClassOrStruct(SyntaxNode node)
        {
            //TODO:変数nodeから型名を検索する
            // 別で似たメソッドあり(リンク部分を別で切り出して一緒にする)
            // リンクではない
            
            MethodDeclarationSyntax methodNode = GetDeclarationNode<MethodDeclarationSyntax>(node);

            // メソッド内で定義されている場合(引数含む)
            string typeName = ValiableToTypeName(methodNode, node);
            if (!String.IsNullOrWhiteSpace(typeName))
                return GetDeclarationNode<MethodDeclarationSyntax>(node);

            // フィールド
            ClassDeclarationSyntax classNode = GetDeclarationNode<ClassDeclarationSyntax>(node);

            List<VariableDeclaratorSyntax> nodes = new List<VariableDeclaratorSyntax>();
            
            // 検索対象のクラスnode取得
            var targetClass = GetDeclarationNode<ClassDeclarationSyntax>(node);

            // 影響範囲候補クラスのフィールドをすべて検索する
            var menbers = targetClass.DescendantNodes().OfType<FieldDeclarationSyntax>();

            // すべてのフィールドの宣言部分を取得
            foreach (FieldDeclarationSyntax menbersyntax in menbers)
            {
                var variables = menbersyntax.DescendantNodes();
                nodes.AddRange(variables.OfType<VariableDeclaratorSyntax>());
            }

            // メンバ変数として定義されているか確認
            string fieldName = ValiableFieldCheck(nodes, node);
            if (!String.IsNullOrWhiteSpace(fieldName))
            {
                 var fieldSyntax = GetFieldDeclarationNode(targetClass,fieldName);
                VariableDeclaratorSyntax declarator = fieldSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

                if(fieldSyntax != null)
                {
                    // Fieldから宣言nodeを取得
                    var initializer = declarator.Initializer;
                    // 宣言nodeを取得する
                    return GetDeclarationNode(initializer);
                }
            }

            ////// 他クラスのパブリックメンバーを参照する
            //string MenberName = PublicMenberCheck(nodes, node, changedClassName);
            //if (!String.IsNullOrWhiteSpace(MenberName))
            //    return MenberName;
            // グローバル変数
            return methodNode;
        }

        //private SyntaxNode ValiableToClassOrStruct(SyntaxNode node)
        //{

        //}

        /// <summary>
        /// 指定した識別子のnodeを取得する(Programクラスnode、SourceList)
        /// </summary>
        /// <typeparam name="T">検索する</typeparam>
        /// <param name="SearchSyntax">検索対象node</param>
        /// <param name="str">プロパティ、メソッド名</param>
        /// <returns></returns>
        private SyntaxNode MenberCheck<T>(T SearchSyntax, string str)
            where T : SyntaxNode
        {
            // クラス(構造体)内のメソッドをすべて取得
            var methodNode = GetMethodDeclarationNode(SearchSyntax, str);

            // strとして宣言されているメソッドが存在した場合 
            if (methodNode != null)
                return methodNode;

            // クラス(構造体)内のフィールドをすべて取得
            var fieldNode = GetFieldDeclarationNode(SearchSyntax, str);

            // strとして宣言されているフィールドが存在した場合 
            if (fieldNode != null)
                return fieldNode;

            return null;
        }

    /// <summary>
    /// nodeが存在するクラスのアクセスビリティの調査
    /// </summary>
    /// <param name="syntaxNode">クラスのアクセスビリティを調べたいNode</param>
    /// <returns>nodeクラスのアクセスビリティ(内部クラスの場合外側から深部に向けて格納していく)</returns>
    private Dictionary<string, string> GetClassAccessibility(SyntaxNode syntaxNodes)
    {
        // ファイルのルートnodeを格納するリスト
        List<CompilationUnitSyntax> sourcesNodes = new List<CompilationUnitSyntax>();

            // 全クラス一覧を格納するリスト
            List<ClassDeclarationSyntax> classNodes = new List<ClassDeclarationSyntax>();

            // アクセスビリティを格納するディクショナリー
            Dictionary<string, string> accessibility = new Dictionary<string, string>();

            // 一時保存用ディクショナリー
            Dictionary<ClassDeclarationSyntax, int> workdDic = new Dictionary<ClassDeclarationSyntax, int>();

            Program.SourceList.ForEach(x => sourcesNodes.Add(x.rootAfter ?? x.rootBefore));

            foreach (CompilationUnitSyntax root in sourcesNodes)
            {
                var classes = root.ChildNodes().ToList().OfType<ClassDeclarationSyntax>();
                classNodes.AddRange(classes);
            }

            // クラス単位で取得する
            foreach (ClassDeclarationSyntax node in classNodes)
            {
                if (SearchSyntax(new List<SyntaxNode> { node }, syntaxNodes))
                    workdDic.Add(node, node.ToString().Length);
            }

            while (workdDic.Count != 0)
            {
                int max = workdDic.Values.Max();
                KeyValuePair<ClassDeclarationSyntax, int> addClass = workdDic.Where(x => x.Value == max).First();
                accessibility.Add(addClass.Key.Identifier.ToString(), addClass.Key.Modifiers.ToString());
                workdDic.Remove(addClass.Key);
            }

            return accessibility;
        }

        /// <summary>
        /// nodeが存在するメソッドのアクセスビリティを取得する
        /// </summary>
        /// <param name="syntaxNode">メソッドのアクセスビリティを調べたいNode</param>
        /// <returns>アクセシビリティ</returns>
        private string GetMethodAccessibility(SyntaxNode syntaxNodes)
        {
            // 全メソッドを格納するリスト
            List<MethodDeclarationSyntax> methodNodes = new List<MethodDeclarationSyntax>();

            // 一時保存用ディクショナリー
            Dictionary<int, string> workDic = new Dictionary<int, string>();

            Program.SourceList.ForEach(x => methodNodes.AddRange(x.methodAfter.Methods ?? x.methodBefore.Methods));

            // 取得したnode格納用リスト
            foreach (MethodDeclarationSyntax method in methodNodes)
            {
                if (method.ChildNodes().OfType<MethodDeclarationSyntax>().Contains(syntaxNodes))
                    workDic.Add(method.Body.ToString().Length, method.Identifier.ToString());
            }

            int max = workDic.Keys.Max();
            return workDic[max];
        }
        
        /// <summary>
        /// リンクの呼び出しクラスを検索する
        /// </summary>
        /// <param name="identifiers">リンク内の識別子リスト(最後が一番左)</param>
        /// <returns>呼び出しクラス名</returns>
        private string GetLinkClassName(List<IdentifierNameSyntax> identifiers)
        {
            string returnClassName = string.Empty;
            string nodeClassName = string.Empty;

            // 呼び出し箇所がforeachだった場合
            if (identifiers.First().Identifier.ToString().ToUpper() == "FOREACH")
            {
                // 最初の呼び出しが変数の場合
                bool isValiable = false;
                // 最初の呼び出しクラス
                string callClassName = string.Empty;
                // ディクショナリー記録ディクショナリー
                Dictionary<string, string> dic = new Dictionary<string, string>();

                identifiers.RemoveAt(0);

                string str = identifiers.Last().ToString();
                identifiers.Remove(identifiers.Last());

                // 一番初めのクラス、構造体の調査
                SyntaxNode workNode;
                workNode = GetDeclarationNode<ClassDeclarationSyntax>(str);

                if (workNode == null)
                   workNode = GetDeclarationNode<StructDeclarationSyntax>(str);

                if (workNode == null)
                    workNode = ValiableToClassOrStruct(identifiers.First());

                // 2番目以降の調査
                SyntaxNode workNode2;
                for (int i = identifiers.Count - 1; i >= 0; i--)
                {
                    workNode2 = identifiers[i];

                    if (workNode2.ToString().ToUpper().Contains("CONVERTALL"))
                    {
                        // キャストしている場合
                        var castCount = SearchTree(new List<SyntaxNode> { workNode2 }, "Cast");
                        var asCount = SearchTree(new List<SyntaxNode> { workNode2 }, "as");
                        if (castCount.Count > 0 || asCount.Count > 0)
                        {
                            // キャストしたクラス名をworkNode2に代入する
                            // TODO：キャスト実際にキャストしてどのように取得できるか確認する
                            var a = workNode2;
                        }
                    }

                    // nodeから特定の文字列の識別子を持つプロパティかメソッドを検索
                    var node = MenberCheck(workNode,workNode2.ToString());

                    if (node as VariableDeclaratorSyntax != null)
                    {
                        // フィールド
                        workNode2 = node;
                        // フィールドの型を検索
                        break;
                    }
                    if (node as PropertyDeclarationSyntax != null)
                    {
                        // プロパティ
                        workNode2 = node;
                        // プロパティの型を検索
                        break;
                    }
                    if (node as MethodDeclarationSyntax != null)
                    {
                        // メソッド
                        workNode2 = node;
                        // 戻り値の型を取得
                        break;

                    }
                    if (node as ClassDeclarationSyntax != null)
                    {
                        // クラス
                        workNode2 = node;
                        break;
                    }
                    if (node as StructDeclarationSyntax != null)
                    {
                        // 構造体
                        workNode2 = node;
                        break;
                    }
                }
            }

            return returnClassName;
        }

        #endregion
    }
}