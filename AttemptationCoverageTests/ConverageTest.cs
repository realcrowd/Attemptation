using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.Coverage.Analysis;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Xsl;

namespace AttemptationCoverageTests
{
    [TestClass]
    public class ConverageTest
    {
        [TestMethod]
        public void AttemptationUnitTestCoverageTest()
        {
            //Process coverageProcess = new Process();
            ////ProcessStartInfo processStartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\VsPerfCmd.exe", @"/Start:Coverage /Output:""C:\Projects\Attemptation\AttemptationCoverageTests\bin\Debug\test.coverage""");
            //ProcessStartInfo processStartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\vsinstr.exe", @"/Coverage ""C:\Projects\Attemptation\AttemptationCoverageTests\bin\Debug\Attemptation.dll""");
            //processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //processStartInfo.RedirectStandardError = true;
            //processStartInfo.RedirectStandardOutput = true;
            //processStartInfo.UseShellExecute = false;
            //coverageProcess.StartInfo = processStartInfo;

            //var output = Run(coverageProcess);

            //Console.WriteLine(output);

            //coverageProcess = new Process();
            //processStartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\VsPerfCmd.exe", @"/Start:Coverage /Output:""C:\Projects\Attemptation\AttemptationCoverageTests\bin\Debug\test.coverage""");
            ////ProcessStartInfo processStartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\vsinstr.exe", @"/Coverage ""C:\Projects\Attemptation\AttemptationCoverageTests\bin\Debug\AttemptationUnitTests.dll""");
            //processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //processStartInfo.RedirectStandardError = true;
            //processStartInfo.RedirectStandardOutput = true;
            //processStartInfo.UseShellExecute = false;
            //coverageProcess.StartInfo = processStartInfo;

            //output = Run(coverageProcess);

            //Console.WriteLine(output);

            //Process myProcess = new Process();
            //ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\mstest.exe", "/testcontainer:" + "AttemptationUnitTests.dll"); // + " /testsettings:AttemptationUnitTests.testsettings /nologo");

            //myProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //myProcessStartInfo.RedirectStandardError = true;
            //myProcessStartInfo.RedirectStandardOutput = true;
            //myProcessStartInfo.UseShellExecute = false;
            //myProcess.StartInfo = myProcessStartInfo;

            //output = Run(myProcess);
            //Console.WriteLine(output);

            //processStartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\VSPerfCmd.exe", "-shutdown");
            //processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //processStartInfo.RedirectStandardError = true;
            //processStartInfo.RedirectStandardOutput = true;
            //processStartInfo.UseShellExecute = false;
            //coverageProcess = new Process();
            //coverageProcess.StartInfo = processStartInfo;


            ////Microsoft.VisualStudio.Coverage.Monitor.dll

            //output = Run(coverageProcess);

            //Console.WriteLine(output);
            CoverageStatistics stats;

            var dir = new System.IO.DirectoryInfo(Directory.GetCurrentDirectory());
            var path = dir.Parent.Parent.FullName;

            var symbolsDir = System.IO.Path.Combine(path, @"AttemptationUnitTests\bin\Release");
            var sourceFile = System.IO.Path.Combine(path, @"test-coverage.coverage");
            //var symbolsDir = @"C:\Projects\Attemptation\AttemptationCoverageTests\bin\Debug";
            //var sourceFile = @"C:\Projects\Attemptation\AttemptationCoverageTests\bin\Debug\test.coverage";
            //using (CoverageInfo info = CoverageInfo.CreateFromFile(
            //                sourceFile, executablePaths: new[] { symbolsDir }, symbolPaths: new[] { symbolsDir }))
            using (CoverageInfo info = CoverageInfo.CreateFromFile(
                            sourceFile, executablePaths: new[] { symbolsDir }, symbolPaths: new[] { symbolsDir }))
            {
                CoverageDS dataSet = info.BuildDataSet(null);
                string outputFile = Path.ChangeExtension(sourceFile, "xml");

                // Unless an output dir is specified
                // the converted files will be stored in the same dir
                // as the source files, with the .XML extension

                dataSet.ExportXml(outputFile);
                stats = GetStatsInfo(info);
                CovertToEmma(path);
            }

            Console.WriteLine("    {0} total blocks covered", stats.BlocksCovered);
            Console.WriteLine("    {0} total blocks not covered", stats.BlocksNotCovered);
            Console.WriteLine("    {0} total lines covered", stats.LinesCovered);
            Console.WriteLine("    {0} total lines partially covered", stats.LinesPartiallyCovered);
            Console.WriteLine("    {0} total lines not covered", stats.LinesNotCovered);
        }

