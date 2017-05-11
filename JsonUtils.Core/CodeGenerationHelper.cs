using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;
namespace JsonUtils.Core
{
    public static class CodeGenerationHelper
    {
        private readonly static ICodeWriter[] CodeWriters = new ICodeWriter[] {
            new CSharpCodeWriter(),
            new VisualBasicCodeWriter(),
            new TypeScriptCodeWriter()
        };

        public static string Prepare(string JSON, string classname, Core.Language language, bool nest, bool pascal, string propertyAttribute, bool hasGetSet = false)
        {
            if (string.IsNullOrEmpty(JSON))
            {
                return null;
            }

            ICodeWriter writer;

            switch(language)
            {
                case Language.CSharp:
                    writer = new CSharpCodeWriter();
                    break;
                case Language.VisualBasic:
                    writer = new VisualBasicCodeWriter();
                    break;
                case Language.TypeScript:
                    writer = new TypeScriptCodeWriter();
                    break;
                case Language.Sql:
                    writer = new SqlCodeWriter();
                    break;

                case Language.Java:
                    writer = new JavaCodeWriter();
                    break;
                case Language.PHP:
                    writer = new PhpCodeWriter();
                    break;
                default:
                    throw new NotImplementedException("This language does not yet exists");
            };
            

            var gen = new JsonClassGenerator();
            gen.Example = JSON;
            gen.InternalVisibility = false;
            gen.CodeWriter = writer;
            gen.ExplicitDeserialization = false;
            if (nest)
                gen.Namespace = "JSONUtils";
            else
                gen.Namespace = null;

            gen.NoHelperClass = false;
            gen.SecondaryNamespace = null;
            gen.UseProperties = ((int)language != 5 && (int)language != 6) || hasGetSet;
            gen.MainClass = classname;
            gen.UsePascalCase = pascal;
            gen.PropertyAttribute = propertyAttribute;

            gen.UseNestedClasses = nest;
            gen.ApplyObfuscationAttributes = false;
            gen.SingleFile = true;
            gen.ExamplesInDocumentation = false;

            gen.TargetFolder = null;
            gen.SingleFile = true;

            using (var sw = new StringWriter())
            {
                gen.OutputStream = sw;
                gen.GenerateClasses();
                sw.Flush();

                return sw.ToString();
            }
        }
    }
}
