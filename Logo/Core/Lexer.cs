using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Logo.Core
{
    public class Lexer
    {
        SourceCode source;

        public Token token { get; private set; }

        char eof = Utils.Utils.eof;
        char newline = Utils.Utils.lexerNewline;
        Position position;

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
            { "bool",  TokenType.BOOL_T },
            { "turtle_t", TokenType.TURTLE },
            { "color", TokenType.COLOR_T },
            { "coordinate", TokenType.COORDINATE_T },
            { "copyof", TokenType.COPYOF }
        };
        Dictionary<char, TokenType> singleChar = new Dictionary<char, TokenType>()
        {
            { '=', TokenType.EQ },
            { '.', TokenType.DOT },
            { ',', TokenType.COMMA },
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
            { '<', TokenType.LT },
            { '>', TokenType.GT },
            { Utils.Utils.eof, TokenType.EOF },
            { Utils.Utils.lexerNewline, TokenType.NL }
        };
        Dictionary<string, TokenType> doubleChar = new Dictionary<string, TokenType>()
        {
            { "<=", TokenType.LE },
            { ">=", TokenType.GE },
            { "==", TokenType.EQEQ },
            { "!=", TokenType.DIFF},
            { "__", TokenType.UU },
        };
        char quoteMark = '"';

        public Lexer(SourceCode source)
        {
            this.source = source;
        }

        char getNextChar(bool skipWhiteSpace = true)
        {
            while (char.IsWhiteSpace(source.getNextChar()) && skipWhiteSpace && source.getCurrChar() != newline && source.getCurrChar() != eof) ;
            return source.getCurrChar();
        }

        public Token advanceToken()
        {
            char c = getNextChar();
            position = source.getPosition();

            if (c == eof)
            {
                token = new Token(TokenType.EOF, source.getPosition());
                return token;
            }

            TokenType result;

            if (doubleChar.ContainsKey(c.ToString() + source.peekChar().ToString()))
            {
                string name = c.ToString() + source.getNextChar().ToString();
                doubleChar.TryGetValue(name, out result);
                token = new Token(result, position);
                return token;
            }

            if (singleChar.ContainsKey(c))
            {
                singleChar.TryGetValue(c, out result);
                token = new Token(result, position);
                return token;
            }

            token = buildCommentToken();
            if (token != null)
                return advanceToken();

            token = buildColorToken();
            if (token != null)
                return token;

            token = buildStringToken();
            if (token != null)
                return token;

            token = buildIdentifierToken();
            if (token != null) return token;

            token = buildNumberToken();
            if (token != null) return token;

            token = new Token(TokenType.ERROR, source.getPosition(), "Token not valid!");
            return token;
        }

        Token buildCommentToken()
        {
            if (source.getCurrChar() != '~' || source.peekChar() != '~')
                return null;
            getNextChar();
            getNextChar();
            StringBuilder sb = new StringBuilder();
            while(source.getCurrChar() != eof && source.getCurrChar() != newline)
            {
                char c = source.getNextChar();
                sb.Append(c);
            }
            return new Token(TokenType.COMMENT, position, sb.ToString());
        }

        Token buildColorToken()
        {
            if (source.getCurrChar() != '0' || source.peekChar() != 'x')
                return null;
            getNextChar();
            StringBuilder sb = new StringBuilder();
            sb.Append("0x");
            List<char> allowed_char = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F' };
            for (int i = 0; i<6; i++)
            {
                char c = getNextChar();
                if (!allowed_char.Contains(c))
                {
                    token = new Token(TokenType.ERROR, position, "Color Token not valid!");
                    return token;
                } else
                {
                    sb.Append(c);
                }
            }
            uint int_color = Convert.ToUInt32(sb.ToString(), 16);
            return new Token(TokenType.COLOR, position, int_color.ToColor());
        }

        Token buildStringToken()
        {
            if (source.getCurrChar() != quoteMark)
                return null;
            position = source.getPosition();
            char c = getNextChar(false);
            StringBuilder sb = new StringBuilder();
            while (c != quoteMark && (c != eof && c != newline))
            {
                if (c != '\\')
                {
                    sb.Append(c);
                }
                else
                {
                    char next = getNextChar(false);
                    switch (next)
                    {
                        case '\"':
                        case '\\':
                            sb.Append(next);
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        default:
                            token = new Token(TokenType.ERROR, position, String.Format("Char '\\{0}' is not supported", c));
                            break;
                    }
                }
                c = getNextChar(false);
            }
            if (c == eof || c == newline)
            {
                token = new Token(TokenType.ERROR, position, "String closing not found");
                return token;
            }
            return new Token(TokenType.STR, position, sb.ToString());
        }

        Token buildIdentifierToken()
        {
            if (!Char.IsLetter(source.getCurrChar()))
                return null;
            StringBuilder sb = new StringBuilder();
            sb.Append(source.getCurrChar());
            while (Char.IsLetter(source.peekChar()) || Char.IsDigit(source.peekChar()) || source.peekChar() == '_')
            {
                sb.Append(source.getNextChar());
            }
            
            TokenType tokenType;
            if (!keywords.TryGetValue(sb.ToString(), out tokenType))
            {
                return new Token(TokenType.IDENTIFIER, position, sb.ToString());
            }
          
            return new Token(tokenType, position);
        }

        Token buildNumberToken()
        {
            if (!Char.IsDigit(source.getCurrChar()))
                return null;
            position = source.getPosition();
            float number = Utils.Utils.charToNumber(source.getCurrChar());
            
            TokenType tokenType = TokenType.INT;
            float precision = 1;
            while (Char.IsDigit(source.peekChar()) || source.peekChar() == '.')
            {
                source.getNextChar();
                if (source.getCurrChar() == '.')
                {
                    if (tokenType == TokenType.FLOAT || !Char.IsDigit(source.peekChar()))
                    {
                        token = new Token(TokenType.ERROR, position, "Number is not valid");
                        return token;
                    }
                    else if (tokenType == TokenType.INT)
                    {
                        tokenType = TokenType.FLOAT;
                    }
                }
                else
                {
                    number = number * 10 + Utils.Utils.charToNumber(source.getCurrChar());
                    if (tokenType == TokenType.FLOAT)
                    {
                        precision *= 10;
                    }
                }
                
            }
            
            if (tokenType == TokenType.FLOAT)
            {
                number /= precision;
                return new Token(tokenType, position, number);
            }
            else
            {
                int value = (int)number;
                return new Token(tokenType, position, value);
            }
        }
    }
}
