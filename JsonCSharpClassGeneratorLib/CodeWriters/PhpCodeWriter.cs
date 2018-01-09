﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class PhpCodeWriter : ICodeWriter
    {
        public string FileExtension
        {
            get { return ".php"; }
        }

        public string DisplayName
        {
            get { return "PHP"; }
        }

        public IList<string> Keywords { get { return new List<string>(); } }

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = !config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "Object";
                case JsonTypeEnum.Array: return "array(" + GetTypeName(type.InternalType, config) + ")";
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
            //var visibility = config.InternalVisibility ? "" : "public";

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
            sw.WriteLine("class {0}", type.AssignedName);
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
            throw new NotImplementedException();
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            throw new NotImplementedException();
        }

        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            sw.WriteLine("<?php");
            sw.Write("\r");
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            sw.Write("\r");
            sw.WriteLine("?>");
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
                    //sw.WriteLine(prefix + "@JsonProperty" + "(\"{0}\")", field.JsonMemberName);
                    sw.WriteLine(prefix + "public function get{0}() {{ \r\t\t return $this->{1} \r\t}}", ChangeFirstChar(field.MemberName), field.MemberName);
                    sw.WriteLine(prefix + "public function set{0}(${1}) {{ \r\t\t $this->{1} = ${1} \r\t}}", ChangeFirstChar(field.MemberName), field.MemberName);
                    sw.WriteLine(prefix + "public ${1}; //{0}", field.Type.GetTypeName(), field.MemberName);
                    sw.WriteLine();
                }
                else
                {                    
                    sw.WriteLine(prefix + "public ${1}; //{0}", field.Type.GetTypeName(), field.MemberName);
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
    }
}
