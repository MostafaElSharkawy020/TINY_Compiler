using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;

        List<Node> Function_Statement_List;
        List<Node> Parameter_List_List;
        //List<Node> Statements_List;
        List<Node> Condition_Statement_List;
        List<Node> Declaration_L_List;
        List<Node> Arithmetic_Operation_List;
        List<Node> Function_Call_List;
        List<Node> Identifier_L_List;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;

            Function_Statement_List = new List<Node>();
            Parameter_List_List = new List<Node>();
            //Statements_List = new List<Node>();
            Condition_Statement_List = new List<Node>();
            Declaration_L_List = new List<Node>();
            Arithmetic_Operation_List = new List<Node>();
            Function_Call_List = new List<Node>();
            Identifier_L_List = new List<Node>();

            this.TokenStream = TokenStream;
            root = Program();

            return root;
        }
        Node Program() 
        {
            Node program = new Node("Program");

            Function_Statement();
            program.Children.AddRange(Function_Statement_List);
            Function_Statement_List.Clear();

            program.Children.Add(Main_Function());

            MessageBox.Show("Success");
            return program;
        }

        Node Main_Function()
        {
            Node mainFunction = new Node("Main_Function");
            mainFunction.Children.Add(Datatype());
            mainFunction.Children.Add(match(Token_Class.Main));
            mainFunction.Children.Add(match(Token_Class.LParanthesis));
            mainFunction.Children.Add(match(Token_Class.RParanthesis));
            mainFunction.Children.Add(Function_Body());
            return mainFunction;
        }

        Node Function_Statement()
        {
            if ((IsvalidToken(Token_Class.Int)|| IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String)) && !IsvalidToken(Token_Class.Main,1))
            {
                Node functionStatement = new Node("Function_Statement");
                functionStatement.Children.Add(Function_Declaration());
                functionStatement.Children.Add(Function_Body());
                functionStatement.Children.Add(Function_Declaration());

                Function_Statement_List.Add(functionStatement);
                Function_Statement();

                return functionStatement;
            }

                return null;
        }

        Node Function_Body()
        {
            Node functionBody = new Node("Function_Body");
            List<Node> Statements_List = new List<Node>();
            functionBody.Children.Add(match(Token_Class.LeftBracesOp));
            functionBody.Children.Add(Statements(Statements_List));
            functionBody.Children.Add(Return_Statement());
            functionBody.Children.Add(match(Token_Class.RightBracesOp));
            return functionBody;
        }

        Node Function_Declaration()
        {
            Node functionDeclaration = new Node("Function_Declaration");
            functionDeclaration.Children.Add(Datatype());
            functionDeclaration.Children.Add(FunctionName());
            functionDeclaration.Children.Add(match(Token_Class.LParanthesis));
            functionDeclaration.Children.Add(Function_Declaration_Dash());
            functionDeclaration.Children.Add(match(Token_Class.RParanthesis));

            return functionDeclaration;
        }

        Node Function_Declaration_Dash()
        {
            if(IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
            {
                return Parameter_List();
            }
            return null;
        }

        Node Parameter_List()
        {
            Node parameterList = new Node("Parameter_List");
            Parameter_List_List.Clear();

            parameterList.Children.Add(Parameter());
            if (Parameter_List_Dash())
                parameterList.Children.AddRange(Parameter_List_List);

            return parameterList;
        }

        bool Parameter_List_Dash() 
        { 
            if(IsvalidToken(Token_Class.Comma))
            {
                Parameter_List_List.Add(match(Token_Class.Comma));
                Parameter_List_List.Add(Parameter());
                Parameter_List_Dash();
                return true;
            }
            return false;
        }

        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Idenifier));
            return parameter;
        }

        Node FunctionName()
        {
            Node functionName = new Node("FunctionName");
            functionName.Children.Add(match(Token_Class.Idenifier));
            return functionName;
        }

        Node Statements(List<Node> Statements_List) 
        {
            Node statements = new Node("Statements");
            //Statements_List.Clear();

            statements.Children.Add(Statement());
            
            if (Statements_Dash(Statements_List))
                statements.Children.AddRange(Statements_List);
            
            return statements;
        }

        bool Statements_Dash(List<Node> Statements_List)
        {
            if(IsvalidToken(Token_Class.Repeat) || IsvalidToken(Token_Class.If)
                ||IsvalidToken(Token_Class.Read)|| IsvalidToken(Token_Class.Write)
                    || IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String)
                    || IsvalidToken(Token_Class.Idenifier))
            {
                Statements_List.Add(Statement());
                Statements_Dash(Statements_List);
                return true;
            }

            return false;
        }

        Node Statement()
        {
           Node statement = new Node("Statement");

            if (IsvalidToken(Token_Class.Repeat))
                statement.Children.Add(Repeat_Statement());
            else if (IsvalidToken(Token_Class.If))
                statement.Children.Add(If_Statement());
            else if (IsvalidToken(Token_Class.Read))
                statement.Children.Add(Read_Statement());
            else if (IsvalidToken(Token_Class.Write))
                statement.Children.Add(Write_Statement());
            else if (IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
                statement.Children.Add(Declaration_Statement());
            else statement.Children.Add(Assigment_Statement());

            return statement;
        }

        Node Repeat_Statement()
        {
            Node repeatStatement = new Node("Repeat_Statement");
            List <Node> Statements_List = new List<Node>();

            repeatStatement.Children.Add(match(Token_Class.Repeat));
            repeatStatement.Children.Add(Statements(Statements_List));
            repeatStatement.Children.Add(match(Token_Class.Until));
            repeatStatement.Children.Add(Condition_Statement());
            return repeatStatement;
        }

        Node If_Statement()
        {
            Node conditionStatement = new Node("If_Statement");
            List<Node> Statements_List = new List<Node>();
            conditionStatement.Children.Add(match(Token_Class.If));
            conditionStatement.Children.Add(Condition_Statement());
            conditionStatement.Children.Add(match(Token_Class.Then));
            conditionStatement.Children.Add(Statements(Statements_List));
            conditionStatement.Children.Add(Elseif_Else_Statement());
            return conditionStatement;
        }

        Node Elseif_Else_Statement()
        {
            Node elseifElseStatement = new Node("Elseif_Else_Statement");
            if(IsvalidToken(Token_Class.End))
            {
                elseifElseStatement.Children.Add(match(Token_Class.End));
            }
            else if (IsvalidToken(Token_Class.ElseIf))
            {
                elseifElseStatement.Children.Add(Else_If_Statement());
            }
            else
            {
                elseifElseStatement.Children.Add(Else_Statement());
            }
            return elseifElseStatement;
        }

        Node Else_Statement()
        {
            Node elseStatement = new Node("Else_Statement");
            List<Node> Statements_List = new List<Node>();
            elseStatement.Children.Add(match(Token_Class.Else));
            elseStatement.Children.Add(Statements(Statements_List));
            elseStatement.Children.Add(match(Token_Class.End));
            return elseStatement;
        }

        Node Else_If_Statement()
        {
            Node elseIfStatement = new Node("Else_If_Statement");
            List<Node> Statements_List = new List<Node>();
            elseIfStatement.Children.Add(match(Token_Class.ElseIf));
            elseIfStatement.Children.Add(Condition_Statement());
            elseIfStatement.Children.Add(match(Token_Class.Then));
            elseIfStatement.Children.Add(Statements(Statements_List));
            elseIfStatement.Children.Add(Elseif_Else_Statement());
            return elseIfStatement;
        }

        Node Condition_Statement()
        {
            Node conditionStatement = new Node("Condition_Statement");
            Condition_Statement_List.Clear();

            conditionStatement.Children.Add(Condition());

            if (Condition_Statement_Dash())
                conditionStatement.Children.AddRange(Condition_Statement_List);

            return conditionStatement;
        }

        bool Condition_Statement_Dash()
        {
            if (IsvalidToken(Token_Class.AndOp) || IsvalidToken(Token_Class.OrOp))
            {
                Condition_Statement_List.Add(Boolean_Operator());
                Condition_Statement_List.Add(Condition());
                Condition_Statement_Dash();
                return true;
            }

            return false;
        }

        Node Boolean_Operator()
        {
            Node booleanOperator = new Node("Boolean_Operator");

            if (InputPointer < TokenStream.Count)
            {
                if (Token_Class.AndOp == TokenStream[InputPointer].token_type)
                {
                    booleanOperator.Children.Add(match(Token_Class.AndOp));
                }
                else
                {
                    booleanOperator.Children.Add(match(Token_Class.OrOp));
                }    
            }

            return booleanOperator;
        }

        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition;
        }

        Node Condition_Operator()
        {
            Node conditionOperator = new Node("Condition_Operator");

            if (InputPointer < TokenStream.Count)
            {
                if (Token_Class.LessThanOp == TokenStream[InputPointer].token_type)
                {
                    conditionOperator.Children.Add(match(Token_Class.LessThanOp));
                }
                else if (Token_Class.GreaterThanOp == TokenStream[InputPointer].token_type)
                {
                    conditionOperator.Children.Add(match(Token_Class.GreaterThanOp));
                }
                else if (Token_Class.EqualOp == TokenStream[InputPointer].token_type)
                {
                    conditionOperator.Children.Add(match(Token_Class.EqualOp));
                }
                else
                {
                    conditionOperator.Children.Add(match(Token_Class.NotEqualOp));
                }
            }

            return conditionOperator;
        }

        Node Return_Statement()
        {
            Node returnStatement = new Node("Return_Statement");
            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(Expression());
            returnStatement.Children.Add(match(Token_Class.Semicolon));
            return returnStatement;
        }

        Node Read_Statement()
        {
            Node readStatement = new Node("Read_Statement");
            readStatement.Children.Add(match(Token_Class.Read));
            readStatement.Children.Add(match(Token_Class.Idenifier));
            readStatement.Children.Add(match(Token_Class.Semicolon));
            return readStatement;
        }

        Node Write_Statement()
        {
            Node writeStatement = new Node("Write_Statement");
            writeStatement.Children.Add(match(Token_Class.Write));
            writeStatement.Children.Add(Write_Expression());
            writeStatement.Children.Add(match(Token_Class.Semicolon));
            return writeStatement;
        }

        Node Write_Expression()
        {
            Node writeExpression = new Node("Write_Expression");
            if(IsvalidToken(Token_Class.Endl))
            {
                writeExpression.Children.Add(match(Token_Class.Endl));
            }
            else
            {
                writeExpression.Children.Add(Expression());
            }
            return writeExpression;
        }

        Node Declaration_Statement()
        {
            Node declarationStatement = new Node("Declaration_Statement");
            declarationStatement.Children.Add(Datatype());
            declarationStatement.Children.Add(Declaration_List());
            declarationStatement.Children.Add(match(Token_Class.Semicolon));
            return declarationStatement;
        }

        Node Declaration_List()
        {
            Node declarationList = new Node("Declaration_List");
            Declaration_L_List.Clear();

            declarationList.Children.Add(Declaration_And_Assigment());

            if (Declaration_List_Dash())
                declarationList.Children.AddRange(Declaration_L_List);

            return declarationList;
        }

        bool Declaration_List_Dash()
        {
            if (IsvalidToken(Token_Class.Comma))
            {
               Declaration_L_List.Add(match(Token_Class.Comma));
                Declaration_L_List.Add(Declaration_And_Assigment());
                Declaration_List_Dash();
                return true;
            }

            return false;
        }

        Node Declaration_And_Assigment()
        {
            Node declarationAndAssigment = new Node("Declaration_And_Assigment");
            if (IsvalidToken(Token_Class.Idenifier) && IsvalidToken(Token_Class.AssigmentOp,1))
            {
                declarationAndAssigment.Children.Add(Assigment_In_Declaration());
            }
            else
            {
                declarationAndAssigment.Children.Add(match(Token_Class.Idenifier));
            }
            return declarationAndAssigment;
        }

        Node Assigment_In_Declaration()
        {
            Node assigmentStatement = new Node("Assigment_In_Declaration");
            assigmentStatement.Children.Add(match(Token_Class.Idenifier));
            assigmentStatement.Children.Add(match(Token_Class.AssigmentOp));
            assigmentStatement.Children.Add(Expression());
            return assigmentStatement;
        }

        Node Datatype()
        {
            Node datatype = new Node("Datatype");
            if (InputPointer < TokenStream.Count)
            {
                if (Token_Class.Int == TokenStream[InputPointer].token_type)
                {
                    datatype.Children.Add(match(Token_Class.Int));
                }
                else if (Token_Class.Float == TokenStream[InputPointer].token_type)
                {
                    datatype.Children.Add(match(Token_Class.Float));
                }
                else
                {
                    datatype.Children.Add(match(Token_Class.String));
                }
            }

                return datatype;
        }

        Node Assigment_Statement()
        {
            Node assigmentStatement = new Node("Assigment_Statement");
            assigmentStatement.Children.Add(match(Token_Class.Idenifier));
            assigmentStatement.Children.Add(match(Token_Class.AssigmentOp));
            assigmentStatement.Children.Add(Expression());
            assigmentStatement.Children.Add(match(Token_Class.Semicolon));
            return assigmentStatement;
        }

        Node Expression()
        {
            Node expression = new Node("Expression");
            if (IsvalidToken(Token_Class.Str))
            {
                expression.Children.Add(match(Token_Class.Str));
            }
            else if (IsvalidToken(Token_Class.Number) || IsvalidToken(Token_Class.Idenifier))
            {
                if (IsvalidToken(Token_Class.PlusOp, 1) || IsvalidToken(Token_Class.MinusOp, 1) ||
                    IsvalidToken(Token_Class.MultiplyOp, 1) || IsvalidToken(Token_Class.DivideOp, 1))
                {
                    expression.Children.Add(Equation());
                }
                else
                {
                    expression.Children.Add(Term());
                }
            }
            else
            {
                expression.Children.Add(Equation());
            }
            return expression;
        }

        Node Equation()
        {
            Node equation = new Node("Equation");
            if (IsvalidToken(Token_Class.LParanthesis))
            {
                equation.Children.Add(Bracket_Equation());
            }
            else
            {
                equation.Children.Add(Arethemtic_Operation());
            }
            return equation;
        }
        

        Node Bracket_Equation()
        {
            Node bracketEquation = new Node("Bracket_Equation");
            bracketEquation.Children.Add(match(Token_Class.LParanthesis));
            bracketEquation.Children.Add(Arethemtic_Operation());
            bracketEquation.Children.Add(match(Token_Class.RParanthesis));
            return bracketEquation;
        }

        Node Arethemtic_Operation()
        {
            Node arethemticOperation = new Node("Arethemtic_Operation");
            Arithmetic_Operation_List.Clear();

            arethemticOperation.Children.Add(Term_Or_Bracket());

            if (Arethemtic_Operation_Dash())
                arethemticOperation.Children.AddRange(Arithmetic_Operation_List);

            return arethemticOperation;
        }

        bool Arethemtic_Operation_Dash()
        {
            if (IsvalidToken(Token_Class.PlusOp) || IsvalidToken(Token_Class.MinusOp) || IsvalidToken(Token_Class.MultiplyOp) || IsvalidToken(Token_Class.DivideOp))
            {
                Arithmetic_Operation_List.Add(Arethemtic_Operator());
                Arithmetic_Operation_List.Add(Term_Or_Bracket());
                Arethemtic_Operation_Dash();
                return true;
            }

            return false;
        }

        Node Term_Or_Bracket()
        {
            Node termOrBracket = new Node("Term_Or_Bracket");
            if (IsvalidToken(Token_Class.Number) || IsvalidToken(Token_Class.Idenifier))
            {
                termOrBracket.Children.Add(Term());
            }
            else
            {
                termOrBracket.Children.Add(Bracket_Equation());
            }
            return termOrBracket;
        }

        Node Arethemtic_Operator()
        {
            Node arethemticOperator = new Node("Arethemtic_Operator");

            if (InputPointer < TokenStream.Count)
            {
                if (Token_Class.PlusOp == TokenStream[InputPointer].token_type)
                {
                    arethemticOperator.Children.Add(match(Token_Class.PlusOp));
                }
                else if (Token_Class.MinusOp == TokenStream[InputPointer].token_type)
                {
                    arethemticOperator.Children.Add(match(Token_Class.MinusOp));
                }
                else if (Token_Class.MultiplyOp == TokenStream[InputPointer].token_type)
                {
                    arethemticOperator.Children.Add(match(Token_Class.MultiplyOp));
                }
                else
                {
                    arethemticOperator.Children.Add(match(Token_Class.DivideOp));
                }
            }

            return arethemticOperator;
        }

        Node Term()
        {
            Node term = new Node("Term");

            if (InputPointer < TokenStream.Count)
            {
                if (Token_Class.Number == TokenStream[InputPointer].token_type)
                {
                    term.Children.Add(match(Token_Class.Number));
                }
                else if (Token_Class.Idenifier == TokenStream[InputPointer].token_type && !IsvalidToken(Token_Class.LParanthesis,1))
                {
                    term.Children.Add(match(Token_Class.Idenifier));
                }
                else
                {
                    term.Children.Add(Function_Call());
                }
            }

            return term;
        }

        Node Function_Call()
        {
            Node functionCall = new Node("Function_Call");
            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(match(Token_Class.LParanthesis));
            functionCall.Children.Add(Function_Call_Dash());
            functionCall.Children.Add(match(Token_Class.RParanthesis));

            return functionCall;
        }

        Node Function_Call_Dash()
        {
            if (IsvalidToken(Token_Class.Idenifier))
                return match(Token_Class.Idenifier);
            return null;
        }

        Node Identifier_List()
        {
            Node identifierList = new Node("Identifier_List");
            Identifier_L_List.Clear();

            identifierList.Children.Add(match(Token_Class.Idenifier));

            if (Identifier_List_Dash())
                identifierList.Children.AddRange(Identifier_L_List);

            return identifierList;
        }

        bool Identifier_List_Dash()
        {
            if (IsvalidToken(Token_Class.Comma))
            {
                Identifier_L_List.Add(match(Token_Class.Comma));
                Identifier_L_List.Add(match(Token_Class.Idenifier));
                Identifier_List_Dash();
                return true;
            }

            return false;
        }

        bool IsvalidToken(Token_Class token ,int increment = 0)
        {
            return (InputPointer + increment < TokenStream.Count && TokenStream[InputPointer + increment].token_type == token);
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
