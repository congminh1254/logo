using Logo.Core.Utils;
using System;
using System.Collections.Generic;

namespace Logo.Core
{
    public class Lexer
    {
        SourceCode source;

        Token token;
        Token previousToken;

        char eof = (char)3;
        char newline = '\n';

        static Dictionary<string, string> keywords = new Dictionary<string, string>()
        {
            { "AND", TokenType.AND},
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
            { '"', TokenType.QUOTE },
            { (char)3, TokenType.EOF },
            { '\n', TokenType.NL }
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

        public List<Token> getAllTokens()
        {
            List<Token> tokens = new List<Token>();
            while (true)
            {
                token = advanceToken();
                tokens.Add(token);
                if (token.getTokenType() == TokenType.EOF)
                    break;
            }
            return tokens;
        }

        public Token getToken()
        {
            return token;
        }

        public bool isWhiteSpace(char c)
        {
            if (c == ' ' || c == '\t' || c == '\r')
                return true;
            return false;
        }

        char getChar()
        {
            return source.getChar();
        }

        char getNextChar(bool skipWhiteSpace = true)
        {
            while (isWhiteSpace(source.getNextChar()) && skipWhiteSpace)
            {

            }
            return source.getChar();
        }

        char previewChar()
        {
            return source.previewChar();
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

            if (result == TokenType.QUOTE)
            {
                token = getStringToken();
                return token;
            }

            if (!string.IsNullOrEmpty(result))
            {
                char c2 = previewChar();
                Position pos = source.getPosition();
                if (c2 == '=' && (c == '=' || c == '!' || c == '>' || c == '<'))
                {
                    c2 = getNextChar();
                    string tokenType;
                    doubleChar.TryGetValue(""+ c + c2, out tokenType);
                    token = new Token(tokenType, pos);
                    return token;
                }
                token = new Token(result, pos);
                return token;
            }

            if (Char.IsLetter(c))
            {
                token = getIdentifierToken();
                return token;
            }

            if (Char.IsDigit(c))
            {
                token = getNumberToken();
                return token;
            }

            token = new Token(TokenType.ERROR, source.getPosition(), "Token not valid!");
            return token;
        }

        Token getStringToken()
        {
            Position position = source.getPosition();
            char prv = '-', c = getNextChar(false);
            string output = "";
            while (c != '"' || prv == '\\')
            {
                if (prv != '\\')
                    output += c;
                prv = c;
                if (c == eof || c == newline)
                {
                    token = new Token(TokenType.ERROR, position, "String closing not found"); 
                    return token;
                }
                c = getNextChar(false);
            }
            return new Token(TokenType.STR, position, output);
        }

        Token getIdentifierToken()
        {
            Position position = source.getPosition();
            string name = ""+source.getChar();
            char c2 = previewChar();
            while (Char.IsLetter(c2) || Char.IsDigit(c2) || c2 == '_')
            {
                name += c2;
                getNextChar();
                c2 = previewChar();
            }
            string tokenType;
            keywords.TryGetValue(name, out tokenType);
            if (string.IsNullOrEmpty(tokenType))
            {
                return new Token(TokenType.IDENTIFIER, position, name);
            }
            return new Token(tokenType, position, name);
        }

        Token getNumberToken()
        {
            Position position = source.getPosition();
            string name = "" + source.getChar();
            char c2 = previewChar();
            string tokenType = TokenType.INT;
            while (Char.IsDigit(c2) || c2 == '.')
            {
                if (c2 == '.')
                {
                    tokenType = TokenType.FLOAT;
                }
                name += c2;
                getNextChar();
                c2 = previewChar();
            }
            if (name[name.Length -1] == '.')
            {
                return new Token(TokenType.ERROR, position, "Number not valid");
            }
            if (tokenType == TokenType.INT)
                return new Token(tokenType, position, int.Parse(name));
            else 
                return new Token(tokenType, position, float.Parse(name));
        }
    }
}
