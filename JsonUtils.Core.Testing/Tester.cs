using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonUtils.Core.Testing
{
    [TestClass]
    public class Tester
    {
        [TestMethod]
        public void CLI()
        {
            var RootObject = "";
            string jsonInput = @"{""employees"": [
                        {  ""firstName"":""John"" , ""lastName"":""Doe"" }, 
                        {  ""firstName"":""Anna"" , ""lastName"":""Smith"" }, 
                        { ""firstName"": ""Peter"" ,  ""lastName"": ""Jones "" }
                        ]
                        }";

            var Language = JsonUtils.Core.Language.CSharp;

            var output = CodeGenerationHelper.Prepare(jsonInput, RootObject, Language, "", false,
                        "None",(JsonUtils.Core.Language.Java == Language || JsonUtils.Core.Language.PHP == Language) && true);

            string t = "";
        }

    }
}



