using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CYA_Adventure_Game_Engine.DSL.Frontend;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;

namespace CYA_Adventure_Game_Engine.DSL.Frontend.AST
{
    // =============== Abstract ===============

    // Statement: an action to be performed.
    public interface Stmt 
    {
        public abstract void Interpret(Environment state);
    }

    // =============== Statements ===============

    // Expr Stmt: basic wrapper to promote Exprs for Stmt positions.
    public class ExprStmt : Stmt
    {
        public Expr Expr;
        public ExprStmt(Expr expr)
        {
            Expr = expr;
        }

        public override string ToString()
        {
            return $"ExprStmt({Expr})";
        }

        public void Interpret(Environment state)
        {
            Expr.Interpret(state);
        }
    }

    public class AssignStmt : Stmt
    {
        public Expr Name;
        public Expr Value;
        public AssignStmt(Expr name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"AssignStmt(Name: {Name}, Value: {Value})";
        }

        public void Interpret(Environment state)
        {
            if (Name is VariableExpr vExpr)
            {
                var name = vExpr.Value;
                object value = Value.Interpret(state);
                state.SetVal(name, value);
            }
            else { throw new Exception("Error, invalid argument passed as variable name."); }
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

        public void Interpret(Environment state)
        {
            var condition = Condition.Interpret(state);
            if (condition is not bool) { throw new Exception("Error, If statement condition does not evaluate to true or false."); }
            if ((bool)condition)
            {
                ThenBranch.Interpret(state);
            }
            else if (ElseBranch is not null)
            {
                ElseBranch.Interpret(state);
            }
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

        public void Interpret(Environment state)
        {
            foreach (Stmt stmt in Statements)
            {
                stmt.Interpret(state);
            }
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

        public void Interpret(Environment state)
        {
            throw new Exception("Error, not yet implemented.");
        }
    }

    // Personal Stmt Types for repr game objects.

    // Start: the starting GoTo for the game.
    public class StartStmt : Stmt
    {
        public StringLitExpr Location;
        public StartStmt(StringLitExpr loc)
        {
            Location = loc;
        }

        public void Interpret(Environment state)
        {
            string loc = (string)Location.Interpret(state);
            state.SetGoTo(loc);
        }
    }

    // GoTo: Controls Flow between scenes.

    public class GoToStmt : Stmt
    {
        public StringLitExpr Location;

        public GoToStmt(StringLitExpr loc)
        {
            Location = loc;
        }
        public override string ToString()
        {
            return $"GoToStmt({Location})";
        }

        public void Interpret(Environment state)
        {
            string loc = (string)Location.Interpret(state);
            state.SetGoTo(loc);
        }
    }

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

        public void Interpret(Environment state)
        {
            state.SetScene(Name, this);
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

        public void Interpret(Environment state)
        {
            state.AddLocal(this);
        }
    }

}
