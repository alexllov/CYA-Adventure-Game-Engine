using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
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
                    rowDict[cols[i]] = record.Interpret(state);
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

        /// <summary>
        /// Creates a Table statement.
        /// Used to store entries with common attributes, e.g. a table of interchangable weapons.
        /// </summary>
        /// <returns>TableStmt</returns>
        /// <exception cref="Exception"></exception>
        public static TableStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "table statement";
            // Consule table token.
            parser.Tokens.Advance();
            // Next token should be variable table name.
            IExpr ID = parser.ParseExpression(0);
            if (ID is not VariableExpr) { throw new Exception($"table declaration should begin with a variable name. Error began at {parser.Tokens.Peek(0)}"); }

            List<List<IExpr>> records = [];
            List<IExpr> row = [];
            while (!parser.HeaderEnds.Contains(parser.Tokens.Peek(0).Type))
            {
                // || = end of current row & start of next => store completed row.
                if (parser.Tokens.Peek(0).Type is TokenType.Pipe && parser.Tokens.Peek(1).Type is TokenType.Pipe)
                {
                    records.Add(row);
                    parser.Tokens.Advance();
                    row = [];
                }
                // Step over '|'s between columns.
                else if (parser.Tokens.Peek(0).Type is TokenType.Pipe)
                {
                    parser.Tokens.Advance();
                }
                else
                {
                    IExpr attribute = parser.ParseExpression(0);
                    row.Add(attribute);
                }

                /*
                 * Catch last record before EOF to add.
                 * This is separate s.t. it doesn't consume that following token which could be the start of a scene.
                 */
                if (parser.HeaderEnds.Contains(parser.Tokens.Peek(1).Type))
                {
                    records.Add(row);
                    parser.Tokens.Consume(TokenType.Pipe);
                    break;
                }
            }
            // If table ends with an End token, consume it.
            parser.Tokens.Match(TokenType.End);

            return new TableStmt((VariableExpr)ID, records);
        }
    }
}