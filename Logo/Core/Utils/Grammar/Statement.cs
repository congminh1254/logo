﻿using System;
using System.Collections.Generic;
using System.Drawing;

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
        public object returnData = null;

        public FunctionStatement(Token identifier, List<DeclarationStatement> parameters, BlockStatement body)
        {
            this.identifier = identifier;
            this.parameters = parameters;
            this.body = body;
        }

        public object Execute(Scope scope)
        {
            if (returnData != null)
                return returnData;
            object returned = body.Execute(scope);
            if (returned is ReturnStatement)
                return ((ReturnStatement)returned).Execute(scope);
            // void with no return value
            return null;
        }
    }

    public class FunctionCallStatement : IStatement
    {
        public FunctionCallExp exp;

        public FunctionCallStatement(AttrExp identifier, List<IExpression> arguments)
        {
            exp = new FunctionCallExp(identifier, arguments);
        }

        public object Execute(Scope scope)
        {
            return exp.Evaluate(scope);
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
            Variable var = scope.getVariable(name);
            if (var == null)
            {
                scope.setVariable(name, var);
            }

            if (variableType == VariableType.TURTLE)
            {
                object val = value.Evaluate(scope);
                if (val is TurtleVar)
                {
                    var.value = val;
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
                    var.value = val2;
                }
                if ((val is string && variableType != VariableType.STR)
                        || (val is int && variableType != VariableType.INT)
                        || (val is float && variableType != VariableType.FLOAT)
                        || (val is bool && variableType != VariableType.BOOL)
                        || (val is TurtleVar && variableType != VariableType.TURTLE))
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!", position));
                }
                var.value = val;
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
        public AttrExp attr;

        public AssignStatement(AttrExp attr, IExpression exp)
        {
            this.attr = attr;
            this.expression = exp;
        }

        public object Execute(Scope scope)
        {
            object val = this.expression.Evaluate(scope);
            if (val is Variable)
                val = ((Variable)val).value;
            Variable variable = null;
            if (attr != null)
            {
                var returnValue = attr.Evaluate(scope);
                if (!(returnValue is Variable) && attr.parent != null)
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Can not get variable of attribute!"));
                    return null;
                }
                if (returnValue != null)
                    variable = (Variable) returnValue;
            }
            if (variable == null)
            {
                scope.setVariableValue(attr.variableName, val);
                return null;
            }
            
            if ((val is string && !(variable.value is string))
                        || (val is int && !(variable.value is int))
                        || (val is float && !(variable.value is float))
                        || (val is bool && !(variable.value is bool))
                        || (val is TurtleVar && !(variable.value is TurtleVar)))
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("New variable type is diffirence than the original type!"));
            }

            //scope.setVariableValue(this.variable, val);
            variable.value = val;
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
