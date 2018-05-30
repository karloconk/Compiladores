/* Date: 29-Jan-2018
* Authors:
*          A01374526 José Karlo Hurtado Corona
*          A01373890 Gabriela Aguilar Lugo
*          A01375996 Alan Joseph Salazar Romero
*
*          To see special comments regarding this software please refer to the README included.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeepLingo {

    class Scanner {

        readonly string input;

        static readonly Regex regex = new Regex(
            @"                             
                (?<And>                   [&]{2}                  )
              | (?<EqualTo>               [=]{2}                  )
              | (?<Assign>                [=]                     )
              | (?<BracketClose>          []]                     )
              | (?<BracketOpen>           [[]                     )
              | (?<CharLiteral>         '([^'\n\\]|\\n|\\r|\\t|\\\\|\\'|\\""|\\u[0-9a-fA-F]{6})'              )
              | (?<Comma>                 [,]                     )
              | (?<CommentLine>           [/]{2}.*[\n]            )
              | (?<CommentBlock>          [/][*] (.|\n)*? [*][/]  )
              | (?<CurlyBracketClose>     [}]                     )
              | (?<CurlyBracketOpen>      [{]                     )
              | (?<Decrease>              [-] {2}                 )
              | (?<Div>                   [/]                     )
              | (?<GreaterThanOrEqual>    (>=)                    )
              | (?<GreaterThan>           [>]                     )
              | (?<IntLiteral>            [-+]?\d+                )              
              | (?<Identifier>            [a-zA-Z]\w*             )
              | (?<Increase>              [+]{2}                  )
              |  (?<Add>                  [+]                     )
              | (?<LessThanOrEqual>       (<=)                    )
              | (?<LessThan>              [<]                     )
              | (?<Mul>                   [*]                     )
              | (?<Newline>                \n                     )
              | (?<NotEqual>              (!=)                    )
              | (?<Not>                   [!]                     )
              | (?<Or>                    [|]{2}                  )
              | (?<ParenthesisClose>      [)]                     )
              | (?<ParenthesisOpen>       [(]                     )
              | (?<Rem>                   [%]                     )
              | (?<Res>                   [-]                     )
              | (?<Semicolon>             [;]                     )
              | (?<StrLiteral>         ""([^""\n\\]|\\n|\\r|\\t|\\\\|\\'|\\""|\\u[0-9a-fA-F]{6})*""             )
              | (?<WhiteSpace> \s        )     # Must go anywhere after Newline.
              | (?<Other>      .         )     # Must be last: match any other character.
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"break", TokenCategory.BREAK},
                {"else", TokenCategory.ELSE},
                {"elseif", TokenCategory.ELSEIF},
                {"if", TokenCategory.IF},
                {"loop", TokenCategory.LOOP},
                {"return", TokenCategory.RETURN},
                {"var", TokenCategory.VAR},
            };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"Add", TokenCategory.ADD},
                {"And", TokenCategory.AND},
                {"Assign", TokenCategory.ASSIGN},
                {"BracketOpen", TokenCategory.BRACKET_OPEN},
                {"BracketClose", TokenCategory.BRACKET_CLOSE},
                {"CharLiteral", TokenCategory.CHAR_LITERAL},
                {"Comma", TokenCategory.COMMA},
                {"CurlyBracketOpen", TokenCategory.CURLY_BRACKET_OPEN},
                {"CurlyBracketClose", TokenCategory.CURLY_BRACKET_CLOSE},
                {"Decrease", TokenCategory.DECREASE},
                {"Div", TokenCategory.DIV},
                {"EqualTo", TokenCategory.EQUAL_TO},
                {"GreaterThan", TokenCategory.GREATER_THAN},
                {"GreaterThanOrEqual", TokenCategory.GREATER_THAN_OR_EQUAL_TO},
                {"Increase", TokenCategory.INCREASE},
                {"IntLiteral", TokenCategory.INT_LITERAL},
                {"LessThan", TokenCategory.LESS_THAN},
                {"LessThanOrEqual", TokenCategory.LESS_THAN_OR_EQUAL_TO},
                {"Mul", TokenCategory.MUL},
                {"Not", TokenCategory.NOT},
                {"NotEqual", TokenCategory.NOT_EQUAL_TO},
                {"Or", TokenCategory.OR},
                {"ParenthesisOpen", TokenCategory.PARENTHESIS_OPEN},
                {"ParenthesisClose", TokenCategory.PARENTHESIS_CLOSE},
                {"Rem", TokenCategory.REM},
                {"Res", TokenCategory.RES},
                {"Semicolon", TokenCategory.SEMICOLON},
                {"StrLiteral", TokenCategory.STR_LITERAL}             
            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Success) {

                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;
                } 
                else if (m.Groups["WhiteSpace"].Success )
                {
                    // Skip white space.
                }
                else if (m.Groups["CommentLine"].Success)
                {
                 // Skip one line comments.
                    row++;
                    columnStart = m.Index + m.Length;
                } 
                else if (m.Groups["CommentBlock"].Success)
                {
                    // Skip multi-line comments, but count the lines
                       foreach (char ch in m.ToString())
                        {
                            if (ch.Equals('\n'))
                            {
                                row++;
                                columnStart = m.Index + m.Length;
                            }
                        }
                }
                else if (m.Groups["Identifier"].Success) {

                    if (keywords.ContainsKey(m.Value)) {

                        // Matched string is a deeplingo keyword.
                        yield return newTok(m, keywords[m.Value]);                                               

                    } else { 

                        // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }

                } else {

                    // Match must be one of the non keywords.
                    foreach (var name in nonKeywords.Keys) {
                        if (m.Groups[name].Success) {
                            yield return newTok(m, nonKeywords[name]);
                            break;
                        }
                    }
                }
            }

            yield return new Token(null, 
                                   TokenCategory.EOF, 
                                   row, 
                                   input.Length - columnStart + 1);
        }
    }
}
