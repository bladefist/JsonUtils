using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;


    public class Util
    {
        public string Result { get; set; }
        public void PrintProperties(object obj, int indent)
        {
            if (obj == null) return;
            string indentString = new string(' ', indent);
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);
                if (property.PropertyType.Assembly == objType.Assembly)
                {
                    Result += string.Format("{0}{1}:", indentString, property.Name);
                    PrintProperties(propValue, indent + 2);
                }
                else
                {
                    Result += string.Format("{0}{1}: {2}", indentString, property.Name, propValue);
                }
            }
        }
    }
