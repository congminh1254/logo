using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public class Token
    {
        public TokenType tokenType { get; private set; }
        public Position position { get; private set; }
        public string textValue { get; private set; } = null;
        public int? intValue { get; private set; } = null;
        public float? floatValue { get; private set; } = null;

        public Token(TokenType tokenType, Position position)
        {
            this.tokenType = tokenType;
            this.position = position;
            if (position == null)
            {
                this.tokenType = TokenType.ERROR;
                this.textValue = "Missing possition for this token.";
            }
        }

        public Token(TokenType tokenType, Position position, string textValue)
        {

            this.position = position;
            if (textValue == null)
            {
                this.tokenType = TokenType.ERROR;
                this.textValue = "Missing text value for token.";
                return;
            }

            if (tokenType != TokenType.IDENTIFIER && tokenType != TokenType.STR)
            {
                this.tokenType = TokenType.ERROR;
                this.textValue = String.Format("Token type must be STR or IDENTIFIER. Value: '{0}', token type: {1}", textValue, tokenType);
                return;
            }

            this.tokenType = tokenType;
            this.textValue = textValue;
        }

        public Token(TokenType tokenType, Position position, int intValue)
        {
            if (position == null)
            {
                this.tokenType = TokenType.ERROR;
                this.textValue = "Missing possition for this token.";
                return;
            }
            this.position = position;

            if (tokenType != TokenType.INT)
            {
                this.tokenType = TokenType.ERROR;
                this.textValue = "Token type must be INT.";
                return;
            }
            this.tokenType = tokenType;
            this.intValue = intValue;
        }

        public Token(TokenType tokenType, Position position, float floatValue)
        {
            if (position == null)
            {
                this.tokenType = TokenType.ERROR;
                this.textValue = "Missing possition for this token.";
                return;
            }
            this.position = position;

            if (tokenType != TokenType.FLOAT)
            {
                this.tokenType = TokenType.ERROR;
                this.textValue = "Token type must be FLOAT.";
                return;
            }
            this.tokenType = tokenType;
            this.floatValue = floatValue;
        }

        public override string ToString()
        {
            string value = "";
            switch (tokenType)
            {
                case TokenType.STR:
                case TokenType.ERROR:
                case TokenType.IDENTIFIER:
                    value = textValue; 
                    break;
                case TokenType.INT:
                    value = intValue.ToString();
                    break;
                case TokenType.FLOAT:
                    value = floatValue.ToString();
                    break;
            }
            return string.Format("Token(type={0},value={1},position={2})", tokenType, value, position.ToString());
        }
    }
}
