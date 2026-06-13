using System;
using System.Collections.Generic;
using System.Text;
using Template.Shared.Models.Common;

namespace Template.Shared.Models.Requests
{
    public interface IRequest
    {
        ValidationResult Validate();
    }
}
