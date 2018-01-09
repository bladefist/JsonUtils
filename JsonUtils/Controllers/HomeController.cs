using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Core.ViewModels;
using Newtonsoft.Json.Linq;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace JsonUtils.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Bookmarklet()
        {
            return View();
        }
        public ActionResult Index(string url)
        {
            var vm = new IndexViewModel();

            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    vm.JSON = new WebClient().DownloadString(url);
                }
                catch (Exception ex)
                {
                    vm.Error = true;
                    vm.ErrorNo = 4;
                }
            }

            if (string.IsNullOrEmpty(vm.JSON))
            {
                vm.JSON = @"{""employees"": [
                        {  ""firstName"":""John"" , ""lastName"":""Doe"" }, 
                        {  ""firstName"":""Anna"" , ""lastName"":""Smith"" }, 
                        { ""firstName"": ""Peter"" ,  ""lastName"": ""Jones "" }
                        ]
                        }".Trim();
            }

            vm.ClassName = "Example";
            vm.PropertyAttribute = "None";
            vm.Nest = false;

            try
            {
                vm.CodeObjects = Server.HtmlEncode(Prepare(vm.JSON, vm.ClassName, 1, vm.Nest, false, vm.PropertyAttribute, vm.Namespace));
            }
            catch (Exception ex)
            {
                vm.Error = true;
                vm.ErrorNo = 3;
            }
            vm.Language = 1;

            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(IndexViewModel model)
        {
            if (string.IsNullOrEmpty(model.ClassName))
            {
                //model.Error = true;
                //model.ErrorNo = 1;
                model.ClassName = "RootObject";
            }
            else if (string.IsNullOrEmpty(model.JSON))
            {
                model.Error = true;
                model.ErrorNo = 2;
            }


            if (model.JSON.ToLower().StartsWith("http"))
            {
                try
                {
                    model.JSON = new WebClient().DownloadString(model.JSON);
                }
                catch (Exception ex)
                {
                    model.Error = true;
                    model.ErrorNo = 4;
                }
            }

            if (_containsKeyword(model.JSON, model.Language))
            {
                model.Error = true;
                model.ErrorNo = 5;
            }

            if (model.Error)
            {
                return View(model);
            }
            
            try
            {
                if (model.Language != 3)
                {

                    model.CodeObjects = Server.HtmlEncode(Prepare(model.JSON, model.ClassName, model.Language, model.Nest, model.Pascal, model.PropertyAttribute, model.Namespace, (model.Language == 5 || model.Language == 6) && model.Properties));
                }
                else
                    model.CodeObjects = "javascript";
            }
            catch (Exception ex)
            {
                model.Error = true;
                model.ErrorNo = 3;               
            }
            
            return View(model);
        }



        private readonly static ICodeWriter[] CodeWriters = new ICodeWriter[] {
            new CSharpCodeWriter(),
            new VisualBasicCodeWriter(),
            new TypeScriptCodeWriter()
        };

        private string Prepare(string JSON, string classname, int language, bool nest, bool pascal, string propertyAttribute, string namesp, bool hasGetSet=false)
        {
            if (string.IsNullOrEmpty(JSON))
            {
                return null;
            }

            ICodeWriter writer;

            if (language == 1)
                writer = new CSharpCodeWriter();
            else if (language == 2)
                writer = new VisualBasicCodeWriter();
            else if (language == 7)
                writer = new TypeScriptCodeWriter();
            else if(language == 4)
                writer = new SqlCodeWriter();
            else if(language == 5)
                writer = new JavaCodeWriter();
            else
                writer = new PhpCodeWriter();

            var gen = new JsonClassGenerator();
            gen.Example = JSON;
            gen.InternalVisibility = false;
            gen.CodeWriter = writer;
            gen.ExplicitDeserialization = false;
            if (nest)
                gen.Namespace = namesp;
            else
                gen.Namespace = null;

            gen.NoHelperClass = false;
            gen.SecondaryNamespace = null;
            gen.UseProperties = (language != 5 && language != 6) || hasGetSet;
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

        private bool _containsKeyword(string Json, int language)
        {
            ICodeWriter writer;

            if (language == 1)
                writer = new CSharpCodeWriter();
            else if (language == 2)
                writer = new VisualBasicCodeWriter();
            else if (language == 7)
                writer = new TypeScriptCodeWriter();
            else if (language == 4)
                writer = new SqlCodeWriter();
            else if (language == 5)
                writer = new JavaCodeWriter();
            else
                writer = new PhpCodeWriter();

            var jtoken = JToken.Parse(Json);
            var hashSet = jtoken.ToHashSet();

            if (writer.Keywords.Any(x => hashSet.Contains(x)))
                return true;

            return false;
        }

        public string Result { get; set; }

        public void R(object d)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(d))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(d);
                Result += string.Format("{0}={1}", name, value);

                if (value is JToken && ((JToken)value).Type == JTokenType.Object)
                    R(value);
            }
        }

    }

}
