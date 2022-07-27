using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ExpressionEvaluator.Execution
{
    public interface IVmExecutionContext
    {
        public IRegister GetSpecialRegister(int index);
        public IRegister PopRegister();
        public void PushRegister(IRegister register);
        public IExecutionContext GetNormalContext();
    }
}
