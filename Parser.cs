/* Date: 05-Mar-2018
* Authors:
*          A01374526 Jos� Karlo Hurtado Corona
*          A01373890 Gabriela Aguilar Lugo
*          A01375996 Alan Joseph Salazar Romero
*
*          To see special comments regarding this software please refer to the README included.
*
*Current LL(1) is presented now:
*
*<program> → <def>*
*<def>     → <var-def>|<fundef>
*‹var-def› → var ‹var-list› ;
*‹var-list› → ‹id-list›
*‹id-list› → ‹id› (, ‹id› ‹id-list-cont›)*
*‹fun-def› → ‹id› ( ‹param-list› ) { ‹var-def-list› ‹stmt-list› }
*‹param-list› → ‹id-list›?
*<var-def-list> -> <var-def>*
*<stmt-list> → (<funcall>;|<id>(= <expr>;| ++; |--;)|if (<expr>){<stmt-list>}(else if (<expr>){<stmt-list>})*(else{<stmt-list>})? | loop {<stmt-list>}| break; | return <expr>; |; )*
*<funcall>  -> <id>(<exprlist>)
*‹expr-list› → (‹expr› (, ‹expr›)*)?
*‹expr-list-cont› →( ‹expr-list-cont›)?
*‹expr› → ‹expr-or›
*‹expr-or› → ‹expr-and› (|| ‹expr-and›)*
*‹expr-and› → ‹expr-comp> (&& ‹expr-comp›)*
*<expr-primary> → <id> | <funcall> | [<expr-list>] | (‹lit-int›|‹lit-char›|‹lit-str›) | (<expr>)
*<expr-unary> → (+ | - | !) <exprunary> |<expr-primary>
*<expr-mul> → <expr-unary> ((* |/|%) <expr-unary>)*
*<expr-add> → <expr-mul> ((+ | -) <expr-mul>)*
*<expr-rel> → <expr-add> ((<|<=|>|>=) <expr-add>)*
*<expr-comp> → <expr-rel> ((==|!=) <expr-rel>)*
*/

using System;
using System.Collections.Generic;

namespace DeepLingo
{

    class Parser
    {
        #region firstOfs

        //<vardef>
        static readonly ISet<TokenCategory> firstOfDeclaration =
            new HashSet<TokenCategory>() {
                TokenCategory.VAR
            };

        //<fundef>
        static readonly ISet<TokenCategory> firstOfFunDef =
        new HashSet<TokenCategory>()
        {
                TokenCategory.IDENTIFIER
        };

        //<id-list>
        static readonly ISet<TokenCategory> firstOfIdList =
        new HashSet<TokenCategory>()
        {
                        TokenCategory.IDENTIFIER
        };

        //<id-list-cont>
        static readonly ISet<TokenCategory> firstOfIdListCont =
        new HashSet<TokenCategory>()
        {
                        TokenCategory.COMMA
        };

        //<stmt-list>
        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.LOOP,
                TokenCategory.BREAK,
                TokenCategory.RETURN,
                TokenCategory.SEMICOLON
            };

        //<elseif>
        static readonly ISet<TokenCategory> elsif =
            new HashSet<TokenCategory>()
            {
                TokenCategory.ELSEIF
            };

        //<exprrel>
        static readonly ISet<TokenCategory> firstOfExprRel =
            new HashSet<TokenCategory>() {
                TokenCategory.LESS_THAN,
                TokenCategory.LESS_THAN_OR_EQUAL_TO,
                TokenCategory.GREATER_THAN,
                TokenCategory.GREATER_THAN_OR_EQUAL_TO
            };

        //<exprmul>
        static readonly ISet<TokenCategory> firstOfExprMul =
            new HashSet<TokenCategory>() {
                TokenCategory.MUL,
                TokenCategory.REM,
                TokenCategory.DIV
            };

        //<exprunary>
        static readonly ISet<TokenCategory> firstofUnary =
            new HashSet<TokenCategory>() {
                TokenCategory.UNARY_PLUS,
                TokenCategory.UNARY_MINUS,
                TokenCategory.NOT
            };

