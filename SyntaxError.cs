/* Date: 05-Mar-2018
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

namespace DeepLingo
{

    class SyntaxError : Exception
    {

        public SyntaxError(TokenCategory expectedCategory,
                           Token token) :
            base(String.Format(
                "Syntax Error: Expecting {0} \n" +
                "but found {1} (\"{2}\") at row {3}, column {4}.",
                expectedCategory,
                token.Category,
                token.Lexeme,
                token.Row,
                token.Column))
        {
        }

        public SyntaxError(ISet<TokenCategory> expectedCategories,
                           Token token) :
            base(String.Format(
                "Syntax Error: Expecting one of {0}\n" +
                "but found {1} (\"{2}\") at row {3}, column {4}.",
                Elements(expectedCategories),
                token.Category,
                token.Lexeme,
                token.Row,
                token.Column))
        {
        }

        static string Elements(ISet<TokenCategory> expectedCategories)
        {
            var sb = new StringBuilder("{");
            var first = true;
            foreach (var elem in expectedCategories)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append(elem);
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
