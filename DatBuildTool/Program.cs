using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatBuildTool
{
    class Program
    {
        static readonly string[] fileExtentions = {".cpp", ".hpp", ".h"};

        static int Main(string[] args)
        {
            string path = "";

            if (args.Length == 0)
            {
                Console.Out.WriteLine("enter starting directory:");
                path = "C:\\Users\\Jacob\\source\\repos\\Dat3dEngine\\Dat3dEngine\\Engine\\Config\\engineconfig.h"; // Console.In.ReadLine();
            }

            processFile(path, Path.GetDirectoryName(path));

            //if (Directory.Exists(path))
            //{
            //    searchDirectory(path, path);
            //}
            //else
            //{
            //    Console.Out.WriteLine("error, bad directory");
            //    return;
            //}
            Console.In.ReadLine();
            return 0;
        }

        static void searchDirectory(string directory, string basePath)
        {
            foreach (string path in Directory.GetDirectories(directory))
            {
                searchDirectory(path, basePath);
            }

            foreach (string path in Directory.GetFiles(directory))
            {
                
                if (fileExtentions.Contains(Path.GetExtension(path).ToLower()))
                {
                    processFile(path, basePath);
                }
            }
        }

        static bool processFile(string filePath, string basePath)
        {
            string genFileName = Path.ChangeExtension(filePath, "gen");
            // Create and Open File
            if (Directory.Exists(genFileName)){
                if (Directory.GetLastWriteTime(genFileName) < Directory.GetLastWriteTime(filePath))
                {
                    Directory.Delete(genFileName);
                } else
                {
                    return true;
                }
            }

            List<ClassStruct> classes = new List<ClassStruct>();
            StreamReader originalFile = new StreamReader(filePath);
            int lineCount = 0;
            string line = "";
            string fileLine;
            bool foundDat = false;
            while ((fileLine = originalFile.ReadLine()) != null)
            {
                lineCount++;
                fileLine = fileLine.Trim();
                // Ignore empty lines and preprocessor stuff
                if (fileLine.Length == 0) continue;
                Console.Out.WriteLine("Line " + lineCount + ": " + fileLine);
                if (fileLine.StartsWith("#")) continue;
                if (fileLine.StartsWith("DatClass(")) foundDat = true;
                if (foundDat)
                {
                    // Add onto current line
                    if (line.Length == 0) line += fileLine;
                    else line += " " + fileLine;


                    if (line.Contains("{"))
                    {
                        // We've got a class
                        if (line.Contains("class"))
                        {
                            ClassStruct newClass = new ClassStruct(line);
                            Console.Out.WriteLine(line);
                            if (!newClass.parseClass(ref originalFile, ref lineCount))
                            {
                                return false;
                            }
                            classes.Add(newClass);
                            foundDat = false;
                            line = "";
                        }
                        // Failure
                        else
                        {
                            return false;
                            line = "";
                        }
                    }
                }
            }

            //StreamWriter outputFile = new StreamWriter(genFileName);
            //outputFile.WriteLine("#pragma once");
            //outputFile.WriteLine("#include <Engine/ReflectionMacros.h>");

            //// Calculate FileTitle
            //string fileTitle = getRelativePath(basePath, filePath);
            //genFileName = genFileName.Substring(2).Replace('/', '_').Replace('\\', '_').Replace('.', '_');
            //Console.Out.WriteLine(genFileName);
            return true;
        }

        static string getRelativePath(string relativeTo, string path)
        {
            string relativeToFull = Path.GetFullPath(relativeTo);
            string pathFull = Path.GetFullPath(path);

            StringComparison comparitor;

#if __linux__
            comparitor = StringComparison.Ordinal;
#else
            comparitor = StringComparison.OrdinalIgnoreCase;
#endif

            if (relativeToFull.Equals(pathFull.Substring(0, relativeToFull.Length), comparitor))
            {
                return "." + pathFull.Substring(relativeToFull.Length);
            } else
            {
                return path;
            }
        }
    }
}
