namespace TypeSync.Common.Constants
{
    public static class DotNetFileExtension
    {
        public static string File => ".cs";
        public static string Project => ".csproj";
        public static string Solution => ".sln";

        public static string[] All => new string[] { File, Project, Solution };
    }
}
