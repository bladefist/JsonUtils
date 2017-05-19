using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class JavaCodeWriter : ICodeWriter
    {
        public string FileExtension
        {
            get { return ".java"; }
        }

        public string DisplayName
        {
            get { return "Java"; }
        }

        public IList<string> Keywords { get {return _getKeywords();} }

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = !config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "Object";
                case JsonTypeEnum.Array: return arraysAsLists ? "List<" + GetTypeName(type.InternalType, config) + ">" : GetTypeName(type.InternalType, config) + "[]";
                case JsonTypeEnum.Dictionary: return "Dictionary<string, " + GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "boolean";
                case JsonTypeEnum.Float: return "double";
                case JsonTypeEnum.Integer: return "int";
                case JsonTypeEnum.Long: return "long";
                case JsonTypeEnum.Date: return "Date";
                case JsonTypeEnum.NonConstrained: return "Object";
                case JsonTypeEnum.NullableBoolean: return "bool?";
                case JsonTypeEnum.NullableFloat: return "double?";
                case JsonTypeEnum.NullableInteger: return "int?";
                case JsonTypeEnum.NullableLong: return "long?";
                case JsonTypeEnum.NullableDate: return "DateTime?";
                case JsonTypeEnum.NullableSomething: return "object";
                case JsonTypeEnum.Object: return type.AssignedName;
                case JsonTypeEnum.String: return "String";
                default: throw new System.NotSupportedException("Unsupported json type");
            }
        }

        public void WriteClass(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type)
        {
            var visibility = config.InternalVisibility ? "" : "public";

            //if (config.UseNestedClasses)
            //{
            //    if (!type.IsRoot)
            //    {
            //        if (config.PropertyAttribute == "DataMember")
            //        {
            //            sw.WriteLine("        [DataContract]");
            //        }

            //        if (ShouldApplyNoRenamingAttribute(config)) sw.WriteLine("        " + NoRenameAttribute);
            //        if (ShouldApplyNoPruneAttribute(config)) sw.WriteLine("        " + NoPruneAttribute);
            //        sw.WriteLine("        {0} class {1}", visibility, type.AssignedName);
            //        sw.WriteLine("        {");
            //    }
            //}
            //else
            //{
            //    if (config.PropertyAttribute == "DataMember")
            //    {
            //        sw.WriteLine("    [DataContract]");
            //    }

            //    if (ShouldApplyNoRenamingAttribute(config)) sw.WriteLine("    " + NoRenameAttribute);
            //    if (ShouldApplyNoPruneAttribute(config)) sw.WriteLine("    " + NoPruneAttribute);
                sw.WriteLine("{0} class {1}", visibility, type.AssignedName);
                sw.WriteLine("{");
            //}

            var prefix = config.UseNestedClasses && !type.IsRoot ? "" : "    ";


            var shouldSuppressWarning = config.InternalVisibility && !config.UseProperties && !config.ExplicitDeserialization;
            if (shouldSuppressWarning)
            {
                sw.WriteLine("#pragma warning disable 0649");
                if (!config.UsePascalCase) sw.WriteLine();
            }

            //if (type.IsRoot && config.ExplicitDeserialization) WriteStringConstructorExplicitDeserialization(config, sw, type, prefix);

            //if (config.ExplicitDeserialization)
            //{
            //    if (config.UseProperties) WriteClassWithPropertiesExplicitDeserialization(sw, type, prefix);
            //    else WriteClassWithFieldsExplicitDeserialization(sw, type, prefix);
            //}
            //else
            //{
                WriteClassMembers(config, sw, type, prefix);
            //}

            if (shouldSuppressWarning)
            {
                sw.WriteLine();
                sw.WriteLine("#pragma warning restore 0649");
                sw.WriteLine();
            }


            if (config.UseNestedClasses && !type.IsRoot)
                sw.WriteLine("        }");

            if (!config.UseNestedClasses)
                sw.WriteLine("}");

            sw.WriteLine();
        }

        public void WriteFileStart(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            foreach (var line in JsonClassGenerator.FileHeader)
            {
                sw.WriteLine("// " + line);
            }
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            throw new NotImplementedException();
        }

        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            sw.WriteLine();
            sw.WriteLine("package {0};", root && !config.UseNestedClasses ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace));
            sw.WriteLine();
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            sw.WriteLine();
        }

        private void WriteClassMembers(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type, string prefix)
        {
            foreach (var field in type.Fields)
            {
                //if (config.UsePascalCase || config.ExamplesInDocumentation) sw.WriteLine();

                //if (config.ExamplesInDocumentation)
                //{
                //    sw.WriteLine(prefix + "/// <summary>");
                //    sw.WriteLine(prefix + "/// Examples: " + field.GetExamplesText());
                //    sw.WriteLine(prefix + "/// </summary>");
                //}

                //if (config.UsePascalCase || config.PropertyAttribute != "None")
                //{
                //    if (config.UsePascalCase && config.PropertyAttribute == "None")
                //        sw.WriteLine(prefix + "@JsonProperty(\"{0}\")", field.JsonMemberName);
                //    else
                //    {
                //        //if (config.PropertyAttribute == "DataMember")
                //        //    sw.WriteLine(prefix + "[" + config.PropertyAttribute + "(Name=\"{0}\")]", field.JsonMemberName);
                //        if (config.PropertyAttribute == "JsonProperty")
                //            sw.WriteLine(prefix + "@" + config.PropertyAttribute + "(\"{0}\")", field.JsonMemberName);
                //    }
                //}

                if (config.UseProperties)
                {
                    sw.WriteLine(prefix + "@JsonProperty" + "(\"{0}\")", field.JsonMemberName);
                    sw.WriteLine(prefix + "public {0} get{1}() {{ \r\t\t return this.{2} \r\t}}", field.Type.GetTypeName(), ChangeFirstChar(field.MemberName), ChangeFirstChar(field.MemberName, false));
                    sw.WriteLine(prefix + "public {0} set{1}({0} {2}) {{ \r\t\t this.{2} = {2} \r\t}}", field.Type.GetTypeName(), ChangeFirstChar(field.MemberName), ChangeFirstChar(field.MemberName, false));
                    sw.WriteLine(prefix + "{0} {1};", field.Type.GetTypeName(), ChangeFirstChar(field.MemberName, false)); 
                    sw.WriteLine();
                }
                else
                {
                    string memberName = ChangeFirstChar(field.MemberName, false);
                    if(field.JsonMemberName != memberName)
                        sw.WriteLine(prefix + "@JsonProperty" + "(\"{0}\")", field.JsonMemberName);
                    sw.WriteLine(prefix + "public {0} {1};", field.Type.GetTypeName(), memberName);
                }
            }

        }

        private static string ChangeFirstChar(string value, bool toCaptial = true)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length == 0)
                return value;
            StringBuilder sb = new StringBuilder();

            sb.Append(toCaptial ? char.ToUpper(value[0]) : char.ToLower(value[0]));
            sb.Append(value.Substring(1));

            return sb.ToString();
        }

        private static List<string> _getKeywords()
        {
            return new List<string>()
            {
                "abstract",
                "assert" ,
                "boolean",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "class",
                "const",
                "default",
                "do",
                "double",
                "else",
                "enum",
                "extends",
                "false",
                "final",
                "finally",
                "float",
                "for",
                "goto",
                "if",
                "implements",
                "import",
                "instanceof",
                "int",
                "interface",
                "long",
                "native",
                "new",
                "null",
                "package",
                "private",
                "protected",
                "public",
                "return",
                "short",
                "static",
                "strictfp",
                "super",
                "switch",
                "synchronized",
                "this",
                "throw",
                "throws",
                "transient",
                "true",
                "try",
                "void",
                "volatile",
                "while",
                "continue"
            };
        }
    }
}
