using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class IndexViewModel
    {
        public string ClassName { get; set; }
        public string JSON { get; set; }

        public int Language { get; set; }

        public string CodeObjects { get; set; }

        public bool Nest { get; set; }

        public bool Error { get; set; }

        public int ErrorNo { get; set; }
    }
}
