using log4net;
using TypeSync.Models;

namespace TypeSync.UseCases
{
    public class ProjectTemplateScaffoldingUseCase : IUseCase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectTemplateScaffoldingUseCase));

        public string Id { get; } = "ScaffoldProject";

        public string Name { get; } = "Scaffold entire Angular project templates based on server side ASP.NET Web APIs.";

        public ProjectTemplateScaffoldingUseCase()
        {
        }

        public Result Handle()
        {
            return Result.CreateSuccess();
        }
    }
}
