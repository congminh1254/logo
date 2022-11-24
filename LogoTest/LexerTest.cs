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
    }
}