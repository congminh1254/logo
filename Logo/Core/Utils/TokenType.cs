﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public enum TokenType
    {
       AND,
       OR,
       NOT,
       EQ,
       DOT,
       COMMA,
       LPAREN,
       RPAREN,
       LCURLY,
       RCURLY,
       COLON,
       HASH,
       UND,
       UU,
       IF,
       ELSE,
       WHILE,
       TRUE,
       FALSE,
       RETURN,
       PLUS,
       MINUS,
       MUL,
       DIV,
       MOD,
       LT,
       LE,
       GT,
       GE,
       EQEQ,
       DIFF,
       INT,
       STR,
       FLOAT,
       COLOR,
       INT_T,
       FLOAT_T,
       STR_T,
       BOOL_T,
       COLOR_T,
       TURTLE,
       COORDINATE_T,
       EOF,
       NL,
       ERROR,
       IDENTIFIER,
       NULL,
       COPYOF,
       COMMENT
    }
}