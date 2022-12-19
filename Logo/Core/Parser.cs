using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logo.Core.Utils.Grammar;

namespace Logo.Core
{
    internal class Parser
    {
        Lexer lexer;
        Dictionary<String, FunctionStatement> functions = new Dictionary<string, FunctionStatement>();
        Token currentToken, nextToken;
        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            this.functions = new Dictionary<String, FunctionStatement>();
            currentToken = null;
            nextToken = lexer.advanceToken();
        }

        Token advanceToken()
        {
            currentToken = nextToken;
            nextToken = lexer.advanceToken();
            while (currentToken.tokenType == TokenType.NL && nextToken.tokenType == TokenType.NL)
            {
                nextToken = lexer.advanceToken();
            }
            return currentToken;
        }

        TokenType peekToken()
        {
            return nextToken.tokenType;
        }

        Token acceptAdvanceToken(TokenType[] expectedType, bool skipNL = true)
        {
            Token returnValue;
            if (expectedType.Length == 1)
            {
                returnValue = acceptToken(advanceToken(), expectedType[0]);
            } else 
                returnValue = acceptTokens(advanceToken(), expectedType);
            if (skipNL)
                skipNLToken();
            return returnValue;
        }

        Token acceptTokens(Token token, TokenType[] expectedTypes)
        {
            foreach (TokenType type in expectedTypes)
            {
                if (token.tokenType == type)
                {
                    return token;
                }
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Unexpected token type " + token.tokenType + ", expected: " + string.Join(", ", expectedTypes), token.position));
            return null;
        }

        bool skipNLToken()
        {
            bool skipped = false;
            while (peekToken() == TokenType.NL)
            {
                advanceToken();
                skipped = true;
            }
            return skipped;
        }

        Token acceptToken(Token token, TokenType expectedType)
        {
            if (expectedType != token.tokenType)
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Unexpected token type " + token.tokenType + ", expected: " + expectedType, token.position));
            }
            return token;
        }

