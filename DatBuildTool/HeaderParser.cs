using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatBuildTool
{
    class HeaderParser
    {
        StreamReader file;
        int currentLine = 0;
        int depth = 0;

        public HeaderParser(string filePath)
        {
            file = new StreamReader(filePath);
        }

        public void parseFile()
        {
            string fileString = file.ReadToEnd();
            string virtualLine = "";
            string buffer = "";
            bool eof = false;

            for (int i = 0; !eof; i++)
            {

            }
        }

        public ClassStruct parseClass(string declaration)
        {

            return null;
        }

        public MemberStruct parseMember(string declaration)
        {

            return null;
        }

        public MemberStruct parseMethod(string declaration)
        {

            return null;
        }
    }
}
