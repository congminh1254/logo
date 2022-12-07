using Logo;
using Logo.Core;
using Logo.Core.Utils;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.IO;
using NUnit.Framework.Internal;
using System.Text;

namespace Logo
{
    [TestFixture]
    public class LexerTest
    {
        public static string newline = Utils.newline;
        List<Token> getAllTokens(Lexer lexer)
        {
            var tokens = new List<Token>();
            Token token = null;
            while (token == null || (token.tokenType != TokenType.EOF))
            {
                token = lexer.advanceToken();
                tokens.Add(token);
            }
            return tokens;
        }

        StreamReader stringToStreamReader(string s)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(s);
            MemoryStream stream = new MemoryStream(byteArray);
            return new StreamReader(stream);
        }

        [Test]
        public void TestSourceCode()
        {
            string filename = "./Tests/Fixtures/ExampleFile.txt";
            using (var reader = new System.IO.StreamReader(filename))
            {
                SourceCode source = new SourceCode(reader);
                char c = source.getNextChar();
                Assert.AreEqual(c, 'a');
                Assert.AreEqual(source.getCurrChar(), 'a');
                Assert.AreEqual(source.peekChar(), ' ');
                Position pos = source.getPosition();
                Assert.AreEqual(pos.line, 0);
                Assert.AreEqual(pos.column, 0);
                c = source.getNextChar();
                Assert.AreEqual(c, ' ');
                pos = source.getPosition();
                Assert.AreEqual(pos.line, 0);
                Assert.AreEqual(pos.column, 1);
                c = source.getNextChar();
                Assert.AreEqual(c, '=');
                pos = source.getPosition();
                Assert.AreEqual(pos.line, 0);
                Assert.AreEqual(pos.column, 2);
                c = source.getNextChar();
                Assert.AreEqual(c, ' ');
                pos = source.getPosition();
                Assert.AreEqual(pos.line, 0);
                Assert.AreEqual(pos.column, 3);
                c = source.getNextChar();
                Assert.AreEqual(c, '1');
                pos = source.getPosition();
                Assert.AreEqual(pos.line, 0);
                Assert.AreEqual(pos.column, 4);
                c = source.getNextChar();
                Assert.AreEqual(c, Utils.eof);
                pos = source.getPosition();
                Assert.AreEqual(pos.line, 0);
                Assert.AreEqual(pos.column, 5);
            }
        }

        [Test]
        public void TestReadFile()
        {
            string filename = "./Tests/Fixtures/ExampleFile.txt";
            using (var reader = new System.IO.StreamReader(filename))
            {
                SourceCode source = new SourceCode(reader);
                Lexer lexer = new Lexer(source);
                List<Token> tokens = getAllTokens(lexer);
                Assert.NotZero(tokens.Count);
                Position position = tokens[0].position;
                Assert.AreEqual(position.column, 0);
                Assert.AreEqual(position.line, 0);
                Assert.IsNotEmpty(position.ToString());
            }
        }

