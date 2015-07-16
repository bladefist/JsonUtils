using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class SqlCodeWriter : ICodeWriter
    {
        public string FileExtension
        {
            get { return ".cs"; }
        }

        public string DisplayName
        {
            get { return "SQL"; }
        }


        private const string NoRenameAttribute = "[Obfuscation(Feature = \"renaming\", Exclude = true)]";
        private const string NoPruneAttribute = "[Obfuscation(Feature = \"trigger\", Exclude = false)]";

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = !config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "object";
                case JsonTypeEnum.Array: return arraysAsLists ? "IList<" + GetTypeName(type.InternalType, config) + ">" : GetTypeName(type.InternalType, config) + "[]";
                case JsonTypeEnum.Dictionary: return "Dictionary<string, " + GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "bit NOT NULL";
                case JsonTypeEnum.Float: return "[decimal](9,2) NOT NULL";
                case JsonTypeEnum.Integer: return "[int] NOT NULL";
                case JsonTypeEnum.Long: return "[bigint] NOT NULL";
                case JsonTypeEnum.Date: return "[datetime]";
                case JsonTypeEnum.NonConstrained: return "object";
                case JsonTypeEnum.NullableBoolean: return "bit NULL";
                case JsonTypeEnum.NullableFloat: return "[decimal](9,2) NULL";
                case JsonTypeEnum.NullableInteger: return "[int] NULL";
                case JsonTypeEnum.NullableLong: return "[bigint] NULL";
                case JsonTypeEnum.NullableDate: return "[datetime] NULL";
                case JsonTypeEnum.NullableSomething: return "object NULL";
                case JsonTypeEnum.Object: return type.AssignedName;
                case JsonTypeEnum.String: return "[varchar](50) NULL";
                default: throw new System.NotSupportedException("Unsupported json type");
            }
        }


        private bool ShouldApplyNoRenamingAttribute(IJsonClassGeneratorConfig config)
        {
            return config.ApplyObfuscationAttributes && !config.ExplicitDeserialization && !config.UsePascalCase;
        }
        private bool ShouldApplyNoPruneAttribute(IJsonClassGeneratorConfig config)
        {
            return config.ApplyObfuscationAttributes && !config.ExplicitDeserialization && config.UseProperties;
        }

        public void WriteFileStart(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            if (config.UseNestedClasses)
            {
                sw.WriteLine("    }");
            }
        }


        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {

        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            sw.WriteLine("}");
        }

        public void WriteClass(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type)
        {
            

            var visibility = config.InternalVisibility ? "internal" : "public";

            sw.WriteLine("create table " + type.AssignedName + " (");
            sw.WriteLine("    [Id] [int] IDENTITY(1,1) NOT NULL,");

 

            var prefix = config.UseNestedClasses && !type.IsRoot ? "            " : "        ";
               
            WriteClassMembers(config, sw, type, prefix);

            sw.WriteLine("CONSTRAINT [PK_" + type.AssignedName + "] PRIMARY KEY CLUSTERED");
            sw.WriteLine("   (");
            sw.WriteLine("      [Id] asc");
            sw.WriteLine("   )");
            sw.WriteLine(")");

            sw.WriteLine();


        }



        private void WriteClassMembers(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type, string prefix)
        {
            foreach (var field in type.Fields)
            {              
                 
               

                if (config.UseProperties)
                {                    
                    sw.WriteLine("    [{0}] {1},", field.MemberName, field.Type.GetTypeName());
                }
            }

        }







    }
}
