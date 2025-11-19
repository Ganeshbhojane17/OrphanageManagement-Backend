using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orphanage.Core.Exception
{
    public class UnAuthorizedException(string message) : System.Exception(message);
}
