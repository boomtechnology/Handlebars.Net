﻿using System;
using System.IO;
using System.Linq;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class WordParser : Parser
    {
        private const string validWordStartCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$.@";

        public override Token Parse(TextReader reader)
        {
            if (IsWord(reader))
            {
                var buffer = AccumulateWord(reader);

                if (buffer.Contains("="))
                {
                    return Token.HashParameter(buffer);
                }
                else
                {
                    return Token.Word(buffer);
                }
            }
            return null;
        }

        private bool IsWord(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return validWordStartCharacters.Contains(peek.ToString());
        }

        private string AccumulateWord(TextReader reader)
        {
            StringBuilder buffer = new StringBuilder();

            var inString = false;

            while (true)
            {
                if (!inString)
                {
                    var peek = (char)reader.Peek();

                    if (peek == '}' || peek == '~' || peek == ')' || char.IsWhiteSpace(peek))
                    {
                        break;
                    }
                }

                var node = reader.Read();

                if (node == -1)
                {
                    throw new InvalidOperationException("Reached end of template before the expression was closed.");
                }

                if (node == '\'' || node == '"')
                {
                    inString = !inString;
                }

                buffer.Append((char)node);
            }
            return buffer.ToString();
        }
    }
}

