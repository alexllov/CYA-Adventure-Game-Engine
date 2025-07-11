using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Is Statement: An expression is evaluated, controling if a "then" branch is ran or not. Optional 'else' branch.
    /// </summary>
    public class IfStmt : IStmt
    {
        public IExpr Condition;
        public IStmt ThenBranch;
        public IStmt ElseBranch;

        public IfStmt(IExpr condition, IStmt thenBranch, IStmt elseBranch = null)
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
}
