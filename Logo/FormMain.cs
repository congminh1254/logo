using Logo.Core;
using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //string filename = "./ExampleCode/Example_02.txt";
            //using (var reader = new StreamReader(filename))
            //{
            //    SourceCode source = new SourceCode(reader);
            //    Lexer lexer = new Lexer(source);
            //    Parser parser = new Parser(lexer);
            //    var function = parser.parse();
            //}
            string code = "func(x: int, y: int) {\r\n" +
                "return x+y\r\n" +
                "}\r\n" +
                "main() {\r\n" +
                "a=5\r\n" +
                "b=6\r\n" +
                "c=func(a, b)\r\n" +
                "return c" +
                "}\r\n";
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));

            Parser parser = new Parser(lexer);
            var result = parser.parse();
            foreach(var item in result )
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(Interpreter.Run(result));
            foreach(var ex in ErrorHandling.exceptions)
            {
                Console.WriteLine(ex.message);
            }
        }
    }
}
