using System.IO;
using TypeSync.Common.Constants;
using TypeSync.Common.Utilities;

namespace TypeSync.Output.Emitters
{
    public class TypeScriptEmitter
    {
        public void Emit(string path, string name, EmittedFileType fileType, string contents)
        {
            string fileTypeName = string.Empty;

            switch (fileType)
            {
                case EmittedFileType.Model:
                    fileTypeName = "model";
                    break;
                case EmittedFileType.Enum:
                    fileTypeName = "enum";
                    break;
                case EmittedFileType.Service:
                    fileTypeName = "service";
                    break;
                default:
                    break;
            }

            string fileName = $"{NameCaseConverter.ToKebabCase(name)}.{fileTypeName}.{TypeScriptFileExtension.File}";

            File.WriteAllText(Path.Combine(path, fileName), contents);
        }
    }
}
