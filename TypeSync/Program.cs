using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Config;
using TypeSync.UseCases;

[assembly: XmlConfigurator(Watch = true)]

namespace TypeSync
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            log.Info("Entering TypeSync");

            try
            {
                var useCases = new List<IUseCase>()
                {
                    new ModelGenerationUseCase(@"C:\Dev\VS2017\TypeSync\Samples\Samples.sln", PathKind.Solution),
                    new WebClientGenerationUseCase(),
                    new ValidatorGenerationUseCase(),
                    new ProjectTemplateScaffoldingUseCase()
                };

                string selectedUseCaseId = args.Length > 0 ? args[0] : string.Empty;

                if (string.IsNullOrEmpty(selectedUseCaseId))
                {
                    Console.WriteLine("Select a use case: ");

                    foreach (var useCase in useCases)
                    {
                        Console.WriteLine("Id: {0}; Name: {1}", useCase.Id, useCase.Name);
                    }

                    selectedUseCaseId = Console.ReadLine();
                }

                var selectedUseCase = useCases.FirstOrDefault(u => u.Id == selectedUseCaseId);

                if (selectedUseCase == null)
                {
                    Console.WriteLine("Selected use case was not found.");
                }
                else
                {
                    selectedUseCase.Execute();
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception thrown: " + ex.Message);
            }

            log.Info("Exiting TypeSync");
        }
    }
}
