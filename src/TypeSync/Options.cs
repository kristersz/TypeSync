using CommandLine;

namespace TypeSync
{
    public class Options
    {
        [Option('u', "usecase")]
        public string UseCase { get; set; }

        [Option('i', "input")]
        public string InputPath { get; set; }

        [Option('o', "output")]
        public string OutputPath { get; set; }

        [Option('g', "generator")]
        public string Generator { get; set; }
    }
}
