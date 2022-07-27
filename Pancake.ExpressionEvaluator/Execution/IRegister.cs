using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ExpressionEvaluator.Execution
{
    public interface IRegister
    {
        public bool IsImmutable { get; }
    }
}