        //<expr-primary>
        static readonly ISet<TokenCategory> firstExprPrimary =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.BRACKET_OPEN,
                TokenCategory.STR_LITERAL,
                TokenCategory.CHAR_LITERAL,
                TokenCategory.INT_LITERAL,
                TokenCategory.PARENTHESIS_OPEN
            };

        //<expr>
        static readonly ISet<TokenCategory> firstExpr =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.BRACKET_OPEN,
                TokenCategory.STR_LITERAL,
                TokenCategory.CHAR_LITERAL,
                TokenCategory.INT_LITERAL,
                TokenCategory.PARENTHESIS_OPEN,
                TokenCategory.UNARY_PLUS,
                TokenCategory.UNARY_MINUS,
                TokenCategory.NOT,
                TokenCategory.MUL,
                TokenCategory.REM,
                TokenCategory.DIV,
                TokenCategory.LESS_THAN,
                TokenCategory.LESS_THAN_OR_EQUAL_TO,
                TokenCategory.GREATER_THAN,
                TokenCategory.GREATER_THAN_OR_EQUAL_TO,
                TokenCategory.ADD,
                TokenCategory.RES,
                TokenCategory.NOT_EQUAL_TO,
                TokenCategory.EQUAL_TO
            };
        #endregion

        #region notMovables
        IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream)
        {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        public TokenCategory CurrentToken
        {
            get { return tokenStream.Current.Category; }
        }

        public Token Expect(TokenCategory category)
        {
            if (CurrentToken == category)
            {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            }
            else
            {
                throw new SyntaxError(category, tokenStream.Current);
            }
        }
        #endregion

        #region methods

        //<Program>//
        public Node Program()
        {
            var Def = new Def();

            while (firstOfDeclaration.Contains(CurrentToken) | firstOfFunDef.Contains(CurrentToken))
            {
                if (firstOfDeclaration.Contains(CurrentToken))
                {
                    Def.Add(VarDef());
                }
                else
                {
                    Def.Add(FunDef());
                }
            }
            Expect(TokenCategory.EOF);

            return new Program() {
                Def
            };
        }


        //<vardef>//
        public Node VarDef()
        {
            var result = new Node();
            result = new VarDef() {
                AnchorToken = Expect(TokenCategory.VAR)
            };
            result.Add(VarList());
            Expect(TokenCategory.SEMICOLON);//-------------------------------------------------
            return result;
        }

        //<fundef>//
        public Node FunDef()
        {
            var result = new FunDef();
            result.Add(new Identifier()
            {
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            });
            
            Expect(TokenCategory.PARENTHESIS_OPEN);

            result.Add(ParamList());

            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACKET_OPEN);

            result.Add(VarDefList());
            result.Add(StmtList());

            Expect(TokenCategory.CURLY_BRACKET_CLOSE);

            return result;
        }

        //<var-list>//
        public Node VarList()
        {
            return IdList();
        }

        //<id-list>//
        public Node IdList()
        {
            var idlistr = new IdList(); 
            switch (CurrentToken)
            {

                case TokenCategory.IDENTIFIER:
                    idlistr.Add(new Identifier()
                    {
                        AnchorToken = Expect(TokenCategory.IDENTIFIER)
                    });
                    break;

                default:
                    throw new SyntaxError(firstOfIdList,
                                          tokenStream.Current);
            }

            while(firstOfIdListCont.Contains(CurrentToken) )
            {
                switch (CurrentToken)
                {
                    case TokenCategory.COMMA:
                        Expect(TokenCategory.COMMA);
                        idlistr.Add(new Identifier()
                        {
                            AnchorToken = Expect(TokenCategory.IDENTIFIER)
                        });
                        break;

                    default:
                        throw new SyntaxError(firstOfIdListCont,
                                              tokenStream.Current);
                }
            }
            return idlistr;
        }

        //<param-list>//
        public Node ParamList()
        {
            if (CurrentToken == TokenCategory.IDENTIFIER)
            {
                return IdList();
            }
            return  new IdList();  //-------------------------
        }

        //<var-def-list>//
        public Node VarDefList()
        {
            var nvardeflist = new VarDefList();
            while (firstOfDeclaration.Contains(CurrentToken))
            {
                nvardeflist.Add(VarDef());
            }
            return nvardeflist;
        }

        //<stmt-list>//
        public Node StmtList()
        {
            var stmtList = new StmtList();
            while (firstOfStatement.Contains(CurrentToken))
            {
                switch (CurrentToken)
                {
                    case TokenCategory.IDENTIFIER:
                        stmtList.Add(stmtId());
                        break;

                    case TokenCategory.IF:
                        stmtList.Add(If());
                        break;

                    case TokenCategory.LOOP:
                        var LooperLupe = new Loop()
                        {
                            AnchorToken = Expect(TokenCategory.LOOP)
                        };
                        Expect(TokenCategory.CURLY_BRACKET_OPEN);
                        LooperLupe.Add(StmtList());
                        stmtList.Add(LooperLupe);
                        Expect(TokenCategory.CURLY_BRACKET_CLOSE);
                        break;

                    case TokenCategory.BREAK:
                        
                        stmtList.Add(new Break()
                        {
                            AnchorToken = Expect(TokenCategory.BREAK)
                        });

                        Expect(TokenCategory.SEMICOLON);
                        break;

                    case TokenCategory.RETURN:
                        var myreturn = new Return()
                        {
                            AnchorToken = Expect(TokenCategory.RETURN)
                        };
                        myreturn.Add(Expression());
                        stmtList.Add(myreturn);                        
                        Expect(TokenCategory.SEMICOLON);
                        break;

                    case TokenCategory.SEMICOLON:
                        Expect(TokenCategory.SEMICOLON);
                        break;

                    default:
                        throw new SyntaxError(firstOfStatement,
                                              tokenStream.Current);
                }
            }
            return stmtList;
        }

        //Side function for statement starting with id//
        public Node stmtId()
        {
            var stmtidN = new Node();
            var myT = Expect(TokenCategory.IDENTIFIER);
            var funky = new FunCall()
            {
                AnchorToken = myT
            };
            stmtidN = new Identifier()
            {
                AnchorToken = myT
            };

            switch (CurrentToken)
            {
                case TokenCategory.ASSIGN:
                    Expect(TokenCategory.ASSIGN);
                    stmtidN = new Assign()
                    {
                        AnchorToken = myT
                    };
                    stmtidN.Add(Expression());
                    Expect(TokenCategory.SEMICOLON);
                    break;

                case TokenCategory.INCREASE:
                    Expect(TokenCategory.INCREASE);
                    stmtidN = new Increase()
                    {
                        AnchorToken = myT
                    };
                    Expect(TokenCategory.SEMICOLON);
                    break;

                case TokenCategory.DECREASE:
                    Expect(TokenCategory.DECREASE);
                    stmtidN = new Decrease()
                    {
                        AnchorToken = myT
                    };
                    Expect(TokenCategory.SEMICOLON);
                    break;

                case TokenCategory.PARENTHESIS_OPEN:
                    stmtidN = funky;
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    stmtidN.Add(ExpressionList());
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    Expect(TokenCategory.SEMICOLON);
                    break;
            }
            return stmtidN;
        }

        //<expr>//
        public Node Expression()
        {
            return ExpressionOr();
        }

        //<expr-or>//
        public Node ExpressionOr()
        {
            var andOne =  ExpressionAnd();
            
            var Orer = andOne;
           
            while (CurrentToken == TokenCategory.OR)
            {
                Orer = new Or()
                {
                    AnchorToken = Expect(TokenCategory.OR)
                };
                Orer.Add(andOne);
                Orer.Add(ExpressionAnd());
                andOne = Orer;
            }
            return Orer;
        }

        //<expr-and>//
        public Node ExpressionAnd()
        {
            var compOne =  ExprComp();
            var Ander = compOne;

            while (CurrentToken == TokenCategory.AND)
            {
                Ander = new And()
                {
                    AnchorToken = Expect(TokenCategory.AND)
                };
                Ander.Add(compOne);
                Ander.Add(ExprComp());
                compOne = Ander;
            }
            return Ander;
        }

        //<expr-comp>//
        public Node ExprComp()
        {
            var relone =  ExprRel();
            var xprComp = relone;

            while (CurrentToken == TokenCategory.EQUAL_TO || CurrentToken == TokenCategory.NOT_EQUAL_TO)
            {
                if (CurrentToken == TokenCategory.EQUAL_TO)
                {
                    xprComp = new EqualTo()
                    {
                        AnchorToken = Expect(TokenCategory.EQUAL_TO)
                    };
                    
                }
                else
                {
                    xprComp = new NotEqualTo()
                    {
                        AnchorToken = Expect(TokenCategory.NOT_EQUAL_TO)
                    } ;
                }
                xprComp.Add(relone);
                xprComp.Add(ExprRel());
                relone = xprComp;
            }
            return xprComp;
        }

        //<expr-rel>//
        public Node ExprRel()
        {
            var addOne =  ExprAdd();
            var xprRel = addOne;

            while (firstOfExprRel.Contains(CurrentToken))
            {
                switch (CurrentToken)
                {
                    case TokenCategory.LESS_THAN:
                        xprRel = new LessThan()
                        {
                            AnchorToken = Expect(TokenCategory.LESS_THAN)
                        };
                        break;

                    case TokenCategory.LESS_THAN_OR_EQUAL_TO:
                        xprRel = new LessThanOrEqual()
                        {
                            AnchorToken = Expect(TokenCategory.LESS_THAN_OR_EQUAL_TO)
                        };
                        break;

                    case TokenCategory.GREATER_THAN:
                        xprRel = new GreaterThan()
                        {
                            AnchorToken = Expect(TokenCategory.GREATER_THAN)
                        };
                        break;

                    case TokenCategory.GREATER_THAN_OR_EQUAL_TO:
                        xprRel = new GreaterThanOrEqual()
                        {
                            AnchorToken = Expect(TokenCategory.GREATER_THAN_OR_EQUAL_TO)
                        };
                        break;

                    default:
                        throw new SyntaxError(firstOfExprRel,
                                              tokenStream.Current);
                }
                xprRel.Add(addOne);
                xprRel.Add(ExprAdd());
                addOne = xprRel;
            }
            return xprRel;
        }

        //<expr-add>//
        public Node ExprAdd()
        {
            var mulOne =  ExprMul();
            var xprAdd = mulOne;

            while (CurrentToken == TokenCategory.ADD || CurrentToken == TokenCategory.RES)
            {
                if (CurrentToken == TokenCategory.ADD)
                {
                    xprAdd = new Add()
                    {
                        AnchorToken = Expect(TokenCategory.ADD)
                    };
                }
                else
                {
                    xprAdd = new Res()
                    {
                        AnchorToken = Expect(TokenCategory.RES)
                    };
                }
                xprAdd.Add(mulOne);
                xprAdd.Add(ExprMul());
                mulOne = xprAdd;
            }
            return xprAdd;
        }

        //<expr-mul>//
        public Node ExprMul()
        {
            var unaryOne =  ExprUnary();
            var xprMul =    unaryOne;
            
            while (firstOfExprMul.Contains(CurrentToken))
            {
                switch (CurrentToken)
                {
                    case TokenCategory.MUL:
                        xprMul = new Multiply()
                        {
                            AnchorToken = Expect(TokenCategory.MUL)
                        };
                        break;

                    case TokenCategory.REM:
                        xprMul = new Remainder()
                        {
                            AnchorToken = Expect(TokenCategory.REM)
                        };
                        break;

                    case TokenCategory.DIV:
                        xprMul = new Div()
                        {
                            AnchorToken = Expect(TokenCategory.DIV)
                        };
                        break;

                    default:
                        throw new SyntaxError(firstOfExprMul,
                                              tokenStream.Current);
                }
                xprMul.Add(unaryOne);
                
                xprMul.Add(ExprUnary());
                unaryOne = xprMul;
            }
            
            return xprMul;
        }

        //expr-unary>//
        public Node ExprUnary()
        {
            var xprUn = new Node();

            if (firstofUnary.Contains(CurrentToken))
            {
                while (firstofUnary.Contains(CurrentToken))
                {
                    switch (CurrentToken)
                    {
                        case TokenCategory.NOT:
                            xprUn = new Not()
                            {
                                AnchorToken = Expect(TokenCategory.NOT)
                            };
                            break;

                        default:
                            throw new SyntaxError(firstofUnary,
                                                  tokenStream.Current);
                    }
                    xprUn.Add(ExprUnary());
                }
            }
            else
            {
                xprUn = ExprPrimary();
            }
            return xprUn;
        }

        //<expr-primary>//
        public Node ExprPrimary()
        {
            var xprPri = new Node();
            var funky = new Node();

            switch (CurrentToken)
            {
                case TokenCategory.IDENTIFIER:
                    var myT = Expect(TokenCategory.IDENTIFIER);
                    funky  = new FunCall()
                    {
                        AnchorToken = myT
                    };
                    xprPri = new Identifier()
                    {
                        AnchorToken = myT
                    };

                    if (CurrentToken == TokenCategory.PARENTHESIS_OPEN)
                    {
                        xprPri = funky;
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        xprPri.Add(ExpressionList());
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                    }
                    break;

                case TokenCategory.BRACKET_OPEN:
                    Expect(TokenCategory.BRACKET_OPEN);
                    xprPri = ExpressionList("Array");
                    Expect(TokenCategory.BRACKET_CLOSE);
                    break;

                case TokenCategory.STR_LITERAL:
                    xprPri = new StringLiteral()
                    {
                        AnchorToken = Expect(TokenCategory.STR_LITERAL)
                    };
                    break;

                case TokenCategory.CHAR_LITERAL:
                    xprPri = new CharLiteral()
                    {
                        AnchorToken = Expect(TokenCategory.CHAR_LITERAL)
                    };
                    break;

                case TokenCategory.INT_LITERAL:
                    xprPri = new IntLiteral()
                    {
                        AnchorToken = Expect(TokenCategory.INT_LITERAL)
                    };
                    break;

                case TokenCategory.PARENTHESIS_OPEN:
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    xprPri = Expression();
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    break;

                default:
                    throw new SyntaxError(firstExprPrimary,
                                            tokenStream.Current);
            }
            return xprPri;
        }

        //<expr-list>//
        public Node ExpressionList()
        {
            var elist = new ExpressionList();

            if (firstExpr.Contains(CurrentToken))
            {
                elist.Add(Expression());
                while (firstOfIdListCont.Contains(CurrentToken) )
                {
                    Expect(TokenCategory.COMMA);
                    elist.Add(Expression());
                }
            }
            return elist;
        }

        //<expr-list> FOR THE ARRAYS//
        public Node ExpressionList(string smth)
        {
            var elist = new ArrayNode();

            if (firstExpr.Contains(CurrentToken))
            {
                elist.Add(Expression());
                while (firstOfIdListCont.Contains(CurrentToken) )
                {
                    Expect(TokenCategory.COMMA);
                    elist.Add(Expression());
                }
            }
            return elist;
        }

        //If side function//
        public Node If()
        {
            var result = new If()
            {
                AnchorToken = Expect(TokenCategory.IF)
            };

            Expect(TokenCategory.PARENTHESIS_OPEN);

            result.Add(Expression());

            Expect(TokenCategory.PARENTHESIS_CLOSE);

            Expect(TokenCategory.CURLY_BRACKET_OPEN);

            result.Add(StmtList());

            Expect(TokenCategory.CURLY_BRACKET_CLOSE);

            var elseIfListr = new ElseIfList();

            while (elsif.Contains(CurrentToken))
            {
                elseIfListr.Add(elseif());
            }

            result.Add(elseIfListr);

            var elser = new Else();

            if (CurrentToken == TokenCategory.ELSE)
            {
                elser = new Else()
                {
                    AnchorToken = Expect(TokenCategory.ELSE)
                };

                Expect(TokenCategory.CURLY_BRACKET_OPEN);

                elser.Add(StmtList());

                Expect(TokenCategory.CURLY_BRACKET_CLOSE);
            }
            result.Add(elser);

            return result;
        }

        //elseif side function//
        public Node elseif()
        {
            var result = new ElseIf()
            {
                AnchorToken = Expect(TokenCategory.ELSEIF)
            };

            Expect(TokenCategory.PARENTHESIS_OPEN);

            result.Add(Expression());

            Expect(TokenCategory.PARENTHESIS_CLOSE);

            Expect(TokenCategory.CURLY_BRACKET_OPEN);

            result.Add(StmtList());

            Expect(TokenCategory.CURLY_BRACKET_CLOSE);

            return result;
        }

        #endregion

    }
}
