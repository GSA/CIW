using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Enum
{
    public enum ErrorCodes : int
    {
        unknown_error = -1,
        unprocessed = 0,
        successfully_processed = 1,
        password_protected = -2,
        wrong_version = -3,
        arra = -4,
        duplicate_user = -5,
        failed_validation = -6
    }
}
