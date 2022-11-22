using CommandLine;

namespace DirectoryNavigator
{
    class Options
    {

        [Verb("run", isDefault: true, HelpText = "Run.")]
        internal class Run
        {

        }


        [Verb("create", isDefault: false, HelpText = "Create a new directory.")]
        internal class Create
        {
            [Option('n', "name", HelpText = "Directory name.", Required = true)]
            public string Name { get; set; }
        }

    }
}
