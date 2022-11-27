using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logo.Core.Utils;

namespace Logo.Core.Utils.Grammar
{
    public class Expression
    {
        public Expression()
        {

        }

        public virtual object evaluate(Scope scope)
        {
            return null;
        }
    }

    public class Literal: Expression
    {
        public object value;
        public Token token;
        public Literal(object value, Token token)
        {
            this.token = token;
            this.value = value;
        }

        public override object evaluate(Scope scope)
        {
            if (token.getTokenType() == TokenType.STR)
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

    public abstract class Binary: Expression
    {
        public Expression left;
        public Expression right;
        public Token op;

        public Binary(Expression left, Expression right, Token op) {
            this.left = left;
            this.right = right;
            this.op = op;
        }
    }

    public abstract class Unary: Expression
    {
        public Expression right;
        public Token op;

        public Unary(Expression right, Token op)
        {
            this.right = right;
            this.op = op;
        }
    }

    public abstract class AdditiveExpression: Binary
    {
        public AdditiveExpression(Expression left, Expression right, Token op) : base(left, right, op) { }
    }

    public class AndExpresstion : Binary
    {
        public AndExpresstion(Expression left, Expression right, Token op): base(left, right, op) { }

        public override object evaluate(Scope scope)
        {
            object left = this.left.evaluate(scope);
            object right = this.right.evaluate(scope);

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
        public Comparison(Expression left, Expression right, Token op) : base(left, right, op) { }

        public override object evaluate(Scope scope)
        {
            object left = this.left.evaluate(scope);
            object right = this.right.evaluate(scope);
            string opType = this.op.getTokenType();

            if (Utils.IsNumber(left) && Utils.IsNumber(right))
            {
                float leftValue = (float)left;
                float rightValue = (float)right;

                bool equals = (leftValue == rightValue);
                bool leftIsLessThanRight = (leftValue < rightValue);
                return compare(equals, leftIsLessThanRight, opType);
            }

            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not compare!!", op.getPosition()));
            return null;
        }

        bool compare(bool equals, bool leftIsLessThanRight, string tokenType)
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

    public abstract class Multiplicative: Binary
    {
        public Multiplicative(Expression left, Expression right, Token op): base(left, right, op) { }
    }

    public class Multiplication: Multiplicative
    {
        public Multiplication(Expression left, Expression right, Token op): base(left, right, op) { }
        public override object evaluate(Scope scope)
        {
            object left = this.left.evaluate(scope);
            object right = this.right.evaluate(scope);
            if ((left is int && left is float) || (right is int && right is float))
            {
                if (left is float || right is float)
                    return (float)left * (float)right;
                return (int)left * (int)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not multiplication this two value", op.getPosition()));
            return null;
        }
    }

    public class Division: Multiplicative
    {
        public Division(Expression left, Expression right, Token op): base(left, right, op) { }

        public override object evaluate(Scope scope)
        {
            object left = this.left.evaluate(scope);
            object right = this.right.evaluate(scope);
            if ((left is int && left is float) || (right is int && right is float))
            {
                return (float)left / (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not divide this two value", op.getPosition()));
            return null;
        }
    }

    public class Equality: Binary
    {
        public Equality(Expression left, Expression right, Token op): base(left, right, op) { }

        public override object evaluate(Scope scope)
        {
            object left = this.left.evaluate(scope);
            object right = this.right.evaluate(scope);
            bool checkingEqual = op.getTokenType() == TokenType.EQEQ;
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
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not compare two data type", op.getPosition()));
            return null;
        }
    }

    public class FunctionCallExp: Expression
    {
        public string identifier;
        public List<Expression> arguments;
        public FunctionCallExp(string identifier, List<Expression> arguments)
        {
            this.identifier = identifier;
            this.arguments = arguments;
        }

        public override object evaluate(Scope scope)
        {
            FunctionStatement func = FunctionStorage.getFunction(identifier);
            if (func != null)
            {
                return func.execute(scope, arguments);
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Function not found!"));
            return null;
        }
    }

    public class Identifier: Expression
    {
        public string identifier;
        Token token;

        public Identifier(string identifier, Token token)
        {
            this.identifier = identifier;
            this.token = token;
        }

        public override object evaluate(Scope scope)
        {
            object value = scope.getVariable(identifier).value;
            if (value == null)
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Variable not found!"));
            }
            return value;
        }
    }

    public class Negation: Unary
    {
        public Negation(Expression right, Token op): base(right, op) { }

        public override object evaluate(Scope scope)
        {
            object right = this.right.evaluate(scope);
            if (right is float || right is int)
            {
                if (right is float)
                    return (float)right * -1;
                if (right is int)
                    return (int)right * -1;
            }
            if (right is bool)
                return !(bool)right;
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not negate this variable!", op.getPosition()));
            return null;
        }
    }

    public class Not: Unary
    {
        public Not(Expression right, Token op): base(right, op) { }

        public override object evaluate(Scope scope)
        {
            object right = this.right.evaluate(scope);

            if (right is bool)
            {
                return !(bool)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not negate this variable!", op.getPosition()));
            return null;
        }
    }

    public class Or: Binary
    {
        public Or(Expression left, Expression right, Token op): base (left, right, op) { }

        public override object evaluate(Scope scope)
        {
            object left = this.left.evaluate(scope);
            object right = this.right.evaluate(scope);
            if (left is bool && right is bool)
            {
                return (bool)left || (bool)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use OR operator with these variables!", op.getPosition()));
            return null;
        }
    }

    public class Subtract: Binary
    {
        public Subtract(Expression left, Expression right, Token op) : base(left, right, op) { }

        public override object evaluate(Scope scope)
        {
            object left = this.left.evaluate(scope);
            object right = this.right.evaluate(scope);
            if ((left is int || left is float) && (right is int || right is float))
            {
                return (float)left - (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use subtract operator with these variables!", op.getPosition()));
            return null;
        }
    }

    public class Sum: Binary
    {
        public Sum(Expression left, Expression right, Token op) : base(left, right, op) { }

        public override object evaluate(Scope scope)
        {
            object left = this.left.evaluate(scope);
            object right = this.right.evaluate(scope);
            if ((left is int || left is float) && (right is int || right is float))
            {
                return (float)left + (float)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use sum operator with these variables!", op.getPosition()));
            return null;
        }
    }
}
