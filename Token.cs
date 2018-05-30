/* Date: 29-Jan-2018
* Authors:
*          A01374526 José Karlo Hurtado Corona
*          A01373890 Gabriela Aguilar Lugo
*          A01375996 Alan Joseph Salazar Romero
*
*          To see special comments regarding this software please refer to the README included.
*/

using System;

namespace DeepLingo {

    class Token {

        readonly string lexeme;

        readonly TokenCategory category;

        readonly int row;

        readonly int column;

        public string Lexeme { 
            get { return lexeme; }
        }

        public TokenCategory Category {
            get { return category; }          
        }

        public int Row {
            get { return row; }
        }

        public int Column {
            get { return column; }
        }

        public Token(string lexeme, 
                     TokenCategory category, 
                     int row, 
                     int column) {
            this.lexeme = lexeme;
            this.category = category;
            this.row = row;
            this.column = column;
        }

        public override string ToString() {
            return string.Format("{{{0}, \"{1}\", @({2}, {3})}}",
                                 category, lexeme, row, column);
        }
    }
}

