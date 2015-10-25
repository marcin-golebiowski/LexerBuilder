﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexerBuilder.LexerBuilder;

namespace LexerBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = "println(\"Hello, World!\");";
            Lexer myLexer = new Lexer(code);

            myLexer.SetBinding(";", TokenType.SemiColon);
            myLexer.SetBinding("(", TokenType.RightParentheses);
            myLexer.SetBinding(")", TokenType.LeftParentheses);
            myLexer.ScanFrom("\"", "\"", TokenType.String);

            foreach (Token token in myLexer.Scan())
                Console.WriteLine(token.TokenType + "\t" + token.Value);

            Console.Read();
        }
    }
}
