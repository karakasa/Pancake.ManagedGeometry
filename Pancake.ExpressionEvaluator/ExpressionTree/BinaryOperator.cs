using Pancake.ExpressionEvaluator.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ExpressionEvaluator.ExpressionTree
{
    public abstract class BinaryOperator : IVmOperation, IFunction
    {
        void IVmOperation.Execute(IVmExecutionContext context)
        {
            var reg1 = context.PopRegister();
            var reg2 = context.PopRegister();

            IRegister result = default;

            Execute(reg1, reg2, ref result);

            context.PushRegister(result);
        }

        void IFunction.Execute(IExecutionContext context, ref IRegister result, IRegister[] args)
        {
            if (args.Length < 2)
            {
                context.ThrowTooFewArguments();
                return;
            }

            Execute(args[0], args[1], ref result);
        }

        public abstract void Execute(IRegister operand1, IRegister operand2, ref IRegister resultRegister);
    }
}
