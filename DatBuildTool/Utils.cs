using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatBuildTool
{
    class Utils
    {
        public static List<string> smartSplit(string theString, char splitCharater)
        {
            char[] deepCharacters = { '[', '{', '<', '(' };
            char[] shallowCharacters = { ']', '}', '>', ')', };
            List<string> itemList = new List<string>();
            string buffer = "";
            int depth = 0;

            foreach (char character in theString)
            {
                if (character == splitCharater && depth == 0)
                {
                    itemList.Add(buffer);
                    buffer = "";
                    continue;
                }
                else if (deepCharacters.Contains(character))
                {
                    depth += 1;
                }
                else if (shallowCharacters.Contains(character))
                {
                    depth -= 1;
                }
                buffer += character;
            }

            itemList.Add(buffer);
            return itemList;
        }
    }
}
