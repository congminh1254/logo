using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public class Token
    {
        public enum ValueType { TEXT, INT, FLOAT, BOOL }
        public TokenType tokenType { get; set; }
        public Position position { get; set; }
        public string textValue { get; set; } = null;
        public int? intValue { get; set; } = null;
        public float? floatValue { get; set; } = null;
        public bool? boolValue { get; set; } = null;
        public ValueType valueType { get; set; }

        public Token(TokenType tokenType, Position position)
        {
            this.tokenType = tokenType;
            this.position = position;
        }

        public Token(TokenType tokenType, Position position, string textValue)
        {
            this.position = position;
            this.tokenType = tokenType;
            this.textValue = textValue;
            this.valueType = ValueType.TEXT;
        }

        public Token(TokenType tokenType, Position position, int intValue)
        {
            this.position = position;
            this.tokenType = tokenType;
            this.intValue = intValue;
            this.valueType = ValueType.INT;
        }

        public Token(TokenType tokenType, Position position, float floatValue)
        {
            this.position = position;
            this.tokenType = tokenType;
            this.floatValue = floatValue;
            this.valueType = ValueType.FLOAT;
        }

        public Token(TokenType tokenType, Position position, bool boolValue)
        {
            this.position = position;
            this.tokenType = tokenType;
            this.boolValue= boolValue;
            this.valueType = ValueType.BOOL;
        }

        public override string ToString()
        {
            string value = "";
            switch (valueType)
            {
                case ValueType.TEXT: 
                    value = textValue; 
                    break;
                case ValueType.INT:
                    value = intValue.ToString();
                    break;
                case ValueType.FLOAT:
                    value = floatValue.ToString();
                    break;
                case ValueType.BOOL:
                    value = boolValue.ToString();
                    break;
            }
            return string.Format("Token(type={0},value={1},position={2})", tokenType, value, position.ToString());
        }
    }
}
