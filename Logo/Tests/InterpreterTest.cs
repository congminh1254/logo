using Logo.Core;
using Logo.Core.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo
{
    [TestFixture]
    public class InterpreterTest
    {
        public Object setup(string code, bool isMainBody)
        {
            if (isMainBody)
            {
                code = "main() {\r\n" + code + "\r\n}\r\n";
            }
            SourceCode source = new SourceCode(Utils.stringToStreamReader(code));
            Lexer lexer = new Lexer(source);
            Parser parser = new Parser(lexer);

            var functions = parser.parse();
            Object returned = Interpreter.Run(functions);
            return returned;
        }

        [Test]
        public void TestReturnInt()
        {
            string body = "return 1";
            Assert.AreEqual(setup(body, true), 1);
        }

        [Test]
        public void TestReturnFloat() {
            string body = "return 1.5";
            Assert.AreEqual(setup(body, true), 1.5);
        }

        [Test]
        public void TestReturnString() {
            string body = "return \"hello\"";
            Assert.AreEqual(setup(body, true), "hello");
        }

        [Test]
        public void TestVariable()
        {
            string body = "a = 5\r\nb=6\r\nreturn a+b";
            Assert.AreEqual(setup(body, true), 11);
        }

        [Test]
        public void TestVariable2()
        {
            string body = "a = 5\r\nb=0.5\r\nreturn a*b";
            Assert.AreEqual(setup(body, true), 2.5);
        }

        [Test]
        public void TestVariable3()
        {
            string body = "a = 5.5\r\nb=5\r\nreturn a/b";
            float result = (float)setup(body, true);
            Assert.That(result, Is.EqualTo(1.1).Within(.0001));
        }

        [Test]
        public void TestVariable4()
        {
            string body = "a = 3.5\r\nb=1.5\r\nreturn a-b";
            Assert.AreEqual(setup(body, true), 2);
        }

        [Test]
        public void TestVariable5()
        {
            string body = "a = \"abc\"\r\nb=\"def\"\r\nreturn a+b";
            Assert.AreEqual(setup(body, true), "abcdef");
        }

        [Test]
        public void TestVariable6()
        {
            string body = "a = \"Result is: \"\r\nb=5.5\r\nreturn a+b";
            Assert.AreEqual(setup(body, true), "Result is: 5.5");
        }

        [Test]
        public void TestIf1()
        {
            string body = "a = 5\r\nif a>4 {\r\nreturn true \r\n } \r\nelse { \r\n return false \r\n }";
            Assert.AreEqual(setup(body, true), true);
        }

        [Test]
        public void TestIf2()
        {
            string body = "a = 5\r\nif a>10 {\r\nreturn true \r\n } \r\nelse { \r\n return false \r\n }";
            Assert.AreEqual(setup(body, true), false);
        }

        [Test]
        public void TestIf3()
        {
            string body = "a = 5\r\nif a>3 AND a<6 {\r\nreturn true \r\n } \r\nelse { \r\n return false \r\n }";
            Assert.AreEqual(setup(body, true), true);
        }

        [Test]
        public void TestIf4()
        {
            string body = "a = 5\r\nif a>3 AND a<4 {\r\nreturn true \r\n } \r\nelse { \r\n return false \r\n }";
            Assert.AreEqual(setup(body, true), false);
        }

        [Test]
        public void TestIf5()
        {
            string body = "a = 5\r\nif a>3 OR a<4 {\r\nreturn true \r\n } \r\nelse { \r\n return false \r\n }";
            Assert.AreEqual(setup(body, true), true);
        }

        [Test]
        public void TestIf6()
        {
            string body = "a = 5\r\nif a>6 OR a>8 {\r\nreturn true \r\n } \r\nelse { \r\n return false \r\n }";
            Assert.AreEqual(setup(body, true), false);
        }

        [Test]
        public void TestWhile1()
        {
            string body = "a = 5\r\nwhile a > 1 {\r\na=a-1 \r\n}\r\nreturn a";
            Assert.AreEqual(setup(body, true), 1);
        }

        [Test]
        public void TestWhile2()
        {
            string body = "a = 5\r\nwhile a > 5 {\r\na=a-1 \r\n}\r\nreturn a";
            Assert.AreEqual(setup(body, true), 5);
        }

        [Test]
        public void TestFunction1()
        {
            string body = "func(x: int, y: int) {\r\n" +
                "return x+y\r\n" +
                "}\r\n" +
                "main() {\r\n" +
                "a=5\r\n" +
                "b=6\r\n" +
                "c=func(a, b)\r\n" +
                "return c" +
                "}\r\n";
            Assert.AreEqual(setup(body, false), 11);
        }

        [Test]
        public void TestTurtle1()
        {

        }
    }
}
