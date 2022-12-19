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

        Token acceptAdvanceToken(TokenType[] expectedType)
        {
            if (expectedType.Length == 1)
            {
                return acceptToken(advanceToken(), expectedType[0]);
            }
            return acceptTokens(advanceToken(), expectedType);
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
            return token;
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
            while (peekToken() != TokenType.EOF)
            {
                skipNLToken();
                FunctionStatement func = parseFunction();
                if (functions.ContainsKey(func.identifier.textValue))
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Function " + func.identifier.textValue + " already defined!", func.identifier.position));
                }
                else
                {
                    functions[func.identifier.textValue] = func;
                }
                skipNLToken();
            }
            return functions;
        }

        FunctionStatement parseFunction()
        {
            Token identifier = acceptAdvanceToken(new TokenType[] { TokenType.IDENTIFIER });
            List<DeclarationStatement> vars = new List<DeclarationStatement>();
            acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
            if (peekToken() != TokenType.RPAREN)
            {
                vars.Add(parseVarFuncDeclare());
                while (peekToken() == TokenType.COMMA)
                {
                    acceptAdvanceToken(new TokenType[] { TokenType.COMMA });
                    vars.Add(parseVarFuncDeclare());
                }
            }
            acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
            acceptAdvanceToken(new TokenType[] { TokenType.LCURLY });
            skipNLToken();
            List<IStatement> statements = parseFunctionBody();
            skipNLToken();
            acceptAdvanceToken(new TokenType[] { TokenType.RCURLY });
            skipNLToken();
            return new FunctionStatement(identifier, vars, statements);
        }

        List<IStatement> parseFunctionBody()
        {
            List<IStatement> result = new List<IStatement>();
            while (peekToken() != TokenType.RCURLY)
            {
                skipNLToken();
                result.Add(parseStatement());
                skipNLToken();
            }
            return result;
        }

        IStatement parseStatement()
        {
            Token token = acceptAdvanceToken(new TokenType[] { TokenType.IDENTIFIER, TokenType.IF, TokenType.WHILE, TokenType.RETURN });
            switch (token.tokenType)
            {
                case TokenType.IDENTIFIER:
                    if (peekToken() == TokenType.LPAREN)
                    {
                        acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
                        List<IExpression> args = parseFunctionCallArgs();
                        acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
                        skipNLToken();
                        return new FunctionCallStatement(token.textValue, args);
                    }
                    else
                    {
                        acceptAdvanceToken(new TokenType[] { TokenType.EQ });
                        IExpression expr = parseExpression();
                        skipNLToken();
                        return new AssignStatement(expr, token.textValue);
                    }
                case TokenType.IF:
                    return parseIf();
                case TokenType.WHILE:
                    return parseWhile();
                case TokenType.RETURN:
                    IExpression exp = parseExpression();
                    skipNLToken();
                    return new ReturnStatement(exp, token);
                default:
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Unexpected token " + token.tokenType, token.position));
                    break;
            }
            return null;
        }

        IStatement parseWhile()
        {
            IExpression condition = parseExpression();
            skipNLToken();
            acceptAdvanceToken(new TokenType[] { TokenType.LCURLY });
            List<IStatement> body = parseFunctionBody();
            acceptAdvanceToken(new TokenType[] { TokenType.RCURLY });
            return new WhileStatement(condition, body);
        }

        DeclarationStatement parseVarFuncDeclare()
        {
            Token identifier = acceptAdvanceToken(new TokenType[] { TokenType.IDENTIFIER });
            acceptAdvanceToken(new TokenType[] { TokenType.COLON });
            Token type = acceptAdvanceToken(new TokenType[] { TokenType.INT_T, TokenType.FLOAT_T, TokenType.BOOL_T, TokenType.TURTLE });
            return new DeclarationStatement(type.tokenType, null, identifier.textValue, identifier.position, true);
        }

        IfStatement parseIf()
        {
            IExpression condition = parseExpression();
            acceptAdvanceToken(new TokenType[] { TokenType.LCURLY });
            skipNLToken();
            List<IStatement> body = parseFunctionBody();
            List<IStatement> elseBody = new List<IStatement>();
            skipNLToken();
            acceptAdvanceToken(new TokenType[] { TokenType.RCURLY });
            if (peekToken() == TokenType.ELSE)
            {
                acceptAdvanceToken(new TokenType[] { TokenType.ELSE });
                acceptAdvanceToken(new TokenType[] { TokenType.LCURLY });
                skipNLToken();
                elseBody = parseFunctionBody();
                skipNLToken();
                acceptAdvanceToken(new TokenType[] { TokenType.RCURLY });
            }
            skipNLToken();
            return new IfStatement(condition, body, elseBody);
        }

        IExpression parseExpression()
        {
            return parseOr();
        }

        IExpression parseOr()
        {
            IExpression expr = parseAnd();
            while (peekToken() == TokenType.OR)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.OR });
                IExpression right = parseAnd();
                expr = new Or(expr, right, token);
            }
            return expr;
        }

        IExpression parseAnd()
        {
            IExpression expr = parseEqual();
            while (peekToken() == TokenType.AND)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.AND });
                IExpression right = parseEqual();
                expr = new AndExpresstion(expr, right, token);
            }
            return expr;
        }

        IExpression parseEqual()
        {
            IExpression expr = parseComparison();
            while (peekToken() == TokenType.EQEQ || peekToken() == TokenType.DIFF)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.EQEQ, TokenType.DIFF });
                IExpression right = parseComparison();
                expr = new Equality(expr, right, token);
            }
            return expr;
        }

        IExpression parseComparison()
        {
            IExpression expr = parseAdditiveExpr();
            while (peekToken() == TokenType.GE || peekToken() == TokenType.LE || peekToken() == TokenType.LT || peekToken() == TokenType.GT)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.GE, TokenType.LE, TokenType.LT, TokenType.GT });
                IExpression right = parseAdditiveExpr();
                expr = new Comparison(expr, right, token);
            }
            return expr;
        }

        IExpression parseMultiplicative()
        {
            IExpression expr = parseUnary();
            while (peekToken() == TokenType.DIV || peekToken() == TokenType.MOD || peekToken() == TokenType.MUL)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.DIV, TokenType.MOD, TokenType.MUL });
                IExpression right = parseUnary();
                if (token.tokenType == TokenType.MUL)
                {
                    expr = new Multiplication(expr, right, token);
                }
                else if (token.tokenType == TokenType.DIV)
                {
                    expr = new Division(expr, right, token);
                }
                else if (token.tokenType == TokenType.MOD)
                {
                    expr = new Modulo(expr, right, token);
                }
            }
            return expr;
        }

        IExpression parseAdditiveExpr()
        {
            IExpression expr = parseMultiplicative();
            while (peekToken() == TokenType.PLUS || peekToken() == TokenType.MINUS)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.PLUS, TokenType.MINUS });
                IExpression right = parseMultiplicative();
                if (token.tokenType == TokenType.PLUS)
                {
                    expr = new Sum(expr, right, token);
                }
                else
                {
                    expr = new Subtract(expr, right, token);
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
                return new NotExpression(expr, token);
            }
            else if (peekToken() == TokenType.MINUS)
            {
                Token token = acceptAdvanceToken(new TokenType[] { TokenType.MINUS });
                IExpression expr = parseBasicExpr();
                return new Negation(expr, token);
            }
            return parseBasicExpr();
        }

        IExpression parseBasicExpr()
        {
            Token token;
            switch (peekToken())
            {
                case TokenType.TURTLE:
                    token = acceptAdvanceToken(new TokenType[] { TokenType.TURTLE });
                    acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
                    acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
                    return new Literal(new TurtleVar(), token);
                case TokenType.LPAREN:
                    acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
                    IExpression expr = parseOr();
                    acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
                    return expr;
                case TokenType.IDENTIFIER:
                    token = acceptAdvanceToken(new TokenType[] { TokenType.IDENTIFIER });
                    if (peekToken() == TokenType.LPAREN)
                    {
                        acceptAdvanceToken(new TokenType[] { TokenType.LPAREN });
                        List<IExpression> args = parseFunctionCallArgs();
                        acceptAdvanceToken(new TokenType[] { TokenType.RPAREN });
                        return new FunctionCallExp(token.textValue, args);
                    }
                    return new Identifier(token.textValue, token);
                case TokenType.INT:
                    token = acceptAdvanceToken(new TokenType[] { TokenType.INT });
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
                default:
                    acceptAdvanceToken(new TokenType[] { TokenType.LPAREN, TokenType.IDENTIFIER, TokenType.FLOAT, TokenType.INT, TokenType.STR, TokenType.TRUE, TokenType.FALSE, TokenType.TURTLE });
                    break;
            }
            return null;
        }

        List<IExpression> parseFunctionCallArgs()
        {
            List<IExpression> args = new List<IExpression>();
            if (peekToken() == TokenType.RPAREN)
            {
                return args;
            }
            args.Add(parseExpression());
            while (peekToken() == TokenType.COMMA)
            {
                acceptAdvanceToken(new TokenType[] { TokenType.COMMA });
                args.Add(parseExpression());
            }
            return args;
        }
    }
}
