using Logo;
using Logo.Core;
using Logo.Core.Utils;
using NUnit.Framework;
using System.Collections.Generic;

namespace Logo
{
    [TestFixture]
    public class LexerTest
    {
        [Test]
        public void TestReadFile()
        {
            string filename = "./Tests/Fixtures/ExampleFile.txt";
            SourceCode source = new SourceCode(filename);
            string text = source.getAllText();
            Assert.IsNotEmpty(text);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Assert.NotZero(tokens.Count);
            Position position = tokens[0].getPosition();
            Assert.AreEqual(position.getColumn(), 1);
            Assert.AreEqual(position.getLine(), 1);
            Assert.IsNotEmpty(position.ToString());
        }

        [Test]
        public void TestEof()
        {
            SourceCode source = new SourceCode("", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[tokens.Count-1];
            Assert.AreEqual(TokenType.EOF, token.getTokenType());
            token = lexer.getToken();
            Assert.AreEqual(TokenType.EOF, token.getTokenType());
        }

        [Test]
        public void TestValidString()
        {
            SourceCode source = new SourceCode(" \"Hello world\"",false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.STR, token.getTokenType());
            Assert.AreEqual("Hello world", token.getTextValue());
            Assert.AreEqual(Token.ValueType.TEXT, token.getValueType());
            Assert.IsNotEmpty(token.ToString());
        }

        [Test]
        public void TestInvalidString()
        {
            SourceCode source = new SourceCode(" \"absdfsf   ", false);
            Lexer lexer = new Lexer(source);
            Token errorToken = lexer.getAllTokens().Find(token => token.getTokenType() == TokenType.ERROR);
            Assert.IsNotNull(errorToken);
            Assert.AreEqual(errorToken.getTokenType(), TokenType.ERROR);
        }

        [Test]
        public void TestValidIntNumber()
        {
            SourceCode source = new SourceCode(" 1   ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.INT, token.getTokenType());
            Assert.AreEqual(1, token.getIntValue());
            Assert.AreEqual(Token.ValueType.INT, token.getValueType());
            Assert.IsNotEmpty(token.ToString());
        }

        [Test]
        public void TestValidFloatNumber()
        {
            SourceCode source = new SourceCode(" 1.75   ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.FLOAT, token.getTokenType());
            Assert.AreEqual(1.75, token.getFloatValue());
            Assert.AreEqual(Token.ValueType.FLOAT, token.getValueType());
            Assert.IsNotEmpty(token.ToString());
        }

        [Test]
        public void TestInvalidNumber()
        {
            SourceCode source = new SourceCode(" 1. ", false);
            Lexer lexer = new Lexer(source);
            Token errorToken = lexer.getAllTokens().Find(token => token.getTokenType() == TokenType.ERROR);
            Assert.IsNotNull(errorToken);
            Assert.AreEqual(errorToken.getTokenType(), TokenType.ERROR);
        }

        [Test]
        public void TestValidBool()
        {
            SourceCode source = new SourceCode(" true false  ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.BOOL, token.getTokenType());
            Assert.AreEqual(true, token.getBooleanValue());
            token = tokens[1];
            Assert.AreEqual(TokenType.BOOL, token.getTokenType());
            Assert.AreEqual(false, token.getBooleanValue());
            Assert.IsNotEmpty(token.ToString());
        }

        [Test]
        public void TestIdentifier()
        {
            SourceCode source = new SourceCode(" abc   ", false);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.getAllTokens();
            Token token = tokens[0];
            Assert.AreEqual(TokenType.IDENTIFIER, token.getTokenType());
        }

        [Test]
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
            Assert.AreEqual(TokenType.BOOL, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.BOOL, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.INT_T, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.STR_T, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.FLOAT_T, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.BOOL_T, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.TURTLE, tokens[i++].getTokenType());
            Assert.AreEqual(TokenType.UUU, tokens[i++].getTokenType());
        }

        [Test]
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

        [Test]
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