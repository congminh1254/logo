using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Logo.Core.Utils;

namespace Logo.Core
{
    public class FileReader
    {
        string filePath;
        string[] lines;
        int position = -1;
        int line = 0;
        string newline = System.Environment.NewLine;
        char newlineChar = '\n';
        char eof = (char)3;

        public FileReader(string input, bool file = true)
        {
            if (file)
            {
                this.filePath = input;
                loadFile();
            }
            else
            {
                lines = input.Split('\n');
            }
        }

        private void loadFile()
        {
            lines = File.ReadAllLines(filePath);
        }

        public string getAllText()
        {
            return string.Join(newline, lines);
        }

        public char getNextChar()
        {
            if (line >= lines.Length)
            {
                return eof;
            } else
            {
                if (position == lines[line].Length - 1)
                {
                    position = -1;
                    line++;
                    return newlineChar;
                }
                position++;
                return lines[line][position];
            }
        }

        public char previewChar()
        {
            if (line >= lines.Length)
                return eof;
            else
            {
                if (position == lines[line].Length - 1)
                {
                    return newlineChar;
                } else
                {
                    return lines[line][position + 1];
                }
            }
        }

        public Position getPostion()
        {
            return new Position(line, position);
        }
    }
}
