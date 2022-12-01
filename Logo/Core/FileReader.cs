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
                    if (line >= lines.Length)
                        return eof;
                    return newlineChar;
                }
                position++;
                return lines[line][position];
            }
        }

        public char previewChar()
        {
            return previewNextNthChar(1);
        }

        public char previewChar2()
        {
            return previewNextNthChar(2);
        }

        public char previewNextNthChar(int n)
        {
            int curLine = line, curPos = position;
            for (int i = 0; i < n; i++)
            {
                if (curLine >= lines.Length
                    || (curPos >= lines[curLine].Length - 1 && curLine == lines.Length - 1))
                    return eof;
                else
                {
                    if (curPos == lines[curLine].Length - 1)
                    {
                        curLine++;
                        curPos = 0;
                        if (i == n - 1)
                            return newlineChar;
                    }
                    else
                    {
                        curPos += 1;
                        if (i == n - 1)
                            return lines[curLine][curPos];
                    }
                }
            }
            return eof;
        }

        public Position getPostion()
        {
            return new Position(line, position);
        }
    }
}
