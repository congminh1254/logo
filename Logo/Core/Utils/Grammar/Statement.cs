using System;
using System.Collections.Generic;

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
        public BlockStatement body;

        public FunctionStatement(Token identifier, List<DeclarationStatement> parameters, BlockStatement body)
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
            object returned = body.Execute(newScope);
            if (returned is ReturnStatement)
            {
                object val = ((ReturnStatement)returned).Execute(newScope);
                return val;
            }
            // void with no return value
            return null;
        }

        private void setScopeVariableValue(Scope innerScope, Scope parentScope, IExpression value, String name)
        {
            var variable = innerScope.getVariable(name);
            var variableType = variable.type;
            var val = value.Evaluate(parentScope);

            if ((val is string && variableType != VariableType.STR)
                || (val is int && variableType != VariableType.INT)
                || (val is float && variableType != VariableType.FLOAT)
                || (val is bool && variableType != VariableType.BOOL)
                || (val is TurtleVar && variableType != VariableType.TURTLE))
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
        public VariableType variableType;
        public IExpression value;
        public string name;
        public bool isParameter;
        public Position position;
        public DeclarationStatement(VariableType variableType, IExpression value, string name, Position pos, bool isParameter)
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

            if (variableType == VariableType.TURTLE)
            {
                object val = value.Evaluate(scope);
                if (val is TurtleVar)
                {
                    scope.putVariable(new Variable(name, VariableType.TURTLE, val));
                }
            }
            if (value != null)
            {
                object val = value.Evaluate(scope);
                if (val is Literal)
                {
                    object val2 = ((Literal)val).value;
                    if ((val2 is string && variableType != VariableType.STR)
                        || (val2 is int && variableType != VariableType.INT)
                        || (val2 is float && variableType != VariableType.FLOAT)
                        || (val2 is bool && variableType != VariableType.BOOL)
                        || (val2 is TurtleVar && variableType != VariableType.TURTLE))
                    {
                        ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!", position));
                    }
                    scope.putVariable(new Variable(name, variableType, val2));
                }
                if ((val is string && variableType != VariableType.STR)
                        || (val is int && variableType != VariableType.INT)
                        || (val is float && variableType != VariableType.FLOAT)
                        || (val is bool && variableType != VariableType.BOOL)
                        || (val is TurtleVar && variableType != VariableType.TURTLE))
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

        public ReturnStatement(IExpression expression)
        {
            this.expression = expression;
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
            object val = this.expression.Evaluate(scope);

            Variable variable = scope.getVariable(this.variable);
            VariableType variableType = VariableType.INT;
            if (!scope.contains(this.variable))
            {
                if (val is string)
                    variableType = VariableType.STR;
                if (val is int)
                    variableType = VariableType.INT;
                if (val is float)
                    variableType = VariableType.FLOAT;
                if (val is TurtleVar)
                    variableType = VariableType.TURTLE;
                if (val is bool)
                    variableType = VariableType.BOOL;
                scope.putVariable(new Variable(this.variable, variableType, val));
            } else
            {
                variableType = variable.type;
            }

            if ((val is string && variableType != VariableType.STR)
                        || (val is int && variableType != VariableType.INT)
                        || (val is float && variableType != VariableType.FLOAT)
                        || (val is bool && variableType != VariableType.BOOL)
                        || (val is TurtleVar && variableType != VariableType.TURTLE))
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
        public IStatement body;
        public IStatement elseBody;

        public IfStatement(IExpression condition, IStatement body, IStatement elseBody)
        {
            this.condition = condition;
            this.body = body;
            this.elseBody = elseBody;
        }

        public object Execute(Scope scope)
        {
            if ((bool)condition.Evaluate(scope))
            {
                object returned = body.Execute(scope);
                if (returned is ReturnStatement)
                {
                    return returned;
                }
            }
            else
            {
                if (elseBody != null)
                {
                    object returned = elseBody.Execute(scope);
                    if (returned is ReturnStatement)
                    {
                        return returned;
                    }
                }
            }
            return null;
        }
    }

    public class WhileStatement : IStatement
    {
        public IExpression condition;
        public IStatement body;

        public WhileStatement(IExpression condition, IStatement body)
        {
            this.condition = condition;
            this.body = body;
        }

        public object Execute(Scope scope)
        {
            while ((bool)condition.Evaluate(scope))
            {
                object returned = body.Execute(scope);
                if (returned is ReturnStatement)
                {
                    return returned;
                }
            }
            return null;
        }
    }

    public class BlockStatement : IStatement
    {
        public List<IStatement> statements;

        public BlockStatement(List<IStatement> statements)
        {
            this.statements = statements;
        }

        public object Execute(Scope scope)
        {
            foreach (IStatement statement in statements)
            {
                if (statement is ReturnStatement)
                {
                    return statement;
                }
                var result = statement.Execute(scope);
                if (result is ReturnStatement)
                {
                    return result;
                }
            }
            return null;
        }
    }

}
