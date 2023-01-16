using Logo.Core;
using Logo.Core.Utils;
using Logo.Core.Utils.Grammar;
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
            string code = rtbCode.Text.Replace("\n", "\r\n").Trim();
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            Parser parser = new Parser(lexer);
            var result = parser.parse();
            Header header = parser.header;
            if (header == null)
                header = new Header(500, 500);
            Interpreter interpreter = new Interpreter();

            object returnedValue = null;
            try
            {
                returnedValue = interpreter.Run(result, header);
        }
            catch (Exception ex)
            {
                MessageBox.Show("One more more errors happened on runtime.\nCheck error list for details!\n"+ex.Message, "Error", MessageBoxButtons.OK);
            }
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
            string code = rtbCode.Text.Replace("\n", "\r\n").Trim();
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            Parser parser = new Parser(lexer);
            var result = parser.parse();
            Console.WriteLine(result);
            listError.Items.Clear();
            Console.WriteLine(ErrorHandling.exceptions.Count);
            foreach (var ex in ErrorHandling.exceptions)
            {
                string[] msg = new string[2] { ex.message, (ex.position != null ? ex.position.ToString() : "") };
                listError.Items.Add(new ListViewItem(msg));
            }
        }
    }
}
