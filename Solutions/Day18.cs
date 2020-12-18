using System;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace AdventOfCode.Solutions
{
    public class Day18
    {
        public class MathGrammar : Grammar
        {
            public MathGrammar(bool withPrecedence)
            {
                var num = new NumberLiteral("num", NumberOptions.IntOnly, typeof(NumNode));

                var expr = new NonTerminal("expr");
                var binex = new NonTerminal("binex", typeof(BinaryNode));
                var binop = new NonTerminal("binop");
                var paren = new NonTerminal("paren");

                expr.Rule = num | paren | binex;
                binex.Rule = expr + binop + expr;
                binop.Rule = ToTerm("+") | "*";
                paren.Rule = "(" + expr + ")";

                Root = expr;
                
                MarkPunctuation("(", ")");
                RegisterBracePair("(", ")");

                MarkTransient(expr, paren, binop);

                if (withPrecedence)
                {
                    RegisterOperators(2, "+");
                    RegisterOperators(1, "*");
                }
                else
                {
                    RegisterOperators(1, "*", "+");
                }

                LanguageFlags = LanguageFlags.CreateAst;
            }
        }

        public interface IValueSource
        {
            long GetValue();
            void Dump(string indent);
        }
        
        public class NumNode : IAstNodeInit, IValueSource
        {
            public long Value { get; private set; }

            public void Init(AstContext context, ParseTreeNode parseNode)
            {
                Value = Convert.ToInt64(parseNode.Token.Value);
            }

            public long GetValue()
            {
                return Value;
            }

            public void Dump(string indent)
            {
                Console.WriteLine(indent + Value);
            }
        }

        public class BinaryNode : IAstNodeInit, IValueSource
        {
            public IValueSource Left { get; private set; }
            public IValueSource Right { get; private set; }
            public char Op { get; private set; }

            public void Init(AstContext context, ParseTreeNode parseNode)
            {
                Left = (IValueSource) parseNode.ChildNodes[0].AstNode;
                Op = parseNode.ChildNodes[1].Token.Text[0];
                Right = (IValueSource) parseNode.ChildNodes[2].AstNode;
            }

            public long GetValue()
            {
                switch (Op)
                {
                    case '+': return Left.GetValue() + Right.GetValue();
                    case '*': return Left.GetValue() * Right.GetValue();
                }
                throw new ArgumentException();
            }

            public void Dump(string indent)
            {
                Left.Dump(indent + "  ");
                Console.WriteLine(indent + Op);
                Right.Dump(indent + "  ");
            }
        }

        private static async Task ExecuteProblem(bool withPrecedence)
        {
            var data = await Data.GetDataLines();
            long sum = 0;
            Parser p = new Parser(new MathGrammar(withPrecedence));
            foreach (var line in data)
            {
                var result = p.Parse(line);
                var astNode = ((IValueSource) result.Root.AstNode);
                var value = astNode.GetValue();
                sum += value;
                Console.WriteLine($"{line} = {value}");
                astNode.Dump("*  ");
            }

            Console.WriteLine(" ---------------- ");
            Console.WriteLine($"Sum: {sum}");
        }

        public static async Task Problem1()
        {
            await ExecuteProblem(false);
        }

        public static async Task Problem2()
        {
            await ExecuteProblem(true);
        }
    }
}
}