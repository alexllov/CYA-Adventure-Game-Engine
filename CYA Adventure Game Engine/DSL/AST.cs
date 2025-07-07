using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    // =============== Expressions ===============

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
            return $"{Value}";
            //return $"NumberLitExpr({Value})";
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
            // TODO: Set up a DebugToString for parser purposes that contians this return.
            //return $"StringLitExpr({Value})";
            return $"{Value}";
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
        public override string ToString() 
        {
            return $"{Name}";
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

    /// <summary>
    /// Used to register Identifiers to values, eg Variable assignment.
    /// </summary>
    public class AssignExpr: Expr
    {
        public Expr Name;
        public Expr Value;

        public AssignExpr(Expr name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() 
        {
            return $"AssignExpr({Name}, {Value})";
        }
    }

    public class DotExpr : Expr
    {
        public Expr Left;
        public Expr Right;
        public DotExpr(Expr left, Expr right)
        {
            Left = left;
            Right = right;
        }
        public override string ToString() 
        {
            return $"DotExpr({Left}, {Right})";
        }
    }

    public class FuncExpr : Expr
    {
        public Expr Method;
        public List<Expr>? Arguments { get; init; }
        public FuncExpr(Expr method, List<Expr> arguments = null)
        {
            Method = method;
            Arguments = arguments;
        }
        public override string ToString()
        {
            if (Arguments != null)
            {
                foreach (Expr arg in Arguments)
                {
                    Console.WriteLine($"arg: {arg}, type: {arg.GetType()}");
                }
                return $"FuncExpr(Method: {Method}, Arguments: [{string.Join(", ", Arguments)}])";
            }
            else
            {
                return $"FuncExpr(Method: {Method}";
            }
        }
    }

    // =============== Statements ===============

    // Default Expr catch to convert Exprs w/o type to a Stmt.

    public class ExprStmt : Stmt
    {
        public Expr _Expr;
        public ExprStmt(Expr expr)
        {
            _Expr = expr;

        }
        public override string ToString() 
        {
            return $"ExprStmt({_Expr})";
        }
    }

    public class FuncExprStmt : Stmt
    {
        public FuncExpr _Expr;
        public FuncExprStmt(FuncExpr expr)
        {
            _Expr = expr;

        }
        public override string ToString()
        {
            return $"FuncExprStmt({_Expr})";
        }
    }

    // TODO: Required??
    // Declaration of variables.
    public class AssignStmt : Stmt
    {
        public AssignExpr Value;
        public AssignStmt(AssignExpr val)
        {
            Value = val;
        }
        public override string ToString() 
        {
            return $"AssignStmt({Value})";
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
        public override string ToString() 
        {
            return $"IfStmt(Condition: {Condition}, ThenBranch: {ThenBranch}, ElseBranch: {ElseBranch})";
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
        public override string ToString() 
        {
            return $"BlockStmt(Statements: [\n    {string.Join("\n    ", Statements)}])";
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
        public override string ToString() 
        {
            return Alias == null ? $"ImportStmt(Module: {Module})" : $"ImportStmt(Module: {Module}, Alias: {Alias})";
        }
    }

    // Personal Stmt Types for repr game objects.
    // Scene.
    public class SceneStmt : Stmt
    {
        public string Name;
        public BlockStmt Body;
        public SceneStmt(string name, BlockStmt body)
        {
            Name = name;
            Body = body;
        }
        public override string ToString() 
        {
            return $"SceneStmt(\n  Name: {Name}, \n  Body: {Body})";
        }
    }

    // Interactable.
    public class InteractableStmt : Stmt
    {
        public Expr Name;
        public Stmt Body;
        public InteractableStmt(Expr name, Stmt body)
        {
            Name = name;
            Body = body;
        }
        public override string ToString()
        {
            return $"InteractableStmt(Name: {Name}, Body: {Body})";
        }
    }


    // TODO
    public class AST
    {
        public List<Stmt> Tree = new List<Stmt>();
        public AST(List<Stmt> statements)
        {
            Tree = statements;
        }
    }
}
