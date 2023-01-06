using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public interface IExpression
    {
        public object Evaluate(Scope scope);
    }

    public class Literal : IExpression
    {
        public object value;
        public Token token;
        public Literal(object value, Token token)
        {
            this.token = token;
            this.value = value;
        }

        public object Evaluate(Scope scope)
        {
            if (token.tokenType == TokenType.STR)
            {
                string s = (string)value;
                return s.Substring(1, s.Length - 2);
            }
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class AndExpresstion : IExpression
    {
        public IExpression left;
        public IExpression right;

        public AndExpresstion(IExpression left, IExpression right, Position position)
        {
            this.left = left;
            this.right = right;
        }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);

            if (left is bool && right is bool)
            {
                return (bool)left && (bool)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("AND statement element must be boolean datatype"));
            return null;
        }
    }

    public class Comparison : IExpression
    {
        public IExpression left, right;
        public Position position;
        public enum ComparisonType
        {
            GE,
            LE,
            GT,
            LT
        }
        public ComparisonType type;
        public Comparison(IExpression left, IExpression right, ComparisonType type, Position position)
        {
            this.left = left;
            this.right = right;
            this.type = type;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);

            if (Utils.IsNumber(left) && Utils.IsNumber(right))
            {
                float leftValue = (float)left;
                float rightValue = (float)right;

                bool equals = (leftValue == rightValue);
                bool leftIsLessThanRight = (leftValue < rightValue);
                return compare(equals, leftIsLessThanRight, type);
            }

            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not compare!!", position));
            return null;
        }

        bool compare(bool equals, bool leftIsLessThanRight, ComparisonType tokenType)
        {
            if (tokenType == ComparisonType.LT && leftIsLessThanRight && !equals)
            {
                return true;
            }
            if (tokenType == ComparisonType.GT && !leftIsLessThanRight && !equals)
            {
                return true;
            }
            if (tokenType == ComparisonType.LE && (leftIsLessThanRight || equals))
            {
                return true;
            }
            return tokenType == ComparisonType.GE && (!leftIsLessThanRight || equals);
        }
    }

    public class Multiplication : IExpression
    {
        public IExpression left, right;
        public Position position;
        public Multiplication(IExpression left, IExpression right, Position position)
        {
            this.left = left;
            this.right = right;
            this.position = position;
        }
        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if ((left is int && left is float) || (right is int && right is float))
            {
                if (left is float || right is float)
                    return (float)left * (float)right;
                return (int)left * (int)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not multiplication this two value", position));
            return null;
        }
    }

    public class Division : IExpression
    {
        public IExpression left, right;
        public Position position;
        public Division(IExpression left, IExpression right, Position position)
        {
            this.left = left;
            this.right = right;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if ((left is int && left is float) || (right is int && right is float))
            {
                return (float)left / (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not divide this two value", position));
            return null;
        }
    }

    public class Modulo : IExpression
    {
        public IExpression left, right;
        public Position position;
        public Modulo(IExpression left, IExpression right, Position position)
        {
            this.left = left;
            this.right = right;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if ((left is int && left is float) || (right is int && right is float))
            {
                return (float)left % (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not divide this two value", position));
            return null;
        }
    }

    public class Equality : IExpression
    {
        public enum EqualityType
        {
            DIFF,
            EQUAL
        }
        public IExpression left, right;
        public Position position;
        public EqualityType type;

        public Equality(IExpression left, IExpression right, EqualityType type, Position position)
        {
            this.left = left;
            this.right = right;
            this.type = type;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            bool checkingEqual = type == EqualityType.EQUAL;
            if (left is bool && right is bool)
            {
                return checkingEqual == ((bool)left == (bool)right);
            }
            if ((left is int || left is float) && (right is int || right is float))
            {
                return checkingEqual == ((float)left == (float)right);
            }
            if (left is string && right is string)
            {
                return checkingEqual == ((string)left).Equals(right);
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not compare two data type", position));
            return null;
        }
    }

    public class FunctionCallExp : IExpression
    {
        public string identifier;
        public List<IExpression> arguments;
        public FunctionCallExp(string identifier, List<IExpression> arguments)
        {
            this.identifier = identifier;
            this.arguments = arguments;
        }

        public object Evaluate(Scope scope)
        {
            FunctionStatement func = FunctionStorage.getFunction(identifier);
            if (func != null)
            {
                return func.Execute(scope, arguments);
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Function not found!"));
            return null;
        }
    }

    public class Identifier : IExpression
    {
        public string identifier;
        Token token;

        public Identifier(string identifier, Token token)
        {
            this.identifier = identifier;
            this.token = token;
        }

        public object Evaluate(Scope scope)
        {
            object value = scope.getVariable(identifier).value;
            if (value == null)
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Variable not found!"));
            }
            return value;
        }
    }

    public class Negation : IExpression
    {
        public IExpression right;
        public Position position;
        public Negation(IExpression right, Position position)
        {
            this.right = right;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object right = this.right.Evaluate(scope);
            if (right is float || right is int)
            {
                if (right is float)
                    return (float)right * -1;
                if (right is int)
                    return (int)right * -1;
            }
            if (right is bool)
                return !(bool)right;
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not negate this variable!", position));
            return null;
        }
    }

    public class NotExpression : IExpression
    {
        public IExpression right;
        public Position position;
        public NotExpression(IExpression right, Position position)
        {
            this.right = right;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object right = this.right.Evaluate(scope);

            if (right is bool)
            {
                return !(bool)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not negate this variable!", position));
            return null;
        }
    }

    public class Or : IExpression
    {
        public IExpression left, right;
        public Position position;
        public Or(IExpression left, IExpression right, Position position)
        {
            this.left = left;
            this.right = right;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if (left is bool && right is bool)
            {
                return (bool)left || (bool)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use OR operator with these variables!", position));
            return null;
        }
    }

    public class Subtract : IExpression
    {
        public IExpression left, right;
        public Position position;
        public Subtract(IExpression left, IExpression right, Position position)
        {
            this.left = left;
            this.right = right;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if ((left is int || left is float) && (right is int || right is float))
            {
                return (float)left - (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use subtract operator with these variables!", position));
            return null;
        }
    }

    public class Sum : IExpression
    {
        public IExpression left, right;
        public Position position;
        public Sum(IExpression left, IExpression right, Position position)
        {
            this.left = left;
            this.right = right;
            this.position = position;
        }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if ((left is int || left is float) && (right is int || right is float))
            {
                return (float)left + (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use sum operator with these variables!", position));
            return null;
        }
    }
}
