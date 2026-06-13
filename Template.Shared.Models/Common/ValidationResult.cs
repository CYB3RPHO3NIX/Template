using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Shared.Models.Common
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new List<string>();
    }
}
