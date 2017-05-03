using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TCXSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
                PrintErrorAndExit("No parameter for TCX file.", -1);

            var tcxFilePath = args[0];
            if(!File.Exists(tcxFilePath))
                PrintErrorAndExit($"File '{tcxFilePath}' not found.", -2);

            var xDoc = XDocument.Load(tcxFilePath);
            XNamespace ns = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2";
            var activites = xDoc.Element(ns + "TrainingCenterDatabase").Element(ns + "Activities").Elements();
            foreach (var activity in activites)
            {
                var id = activity.Element(ns + "Id").Value;
                var xOutputBase =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<TrainingCenterDatabase" +
                "  xsi:schemaLocation=\"http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2 http://www.garmin.com/xmlschemas/TrainingCenterDatabasev2.xsd\"" +
                "  xmlns:ns5=\"http://www.garmin.com/xmlschemas/ActivityGoals/v1\"" +
                "  xmlns:ns3=\"http://www.garmin.com/xmlschemas/ActivityExtension/v2\"" +
                "  xmlns:ns2=\"http://www.garmin.com/xmlschemas/UserProfile/v2\"" +
                "  xmlns=\"http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2\"" +
                "  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:ns4=\"http://www.garmin.com/xmlschemas/ProfileExtension/v1\">" +
                "  <Activities>" +
                $"    {activity.ToString()}" +
                "  </Activities>" +
                "  <Author xsi:type=\"Application_t\">" +
                "    <Name>TCXSplitter</Name>" +
                "    <Build>" +
                "      <Version>" +
                "        <VersionMajor>1</VersionMajor>" +
                "        <VersionMinor>0</VersionMinor>" +
                "        <BuildMajor>0</BuildMajor>" +
                "        <BuildMinor>0</BuildMinor>" +
                "      </Version>" +
                "    </Build>" +
                "    <LangID>en</LangID>" +
                "    <PartNumber>-</PartNumber>" +
                "  </Author>" +
                "</TrainingCenterDatabase>";
                var fileName = Path.Combine(Path.GetDirectoryName(tcxFilePath), MakeValidFileName(id + ".tcx"));
                File.WriteAllText(fileName, xOutputBase);
                Console.WriteLine($"Created file {fileName}");
            }
            Console.WriteLine("Finished! Press a key to close the program.");
            Console.Read();
        }

        static void PrintErrorAndExit(string error, int code)
        {
            Console.WriteLine($"Error:{Environment.NewLine}" +
                $"\t{error}" +
                $"Usage: {Environment.NewLine}" +
                $"\tTCXSplitter.exe C:\\Path\\To\\TCX\\File.tcx");
            Environment.Exit(code);
        }

        static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

    }
}