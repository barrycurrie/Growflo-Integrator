using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Validation
{
    public interface IValidator<T>
    {
        List<BrokenRule> Validate(T entity);
    }
}
