﻿using Logo.Core;
using Logo.Core.Utils;
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
            SourceCode source = new SourceCode("C:\\Users\\mcong\\source\\repos\\logo\\Logo\\ExampleCode\\Example_01.txt");

            Lexer lexer = new Lexer(source);
            while (true)
            {
                Token token = lexer.advanceToken();
                Console.WriteLine(token.toString());
                if (token.getTokenType() == TokenType.EOF)
                    break;
            }
        }
    }
}
