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
            string code = "main() {\r\n     if a<b AND c==d OR NOT (a > 0 OR a > 3) {\r\n    return 0\r\n    }\r\n    \r\n    }";
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            Parser parser = new Parser(lexer);

            var result = parser.parse();
        }
    }
}