        [Test]
        public void TestEof()
        {
            MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(""));
            StreamReader reader = new StreamReader(stream);
            SourceCode source = new SourceCode(reader);
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            Token token = tokens[tokens.Count-1];
            Assert.AreEqual(TokenType.EOF, token.tokenType);
            token = lexer.token;
            Assert.AreEqual(TokenType.EOF, token.tokenType);
        }

        [Test]
        public void TestInvalidString()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" \"absdfsf   "));
            Lexer lexer = new Lexer(source);
            Token errorToken = getAllTokens(lexer).Find(token => token.tokenType == TokenType.ERROR);
            Assert.IsNotNull(errorToken);
            Assert.AreEqual(errorToken.tokenType, TokenType.ERROR);
        }

        [Test]
        public void TestValidIntNumber()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" 1   "));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            Token token = tokens[0];
            Assert.AreEqual(TokenType.INT, token.tokenType);
            Assert.AreEqual(1, token.intValue);
            Assert.IsNotEmpty(token.ToString());
        }

        [Test]
        public void TestValidFloatNumber()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" 1.75 3.567  "));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            Token token = tokens[0];
            Assert.AreEqual(TokenType.FLOAT, token.tokenType);
            Assert.AreEqual(1.75, token.floatValue);
            token = tokens[1];
            Assert.AreEqual(3.567, Math.Round((Decimal)token.floatValue, 3, MidpointRounding.AwayFromZero));
            Assert.IsNotEmpty(token.ToString());
        }

        [Test]
        public void TestInvalidNumber()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" 1. "));
            Lexer lexer = new Lexer(source);
            Token errorToken = getAllTokens(lexer).Find(token => token.tokenType == TokenType.ERROR);
            Assert.IsNotNull(errorToken);
            Assert.AreEqual(errorToken.tokenType, TokenType.ERROR);
        }

        [Test]
        public void TestInvalidNumber2()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" 1.1.1 "));
            Lexer lexer = new Lexer(source);
            Token errorToken = getAllTokens(lexer).Find(token => token.tokenType == TokenType.ERROR);
            Assert.IsNotNull(errorToken);
            Assert.AreEqual(errorToken.tokenType, TokenType.ERROR);
        }

        [Test]
        public void TestValidBool()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" true false  "));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            Token token = tokens[0];
            Assert.AreEqual(TokenType.TRUE, token.tokenType);
            token = tokens[1];
            Assert.AreEqual(TokenType.FALSE, token.tokenType);
            Assert.IsNotEmpty(token.ToString());
        }

        [Test]
        public void TestValidString()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" \"abc\" "));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            Token token = tokens[0];
            Assert.AreEqual(TokenType.STR, token.tokenType);
            Assert.AreEqual("abc", token.textValue);
        }

        [Test]
        public void TestValidString2()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" \"abc\\\\\\\" \""));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            Token token = tokens[0];
            Assert.AreEqual(TokenType.STR, token.tokenType);
            Assert.AreEqual("abc\\\" ", token.textValue);
        }

        [Test]
        public void TestIdentifier()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" abc   "));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            Token token = tokens[0];
            Assert.AreEqual(TokenType.IDENTIFIER, token.tokenType);
        }

        [Test]
        public void TestKeyword() {
            SourceCode source = new SourceCode(stringToStreamReader(" AND OR NOT if else while return true false int str float bool Turtle __  "));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            int i = 0;
            Assert.AreEqual(TokenType.AND, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.OR, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.NOT, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IF, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.ELSE, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.WHILE, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.RETURN, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.TRUE, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.FALSE, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.INT_T, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.STR_T, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.FLOAT_T, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.BOOL_T, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.TURTLE, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.UU, tokens[i++].tokenType);
        }

        [Test]
        public void TestIf() {
            SourceCode source = new SourceCode(stringToStreamReader(" if a==b {" + newline +
                "a = a+1" + newline +
                "} else { " + newline +
                "b = b+1.5" + newline +
                "}   "));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            int i = 0;
            Assert.AreEqual(TokenType.IF, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.EQEQ, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.LCURLY, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.NL, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.EQ, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.PLUS, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.INT, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.NL, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.RCURLY, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.ELSE, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.LCURLY, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.NL, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.EQ, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.PLUS, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.FLOAT, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.NL, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.RCURLY, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.EOF, tokens[i++].tokenType);
        }

        [Test]
        public void TestLoop()
        {
            SourceCode source = new SourceCode(stringToStreamReader(" while a<10 {" + newline +
                "a = a+1" + newline +
                "}   "));
            Lexer lexer = new Lexer(source);
            List<Token> tokens = getAllTokens(lexer);
            int i = 0;
            Assert.AreEqual(TokenType.WHILE, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.LT, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.INT, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.LCURLY, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.NL, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.EQ, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.PLUS, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.INT, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.NL, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.RCURLY, tokens[i++].tokenType);
            Assert.AreEqual(TokenType.EOF, tokens[i++].tokenType);
        }
    }
}