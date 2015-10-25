﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexerBuilder.LexerBuilder
{
    public class Lexer
    {
        public string SingleLineComment { get; set; }

        private Dictionary<string, TokenType> bindings = new Dictionary<string, TokenType>();
        private Dictionary<string, Tuple<string, TokenType>> scanFrom = new Dictionary<string, Tuple<string, TokenType>>();
        private List<string> ignoreChar = new List<string>();
        private Dictionary<string, string> ignoreFrom = new Dictionary<string, string>();

        private string code;
        private int position;
        private List<Token> result;

        private char peekLetter { get { return (char)peekChar(); } }
        private char readLetter { get { return (char)readChar(); } }

        public Lexer()
        {
        }

        public List<Token> Scan(string code)
        {
            this.code = code;
            position = 0;
            result = new List<Token>();

            whiteSpace();

            while (peekChar() != -1)
            {
                if (char.IsLetterOrDigit(peekLetter))
                    result.Add(scanData());
                else if (bindings.ContainsKey(peekLetter.ToString()))
                    result.Add(new Token(bindings[peekLetter.ToString()], readLetter.ToString()));
                else if (scanFrom.ContainsKey(peekLetter.ToString()))
                {
                    Tuple<string, TokenType> entry = scanFrom[peekLetter.ToString()];
                    result.Add(scanDataFrom(peekLetter.ToString(), entry.Item1, entry.Item2));
                }
                else if (ignoreChar.Contains(peekLetter.ToString()))
                    readChar();
                else if (ignoreFrom.ContainsKey(peekLetter.ToString()))
                    ignoreDataFrom(ignoreFrom[peekLetter.ToString()]);
                else
                    Console.WriteLine("Unknown Token " + readLetter.ToString());

                whiteSpace();
            }

            return result;
        }

        public void SetBinding(string to, TokenType with)
        {
            bindings.Add(to, with);
        }

        public void ScanFrom(string from, string to, TokenType tokenType)
        {
            scanFrom.Add(from, Tuple.Create(to, tokenType));
        }

        public void IgnoreChar(string character)
        {
            ignoreChar.Add(character);
        }

        public void IgnoreFrom(string from, string to)
        {
            ignoreFrom.Add(from, to);
        }

        private Token scanData()
        {
            string result = "";
            double temp = 0;
            while ((char.IsLetterOrDigit((char)peekChar()) && peekChar() != -1) || ((char)(peekChar()) == '.'))
                result += ((char)readChar()).ToString();
            if (double.TryParse(result, out temp))
                return new Token(TokenType.Number, result);

            return new Token(TokenType.Identifier, result);
        }

        private Token scanDataFrom(string from, string to, TokenType resultType)
        {
            readChar();
            string res = "";
            while (peekLetter.ToString() != to && peekChar() != -1)
                res += readLetter.ToString();

            readChar();

            return new Token(resultType, res);
        }

        private void ignoreDataFrom(string to)
        {
            readChar();
            while (peekLetter.ToString() != to && peekChar() != -1)
                readChar();
            readChar();
        }

        private void whiteSpace()
        {
            while (char.IsLetterOrDigit((char)peekChar()))
                readChar();
        }

        private int peekChar(int n = 0)
        {
            if (position + n < code.Length)
                return code[position];
            return -1;
        }

        private int readChar()
        {
            if (position < code.Length)
                return code[position++];
            return -1;
        }
    }
}