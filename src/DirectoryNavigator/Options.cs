using CommandLine;

namespace DirectoryNavigator
{
    class Options
    {
        internal class DirectoryPath
        {
            [Option('p', "path", HelpText = "Directory path.", Required = true)]
            public string Path { get; set; }
        }


        [Verb("count", isDefault: true, HelpText = "Count files, directories and total file size.")]
        internal class Count : DirectoryPath
        {

        }

        [Verb("scan", HelpText = "Scan parent-child directories")]
        internal class Scan : DirectoryPath
        {

        }
    }
}
