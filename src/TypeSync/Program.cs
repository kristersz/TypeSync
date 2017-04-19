using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Config;
using TypeSync.Providers;
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

#if DEBUG
            args = new string[] { "GenerateModels" };
#endif

            try
            {
                var useCases = new List<IUseCase>()
                {
                    new ModelGenerationUseCase(new JsonConfigurationProvider()),
                    new WebClientGenerationUseCase(new JsonConfigurationProvider()),
                    new ValidatorGenerationUseCase(),
                    new ProjectTemplateScaffoldingUseCase()
                };

                string selectedUseCaseId = args.Length > 0 ? args[0] : string.Empty;

                if (string.IsNullOrEmpty(selectedUseCaseId))
                {
                    Console.WriteLine("Select a use case: ");

                    foreach (var useCase in useCases)
                    {
                        Console.WriteLine($"Id: {useCase.Id}; Description: {useCase.Description}");
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
                    var result = selectedUseCase.Handle();

                    if (!result.Success)
                    {
                        Console.WriteLine($"error {result.ErrorCode}: {result.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception thrown: " + ex.Message + "\n" + ex.StackTrace);
            }

            log.Info("Exiting TypeSync");
        }
    }
}
