using Logo;
using Logo.Core;
using Logo.Core.Utils;

namespace LogoTest
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void TestEof()
        {
            SourceCode source = new SourceCode("", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens.Last<Token>();
            Assert.AreEqual(TokenType.EOF, token.getTokenType());
        }

        [TestMethod]
        public void TestValidString()
        {
            SourceCode source = new SourceCode(" \"Hello world\"",false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.STR, token.getTokenType());
            Assert.AreEqual("Hello world", token.getTextValue());
            Assert.AreEqual(Token.ValueType.TEXT, token.getValueType());
        }

        [TestMethod]
        public void TestInvalidString()
        {
            SourceCode source = new SourceCode("./Fixtures/Lexer_InvalidString.txt");
            Lexer lexer = new Lexer(source);
            Token? errorToken = lexer.getAllTokens().Find(token => token.getTokenType() == TokenType.ERROR);
            Assert.IsNotNull(errorToken);
            Assert.AreEqual(errorToken.getTokenType(), TokenType.ERROR);
            Assert.AreEqual(errorToken.getPosition().getLine(), 2);
            Assert.AreEqual(errorToken.getPosition().getColumn(), 9);
        }

        [TestMethod]
        public void TestValidIntNumber()
        {
            SourceCode source = new SourceCode(" 1   ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.INT, token.getTokenType());
            Assert.AreEqual(1, token.getIntValue());
            Assert.AreEqual(Token.ValueType.INT, token.getValueType());
        }

        [TestMethod]
        public void TestValidFloatNumber()
        {
            SourceCode source = new SourceCode(" 1.75   ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.FLOAT, token.getTokenType());
            Assert.AreEqual(1.75, token.getFloatValue());
            Assert.AreEqual(Token.ValueType.FLOAT, token.getValueType());
        }

        [TestMethod]
        public void TestInvalidNumber()
        {
            SourceCode source = new SourceCode("./Fixtures/Lexer_InvalidNumber.txt");
            Lexer lexer = new Lexer(source);
            Token? errorToken = lexer.getAllTokens().Find(token => token.getTokenType() == TokenType.ERROR);
            Assert.IsNotNull(errorToken);
            Assert.AreEqual(errorToken.getTokenType(), TokenType.ERROR);
            Assert.AreEqual(errorToken.getPosition().getLine(), 2);
            Assert.AreEqual(errorToken.getPosition().getColumn(), 9);
        }

        [TestMethod]
        public void TestIdentifier()
        {
            SourceCode source = new SourceCode(" abc   ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.IDENTIFIER, token.getTokenType());
        }

        [TestMethod]
        public void TestKeyword() {
            SourceCode source = new SourceCode(" AND OR NOT if else while return true false int str float bool Turtle ___  ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            int i = 0;
            Assert.AreEqual(TokenType.AND, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.OR, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.NOT, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IF, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.ELSE, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.WHILE, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.RETURN, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.TRUE, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.FALSE, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.INT_T, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.STR_T, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.FLOAT_T, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.BOOL_T, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.TURTLE, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.UUU, tokens[i++].getTokenType());
        }

        [TestMethod]
        public void TestIf() {
            SourceCode source = new SourceCode(" if a==b {\n" +
                "a = a+1\n" +
                "} else { \n" +
                "b = b+1.5\n" +
                "}   ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            int i = 0;
            Assert.AreEqual(TokenType.IF, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.EQEQ, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.LCURLY, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.NL, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.EQ, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.PLUS, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.INT, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.NL, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.RCURLY, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.ELSE, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.LCURLY, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.NL, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.EQ, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.PLUS, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.FLOAT, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.NL, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.RCURLY, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.EOF, tokens[i++].getTokenType());
        }

        [TestMethod]
        public void TestLoop()
        {
            SourceCode source = new SourceCode(" while a<10 {\n" +
                "a = a+1\n" +
                "}   ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            int i = 0;
            Assert.AreEqual(TokenType.WHILE, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.LT, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.INT, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.LCURLY, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.NL, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.EQ, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.PLUS, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.INT, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.NL, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.RCURLY, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.EOF, tokens[i++].getTokenType());
        }
    }
}