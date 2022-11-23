using Logo.Core.Utils;
using System.Collections.Generic;

namespace Logo.Core
{
    public class Lexer
    {
        SourceCode source;

        Token token;
        Token previousToken;

        char eof = (char)3;

        static Dictionary<string, string> keywords = new Dictionary<string, string>()
        {
            {"AND", TokenType.AND},
            { "OR", TokenType.OR },
            { "NOT", TokenType.NOT },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "while", TokenType.WHILE },
            { "return", TokenType.RETURN },
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
            { "int", TokenType.INT },
            { "str", TokenType.STR },
            { "float", TokenType.FLOAT },
            { "bool", TokenType.BOOL },
            { "turtle", TokenType.TURTLE },
            { "___", TokenType.UUU }
        };
        static Dictionary<char, string> singleChar = new Dictionary<char, string>()
        {
            { '=', TokenType.EQ },
            { '.', TokenType.DOT },
            { ',', TokenType.COMMA },
            { ';', TokenType.SEMI},
            { '(', TokenType.LPAREN},
            { ')', TokenType.RPAREN },
            { '{', TokenType.LCURLY },
            { '}', TokenType.RCURLY },
            { ':', TokenType.COLON },
            { '#', TokenType.HASH},
            { '_', TokenType.UND },
            { '+', TokenType.PLUS },
            { '-', TokenType.MINUS },
            { '*', TokenType.MUL },
            { '/', TokenType.DIV },
            { '%', TokenType.MOD },
        };
        static Dictionary<string, string> doubleChar = new Dictionary<string, string>()
        {
            { "<=", TokenType.LE },
            { ">=", TokenType.GE },
            { "==", TokenType.EQEQ },
            { "!=", TokenType.DIFF},
            { "\\"+"\"", TokenType.QUOTE }
        };

        public Lexer(SourceCode source)
        {
            this.source = source;
        }

        public Token getToken()
        {
            return token;
        }

        public bool isWhiteSpace(char c)
        {
            if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
                return true;
            return false;
        }

        char getNextChar()
        {
            while (isWhiteSpace(source.getNextChar()))
            {

            }
            return source.getChar();
        }

        public Token advanceToken()
        {
            previousToken = token;
            char c = getNextChar();

            if (c == eof)
            {
                token = new Token(TokenType.EOF, source.getPosition(), "");
                return token;
            }

            string result = null;
            singleChar.TryGetValue(c, out result);

            if (!string.IsNullOrEmpty(result) )


        }
    }
}
