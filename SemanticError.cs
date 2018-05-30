/* Date: 16-APR-2018
* Authors:
*          A01374526 Jos� Karlo Hurtado Corona
*          A01373890 Gabriela Aguilar Lugo
*          A01375996 Alan Joseph Salazar Romero
*
*          To see special comments regarding this software please refer to the README included.
*/

using System;

namespace DeepLingo {

    class SemanticError: Exception {

        public SemanticError(string message, Token token):
            base(String.Format(
                "Semantic Error: {0} \n" +
                "at row {1}, column {2}.",
                message,
                token.Row,
                token.Column)) {
        }
        public SemanticError(string message):
            base(String.Format(
                "Semantic Error: {0} \n",
                message)) {
        }
    }
}
