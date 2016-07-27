using System.IO;

namespace AsmSpy
{
    using System;

    public class UnitTests
    {
        public void AnalyseAssemblies_should_output_correct_info_only_conflicts()
        {
            const String path = @"D:\Source\sutekishop\Suteki.Shop\Suteki.Shop.CreateDb\bin\Debug";
            Program.AnalyseAssemblies(new DirectoryInfo(path), true, false);
        }

        public void AnalyseAssemblies_should_output_correct_info()
        {
            const String path = @"D:\Source\sutekishop\Suteki.Shop\Suteki.Shop.CreateDb\bin\Debug";
            Program.AnalyseAssemblies(new DirectoryInfo(path), false, false);
        }
    }
}