using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class VisualBasicCodeWriter : ICodeWriter
    {
        public string FileExtension
        {
            get { return ".vb"; }
        }

        public string DisplayName
        {
            get { return "Visual Basic .NET"; }
        }

        public IList<string> Keywords { get { return _getKeywords(); } }

        private const string NoRenameAttribute = "<Obfuscation(Feature:=\"renaming\", Exclude:=true)>";
        private const string NoPruneAttribute = "<Obfuscation(Feature:=\"trigger\", Exclude:=false)>";

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "Object";
                case JsonTypeEnum.Array: return arraysAsLists ? "IList(Of " + GetTypeName(type.InternalType, config) + ")" : GetTypeName(type.InternalType, config) + "()";
                case JsonTypeEnum.Dictionary: return "Dictionary(Of String, " + GetTypeName(type.InternalType, config) + ")";
                case JsonTypeEnum.Boolean: return "Boolean";
                case JsonTypeEnum.Float: return "Double";
                case JsonTypeEnum.Integer: return "Integer";
                case JsonTypeEnum.Long: return "Long";
                case JsonTypeEnum.Date: return "DateTime";
                case JsonTypeEnum.NonConstrained: return "Object";
                case JsonTypeEnum.NullableBoolean: return "Boolean?";
                case JsonTypeEnum.NullableFloat: return "Double?";
                case JsonTypeEnum.NullableInteger: return "Integer?";
                case JsonTypeEnum.NullableLong: return "Long?";
                case JsonTypeEnum.NullableDate: return "DateTime?";
                case JsonTypeEnum.NullableSomething: return "Object";
                case JsonTypeEnum.Object: return type.AssignedName;
                case JsonTypeEnum.String: return "String";
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

        public void WriteClass(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type)
        {
            var visibility = config.InternalVisibility ? "Friend" : "Public";

            if (config.UseNestedClasses)
            {
                sw.WriteLine("    {0} Partial Class {1}", visibility, config.MainClass);
                if (!type.IsRoot)
                {
                    if (ShouldApplyNoRenamingAttribute(config)) sw.WriteLine("        " + NoRenameAttribute);
                    if (ShouldApplyNoPruneAttribute(config)) sw.WriteLine("        " + NoPruneAttribute);
                    sw.WriteLine("        {0} Class {1}", visibility, type.AssignedName);
                }
            }
            else
            {
                if (ShouldApplyNoRenamingAttribute(config)) sw.WriteLine("    " + NoRenameAttribute);
                if (ShouldApplyNoPruneAttribute(config)) sw.WriteLine("    " + NoPruneAttribute);
                sw.WriteLine("    {0} Class {1}", visibility, type.AssignedName);
            }

            var prefix = config.UseNestedClasses && !type.IsRoot ? "            " : "        ";

            WriteClassMembers(config, sw, type, prefix);

            if (config.UseNestedClasses && !type.IsRoot)
                sw.WriteLine("        End Class");

            sw.WriteLine("    End Class");
            sw.WriteLine();

        }


        private void WriteClassMembers(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type, string prefix)
        {
            foreach (var field in type.Fields)
            {
                if (config.UsePascalCase || config.ExamplesInDocumentation) sw.WriteLine();

                if (config.ExamplesInDocumentation)
                {
                    sw.WriteLine(prefix + "''' <summary>");
                    sw.WriteLine(prefix + "''' Examples: " + field.GetExamplesText());
                    sw.WriteLine(prefix + "''' </summary>");
                }


                if (config.UsePascalCase)
                {
                    sw.WriteLine(prefix + "<JsonProperty(\"{0}\")>", field.JsonMemberName);
                }

                var validVbName = VisualBasicReservedWords.IsReserved(field.MemberName) ? $"[{field.MemberName}]" : field.MemberName;

                if (config.UseProperties)
                {
                    sw.WriteLine(prefix + "Public Property {1} As {0}", field.Type.GetTypeName(), validVbName);
                }
                else
                {
                    sw.WriteLine(prefix + "Public {1} As {0}", field.Type.GetTypeName(), validVbName);
                }
            }

        }





        public void WriteFileStart(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            foreach (var line in JsonClassGenerator.FileHeader)
            {
                sw.WriteLine("' " + line);
            }
            sw.WriteLine();
            sw.WriteLine("Imports System");
            sw.WriteLine("Imports System.Collections.Generic");
            if (ShouldApplyNoRenamingAttribute(config) || ShouldApplyNoPruneAttribute(config))
                sw.WriteLine("Imports System.Reflection");
            if (config.UsePascalCase)
                sw.WriteLine("Imports Newtonsoft.Json");
            sw.WriteLine("Imports Newtonsoft.Json.Linq");
            if (config.SecondaryNamespace != null && config.HasSecondaryClasses && !config.UseNestedClasses)
            {
                sw.WriteLine("Imports {0}", config.SecondaryNamespace);
            }
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, TextWriter sw)
        {
        }


        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            sw.WriteLine();
            sw.WriteLine("Namespace Global.{0}", root && !config.UseNestedClasses ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace));
            sw.WriteLine();
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {

            sw.WriteLine("End Namespace");

        }

        private static List<string> _getKeywords()
        {
            return new List<string>() { "AddHandler"
                                        ,"AddressOf"
                                        ,"Alias"
                                        ,"And"
                                        ,"AndAlso"
                                        ,"As"
                                        ,"Boolean"
                                        ,"ByRef"
                                        ,"Byte"
                                        ,"ByVal"
                                        ,"Call"
                                        ,"Case"
                                        ,"Catch"
                                        ,"CBool"
                                        ,"CByte"
                                        ,"CChar"
                                        ,"CDate"
                                        ,"CDec"
                                        ,"CDbl"
                                        ,"Char"
                                        ,"CInt"
                                        ,"Class"
                                        ,"CLng"
                                        ,"CObj"
                                        ,"Const"
                                        ,"Continue"
                                        ,"CSByte"
                                        ,"CShort"
                                        ,"CSng"
                                        ,"CStr"
                                        ,"CType"
                                        ,"CUInt"
                                        ,"CULng"
                                        ,"CUShort"
                                        ,"Date"
                                        ,"Decimal"
                                        ,"Declare"
                                        ,"Default"
                                        ,"Delegate"
                                        ,"Dim"
                                        ,"DirectCast"
                                        ,"Do"
                                        ,"Double"
                                        ,"Each"
                                        ,"Else"
                                        ,"ElseIf"
                                        ,"End"
                                        ,"EndIf"
                                        ,"Enum"
                                        ,"Erase"
                                        ,"Error"
                                        ,"Event"
                                        ,"Exit"
                                        ,"False"
                                        ,"Finally"
                                        ,"For"
                                        ,"Friend"
                                        ,"Function"
                                        ,"Get"
                                        ,"GetType"
                                        ,"GetXMLNamespace"
                                        ,"Global"
                                        ,"GoSub"
                                        ,"GoTo"
                                        ,"Handles"
                                        ,"If"
                                        ,"If()"
                                        ,"Implements"
                                        ,"Imports"
                                        ,"In"
                                        ,"Inherits"
                                        ,"Integer"
                                        ,"Interface"
                                        ,"Is"
                                        ,"IsNot"
                                        ,"Let"
                                        ,"Lib"
                                        ,"Like"
                                        ,"Long"
                                        ,"Loop"
                                        ,"Me"
                                        ,"Mod"
                                        ,"Module"
                                        ,"MustInherit"
                                        ,"MustOverride"
                                        ,"MyBase"
                                        ,"MyClass"
                                        ,"Namespace"
                                        ,"Narrowing"
                                        ,"New"
                                        ,"Next"
                                        ,"Not"
                                        ,"Nothing"
                                        ,"NotInheritable"
                                        ,"NotOverridable"
                                        ,"Object"
                                        ,"Of"
                                        ,"On"
                                        ,"Operator"
                                        ,"Option"
                                        ,"Optional"
                                        ,"Or"
                                        ,"OrElse"
                                        ,"Overloads"
                                        ,"Overridable"
                                        ,"Overrides"
                                        ,"ParamArray"
                                        ,"Partial"
                                        ,"Private"
                                        ,"Property"
                                        ,"Protected"
                                        ,"Public"
                                        ,"RaiseEvent"
                                        ,"ReadOnly"
                                        ,"ReDim"
                                        ,"REM"
                                        ,"RemoveHandler"
                                        ,"Resume"
                                        ,"Return"
                                        ,"SByte"
                                        ,"Select"
                                        ,"Set"
                                        ,"Shadows"
                                        ,"Shared"
                                        ,"Short"
                                        ,"Single"
                                        ,"Static"
                                        ,"Step"
                                        ,"Stop"
                                        ,"String"
                                        ,"Structure"
                                        ,"Sub"
                                        ,"SyncLock"
                                        ,"Then"
                                        ,"Throw"
                                        ,"To"
                                        ,"True"
                                        ,"Try"
                                        ,"TryCast"
                                        ,"TypeOf"
                                        ,"Variant"
                                        ,"Wend"
                                        ,"UInteger"
                                        ,"ULong"
                                        ,"UShort"
                                        ,"Using"
                                        ,"When"
                                        ,"While"
                                        ,"Widening"
                                        ,"With"
                                        ,"WithEvents"
                                        ,"WriteOnly"
                                        ,"Xor"
                                        ,"#Const"
                                        ,"#Else"
                                        ,"#ElseIf"
                                        ,"#End"
                                        ,"#If"
                                        ,"="
                                        ,"&"
                                        ,"&="
                                        ,"*"
                                        ,"*="
                                        ,"/"
                                        ,"/="
                                        ,@"\"
                                        ,@"\="
                                        ,"^"
                                        ,"^="
                                        ,"+"
                                        ,"+="
                                        ,"-"
                                        ,"-="
                                        ,">>"
                                        ,"<<"
                                        ,"<<="};
        }
    }
}