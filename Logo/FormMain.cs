using Logo.Core;
using Logo.Core.Utils;
using Logo.Core.Utils.Grammar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            try
            {
                string code = "add(a: int, b: int)\n" +
                    "{\n" +
                    "   return a+b\n" +
                    "}\n";
                Lexer lexer = new Lexer(new SourceCode(code, false));
                Parser parser = new Parser(lexer);

                Dictionary<string, FunctionStatement> result = parser.parse();
                Console.WriteLine("Length: " + result.Count);
                foreach (KeyValuePair<string, FunctionStatement> pair in result)
                {
                    Console.Write("Key: " + pair.Key + ", value: " + pair.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            foreach (ErrorHandling.LogoException exc in ErrorHandling.exceptions)
            {
                Console.WriteLine(exc.toString());
            };
        }
    }
}
