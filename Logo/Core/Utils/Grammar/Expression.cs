using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;


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
                // return s.Substring(1, s.Length - 2);
                return s;
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
            if (left is bool && !(bool)left)
                return false;
            if (!(left is bool))
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("AND statement element must be boolean datatype"));
                return null;
            }
            object right = this.right.Evaluate(scope);
            if (right is bool)
                return right;
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
                float leftValue = 0;
                if (left is int)
                    leftValue = (int)left;
                else if (left is float)
                    leftValue = (float)left;
                float rightValue = 0;
                if (right is int)
                    rightValue = (int)right;
                else if (right is float)
                    rightValue = (float)right;

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
            if (left is int && right is int)
            {
                return (int)left * (int)right;
            }
            if (left is float && right is float)
            {
                return (float)left * (float)right;
            }
            if (left is int && right is float)
            {
                return (int)left * (float)right;
            }
            if (left is float && right is int)
            {
                return (float)left * (int)right;
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
            if (left is int && right is int)
            {
                return (int)left / (int)right;
            }
            if (left is float && right is float)
            {
                return (float)left / (float)right;
            }
            if (left is int && right is float)
            {
                return (int)left / (float)right;
            }
            if (left is float && right is int)
            {
                return (float)left / (int)right;
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
            if (left is int && right is int)
            {
                return (int)left % (int)right;
            }
            if (left is float && right is float)
            {
                return (float)left % (float)right;
            }
            if (left is int && right is float)
            {
                return (int)left % (float)right;
            }
            if (left is float && right is int)
            {
                return (float)left % (int)right;
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
            if (left is int && right is int)
            {
                return (int)left == (int)right;
            }
            if (left is float && right is float)
            {
                return (float)left == (float)right;
            }
            if (left is int && right is float)
            {
                return (int)left == (float)right;
            }
            if (left is float && right is int)
            {
                return (float)left == (int)right;
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
        public List<IExpression> arguments;
        public AttrExp attr;

        public FunctionCallExp(AttrExp attr, List<IExpression> arguments)
        {
            this.attr = attr;
            this.arguments = arguments;
        }

        public object Evaluate(Scope scope)
        {
            FunctionStatement func = null;
            ChildFunction childFunc = null;
            List<DeclarationStatement> requested_params = null;
            if (attr.child != null)
            {
                var returnedValue = attr.Evaluate(scope, arguments.Count);
                if (returnedValue is Variable)
                {
                    returnedValue = ((Variable)returnedValue).value;
                }
                if (returnedValue is ChildFunction)
                {
                    childFunc = (ChildFunction)returnedValue;
                    requested_params = childFunc.parameters;
                }
            } else
            {
                func = FunctionStorage.getFunction(attr.variableName);
                if (func != null)
                    requested_params = func.parameters;
            }
            if (requested_params != null)
            {
                Scope newScope = new Scope();
                if (requested_params.Count != arguments.Count)
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Provided arguments is not matched with Function declaration!"));
                    return null;
                }
                for (int i = 0; i < requested_params.Count; i++)
                {
                    if (arguments[i] is Identifier)
                    {
                        AttrExp name = ((Identifier)arguments[i]).attrIdentifier;
                        object result = name.Evaluate(scope);
                        if (!(result is Variable))
                        {
                            ErrorHandling.pushError(new ErrorHandling.LogoException("Variable not found!"));
                            return null;
                        }
                        newScope.setVariable(requested_params[i].name, (Variable)result);
                    }
                    else
                    {
                        object value = arguments[i].Evaluate(scope);
                        newScope.setVariableValue(requested_params[i].name, value);
                    }
                }
                if (scope.contains("Board"))
                    newScope.setVariable("Board", scope.getVariable("Board"));
                if (childFunc != null || func != null)
                {
                    object result = null;
                    if (childFunc != null)
                        result = childFunc.invoke(newScope);
                    else if (func != null)
                        result = func.Execute(newScope);
                    if (result is ReturnStatement)
                    {
                        return ((ReturnStatement)result).Execute(newScope);
                    }
                    return result;
                }
            }
            if (attr.parent == null)
            {
                if (attr.variableName.Equals("Turtle"))
                {
                    return executeSystemFunction(scope);
                }
                if (attr.variableName.Equals("Coordinate"))
                {
                    return executeSystemFunction(scope);
                }
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Function not found!"));
            return null;
        }

        object executeSystemFunction(Scope scope)
        {
            if (attr.parent == null)
            {
                if (attr.variableName.Equals("Turtle"))
                {
                    if (arguments.Count != 0 && arguments.Count != 2)
                    {
                        ErrorHandling.pushError(new ErrorHandling.LogoException("Turtle type only must have 0 or 2 params!"));
                        return null;
                    }
                    if (arguments.Count == 2)
                    {
                        int x = (int)arguments[0].Evaluate(scope);
                        int y = (int)arguments[1].Evaluate(scope);
                        return new TurtleVar(x, y);
                    }
                    return new TurtleVar();
                }
                if (attr.variableName.Equals("Coordinate"))
                {
                    if (arguments.Count != 2)
                    {
                        ErrorHandling.pushError(new ErrorHandling.LogoException("Coordinate type only must have 2 params!"));
                        return null;
                    }
                    int x = (int)arguments[0].Evaluate(scope);
                    int y = (int)arguments[1].Evaluate(scope);
                    return new Coordinate(x, y);
                }
            }
            return null;
        }
    }

    public class AttrExp: IExpression
    {
        public string variableName;
        public IExpression parent;
        public string child;

        public AttrExp(string variableName, string child = null)
        {
            this.variableName = variableName;
            this.child = child;
        }

        public AttrExp(IExpression parent, string child)
        {
            this.parent = parent;
            this.child = child;
        }

        public object Evaluate(Scope scope)
        {
            if (variableName != null)
            { 
                if (scope.getVariable(variableName) == null && child != null)
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Variable not found!"));
                    return null;
                }
                
                var variable = scope.getVariable(variableName);
                if (variable != null && child == null)
                    return variable;
                if (variable != null && child != null)
                {
                    var value = variable.value;
                    if (value is TurtleVar || value is Board || value is TurtlePen) {
                        var props = value.GetType().GetProperty(child);
                        if (props == null)
                        {
                            ErrorHandling.pushError(new ErrorHandling.LogoException($"Property {child} not found!"));
                            return null;
                        }
                        var obj = props.GetValue(value);
                        return obj;
                    }
                } else
                {
                    return null;
                }
                // TODO
                return null;
            } else if (child != null)
            {
                var obj = parent.Evaluate(scope);
                if (obj is Variable)
                    obj = ((Variable)obj).value;
                if (obj is TurtlePen)
                {
                    var props = obj.GetType().GetProperty(child);
                    if (props == null)
                    {
                        ErrorHandling.pushError(new ErrorHandling.LogoException($"Property {child} not found "));
                        return null;
                    }
                    obj = props.GetValue(obj);
                    return obj;
                    
                }
                // TODO
                return null;
            }
            return null;
        }

        public object Evaluate(Scope scope, int argsCount)
        {
            return Evaluate(scope);
        }
    }

    public class CopyOfExp: IExpression {
        public string identifier;
        
        public CopyOfExp(string identifier) { this.identifier = identifier; }

        public object Evaluate(Scope scope)
        {
            return scope.getVariable(identifier).value;
        }
    }

    public class Identifier : IExpression
    {
        //public string identifier;
        public AttrExp attrIdentifier;

        //public Identifier(string identifier)
        //{
        //    this.identifier = identifier;
        //}

        public Identifier(AttrExp attrExp)
        {
            this.attrIdentifier = attrExp;
        }

        public object Evaluate(Scope scope)
        {
            var value = attrIdentifier.Evaluate(scope);
            if (value is Variable)
                return ((Variable)value).value;
            if (value == null)
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Variable not found!"));
                return null;
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
            if (right is float)
                return (float)right * -1;
            if (right is int)
                return (int)right * -1;

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
            if (left is bool && (bool)left)
                return true;
            object right = this.right.Evaluate(scope);
            if (right is bool)
                return (bool)right;
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
            if (left is int && right is int)
            {
                return (int)left - (int)right;
            }
            if (left is float && right is float)
            {
                return (float)left - (float)right;
            }
            if (left is int && right is float)
            {
                return (int)left - (float)right;
            }
            if (left is float && right is int)
            {
                return (float)left - (int)right;
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
            if (left is int && right is int)
            {
                return (int)left + (int)right;
            }
            if (left is float && right is float)
            {
                return (float)left + (float)right;
            }
            if (left is int && right is float)
            {
                return (int)left + (float)right;
            }
            if (left is float && right is int)
            {
                return (float)left + (int)right;
            }
            if (left is string && right is string)
            {
                return (string)left + (string)right;
            }
            if (left is string && right is int)
            {
                return (string)left + (int)right;
            }
            if (left is string && right is float)
            {
                return (string)left + (float)right;
            }
            if (left is int && right is string)
            {
                return (int)left + (string)right;
            }
            if (left is float && right is string)
            {
                return (float)left + (string)right;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Can not use sum operator with these variables!", position));
            return null;
        }
    }
}
