using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonUtils.Core.Testing
{
    [TestClass]
    public class TestAutoGen
    {
        [TestMethod]
        public void AutoGen()
        {
            var RootObject = "";
            string jsonInput = @"{""employees"": [
                        {  ""firstName"":""John"" , ""lastName"":""Doe"" }, 
                        {  ""firstName"":""Anna"" , ""lastName"":""Smith"" }, 
                        { ""firstName"": ""Peter"" ,  ""lastName"": ""Jones "" }
                        ]
                        }";

            var Language = "";

            CodeGenerationHelper.Prepare(jsonInput, RootObject, Language, false, false,
                        "None", (model.Language == 5 || model.Language == 6) && true);
        }

    }
}



