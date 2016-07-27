namespace AsmSpy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Native;

    public static class Program
    {
        private static readonly ConsoleColor[] ConsoleColors = {
            ConsoleColor.Green,
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Blue,
            ConsoleColor.Cyan,
            ConsoleColor.Magenta,
        };

        private static readonly String[] HelpSwitches = { "/?", "-?", "-help", "--help" };
        private static readonly String[] NonSystemSwitches = { "/n", "nonsystem", "/nonsystem" };
        private static readonly String[] AllSwitches = { "/a", "all", "/all" };

        private static void Main(String[] args)
        {
            if (
                args.Length > 3 || 
                args.Length < 1 || 
                args.Any(a => HelpSwitches.Contains(a, StringComparer.OrdinalIgnoreCase)))
            {
                PrintUsage();
                return;
            }

            var directoryPath = args[0];
            if (!Directory.Exists(directoryPath))
            {
                PrintDirectoryNotFound(directoryPath);
                return;
            }


            var onlyConflicts = !args.Skip(1).Any(x => AllSwitches.Contains(x, StringComparer.OrdinalIgnoreCase));  
            var skipSystem = args.Skip(1).Any(x => NonSystemSwitches.Contains(x, StringComparer.OrdinalIgnoreCase));

            AnalyseAssemblies(new DirectoryInfo(directoryPath), onlyConflicts, skipSystem);
        }

        public static void AnalyseAssemblies(DirectoryInfo directoryInfo, Boolean onlyConflicts, Boolean skipSystem)
        {
            var assemblyFiles = directoryInfo.GetFiles("*.dll").Concat(directoryInfo.GetFiles("*.exe")).ToList();

            if (!assemblyFiles.Any())
            {
                Console.WriteLine( $"No dll files found in directory: '{directoryInfo.FullName}'" );
                return;
            }

            Console.WriteLine("Check assemblies in:");
            Console.WriteLine(directoryInfo.FullName);
            Console.WriteLine("");

            var assemblies = new Dictionary<String, IList<ReferencedAssembly>>(StringComparer.OrdinalIgnoreCase);
            foreach (var fileInfo in assemblyFiles.OrderBy(asm => asm.Name))
            {
                Assembly assembly;
                try
                {
                    if (!fileInfo.IsAssembly())
                    {
                        continue;
                    }
                    assembly = Assembly.ReflectionOnlyLoadFrom(fileInfo.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine( $"Failed to load assembly '{fileInfo.FullName}': {ex.Message}" );
                    continue;
                }

                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {
                    if (!assemblies.ContainsKey(referencedAssembly.Name))
                    {
                        assemblies.Add(referencedAssembly.Name, new List<ReferencedAssembly>());
                    }
                    assemblies[referencedAssembly.Name]
                        .Add(new ReferencedAssembly(referencedAssembly.Version, assembly));
                }
            }

            if (onlyConflicts)
                Console.WriteLine("Detailing only conflicting assembly references.");

            foreach (var assemblyReferences in assemblies.OrderBy(i => i.Key))
            {
                if ( skipSystem && ( assemblyReferences.Key.StartsWith( "System" ) || assemblyReferences.Key.StartsWith( "mscorlib" ) ) ) {
                    continue;
                }

                if ( onlyConflicts && assemblyReferences.Value.GroupBy( x => x.VersionReferenced ).Count() == 1 ) {
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Reference: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine( $"{assemblyReferences.Key}" );

                var referencedAssemblies = new List<Tuple<String, String>>();
                var versionsList = new List<String>();
                var asmList = new List<String>();
                foreach (var referencedAssembly in assemblyReferences.Value)
                {
                    var s1 = referencedAssembly.VersionReferenced.ToString();
                    var s2 = referencedAssembly.ReferencedBy.GetName().Name;
                    var tuple = new Tuple<String, String>(s1, s2);
                    referencedAssemblies.Add(tuple);
                }

                foreach (var referencedAssembly in referencedAssemblies)
                {
                    if (!versionsList.Contains(referencedAssembly.Item1))
                    {
                        versionsList.Add(referencedAssembly.Item1);
                    }
                    if (!asmList.Contains(referencedAssembly.Item1))
                    {
                        asmList.Add(referencedAssembly.Item1);
                    }
                }

                foreach (var referencedAssembly in referencedAssemblies)
                {
                    var versionColor = ConsoleColors[versionsList.IndexOf(referencedAssembly.Item1) % ConsoleColors.Length];

                    Console.ForegroundColor = versionColor;
                    Console.Write( $"   {referencedAssembly.Item1}" );

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" by ");

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine( $"{referencedAssembly.Item2}" );
                }

                Console.WriteLine();
            }
        }

        private static void PrintDirectoryNotFound(String directoryPath)
        {
            Console.WriteLine( $"Directory: \'{directoryPath}\' does not exist." );
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("AsmSpy <directory to load assemblies from> [options]");
            Console.WriteLine();

            Console.WriteLine("Switches:");
            Console.WriteLine("/all       : list all assemblies and references. Supported formats:  " + String.Join(",", AllSwitches));
            Console.WriteLine("/nonsystem : list system assemblies. Supported formats:  " + String.Join(",", NonSystemSwitches));
            Console.WriteLine();

            Console.WriteLine("E.g.");
            Console.WriteLine(@"AsmSpy C:\Source\My.Solution\My.Project\bin\Debug");
            Console.WriteLine(@"AsmSpy C:\Source\My.Solution\My.Project\bin\Debug all");
            Console.WriteLine(@"AsmSpy C:\Source\My.Solution\My.Project\bin\Debug nonsystem");
        }
    }
}
