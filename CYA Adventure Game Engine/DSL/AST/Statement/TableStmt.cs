using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
using CYA_Adventure_Game_Engine.DSL.Frontend;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Handles the assignment of a value to a variable name.
    /// </summary>
    public class TableStmt : IStmt
    {
        public IExpr Name;
        public List<List<IExpr>> Records;

        public TableStmt(IExpr name, List<List<IExpr>> records)
        {
            Name = name;
            Records = records;
        }

        /// <summary>
        /// Debug Method.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"AssignStmt(Name: {Name}, Records: {Records})";
        }

        // TODO: Add clear error msg for a row w/ wrong number of attributes.
        public Table BuildTable(Environment state)
        {
            // Get Attribute Names.
            //List<IExpr> cols = Records[0];
            List<string> cols = Records[0].Select(i => (string)i.Interpret(state)).ToList();
            Dictionary<string, TableRow> table = new();

            List<List<IExpr>> dataRows = Records[1..];
            foreach (List<IExpr> row in dataRows)
            {
                int i = 0;
                Dictionary<string, object> rowDict = new();
                foreach (var record in row)
                {
                    rowDict[(string)cols[i]] = record.Interpret(state);
                    i++;
                }
                table[(string)row[0].Interpret(state)] = new TableRow(rowDict);
            }
            return new Table(cols, table);
        }


        public void Interpret(Environment state)
        {
            Table table = BuildTable(state);
            if (Name is VariableExpr vExpr)
            {
                var name = vExpr.Value;
                state.SetVal(name, table);
            }
            else { throw new Exception("Error, invalid argument passed as table name."); }
        }
    }
}