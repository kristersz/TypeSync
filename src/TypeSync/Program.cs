using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using log4net;
using log4net.Config;
using TypeSync.Common.Constants;
using TypeSync.Models;
using TypeSync.UseCases;

[assembly: XmlConfigurator(Watch = true)]

namespace TypeSync
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            log.Info("Starting TypeSync");

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => Run(options))
                .WithNotParsed(errors => PrintParserErrors(errors));

            log.Info("Exiting TypeSync");
        }

        private static void Run(Options options)
        {
            try
            {
                var validationResult = ValidateArguments(options);

                if (!validationResult.Success)
                {
                    Console.WriteLine(validationResult.ErrorString());
                    return;
                }

                var configuration = MapConfiguration(options);

                UseCase useCaseEnum;
                if (Enum.TryParse(options.UseCase, true, out useCaseEnum))
                {
                    var useCase = UseCaseFactory.Create(useCaseEnum, configuration);

                    var result = useCase.Handle();

                    if (!result.Success)
                    {
                        Console.WriteLine(validationResult.ErrorString());
                    }
                }
                else
                {
                    Console.WriteLine("Unknown use case was specified");
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception thrown: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private static void PrintParserErrors(IEnumerable<Error> errors)
        {
            Console.WriteLine("Argument errors:");

            foreach (var error in errors)
            {
                Console.WriteLine($"\t {error.Tag.ToString()}");
            }
        }

        private static Result ValidateArguments(Options options)
        {
            var result = new Result();

            if (string.IsNullOrEmpty(options.UseCase))
            {
                result.ErrorMessage = "Use case must be specified with the '-u' flag";
                return result;
            }

            if (string.IsNullOrEmpty(options.InputPath))
            {
                result.ErrorMessage = "Input path must be specified with the '-i' flag";
                return result;
            }

            if (string.IsNullOrEmpty(options.OutputPath))
            {
                result.ErrorMessage = "Output path must be specified with the '-i' flag";
                return result;
            }

            if (string.IsNullOrEmpty(options.Generator))
            {
                result.ErrorMessage = "Generator must be specified with the '-g' flag";
                return result;
            }

            var supportedExtensions = DotNetFileExtension.All;
            var extension = Path.GetExtension(options.InputPath);

            if (!supportedExtensions.Contains(extension))
            {
                result.ErrorMessage = string.Format("Unsupported path extension - {0}.  Supported extensions: {1}", extension, string.Join(", ", supportedExtensions));
                return result;
            }

            // if we got this far, the arguments are valid
            result.Success = true;

            return result;
        }

        private static Configuration MapConfiguration(Options options)
        {
            return new Configuration()
            {
                InputPath = options.InputPath,
                OutputPath = options.OutputPath,
                Generator = options.Generator
            };
        }
    }
}
