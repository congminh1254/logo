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
        public Parser(Lexer lexer) {
            this.lexer= lexer;
            this.functions = new Dictionary<String, FunctionStatement>();
            currentToken = null;
            nextToken = lexer.getToken();
        }

        Token advanceToken()
        {
            currentToken = nextToken;
            nextToken = lexer.getToken();
            return currentToken;
        }

        Token previewToken()
        {
            return nextToken;
        }

        string peekToken()
        {
            Console.WriteLine("peekToken: " + nextToken.getTokenType());
            return nextToken.getTokenType();
        }

        void skipNewlineToken()
        {
            while (previewToken().getTokenType() == TokenType.NL)
                acceptToken(advanceToken(), TokenType.NL);
        }

        Token acceptAdvanceToken(string[] expectedType)
        {
            if (expectedType.Length == 1)
            {
                return acceptToken(advanceToken(), expectedType[0]);
            }
            return acceptTokens(advanceToken(), expectedType);
        }

        Token acceptTokens(Token token, string[] expectedTypes)
        {
            Console.WriteLine(token.toString());
            foreach (string type in expectedTypes)
            {
                if (token.getTokenType() == type)
                {
                    return token;
                }
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Unexpected token type " + token.getTokenType() + ", expected: " + string.Join(", ", expectedTypes), token.getPosition()));
            return token;
        }

        Token acceptToken(Token token, string expectedType)
        {
            Console.WriteLine(token.toString());
            if (expectedType != token.getTokenType())
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Unexpected token type " + token.getTokenType() + ", expected: " + expectedType, token.getPosition()));
            }
            return token;
        }

        public Dictionary<string, FunctionStatement> parse()
        {
            skipNewlineToken();
            while (previewToken().getTokenType() != TokenType.EOF)
            {
                Console.WriteLine(nextToken.getTokenType());
                Console.ReadLine();
                FunctionStatement func = parseFunction();
                if (functions.ContainsKey(func.identifier.getTextValue()))
                {
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Function " + func.identifier.getTextValue() + " already defined!", func.identifier.getPosition()));
                }
                skipNewlineToken();
            }
            return functions;
        }

        FunctionStatement parseFunction()
        {
            Console.WriteLine("Parse function!!!");
            Token identifier = acceptAdvanceToken(new string[] { TokenType.IDENTIFIER });
            List<DeclarationStatement> vars = new List<DeclarationStatement>();
            acceptAdvanceToken(new string[] { TokenType.LPAREN });
            if (peekToken() != TokenType.RPAREN)
            {
                vars.Add(parseVarFuncDeclare());
                while (peekToken() == TokenType.COMMA)
                {
                    acceptAdvanceToken(new string[] { TokenType.COMMA });
                    vars.Add(parseVarFuncDeclare());
                }
            }
            acceptAdvanceToken(new string[] { TokenType.RPAREN });
            acceptAdvanceToken(new string[] { TokenType.NL });
            acceptAdvanceToken(new string[] { TokenType.LCURLY});
            acceptAdvanceToken(new string[] { TokenType.NL });

            List<Statement> statements = parseFunctionBody();
            while (peekToken() == TokenType.NL)
            {
                acceptAdvanceToken(new string[] { TokenType.NL });
            }
            acceptAdvanceToken(new string[] { TokenType.NL });

            return new FunctionStatement(identifier, vars, statements);
        }

        List<Statement> parseFunctionBody()
        {
            List<Statement> result = new List<Statement>();
            while (peekToken() != TokenType.RCURLY)
            {
                result.Add(parseStatement());
            }
            return result;
        }

        Statement parseStatement()
        {
            Token token = acceptAdvanceToken(new string[] { TokenType.IDENTIFIER, TokenType.IF, TokenType.WHILE, TokenType.RETURN });
            switch (token.getTokenType())
            {
                case TokenType.IDENTIFIER:
                    if (peekToken() == TokenType.LPAREN) {
                        acceptAdvanceToken(new string[] { TokenType.LPAREN });
                        List<Expression> args = parseFunctionCallArgs();
                        acceptAdvanceToken(new string[] {TokenType.RPAREN });
                        acceptAdvanceToken(new string[] { TokenType.NL });
                        return new FunctionCallStatement(token.getTextValue(), args);
                    }
                    break;
                case TokenType.IF:
                    return parseIf();
                case TokenType.WHILE:
                    return parseWhile();
                case TokenType.RETURN:
                    Expression exp = parseExpression();
                    acceptAdvanceToken(new string[] { TokenType.NL });
                    return new ReturnStatement(exp, token);
                default:
                    ErrorHandling.pushError(new ErrorHandling.LogoException("Unexpected token " + token.getTokenType(), token.getPosition()));
                    break;
            }
            return null;
        }

        Statement parseWhile()
        {
            Expression condition = parseExpression();
            acceptAdvanceToken(new string[] { TokenType.NL });
            acceptAdvanceToken(new string[] { TokenType.LCURLY});
            List<Statement> body = parseFunctionBody();
            acceptAdvanceToken(new string[] { TokenType.RCURLY });
            return new WhileStatement(condition, body);
        }

        DeclarationStatement parseVarFuncDeclare()
        {
            Token identifier = acceptAdvanceToken(new string[] { TokenType.IDENTIFIER });
            acceptAdvanceToken(new string[] { TokenType.COLON });
            Token type = acceptAdvanceToken(new string[] { TokenType.INT_T, TokenType.FLOAT_T, TokenType.BOOL_T, TokenType.TURTLE_T });
            return new DeclarationStatement(type.getTokenType(), null, identifier.getTextValue(), identifier.getPosition(), true);
        }

        IfStatement parseIf()
        {
            Expression condition = parseExpression();
            acceptAdvanceToken(new string[] { TokenType.LCURLY});
            List<Statement> body = new List<Statement>();
            List<Statement> elseBody = new List<Statement>();
            acceptAdvanceToken(new string[] { TokenType.RCURLY });
            acceptAdvanceToken(new string[] { TokenType.NL });
            if (peekToken() == TokenType.ELSE)
            {
                acceptAdvanceToken(new string[] { TokenType.ELSE });
                acceptAdvanceToken(new string[] { TokenType.NL });
                acceptAdvanceToken(new string[] { TokenType.LCURLY });
                elseBody = parseFunctionBody();
                acceptAdvanceToken(new string[] { TokenType.RCURLY });
                acceptAdvanceToken(new string[] { TokenType.NL });
            }
            return new IfStatement(condition, body, elseBody);
        }

        Expression parseExpression()
        {
            return parseOr();
        }

        Expression parseOr()
        {
            Expression expr = parseAnd();
            while (peekToken() == TokenType.OR)
            {
                Token token = acceptAdvanceToken(new string[] { TokenType.OR });
                Expression right = parseAnd();
                expr = new Or(expr, right, token);
            }
            return expr;
        }

        Expression parseAnd()
        {
            Expression expr = parseEqual();
            while (peekToken() == TokenType.AND)
            {
                Token token = acceptAdvanceToken(new string[] { TokenType.AND });
                Expression right = parseEqual();
                expr = new Or(expr, right, token);
            }
            return expr;
        }

        Expression parseEqual()
        {
            Expression expr = parseComparison();
            while (peekToken() == TokenType.EQEQ || peekToken() == TokenType.DIFF)
            {
                Token token = acceptAdvanceToken(new string[] { TokenType.EQEQ, TokenType.DIFF });
                Expression right = parseComparison();
                expr = new Or(expr, right, token);
            }
            return expr;
        }

        Expression parseComparison()
        {
            Expression expr = parseAdditiveExpr();
            while (peekToken() == TokenType.GE || peekToken() == TokenType.LE || peekToken() == TokenType.LT || peekToken() == TokenType.GT)
            {
                Token token = acceptAdvanceToken(new string[] { TokenType.GE, TokenType.LE, TokenType.LT, TokenType.GT });
                Expression right = parseAdditiveExpr();
                expr = new Comparison(expr, right, token);
            }
            return expr;
        }

        Expression parseAdditiveExpr()
        {
            Expression expr = parseUnary();
            while (peekToken() == TokenType.DIV || peekToken() == TokenType.MOD || peekToken() == TokenType.MUL)
            {
                Token token = acceptAdvanceToken(new string[] { TokenType.DIV, TokenType.MOD, TokenType.MUL });
                Expression right = parseUnary();
                if (token.getTokenType() == TokenType.MUL)
                {
                    expr = new Multiplication(expr, right, token);
                }
                else if (token.getTokenType() == TokenType.DIV)
                {
                    expr = new Division(expr, right, token);
                }
                else if (token.getTokenType() == TokenType.MOD)
                {
                    expr = new Division(expr, right, token);
                }
            }
            return expr;
        }
        
        Expression parseUnary()
        {
            if (peekToken() == TokenType.NOT)
            {
                Token token = acceptAdvanceToken(new string[] { TokenType.NOT });
                Expression expr = parseBasicExpr();
                return new Not(expr, token);
            } else if (peekToken() == TokenType.MINUS)
            {
                Token token = acceptAdvanceToken(new string[] { TokenType.MINUS });
                Expression expr = parseBasicExpr();
                return new Negation(expr, token);
            }
            return parseBasicExpr();
        }

        Expression parseBasicExpr()
        {
            Token token;
            switch (peekToken())
            {
                case TokenType.LPAREN:
                    acceptAdvanceToken(new string[] { TokenType.LPAREN });
                    Expression expr = parseOr();
                    acceptAdvanceToken(new string[] { TokenType.RPAREN });
                    return expr;
                case TokenType.IDENTIFIER:
                    token = acceptAdvanceToken(new string[] { TokenType.IDENTIFIER });
                    if (peekToken() == TokenType.LPAREN)
                    {
                        acceptAdvanceToken(new string[] { TokenType.LPAREN });
                        List<Expression> args = parseFunctionCallArgs();
                        acceptAdvanceToken(new string[] { TokenType.RPAREN });
                        return new FunctionCallExp(token.getTextValue(), args);
                    }
                    return new Identifier(token.getTextValue(), token);
                case TokenType.INT:
                    token = acceptAdvanceToken(new string[] { TokenType.INT });
                    return new Literal(token.getIntValue(), token);
                case TokenType.FLOAT:
                    token = acceptAdvanceToken(new string[] { TokenType.FLOAT });
                    return new Literal(token.getFloatValue(), token);
                case TokenType.STR:
                    token = acceptAdvanceToken(new string[] { TokenType.STR });
                    return new Literal(token.getTextValue(), token);
                case TokenType.BOOL:
                    token = acceptAdvanceToken(new string[] { TokenType.BOOL });
                    return new Literal(token.getBooleanValue(), token);
                default:
                    acceptAdvanceToken(new string[] {TokenType.LPAREN, TokenType.IDENTIFIER, TokenType.FLOAT, TokenType.INT, TokenType.STR, TokenType.BOOL, TokenType.TURTLE});
                    break;
            }
            return null;
        }

        List<Expression> parseFunctionCallArgs()
        {
            List<Expression> args = new List<Expression>();
            if (peekToken() == TokenType.RPAREN)
            {
                return args;
            }
            args.Add(parseExpression());
            while (peekToken() == TokenType.COMMA)
            {
                acceptAdvanceToken(new string[] { TokenType.COMMA });
                args.Add(parseExpression());
            }
            return args;
        }
    }
}
