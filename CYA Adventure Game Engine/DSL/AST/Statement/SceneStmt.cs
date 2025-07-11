using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Contains Scene object.
    /// </summary>
    public class SceneStmt : IStmt
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
}
