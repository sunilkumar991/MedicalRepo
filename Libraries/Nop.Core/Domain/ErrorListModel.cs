using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain
{
    public class ErrorListModel
    {
        public ErrorListModel()
        {
            ErrorList = new List<string>();
        }
        public bool IsAttributeError { get; set; }
        public IList<string> ErrorList { get; set; }
    }
}
