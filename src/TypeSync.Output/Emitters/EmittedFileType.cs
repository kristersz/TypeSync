using System.ComponentModel;

namespace TypeSync.Output.Emitters
{
    public enum EmittedFileType
    {
        [Description("model")]
        Model,

        [Description("enum")]
        Enum,

        [Description("service")]
        Service
    }
}
