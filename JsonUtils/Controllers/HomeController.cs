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


namespace JsonUtils.Web.Controllers
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
                vm.CodeObjects = Server.HtmlEncode(JsonUtils.Core.CodeGenerationHelper.Prepare(vm.JSON, vm.ClassName, JsonUtils.Core.Language.CSharp, vm.Nest, false, vm.PropertyAttribute));
            }
            catch (Exception ex)
            {
                vm.Error = true;
                vm.ErrorNo = 3;
            }
            vm.Language = JsonUtils.Core.Language.CSharp;

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


            if (model.Error)
            {
                return View(model);
            }
            
            try
            {
                if (model.Language !=  Core.Language.JavaScript)
                {

                    model.CodeObjects =
                        Server.HtmlEncode(JsonUtils.Core.CodeGenerationHelper.Prepare(model.JSON, model.ClassName, model.Language, model.Nest, model.Pascal,
                        model.PropertyAttribute, (model.Language == Core.Language.Java || model.Language == Core.Language.PHP) && model.Properties));
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
