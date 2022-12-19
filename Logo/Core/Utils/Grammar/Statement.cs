using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public interface IStatement
    {
        object Execute(Scope scope);
    }

    public class FunctionStatement : IStatement
    {
        public Token identifier;
        public List<DeclarationStatement> parameters;
        public List<IStatement> body;

        public FunctionStatement(Token identifier, List<DeclarationStatement> parameters, List<IStatement> body)
        {
            this.identifier = identifier;
            this.parameters = parameters;
            this.body = body;
        }

        public object Execute(Scope scope)
        {
            return Execute(scope, null);
        }

        public object Execute(Scope scope, List<IExpression> args)
        {
            int i = 0;
            Scope newScope = new Scope();
            if (identifier.textValue != "main")
            {
                foreach (DeclarationStatement statement in parameters)
                {
                    statement.Execute(newScope);
                    setScopeVariableValue(newScope, scope, args[i++], statement.name);
                }
            }

            foreach (IStatement statement in body)
            {
                if (statement is ReturnStatement)
                {
                    object val = statement.Execute(newScope);
                    return val;
                }
                object returned = statement.Execute(newScope);

                if (returned is ReturnStatement)
                {
                    object val = ((ReturnStatement)returned).Execute(newScope);
                    return val;
                }
            }
            // void with no return value
            return null;
        }

        private void setScopeVariableValue(Scope innerScope, Scope parentScope, IExpression value, String name)
        {
            var variable = innerScope.getVariable(name);
            var variableType = variable.type;
            var val = value.Evaluate(parentScope);

            if ((val is string && variableType != TokenType.STR)
                || (val is int && variableType != TokenType.STR)
                || (val is float && variableType != TokenType.STR)
                || (val is bool && variableType != TokenType.STR)
                || (val is TurtleVar && variableType != TokenType.TURTLE))
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!"));
            }
        }
    }

    public class FunctionCallStatement : IStatement
    {
        public string identifier;
        public List<IExpression> arguments;

        public FunctionCallStatement(string identifier, List<IExpression> arguments)
        {
            this.identifier = identifier;
            this.arguments = arguments;
        }

        public object Execute(Scope scope)
        {
            if (identifier.Equals("prompt"))
            {
                return executeSystemFunction(scope, arguments);
            }
            return FunctionStorage.getFunction(identifier).Execute(scope);
        }

        object executeSystemFunction(Scope scope, List<IExpression> arguments)
        {
            if (identifier.Equals("prompt"))
            {
                return null;
            }
            return null;
        }
    }

    public class DeclarationStatement : IStatement
    {
        public TokenType variableType;
        public IExpression value;
        public string name;
        public bool isParameter;
        public Position position;
        public DeclarationStatement(TokenType variableType, IExpression value, string name, Position pos, bool isParameter)
        {
            this.variableType = variableType;
            this.value = value;
            this.name = name;
            this.isParameter = isParameter;
            this.position = pos;
        }

        public object Execute(Scope scope)
        {
            scope.putVariable(new Variable(this));

            if (variableType == TokenType.TURTLE)
            {
                object val = value.Evaluate(scope);
                if (val is TurtleVar)
                {
                    scope.putVariable(new Variable(name, TokenType.TURTLE, val));
                }
            }
            if (value != null)
            {
                object val = value.Evaluate(scope);
                if (val is Literal)
                {
                    object val2 = ((Literal)val).value;
                    if ((val2 is string && variableType != TokenType.STR)
                        || (val2 is int && variableType != TokenType.INT)
                        || (val2 is float && variableType != TokenType.FLOAT)
                        || (val2 is bool && (variableType != TokenType.TRUE && variableType != TokenType.FALSE))
                        || (val2 is TurtleVar && variableType != TokenType.TURTLE))
                    {
                        ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!", position));
                    }
                    scope.putVariable(new Variable(name, variableType, val2));
                }
                if ((val is string && variableType != TokenType.STR)
                        || (val is int && variableType != TokenType.INT)
                        || (val is float && variableType != TokenType.FLOAT)
                        || (val is bool && (variableType != TokenType.TRUE && variableType != TokenType.FALSE))
                        || (val is TurtleVar && variableType != TokenType.TURTLE))
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!", position));
                }
                scope.putVariable(new Variable(name, variableType, val));
            }
            return null;
        }
    }

    public class ReturnStatement : IStatement
    {
        public IExpression expression;
        Token token;

        public ReturnStatement(IExpression expression, Token token)
        {
            this.expression = expression;
            this.token = token;
        }

        public object Execute(Scope scope)
        {
            return expression.Evaluate(scope);
        }
    }

    public class AssignStatement : IStatement
    {
        public IExpression expression;
        public string variable;

        public AssignStatement(IExpression expression, string variable)
        {
            this.expression = expression;
            this.variable = variable;
        }

        public object Execute(Scope scope)
        {
            Variable variable = scope.getVariable(this.variable);
            TokenType variableType = variable.type;
            object val = this.expression.Evaluate(scope);

            if ((val is string && variableType != TokenType.STR)
                        || (val is int && variableType != TokenType.INT)
                        || (val is float && variableType != TokenType.FLOAT)
                        || (val is bool && (variableType != TokenType.TRUE && variableType != TokenType.FALSE))
                        || (val is TurtleVar && variableType != TokenType.TURTLE))
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!"));
            }

            scope.setVariableValue(this.variable, val);
            return null;
        }
    }

    public class IfStatement : IStatement
    {
        public IExpression condition;
        public List<IStatement> body;
        public List<IStatement> elseBody;

        public IfStatement(IExpression condition, List<IStatement> body, List<IStatement> elseBody)
        {
            this.condition = condition;
            this.body = body;
            this.elseBody = elseBody;
        }

        public object Execute(Scope scope)
        {
            if ((bool)condition.Evaluate(scope))
            {
                foreach (IStatement statement in body)
                {
                    if (statement is ReturnStatement)
                    {
                        return statement;
                    }
                    statement.Execute(scope);
                }
            }
            else
            {
                if (elseBody != null)
                {
                    foreach (IStatement statement in elseBody)
                    {
                        if (statement is ReturnStatement)
                        {
                            return statement;
                        }
                        statement.Execute(scope);
                    }
                }
            }
            return null;
        }
    }

    public class WhileStatement : IStatement
    {
        public IExpression condition;
        public List<IStatement> body;

        public WhileStatement(IExpression condition, List<IStatement> body)
        {
            this.condition = condition;
            this.body = body;
        }

        public object Execute(Scope scope)
        {
            while ((bool)condition.Evaluate(scope))
            {
                foreach (IStatement statement in body)
                {
                    if (statement is ReturnStatement)
                    {
                        return statement;
                    }
                    statement.Execute(scope);
                }
            }
            return null;
        }
    }

}
