using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel;

public enum Token_Class
{
    End, Else, If, Read, Then, Until, Write,
    Int, Float, String, Repeat, ElseIf, Return, Endl,Main,

    Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, AssigmentOp,
    AndOp, OrOp, LeftBracesOp, RightBracesOp,

    Idenifier, Number, Str
}

namespace TINY_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("main", Token_Class.Main);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("{", Token_Class.LeftBracesOp);
            Operators.Add("}", Token_Class.RightBracesOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add(":=", Token_Class.AssigmentOp);
        }

        public void StartScanning(string SourceCode)
        {
            SourceCode += "   ";
            Tokens.Clear();
            Errors.Error_List.Clear();

            for (int i = 0; i < SourceCode.Length; i++)
            {

                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();


                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    i++;
                    CurrentChar = SourceCode[i];

                    while (true)
                    {
                        string CurrentAndNextChars = SourceCode[i].ToString() + SourceCode[i + 1].ToString();
                        if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t' || Operators.ContainsKey(CurrentChar.ToString())
                            ||Operators.ContainsKey(CurrentAndNextChars))
                        {
                            break;
                        }
                        CurrentLexeme += CurrentChar.ToString();
                        i++;
                        CurrentChar = SourceCode[i];

                        
                    }

                    FindTokenClass(CurrentLexeme);
                    i--;
                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    i++;
                    CurrentChar = SourceCode[i];
                    

                    while (true)
                    {
                        
                        if (i + 1 == SourceCode.Length)
                        {
                            break;
                        }

                        string CurrentAndNextChars = SourceCode[i].ToString() + SourceCode[i + 1].ToString();

                        if(CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t' || Operators.ContainsKey(CurrentChar.ToString())
                            || Operators.ContainsKey(CurrentAndNextChars))
                            { break; }

                        CurrentLexeme += CurrentChar.ToString();
                        i++;
                        CurrentChar = SourceCode[i];

                        

                    }

                    FindTokenClass(CurrentLexeme);
                    i--;
                }



                else if (CurrentChar == '/')
                {
                    char NextChar = SourceCode[i + 1];
                    if (NextChar == '*')
                    {
                        do
                        {
                            i++;

                            if (i + 1 == SourceCode.Length)
                            {
                                Errors.Error_List.Add("Unclosed Comment!");
                                break;
                            }

                            CurrentChar = SourceCode[i];
                            NextChar = SourceCode[i + 1];

                        } while (CurrentChar != '*' || NextChar != '/');

                        i++;
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }

                }
                else if ((CurrentChar == '<' && SourceCode[i + 1] == '>') || (CurrentChar == '&' && SourceCode[i + 1] == '&')
                    || (CurrentChar == ':' && SourceCode[i + 1] == '=') || (CurrentChar == '|' && SourceCode[i + 1] == '|'))
                {
                    CurrentLexeme += SourceCode[i + 1].ToString();
                    FindTokenClass(CurrentLexeme);
                    i++;
                }
                else if(CurrentChar == '\"')
                {
                    i++;
                    CurrentChar = SourceCode[i];
                    bool isStr = true;

                    while (CurrentChar != '\"')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        i++;
                        CurrentChar = SourceCode[i];

                        if(CurrentChar == '\n')
                        {
                            isStr = false;
                            break;
                        }
                    }

                    if (isStr)
                    {
                        CurrentLexeme += '\"';
                        Token Tok = new Token();
                        Tok.lex = CurrentLexeme;
                        Tok.token_type = Token_Class.Str;
                        Tokens.Add(Tok);

                    }
                    else
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    
                }
                else
                {
                    FindTokenClass(CurrentLexeme);
                }
            }

            TINY_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {

            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }
            //Is it a Constant?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
            }

        }



        bool isIdentifier(string lex)
        {
            // Check if the lex is an identifier or not.
            Regex regex = new Regex(@"^[A-Za-z]([A-Za-z0-9]*)$", RegexOptions.Compiled);

            return regex.IsMatch(lex);
        }
        bool isNumber(string lex)
        {
            // Check if the lex is a constant (Number) or not.
            Regex regex = new Regex(@"^\d+(\.(\d+))?$", RegexOptions.Compiled);

            return regex.IsMatch(lex);
        }
    }
}
