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
        string tokenType { get; set; }
        Position position { get; set; }
        string textValue { get; set; }
        int intValue { get; set; }
        float floatValue { get; set; }
        bool boolValue { get; set; }
        private ValueType valueType;

        public Token(string tokenType, Position position)
        {
            this.tokenType = tokenType;
            this.position = position;
        }

        public Token(string tokenType, Position position, string textValue)
        {
            this.position = position;
            this.tokenType = tokenType;
            this.textValue = textValue;
            this.valueType = ValueType.TEXT;
        }

        public Token(string tokenType, Position position, int intValue)
        {
            this.position = position;
            this.tokenType = tokenType;
            this.intValue = intValue;
            this.valueType = ValueType.INT;
        }

        public Token(string tokenType, Position position, float floatValue)
        {
            this.position = position;
            this.tokenType = tokenType;
            this.floatValue = floatValue;
            this.valueType = ValueType.FLOAT;
        }

        public Token(string tokenType, Position position, bool boolValue)
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

        public string getTokenType()
        {
            return this.tokenType;
        }

        public ValueType getValueType() 
        {
            return valueType; 
        }

        public Position getPosition()
        {
            return position;
        }

        public int getIntValue()
        {
            return intValue;
        }

        public float getFloatValue()
        {
            return floatValue;
        }

        public bool getBooleanValue()
        {
            return boolValue;
        }

        public string getTextValue()
        {
            return textValue;
        }
    }
}
