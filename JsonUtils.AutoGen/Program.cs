using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JsonUtils.Core;

namespace JsonUtils.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Values are available here
                if (options.Verbose) Console.WriteLine("Reading json schema at : {0}", options.InputFile);

                if(! File.Exists(options.InputFile))
                {
                    if (options.Verbose)
                    {
                        Console.WriteLine("The file does not exists");
                    }else
                    {
                        throw new FileNotFoundException("The json file does not exists");
                    }
                }
                //JsonUtils.Core.Language
                JsonUtils.Core.Language Language = (JsonUtils.Core.Language)Enum.Parse(typeof(JsonUtils.Core.Language), options.GenerateLang, true);

                if (options.Verbose) Console.WriteLine("Reading the file ...");
                var jsonInput = File.ReadAllText(options.InputFile).Trim() ;

                if (options.Verbose) Console.WriteLine("Generating the code ...");
                var output = CodeGenerationHelper.Prepare(jsonInput, options.RootObject, Language, options.NameSpace, false,
                       "None", (JsonUtils.Core.Language.Java == Language || JsonUtils.Core.Language.PHP == Language) && true);

                if(string.IsNullOrEmpty(options.OutputFile))
                {
                    Console.Write(output);
                }else
                {
                    if (options.Verbose) Console.WriteLine("Saving the code");
                    File.WriteAllText(options.OutputFile, output);
                    if (options.Verbose) Console.WriteLine("Saved to "+ options.OutputFile);
                }

            }


        }


    }

    class Options
    {
        [Option('i', "input", Required = true,
          HelpText = "Input json schema file to be processed.")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = false,
          HelpText = "Output file to be generated, will output to console if not supplied.")]
        public string OutputFile { get; set; }
        [Option('n', "namespace", Required = false,
       HelpText = "the namespace used")]
        public string NameSpace { get; set; }

        [Option('r',"root",Required =true, HelpText ="The root name of the object, Eg. Person")]
        public string RootObject { get; set; }
        [Option('g', "gen", Required = true, HelpText = "The language generated, eg. c#, typescript, java")]
        public string  GenerateLang { get; set; }

        // omitting long name, default --verbose
        [Option(DefaultValue = true,
        HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