        private static void CovertToEmma(string path)
        {
            XslCompiledTransform myXslTransform;
            myXslTransform = new XslCompiledTransform();
            myXslTransform.Load("MSTestCoverageToEmma.xslt");
            myXslTransform.Transform(System.IO.Path.Combine(path, @"test-coverage.xml"), System.IO.Path.Combine(path, @"emma-test-coverage.xml"));
        }

        private static CoverageStatistics GetStatsInfo(CoverageInfo info)
        {
            CoverageStatistics totalStats = new CoverageStatistics();

            List<BlockLineRange> lines = new List<BlockLineRange>();

            foreach (ICoverageModule module in info.Modules)
            {
                byte[] coverageBuffer = module.GetCoverageBuffer(null);

                using (ISymbolReader reader = module.Symbols.CreateReader())
                {
                    uint methodId;
                    string methodName;
                    string undecoratedMethodName;
                    string className;
                    string namespaceName;

                    lines.Clear();
                    while (reader.GetNextMethod(
                        out methodId,
                        out methodName,
                        out undecoratedMethodName,
                        out className,
                        out namespaceName,
                        lines))
                    {
                        CoverageStatistics stats = CoverageInfo.GetMethodStatistics(coverageBuffer, lines);

                        Console.WriteLine("Method {0}{1}{2}{3}{4} has:",
                            namespaceName == null ? "" : namespaceName,
                            string.IsNullOrEmpty(namespaceName) ? "" : ".",
                            className == null ? "" : className,
                            string.IsNullOrEmpty(className) ? "" : ".",
                            methodName
                            );

                        Console.WriteLine("    {0} blocks covered", stats.BlocksCovered);
                        Console.WriteLine("    {0} blocks not covered", stats.BlocksNotCovered);
                        Console.WriteLine("    {0} lines covered", stats.LinesCovered);
                        Console.WriteLine("    {0} lines partially covered", stats.LinesPartiallyCovered);
                        Console.WriteLine("    {0} lines not covered", stats.LinesNotCovered);

                        totalStats.BlocksCovered += stats.BlocksCovered;
                        totalStats.BlocksNotCovered += stats.BlocksNotCovered;
                        totalStats.LinesCovered += stats.LinesCovered;
                        totalStats.LinesNotCovered += stats.LinesNotCovered;
                        totalStats.LinesPartiallyCovered += stats.LinesPartiallyCovered;


                        lines.Clear();
                    }
                }
            }

            return totalStats;
        }

        private string Run(Process myProcess, bool skipWait = false)
        {
            var sbOutput = new StringBuilder();
            var sbError = new StringBuilder();

            if (skipWait)
            {
                myProcess.Start();
                return string.Empty;
            }


            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                DataReceivedEventHandler outputReceived = (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        sbOutput.AppendLine(e.Data);
                    }
                };

                myProcess.OutputDataReceived += outputReceived;

                DataReceivedEventHandler errorReceived = (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        sbError.AppendLine(e.Data);
                    }
                };

                myProcess.ErrorDataReceived += errorReceived;

                myProcess.Start();

                myProcess.BeginOutputReadLine();
                myProcess.BeginErrorReadLine();

                string output;

                if (myProcess.WaitForExit(60000) &&
                    outputWaitHandle.WaitOne(60000) &&
                    errorWaitHandle.WaitOne(60000))
                {
                    output = (sbError.Length > 0) ? sbError.ToString() : sbOutput.ToString();
                }
                else
                {
                    output = "timeout";
                }

                myProcess.OutputDataReceived -= outputReceived;
                myProcess.ErrorDataReceived -= errorReceived;

                return output;
            }
        }
    }
}
