using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Execeptions
{
    public class AlreadyExisitException : Exception
    {
        public AlreadyExisitException(string? message) : base(message)
        {
        }
    }
}
