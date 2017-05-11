using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonUtils.AutoGen
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
            }


        }


    }

    class Options
    {
        [Option('i', "input", Required = true,
          HelpText = "Input json schema file to be processed.")]
        public string InputFile { get; set; }

        [Option('r',"root",Required =true, HelpText ="The root name of the object, Eg. Person")]
        public string RootObject { get; set; }
        [Option('g', "gen", Required = true, HelpText = "The language generated, eg. c#, typescript, java")]
        public string GenerateLang { get; set; }

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
