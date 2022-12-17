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

    public abstract class Binary : IExpression
    {
        public IExpression left;
        public IExpression right;
        public Token op;

        public Binary(IExpression left, IExpression right, Token op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
        }

        public object Evaluate(Scope score)
        {
            return null;
        }
    }

    public abstract class Unary : IExpression
    {
        public IExpression right;
        public Token op;

        public Unary(IExpression right, Token op)
        {
            this.right = right;
            this.op = op;
        }
        public object Evaluate(Scope score)
        {
            return null;
        }
    }

    public abstract class AdditiveExpression : Binary
    {
        public AdditiveExpression(IExpression left, IExpression right, Token op) : base(left, right, op) { }
    }

    public class AndExpresstion : Binary
    {
        public AndExpresstion(IExpression left, IExpression right, Token op) : base(left, right, op) { }

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

    public class Comparison : Binary
    {
        public Comparison(IExpression left, IExpression right, Token op) : base(left, right, op) { }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            TokenType opType = this.op.tokenType;

            if (Utils.IsNumber(left) && Utils.IsNumber(right))
            {
                float leftValue = (float)left;
                float rightValue = (float)right;

                bool equals = (leftValue == rightValue);
                bool leftIsLessThanRight = (leftValue < rightValue);
                return compare(equals, leftIsLessThanRight, opType);
            }

            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not compare!!", op.position));
            return null;
        }

        bool compare(bool equals, bool leftIsLessThanRight, TokenType tokenType)
        {
            if (tokenType == TokenType.LT && leftIsLessThanRight && !equals)
            {
                return true;
            }
            if (tokenType == TokenType.GT && !leftIsLessThanRight && !equals)
            {
                return true;
            }
            if (tokenType == TokenType.LE && (leftIsLessThanRight || equals))
            {
                return true;
            }
            return tokenType == TokenType.GE && (!leftIsLessThanRight || equals);
        }
    }

    public abstract class Multiplicative : Binary
    {
        public Multiplicative(IExpression left, IExpression right, Token op) : base(left, right, op) { }
    }

    public class Multiplication : Multiplicative
    {
        public Multiplication(IExpression left, IExpression right, Token op) : base(left, right, op) { }
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
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not multiplication this two value", op.position));
            return null;
        }
    }

    public class Division : Multiplicative
    {
        public Division(IExpression left, IExpression right, Token op) : base(left, right, op) { }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if ((left is int && left is float) || (right is int && right is float))
            {
                return (float)left / (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not divide this two value", op.position));
            return null;
        }
    }

    public class Equality : Binary
    {
        public Equality(IExpression left, IExpression right, Token op) : base(left, right, op) { }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            bool checkingEqual = op.tokenType == TokenType.EQEQ;
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
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not compare two data type", op.position));
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

    public class Negation : Unary
    {
        public Negation(IExpression right, Token op) : base(right, op) { }

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
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not negate this variable!", op.position));
            return null;
        }
    }

    public class Not : Unary
    {
        public Not(IExpression right, Token op) : base(right, op) { }

        public object Evaluate(Scope scope)
        {
            object right = this.right.Evaluate(scope);

            if (right is bool)
            {
                return !(bool)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not negate this variable!", op.position));
            return null;
        }
    }

    public class Or : Binary
    {
        public Or(IExpression left, IExpression right, Token op) : base(left, right, op) { }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if (left is bool && right is bool)
            {
                return (bool)left || (bool)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use OR operator with these variables!", op.position));
            return null;
        }
    }

    public class Subtract : Binary
    {
        public Subtract(IExpression left, IExpression right, Token op) : base(left, right, op) { }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if ((left is int || left is float) && (right is int || right is float))
            {
                return (float)left - (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use subtract operator with these variables!", op.position));
            return null;
        }
    }

    public class Sum : Binary
    {
        public Sum(IExpression left, IExpression right, Token op) : base(left, right, op) { }

        public object Evaluate(Scope scope)
        {
            object left = this.left.Evaluate(scope);
            object right = this.right.Evaluate(scope);
            if ((left is int || left is float) && (right is int || right is float))
            {
                return (float)left + (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use sum operator with these variables!", op.position));
            return null;
        }
    }
}
