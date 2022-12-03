using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Logo.Core
{
    public class Lexer
    {
        SourceCode source;

        public Token token { get; set; }

        char eof = Utils.Utils.eof;
        string newline = Utils.Utils.newline;

        Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
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
            { "int", TokenType.INT_T },
            { "str", TokenType.STR_T },
            { "float", TokenType.FLOAT_T },
            { "bool", TokenType.BOOL_T },
            { "Turtle", TokenType.TURTLE },
        };
        Dictionary<char, TokenType> singleChar = new Dictionary<char, TokenType>()
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
            { '<', TokenType.LT },
            { '>', TokenType.GT },
            { Utils.Utils.eof, TokenType.EOF }
        };
        Dictionary<string, TokenType> doubleChar = new Dictionary<string, TokenType>()
        {
            { "<=", TokenType.LE },
            { ">=", TokenType.GE },
            { "==", TokenType.EQEQ },
            { "!=", TokenType.DIFF},
            { "\\"+"\"", TokenType.QUOTE },
            { "__", TokenType.UU }
        };

        public Lexer(SourceCode source)
        {
            if (newline.Length > 1)
                doubleChar.Add(newline, TokenType.NL);
            else
                singleChar.Add(newline[0], TokenType.NL);
            this.source = source;
        }

        char getNextChar(bool skipWhiteSpace = true)
        {
            char c = source.peekChar();
            while ((char.IsWhiteSpace(c) && c!= newline[0] && !(newline.Length == 2 && newline[1] == c)) && skipWhiteSpace && c != eof)
            {
                source.getNextChar();
                c = source.peekChar();
            }
            return source.getNextChar();
        }

        bool checkNewline(char c)
        {
            if ((newline.Length > 1 && newline[0] == c && newline[1] == source.peekChar()) || (newline.Length == 0 && newline[0] == c))
            {
                return true;
            }
            return false;
        }

        public Token advanceToken()
        {
            char c = getNextChar();
            if (c == eof)
            {
                token = new Token(TokenType.EOF, source.getPosition(), "");
                return token;
            }

            TokenType result;
            singleChar.TryGetValue(c, out result);

            token = buildStringToken();
            if (token != null)
                return token;

            char c2 = source.peekChar();
            if (singleChar.ContainsKey(c))
            {
                Position pos = source.getPosition();
                if (c2 == '=' && (c == '=' || c == '!' || c == '>' || c == '<'))
                {
                    c2 = getNextChar();
                    TokenType tokenType;
                    doubleChar.TryGetValue(""+ c + c2, out tokenType);
                    token = new Token(tokenType, pos);
                    return token;
                }
                if (c2 == '_' && c == '_')
                {
                    return new Token(TokenType.UU, pos);
                }
                token = new Token(result, pos);
                return token;
            }
            if ((newline.Length == 1 && c == newline[0]) || (newline.Length == 2 && c == newline[0] && c2 == newline[1]))
            {
                c2 = getNextChar();
                token = new Token(TokenType.NL, source.getPosition());
                return token;
            }
            token = buildIdentifierToken();
            if (token != null) return token;

            token = buildNumberToken();
            if (token != null) return token;

            token = new Token(TokenType.ERROR, source.getPosition(), "Token not valid!");
            return token;
        }

        Token buildStringToken()
        {
            TokenType result;
            singleChar.TryGetValue(source.getCurrChar(), out result);
            if (result != TokenType.QUOTE)
                return null;
            Position position = source.getPosition();
            char prv = '-', c = getNextChar(false);
            string output = "";
            while (c != '"' || prv == '\\')
            {
                if (prv != '\\')
                {
                    if (c != '\\') {
                        output += c;
                    }
                    prv = c;
                }
                else
                {
                    prv = '-';
                    switch (c)
                    {
                        case '\"':
                        case '\\':
                            output += c;
                            break;
                        case 'n':
                            output += '\n';
                            break;
                        case 't':
                            output += '\t';
                            break;
                        default:
                            token = new Token(TokenType.ERROR, position, String.Format("Char '\\{0}' is not supported", c));
                            break;
                    }
                }
                
                if (c == eof || checkNewline(c))
                {
                    token = new Token(TokenType.ERROR, position, "String closing not found"); 
                    return token;
                }
                c = getNextChar(false);
            }
            return new Token(TokenType.STR, position, output);
        }

        Token buildIdentifierToken()
        {
            if (!Char.IsLetter(source.getCurrChar()))
                return null;
            Position position = source.getPosition();
            string name = ""+source.getCurrChar();
            char c2 = source.peekChar();
            while (Char.IsLetter(c2) || Char.IsDigit(c2) || c2 == '_')
            {
                name += c2;
                source.getNextChar();
                c2 = source.peekChar();
            }
            
            TokenType tokenType;
            if (!keywords.TryGetValue(name, out tokenType))
            {
                return new Token(TokenType.IDENTIFIER, position, name);
            } else if (tokenType == TokenType.TRUE || tokenType == TokenType.FALSE)
                return new Token(TokenType.BOOL, position, (tokenType == TokenType.TRUE));
            return new Token(tokenType, position, name);
        }

        Token buildNumberToken()
        {
            if (!Char.IsDigit(source.getCurrChar()))
                return null;
            Position position = source.getPosition();
            string name = "" + source.getCurrChar();
            char c2 = source.peekChar();
            TokenType tokenType = TokenType.INT;
            while (Char.IsDigit(c2) || c2 == '.')
            {
                if (c2 == '.')
                {
                    if (tokenType == TokenType.INT)
                    {
                        tokenType = TokenType.FLOAT;
                    } else
                    {
                        return new Token(TokenType.ERROR, position, "Number is not valid");
                    }
                }
                name += c2;
                source.getNextChar();
                c2 = source.peekChar();
            }
            if (name[name.Length -1] == '.')
            {
                return new Token(TokenType.ERROR, position, "Number not valid");
            }
            if (tokenType == TokenType.INT)
            {
                int value = Utils.Utils.charToNumber(name[0]);
                for (int i = 1; i < name.Length; i++)
                    value = value * 10 + Utils.Utils.charToNumber(name[i]);
                return new Token(tokenType, position, value);
            }
            else
            {
                float f = Utils.Utils.charToNumber(name[0]);
                int div = 1;
                bool precision = false;
                for (int i = 1; i<name.Length; i++)
                {
                    if (name[i] != '.')
                    {
                        if (!precision)
                        {
                            f = f * 10 + Utils.Utils.charToNumber(name[i]);
                        } else
                        {
                            div *= 10;
                            f = f + Utils.Utils.charToNumber(name[i]) / div;
                        }
                    } else
                    {
                        precision= true;
                    }
                }
                return new Token(tokenType, position, float.Parse(name));
            }
        }
    }
}
