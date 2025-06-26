using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{

    // Expression: evaluates to a value.
    public abstract class Expr { }

    // Statement: an action to be performed.
    public abstract class Stmt { }

    // Literal values: number, string lit...
    public class NumberLitExpr : Expr
    {
        public double Value;
        public NumberLitExpr(double val)
        {
            Value = val;
        }

        public override string ToString() 
        {
            return $"NumberLitExpr({Value})";
        }
    }

    public class StringLitExpr : Expr
    {
        public string Value;
        public StringLitExpr(string val)
        {
            Value = val;
        }
        public override string ToString() 
        {
            return $"StringLitExpr({Value})";
        }   
    }

    // Variable: named values.
    public class VariableExpr : Expr
    {
        public string Name;
        public VariableExpr(string name)
        {
            Name = name;
        }
    }

    // Prefix Expr (operands for Pratt Parser): e.g. '-'1, '!'true, '+'a...
    public class PrefixExpr : Expr
    {
        public TokenType Operator;
        public Expr Operand;
        public PrefixExpr(TokenType type, Expr operand)
        {
            Operator = type;
            Operand = operand;
        }

        public override string ToString() 
        {
            return $"PrefixExpr({Operator}, {Operand})";
        }
    }

    // Binary operations: e.g. 1 + 2,
    // made up of a left side, an operator, & right side.
    public class BinaryExpr : Expr
    {
        public Expr Left;
        public TokenType Operator;
        public Expr Right;

        public BinaryExpr(Expr left, TokenType oper, Expr right)
        {
            Left = left;
            Operator = oper;
            Right = right;
        }
        public override string ToString() 
        {
            return $"BinaryExpr({Left}, {Operator}, {Right})";
        }   
    }

    // Declaration of variables.
    public class VarDeclr : Stmt
    {
        public string Name;
        public Expr Value;
        public VarDeclr(string name, Expr val)
        {
            Name = name;
            Value = val;
        }
    }

    // Conditional Statements.
    public class IfStmt : Stmt
    {
        public Expr Condition;
        public Stmt ThenBranch;
        public Stmt ElseBranch;

        public IfStmt(Expr condition, Stmt thenBranch, Stmt elseBranch = null)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }
    }

    // Block statements. Used for const of Scenes & Choices(?).
    //TODO ^^ clean up this comment.
    public class BlockStmt : Stmt
    {
        public List<Stmt> Statements;
        public BlockStmt(List<Stmt> statements)
        {
            Statements = statements;
        }
    }

    // Imports.
    public class ImportStmt : Stmt
    {
        public string Module;
        public string Alias; // Optional alias for the module.
        public ImportStmt(string module, string alias = null)
        {
            Module = module;
            Alias = alias;
        }
    }

    // Personal Stmt Types for repr game objects.
    // Scenes.
    public class SceneStmt : Stmt
    {
        public string Name;
        public BlockStmt Body;
        public SceneStmt(string name, BlockStmt body)
        {
            Name = name;
            Body = body;
        }
    }

    // Text.
    public class TextStmt : Stmt
    {
        public string Text;
        public TextStmt(string text)
        {
            Text = text;
        }
    }

    // Choice.
    public class ChoiceStmt : Stmt
    {
        public string Label;
        public string TargetScene;
        public BlockStmt Events;
    }

    // TODO: needs reworking probably.
    public class Event : Stmt
    {
        public string Name;
        public BlockStmt Body;
        public Event(string name, BlockStmt body)
        {
            Name = name;
            Body = body;
        }
    }

    // Base IO Events
    /// <summary>
    /// Ask stmt: used to take a user input & record to var for Personalisation.
    /// </summary>
    public class AskStmt : Stmt
    {
        public string Prompt;
        public string VarName;

        public AskStmt(string prompt, string varName)
        {
            Prompt = prompt;
            VarName = varName;
        }
    }

    /// <summary>
    /// Say stmt: used to output text to the user.
    /// </summary>
    public class SayStmt : Stmt
    {
        public string Text;
        public SayStmt(string text)
        {
            Text = text;
        }
    }



    // TODO
    internal class AST
    {
    }
}
