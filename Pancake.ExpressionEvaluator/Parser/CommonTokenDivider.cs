using Pancake.ExpressionEvaluator.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ExpressionEvaluator.Parser
{
    public ref struct CommonTokenDivider
    {
        private ReadOnlySpan<char> _expr;
        private int _curPosition;
        public void ParseExpression(ReadOnlySpan<char> expression)
        {
            _expr = expression;
            _curPosition = 0;

            while ((_curPosition = MatchNextToken(out var token)) < expression.Length)
            {

            }
        }

        private int MatchNextToken(out IToken token)
        {
            token = default;
            return 0;
        }

        private bool TestToken(ReadOnlySpan<char> toBeTested, int startPosition, ReadOnlySpan<char> toTest)
        {
            if (toTest.Length + startPosition > toBeTested.Length) return false;

            return toBeTested.Slice(startPosition, toTest.Length).SequenceEqual(toTest);
        }
        private bool TestToken(ReadOnlySpan<char> toBeTested, int startPosition, char toTest)
        {
            if (startPosition >= toBeTested.Length) return false;

            return toBeTested[startPosition] == toTest;
        }
    }
}
