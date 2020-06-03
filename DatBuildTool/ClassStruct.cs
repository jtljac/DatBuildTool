using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatBuildTool
{
    enum DatType
    {
        NONE,
        MEMBER,
        FUNCTION,
        CLASS
    }

    class MemberStruct
    {
        List<string> modifiers = new List<string>();
        string type;
        string name;

        public MemberStruct(string line)
        {
            List<string> components = Utils.smartSplit(line.Substring(0, line.IndexOf(';')).Trim(), ' ');
            type = components[1];
            if (components[2].Contains("="))
            {
                name = components[2].Substring(0, line.IndexOf('='));
            } else
            {
                name = components[2];
            }
        }

        void parseModifiers(string Modifiers)
        {
            foreach (string modifier in Modifiers.Split(','))
            {
                modifiers.Add(modifier.Trim());
            }
        }
    }

    class MethodStruct
    {
        List<string> modifiers = new List<string>();
        bool isStatic = false;
        bool isConst = false;
        string returnType;
        string name;
        List<ArgumentStruct> arguments = new List<ArgumentStruct>();

        MethodStruct(string line)
        {
            List<string> components = Utils.smartSplit(line.Substring(0, line.IndexOf(';')).Trim(), ' ');
            type = components[1];
            if (components[2].Contains("="))
            {
                name = components[2].Substring(0, line.IndexOf('='));
            }
            else
            {
                name = components[2];
            }
        }

        void parseModifiers(string Modifiers)
        {
            foreach (string modifier in Modifiers.Split(','))
            {
                modifiers.Add(modifier.Trim());
            }
        }
    }

    class ArgumentStruct
    {
        string name;
        string type;
    }

    class ClassStruct
    {
        string name;
        List<string> modifiers = new List<string>();
        int generatedLine = -1;
        List<MemberStruct> members = new List<MemberStruct>();
        List<MethodStruct> methods = new List<MethodStruct>();
        List<ClassStruct> subClasses = new List<ClassStruct>();
        string error = "";

        public ClassStruct(string line)
        {
            List<string> components = Utils.smartSplit(line.Substring(0, line.IndexOf('{')).Trim(), ' ');
            parseModifiers(components[0].Substring(components[0].IndexOf('(') + 1, components[0].IndexOf(')') - components[0].IndexOf('(') - 1));
        }

        void parseModifiers(string Modifiers)
        {
            foreach(string modifier in Modifiers.Split(','))
            {
                modifiers.Add(modifier.Trim());
            }
        }

        public bool parseClass(ref StreamReader File, ref int lineCount)
        {
            string fileLine;
            string line = "";
            DatType datType = DatType.NONE;
            int depth = 0;
            while ((fileLine = File.ReadLine()) != null && depth >= 0)
            {
                lineCount++;
                fileLine = fileLine.Trim();
                // Ignore empty lines and preprocessor stuff
                if (fileLine.Length == 0) continue;
                Console.Out.WriteLine("Line " + lineCount + ": " + fileLine);
                if (fileLine.StartsWith("#")) continue;
                if (fileLine.StartsWith("//")) continue;

                // Dat Things
                if (fileLine.StartsWith("DatClass(")) datType = DatType.CLASS;
                else if (fileLine.StartsWith("DatMember(")) datType = DatType.MEMBER;
                else if (fileLine.StartsWith("DatFunction(")) datType = DatType.FUNCTION;

                // Engine Generated stuff
                if (fileLine.StartsWith("ENGINE_GENERATED()"))
                {
                    if (generatedLine == -1)
                    {
                        generatedLine = lineCount;
                    } else
                    {
                        return false;
                    }
                }

                if (fileLine.Contains('{')) depth++;
                else if (fileLine.Contains('}')) depth--;

                // Bunch together lines for ease
                if (datType != DatType.NONE)
                {
                    // Add onto current line
                    if (line.Length == 0) line += fileLine;
                    else line += " " + fileLine;
                }

                // Specifics
                switch (datType)
                {
                    case DatType.MEMBER:
                        if (line.Contains(';'))
                        {
                            MemberStruct member = new MemberStruct(line);
                            members.Add(member);
                            line = "";
                            datType = DatType.NONE;
                        }
                        break;
                    case DatType.FUNCTION:
                        if (line.Contains('{') || line.Contains(';'))
                        {
                            MethodStruct method = new MethodStruct(line);
                            methods.Add(method);
                            line = "";
                            datType = DatType.NONE;
                        }
                        break;
                }
            }
            return true;
        }
    }
}
