using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Core.ViewModels
{
    public class IndexViewModel
    {
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string JSON { get; set; }

        public List<SelectListItem> PropertyAttributeOptions
        {
            get {
                return new List<SelectListItem>() { 
                    new SelectListItem() { Text = "None", Value = "None" },
                    new SelectListItem() { Text = "DataMember", Value = "DataMember" },
                    new SelectListItem() { Text = "JsonProperty", Value = "JsonProperty" }
                };
            }
        }

        public string PropertyAttribute { get; set; }

        public int Language { get; set; }

        public string CodeObjects { get; set; }

        public bool Nest { get; set; }
        public bool Pascal { get; set; }
        public bool Properties { get; set; }

        public bool Error { get; set; }

        public int ErrorNo { get; set; }
    }
}
