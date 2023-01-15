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
            ////string filename = "./ExampleCode/Example_02.txt";
            ////using (var reader = new StreamReader(filename))
            ////{
            ////    SourceCode source = new SourceCode(reader);
            ////    Lexer lexer = new Lexer(source);
            ////    Parser parser = new Parser(lexer);
            ////    var function = parser.parse();
            ////}
            //string code = 
            //    "#__300_300__\r\n" +
            //    "main() {\r\n" +
            //    "turtle = Turtle()\r\n" +
            //    "turtle.Pen.Enable = true\r\n" +
            //    "a = turtle.Pen.Enable\r\n" +
            //    "a = turtle.Write(\"124\")\r\n" +
            //    "return turtle.Pen.Enable\r\n"+
            //    "}\r\n";
            //Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            ////while (true)
            ////{
            ////    Token token = lexer.advanceToken();
            ////    Console.WriteLine(token);
            ////    if (token.tokenType == TokenType.EOF)
            ////    {
            ////        break;
            ////    }

            ////}
            //Parser parser = new Parser(lexer);
            //var result = parser.parse();

            //foreach (var item in result)
            //{
            //    Console.WriteLine(item);
            //}
            //Interpreter interpreter = new Interpreter();
            //Console.WriteLine(interpreter.Run(result, parser.header));
            //foreach (var ex in ErrorHandling.exceptions)
            //{
            //    Console.WriteLine(ex.message);
            //}
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Open a Logo file";
            openFileDialog.DefaultExt = "logo";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = openFileDialog.FileName;
                string text = File.ReadAllText(filename);
                rtbCode.Text = text;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            ErrorHandling.clear();
            string code = rtbCode.Text;
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            Parser parser = new Parser(lexer);
            var result = parser.parse();

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
            Interpreter interpreter = new Interpreter();
            Console.WriteLine(interpreter.Run(result, parser.header));
            foreach (var ex in ErrorHandling.exceptions)
            {
                Console.WriteLine(ex.message);
            }
            if (interpreter.result != null)
            {
                pbResult.Image = interpreter.result;
                Console.WriteLine("Image loaded!");
            }

            if (ErrorHandling.exceptions.Count > 0)
            {
                MessageBox.Show("One more more errors happened on runtime.\nCheck error list for details!", "Error", MessageBoxButtons.OK);
                foreach (var ex in ErrorHandling.exceptions)
                {
                    string[] msg = new string[2] { ex.message, (ex.position != null ? ex.position.ToString() : "") };
                    listError.Items.Add(new ListViewItem(msg));
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.Title = "Save to Logo file";
            saveFileDialog.DefaultExt = "logo";
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                File.WriteAllText(filename, rtbCode.Text);
            }
        }

        private void timerLexer_Tick(object sender, EventArgs e)
        {
            
            
        }

        private void rtbCode_TextChanged(object sender, EventArgs e)
        {
            ErrorHandling.clear();
            string code = rtbCode.Text;
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            Parser parser = new Parser(lexer);
            var result = parser.parse();

            listError.Items.Clear();
            Console.WriteLine(ErrorHandling.exceptions.Count);
            foreach (var ex in ErrorHandling.exceptions)
            {
                string[] msg = new string[2] { ex.message, ex.position.ToString() };
                listError.Items.Add(new ListViewItem(msg));
            }
        }
    }
}
