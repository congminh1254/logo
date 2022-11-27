using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public class Statement
    {
        public Statement() {
            
        }

        public virtual object execute(Scope scope)
        {
            return null;
        }
    }

    public class FunctionStatement : Statement
    {
        public Token identifier;
        public List<DeclarationStatement> parameters;
        public List<Statement> body;

        public FunctionStatement(Token identifier, List<DeclarationStatement> parameters, List<Statement> body)
        {
            this.identifier = identifier;
            this.parameters = parameters;
            this.body = body;
        }

        public override object execute(Scope scope)
        {
            return execute(scope, null);
        }

        public object execute(Scope scope, List<Expression> args)
        {
            int i = 0;
            Scope newScope = new Scope();
            if (identifier.getTextValue() != "main") {
                foreach (DeclarationStatement statement in parameters)
                {
                    statement.execute(newScope);
                    setScopeVariableValue(newScope, scope, args[i++], statement.name);
                }
            }

            foreach (Statement statement in body) {
                if (statement is ReturnStatement)
                {
                    object val = statement.execute(newScope);
                    return val;
                }
                object returned = statement.execute(newScope);

                if (returned is ReturnStatement)
                {
                    object val = ((ReturnStatement)returned).execute(newScope);
                    return val;
                }
            }
            // void with no return value
            return null;
        }

        private void setScopeVariableValue(Scope innerScope, Scope parentScope, Expression value, String name)
        {
            var variable = innerScope.getVariable(name);
            var variableType = variable.type;
            var val = value.evaluate(parentScope);

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

    public class FunctionCallStatement: Statement
    {
        public string identifier;
        public List<Expression> arguments;

        public FunctionCallStatement(string identifier, List<Expression> arguments)
        {
            this.identifier = identifier;
            this.arguments = arguments;
        }

        public override object execute(Scope scope)
        {
            if (identifier.Equals("prompt"))
            {
                return executeSystemFunction(scope, arguments);
            }
            return FunctionStorage.getFunction(identifier).execute(scope);
        }

        object executeSystemFunction(Scope scope, List<Expression> arguments)
        {
            if (identifier.Equals("prompt"))
            {
                return null;
            }
            return null;
        }
    }

    public class DeclarationStatement: Statement
    {
        public string variableType;
        public Expression value;
        public string name;
        public bool isParameter;
        public Position position;
        public DeclarationStatement(string variableType, Expression value, string name, Position pos, bool isParameter)
        {
            this.variableType = variableType;
            this.value = value;
            this.name = name;
            this.isParameter = isParameter;
            this.position= pos;
        }

        public override object execute(Scope scope)
        {
            scope.putVariable(new Variable(this));

            if (variableType == TokenType.TURTLE)
            {
                object val = value.evaluate(scope);
                if (val is TurtleVar)
                {
                    scope.putVariable(new Variable(name, TokenType.TURTLE, val));
                }
            }
            if (value !=null)
            {
                object val = value.evaluate(scope);
                if (val is Literal) {
                    object val2 = ((Literal)val).value;
                    if ((val2 is string && variableType != TokenType.STR)
                        || (val2 is int && variableType != TokenType.INT)
                        || (val2 is float && variableType != TokenType.FLOAT)
                        || (val2 is bool && variableType != TokenType.BOOL)
                        || (val2 is TurtleVar && variableType != TokenType.TURTLE))
                    {
                        ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!", position));
                    }
                    scope.putVariable(new Variable(name, variableType, val2));
                }
                if ((val is string && variableType != TokenType.STR)
                        || (val is int && variableType != TokenType.INT)
                        || (val is float && variableType != TokenType.FLOAT)
                        || (val is bool && variableType != TokenType.BOOL)
                        || (val is TurtleVar && variableType != TokenType.TURTLE))
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!", position));
                }
                scope.putVariable(new Variable(name, variableType, val));
            }
            return null;
        }
    }

    public class ReturnStatement : Statement
    {
        public Expression expression;
        Token token;

        public ReturnStatement(Expression expression, Token token)
        {
            this.expression = expression;
            this.token = token;
        }

        public override object execute(Scope scope)
        {
            return expression.evaluate(scope);
        }
    }

    public class AssignStatement: Statement
    {
        public Expression expression;
        public string variable;

        public override object execute(Scope scope)
        {
            Variable variable = scope.getVariable(this.variable);
            string variableType = variable.type;
            object val = this.expression.evaluate(scope);

            if ((val is string && variableType != TokenType.STR)
                        || (val is int && variableType != TokenType.INT)
                        || (val is float && variableType != TokenType.FLOAT)
                        || (val is bool && variableType != TokenType.BOOL)
                        || (val is TurtleVar && variableType != TokenType.TURTLE))
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!"));
            }

            scope.setVariableValue(this.variable, val);
            return null;
        }
    }

    public class IfStatement: Statement
    {
        public Expression condition;
        public List<Statement> body;
        public List<Statement> elseBody;

        public IfStatement(Expression condition, List<Statement> body, List<Statement> elseBody)
        {
            this.condition = condition;
            this.body = body;
            this.elseBody = elseBody;
        }

        public override object execute(Scope scope)
        {
            if ((bool) condition.evaluate(scope))
            {
                foreach (Statement statement in body)
                {
                    if (statement is ReturnStatement)
                    {
                        return statement;
                    }
                    statement.execute(scope);
                }
            } else
            {
                if (elseBody != null)
                {
                    foreach(Statement statement in elseBody)
                    {
                        if (statement is ReturnStatement)
                        {
                            return statement;
                        }
                        statement.execute(scope);
                    }
                }
            }
            return null;
        }
    }

    public class WhileStatement: Statement
    {
        public Expression condition;
        public List<Statement> body;

        public WhileStatement(Expression condition, List<Statement> body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override object execute(Scope scope)
        {
            while((bool) condition.evaluate(scope))
            {
                foreach (Statement statement in body)
                {
                    if (statement is ReturnStatement)
                    {
                        return statement;
                    }
                    statement.execute(scope);
                }
            }
            return null;
        }
    }
}