        public Dictionary<string, FunctionStatement> parse()
        {
            FunctionStatement func = tryParseFunction();
            while (func != null)
            {
                if (functions.ContainsKey(func.identifier.textValue))
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Function " + func.identifier.textValue + " already defined!", func.identifier.position));
                }
                else
                {
                    functions[func.identifier.textValue] = func;
                }
                func = tryParseFunction();
            }
            return functions;
        }

        FunctionStatement tryParseFunction()
        {
            if (nextToken.tokenType != TokenType.IDENTIFIER)
            {
                return null;
            }
            Token identifier = acceptAdvanceToken(new TokenType[] { TokenType.IDENTIFIER });
            acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
            List<DeclarationStatement> vars = new List<DeclarationStatement>();
            DeclarationStatement var = tryParseVarFuncDeclare();
            if (var != null)
            {
                vars.Add(var);
                while (peekToken() == TokenType.COMMA)
                {
                    acceptAdvanceToken(new TokenType[] { TokenType.COMMA });
                    var = tryParseVarFuncDeclare();
                    if (var != null)
                        vars.Add(var);
                    else
                        ErrorHandling.pushError(new ErrorHandling.LogoException("Syntax Error"));
                }
            }
            acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
            BlockStatement block = tryParseFunctionBody();
            if (block == null)
                ErrorHandling.pushError(new ErrorHandling.LogoException("Unable to parse body of function", nextToken.position));
            return new FunctionStatement(identifier, vars, block);
        }

        BlockStatement tryParseFunctionBody()
        {
            if (peekToken() != TokenType.LCURLY)
            {
                return null;
            }
            acceptAdvanceToken(new TokenType[] { TokenType.LCURLY });
            List<IStatement> result = new List<IStatement>();
            IStatement statement = tryParseStatement();
            while (statement != null)
            {
                result.Add(statement);
                statement = tryParseStatement();
            }
            acceptAdvanceToken(new TokenType[] { TokenType.RCURLY });
            return new BlockStatement(result);
        }

        IStatement tryParseStatement()
        {
            var statement = tryParseAssignOrFunctionCall();
            if (statement != null)
            {
                return statement;
            }

            statement = tryParseIf();
            if (statement != null)
            { 
                return statement; 
            }

            statement = tryParseWhile();
            if (statement != null)
            { 
                return statement; 
            }
            
            statement = tryParseReturn();
            if (statement != null) { 
                return statement; 
            }

            return null;
        }

        IStatement tryParseAssignOrFunctionCall()
        {
            if (peekToken() != TokenType.IDENTIFIER)
            {
                return null;
            }
            var token = acceptAdvanceToken(new TokenType[] { TokenType.IDENTIFIER });
            if (peekToken() == TokenType.LPAREN)
            {
                acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
                List<IExpression> args = parseFunctionCallArgs();
                acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
                return new FunctionCallStatement(token.textValue, args);
            }
            else
            {
                acceptAdvanceToken(new TokenType[] { TokenType.EQ });
                IExpression expr = parseExpression();
                return new AssignStatement(expr, token.textValue);
            }
        }

        IStatement tryParseWhile()
        {
            if (peekToken() != TokenType.WHILE)
            {
                return null;
            }
            acceptAdvanceToken(new TokenType[] { TokenType.WHILE });
            IExpression condition = parseExpression();
            BlockStatement block = tryParseFunctionBody();
            if (block != null)
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Error parsing body for While statement", nextToken.position));
            }
            return new WhileStatement(condition, block);
        }

        DeclarationStatement tryParseVarFuncDeclare()
        {
            if (peekToken() != TokenType.IDENTIFIER)
                return null;
            Token identifier = acceptAdvanceToken(new TokenType[] { TokenType.IDENTIFIER });
            acceptAdvanceToken(new TokenType[] { TokenType.COLON });
            Token type = acceptAdvanceToken(new TokenType[] { TokenType.INT_T, TokenType.FLOAT_T, TokenType.BOOL_T, TokenType.TURTLE });
            if (type == null)
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Function argument not valid", identifier.position));
                return null;
            }
            Dictionary<TokenType, VariableType> mapping = new Dictionary<TokenType, VariableType>()
            {
                {TokenType.STR_T, VariableType.STR},
                {TokenType.INT_T, VariableType.INT},
                {TokenType.FLOAT_T, VariableType.FLOAT},
                {TokenType.TURTLE, VariableType.TURTLE},
                {TokenType.BOOL_T, VariableType.BOOL},
            };
            if (mapping.ContainsKey(type.tokenType)) {
                VariableType varType;
                mapping.TryGetValue(type.tokenType, out varType);
                return new DeclarationStatement(varType, null, identifier.textValue, identifier.position, true);
            } else return null;
        }

        IfStatement tryParseIf()
        {
            if (peekToken() != TokenType.IF)
            {
                return null;
            }
            acceptAdvanceToken(new TokenType[] { TokenType.IF });
            IExpression condition = parseExpression();

            BlockStatement body = tryParseFunctionBody();
            if (body == null)
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Error while parsing body for If statement", nextToken.position));
                return null;
            }
            BlockStatement elseBody = null;
            if (peekToken() == TokenType.ELSE)
            {
                acceptAdvanceToken(new TokenType[] { TokenType.ELSE });
                elseBody = tryParseFunctionBody();
                if (elseBody == null)
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Error while parsing else body for If statement", nextToken.position));
                    return null;
                }
            }
            return new IfStatement(condition, body, elseBody);
        }

        IStatement tryParseReturn()
        {
            if (peekToken() != TokenType.RETURN)
            {
                return null;
            }
            acceptAdvanceToken(new TokenType[] { TokenType.RETURN });
            IExpression exp = parseExpression();
            return new ReturnStatement(exp);
        } 

        IExpression parseExpression()
        {
            return parseOr();
        }

        IExpression parseOr()
        {
            IExpression expr = parseAnd();
            if (expr == null)
                return null;
            while (peekToken() == TokenType.OR)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.OR });
                IExpression right = parseAnd();
                expr = new Or(expr, right, token.position);
            }
            return expr;
        }

        IExpression parseAnd()
        {
            IExpression expr = parseEqual();
            if (expr == null)
                return null;
            while (peekToken() == TokenType.AND)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.AND });
                IExpression right = parseEqual();
                expr = new AndExpresstion(expr, right, token.position);
            }
            return expr;
        }

        IExpression parseEqual()
        {
            IExpression expr = parseComparison();
            if (expr == null)
                return null;
            while (peekToken() == TokenType.EQEQ || peekToken() == TokenType.DIFF)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.EQEQ, TokenType.DIFF });
                IExpression right = parseComparison();
                Equality.EqualityType type = (token.tokenType == TokenType.EQEQ) ? Equality.EqualityType.EQUAL : Equality.EqualityType.DIFF;
                expr = new Equality(expr, right, type, token.position);
            }
            return expr;
        }

        IExpression parseComparison()
        {
            IExpression expr = parseAdditiveExpr();
            if (expr == null)
                return null;
            while (peekToken() == TokenType.GE || peekToken() == TokenType.LE || peekToken() == TokenType.LT || peekToken() == TokenType.GT)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.GE, TokenType.LE, TokenType.LT, TokenType.GT });
                IExpression right = parseAdditiveExpr();
                Comparison.ComparisonType type;
                switch (token.tokenType)
                {
                    case TokenType.GT:
                        type = Comparison.ComparisonType.GT;
                        break;
                    case TokenType.LE:
                        type = Comparison.ComparisonType.LE;
                        break;
                    case TokenType.GE:
                        type = Comparison.ComparisonType.GE;
                        break;
                    case TokenType.LT:
                        type = Comparison.ComparisonType.LT;
                        break;
                    default:
                        return null;
                }
                expr = new Comparison(expr, right, type, token.position);
            }
            return expr;
        }

        IExpression parseMultiplicative()
        {
            IExpression expr = parseUnary();
            if (expr == null)
                return null;
            while (peekToken() == TokenType.DIV || peekToken() == TokenType.MOD || peekToken() == TokenType.MUL)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.DIV, TokenType.MOD, TokenType.MUL });
                IExpression right = parseUnary();
                switch (token.tokenType)
                {
                    case TokenType.MUL:
                        expr = new Multiplication(expr, right, token.position);
                        break;
                    case TokenType.DIV:
                        expr = new Division(expr, right, token.position);
                        break;
                    case TokenType.MOD:
                        expr = new Modulo(expr, right, token.position);
                        break;
                }
            }
            return expr;
        }

        IExpression parseAdditiveExpr()
        {
            IExpression expr = parseMultiplicative();
            if (expr == null)
                return null;
            while (peekToken() == TokenType.PLUS || peekToken() == TokenType.MINUS)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.PLUS, TokenType.MINUS });
                IExpression right = parseMultiplicative();
                if (token.tokenType == TokenType.PLUS)
                {
                    expr = new Sum(expr, right, token.position);
                }
                else
                {
                    expr = new Subtract(expr, right, token.position);
                }
            }
            return expr;
        }

        IExpression parseUnary()
        {
            if (peekToken() == TokenType.NOT)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.NOT });
                IExpression expr = parseBasicExpr();
                return new NotExpression(expr, token.position);
            }
            else if (peekToken() == TokenType.MINUS)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.MINUS });
                IExpression expr = parseBasicExpr();
                return new Negation(expr, token.position);
            }
            return parseBasicExpr();
        }

        Literal tryParseTurtle()
        {
            if (peekToken() != TokenType.TURTLE) return null;
            var token = acceptAdvanceToken(new TokenType[] { TokenType.TURTLE });
            acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
            acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
            return new Literal(new TurtleVar(), token);
        }

        IExpression tryParseGroupStatement()
        {
            if (peekToken() != TokenType.LPAREN) return null;
            acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
            IExpression expr = parseOr();
            acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
            return expr;
        }

        IExpression tryParseFunctionCallExpr(Token token)
        {
            if (peekToken() != TokenType.LPAREN)
                return null;
            acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
            List<IExpression> args = parseFunctionCallArgs();
            acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
            return new FunctionCallExp(token.textValue, args);
        }

        IExpression tryParseIdentifierOrFuncCall()
        {
            if (peekToken() != TokenType.IDENTIFIER) return null;
            Token token = acceptAdvanceToken(new TokenType[] { TokenType.IDENTIFIER });
            IExpression expr = tryParseFunctionCallExpr(token);
            if (expr != null)
                return expr;
            return new Identifier(token.textValue, token);
        }

        IExpression tryParseVariableValue()
        {
            TokenType type = peekToken();
            if (type != TokenType.INT && type != TokenType.FLOAT && type != TokenType.STR && type != TokenType.TRUE && type != TokenType.FALSE)
                return null;
            switch (peekToken())
            {
                case TokenType.INT:
                    var token = acceptAdvanceToken(new TokenType[] { TokenType.INT });
                    return new Literal(token.intValue, token);
                case TokenType.FLOAT:
                    token = acceptAdvanceToken(new TokenType[] { TokenType.FLOAT });
                    return new Literal(token.floatValue, token);
                case TokenType.STR:
                    token = acceptAdvanceToken(new TokenType[] { TokenType.STR });
                    return new Literal(token.textValue, token);
                case TokenType.TRUE:
                    token = acceptAdvanceToken(new TokenType[] { TokenType.TRUE });
                    return new Literal(true, token);
                case TokenType.FALSE:
                    token = acceptAdvanceToken(new TokenType[] { TokenType.FALSE });
                    return new Literal(false, token);
            }
            return null;
        }

        IExpression parseBasicExpr()
        {
            IExpression expr = null;
            expr = tryParseTurtle();
            if (expr != null)
                return expr;

            expr = tryParseGroupStatement();
            if (expr != null)
                return expr;

            expr = tryParseIdentifierOrFuncCall();
            if (expr != null)
                return expr;

            expr = tryParseVariableValue();
            if (expr != null)
                return expr;

            return null;
        }

        List<IExpression> parseFunctionCallArgs()
        {
            List<IExpression> args = new List<IExpression>();// TODO try parse args
            var expr = parseExpression();
            if (expr != null)
            {
                args.Add(expr);
                while (peekToken() == TokenType.COMMA)
                {
                    acceptAdvanceToken(new TokenType[] { TokenType.COMMA });
                    expr = parseExpression();
                    if (expr != null)
                        args.Add(expr);
                    else 
                        ErrorHandling.pushError(new ErrorHandling.LogoException("Function call arguments is not valid!", nextToken.position));
                }
            }
            
            return args;
        }
    }
}
