using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
namespace CYA_Adventure_Game_Engine.DSL.Frontend.Parser;


public class Parser
{
    /// <summary>
    /// Header ends: the list of Token Types that will end the collection of stmts into any Header.
    /// These Include: 'Scene', 'Table', 'Code', 'End', 'EOF'.
    /// </summary>
    public readonly List<TokenType> HeaderEnds =
    [
        TokenType.Scene,
        TokenType.Table,
        TokenType.Overlay,
        TokenType.End,
        TokenType.EOF
    ];



    // Error Message Helpers.
    public Token StartOfCurrentStmt;
    public string CurrentStmtParsing;

    // Parser Components.
    public readonly TokenList Tokens;
    private List<IParserExtender> ParserExtenders;

    public List<IStmt> AST = [];

    public Parser(TokenList tokens, List<IParserExtender>? parserExtenders = null)
    {
        ParserExtenders = (parserExtenders is not null) ? parserExtenders : [];
        Tokens = tokens;
    }

    /// <summary>
    /// Takes the Token list initially passed into the parser and constructs an AST from it.
    /// </summary>
    public AbstSyntTree Parse()
    {
        while (Tokens.Peek(0).Type != TokenType.EOF)
        {
            StartOfCurrentStmt = Tokens.Peek(0);
            AST.Add(ParseStmt());
            //Console.WriteLine(AST.Last());
        }
        return new AbstSyntTree(AST);
    }

    /// <summary>
    /// Parses the next statement. Returns an IStmt.
    /// </summary>
    /// <returns></returns>
    public IStmt ParseStmt()
    {
        CurrentStmtParsing = "identifying statement type";
        Token token = Tokens.Peek(0);

        return (token.Type, Tokens.Peek(1).Type) switch
        {
            (TokenType.Import, _) => ImportStmt.Parse(this),
            // []'s Hold If stmts & Choices.
            (TokenType.LBracket, TokenType.If) => IfStmt.Parse(this),
            (TokenType.LBracket, TokenType.String) => ChoiceStmt.Parse(this),
            (TokenType.LBracket, TokenType.Identifier) => ChoiceStmt.Parse(this),
            (TokenType.LBracket, _) => throw new Exception($"Unexpected token type following '[': {token.Type}, on line: {token.position[0]} in file: {token.SourceFile}. Error started in {StartOfCurrentStmt}, and occurred during {CurrentStmtParsing}"),
            // GoTo.
            (TokenType.GoTo, _) => GoToStmt.Parse(this),
            // Scene & Components.
            (TokenType.Scene, _) => SceneStmt.Parse(this),
            // Table.
            (TokenType.Table, _) => TableStmt.Parse(this),
            // Overlay: run & exit allow for overlay control.
            (TokenType.Overlay, _) => OverlayStmt.Parse(this),
            (TokenType.Run, _) => RunStmt.Parse(this),
            (TokenType.Exit, _) => ExitStmt.Parse(this),
            // Assignment.
            (TokenType.Identifier, TokenType.Assign) => AssignStmt.Parse(this),
            // Default: foreign statement
            // or loose expression, to be wrapped as Expression Statement.
            _ => TryExtenders(),
        };
    }
    private IStmt TryExtenders()
    {
        foreach (IParserExtender pe in ParserExtenders)
        {
            if (pe.TryParseStmt(this, out IStmt? stmt)) { return stmt; }
        }
        return ExprStmt.Parse(this);
    }
}