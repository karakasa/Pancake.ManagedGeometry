using Pancake.ExpressionEvaluator.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ExpressionEvaluator.ExpressionTree
{
    public interface IFunction
    {
        public void Execute(IExecutionContext context, ref IRegister result, IRegister[] args);
    }
}
