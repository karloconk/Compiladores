/* Date: 16-APR-2018
* Authors:
*          A01374526 José Karlo Hurtado Corona
*          A01373890 Gabriela Aguilar Lugo
*          A01375996 Alan Joseph Salazar Romero
*
*          To see special comments regarding this software please refer to the README included.
*/

using System;
using System.Collections.Generic;

namespace DeepLingo 
{

    class SemanticAnalyzer 
    {

#region tables

        
        //-----------------------------------------------------------
        public SymbolTable GlobalVarsTable  {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public FuncTable FunctionTable {
            get;
            private set;
        }      
        
        //-----------------------------------------------------------
        public FuncMethods FunMethods {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            GlobalVarsTable = new SymbolTable();
            FunctionTable   = new FuncTable();
            //For second run, the dictionary of particular Funcs            
            FunMethods      = new FuncMethods();
            //Fill in the non-user defined functions
                         
            FunctionTable["printi"]  =  new GFuncStruct("p",1,new FunContainer());
            FunctionTable["printc"]  =  new GFuncStruct("p",1,new FunContainer());
            FunctionTable["prints"]  =  new GFuncStruct("p",1,new FunContainer());
            FunctionTable["println"] =  new GFuncStruct("p",0,new FunContainer());
            FunctionTable["readi"]   =  new GFuncStruct("p",0,new FunContainer());
            FunctionTable["reads"]   =  new GFuncStruct("p",0,new FunContainer());
            FunctionTable["new"]     =  new GFuncStruct("p",1,new FunContainer());
            FunctionTable["size"]    =  new GFuncStruct("p",1,new FunContainer());
            FunctionTable["add"]     =  new GFuncStruct("p",2,new FunContainer());
            FunctionTable["get"]     =  new GFuncStruct("p",2,new FunContainer());
            FunctionTable["set"]     =  new GFuncStruct("p",3,new FunContainer());
            
        }

#endregion

#region 1stRun
        string currentFunc = "";
        //-----------------------------------------------------------
        //<Program>//
        public void Visit(Program node,int i) 
        {
            Visit((dynamic) node[0],1);
        }
        
        //-----------------------------------------------------------
        //<def>//
        public void Visit(Def node, int i) {
            VisitChildren(node,i);
        }

        //-----------------------------------------------------------
        //<vardef>//
        public void Visit(VarDef node,int i) {
            Visit((dynamic) node[0], i);
        }

        //-----------------------------------------------------------
        //<var-list>// <-- //<id-list>//
        //<paramList>// <-- //<id-List>
        public void Visit(IdList node, int i) {
            VisitChildren(node,i); 
        }

        //-----------------------------------------------------------
        //<id>//
        public void Visit(Identifier node, int i) {

            var variableName = node.AnchorToken.Lexeme;

            if(i == 1){
                if (GlobalVarsTable.Contains(variableName)) {
                    throw new SemanticError(
                        "Duplicated var: " + variableName,
                        node.AnchorToken);
                } 
                else
                {
                    GlobalVarsTable[variableName] = "Global Var";       
                }
            }
            else if(i==2)
            {
                if (FunctionTable.Contains(variableName)) 
                {
                    throw new SemanticError(
                        "Duplicated function: " + variableName,
                        node.AnchorToken);
                } 
                else
                {
                    currentFunc = variableName;
                    FunctionTable[variableName] = new GFuncStruct("u",0,new FunContainer());       
                }       
            }            
        }

        //-----------------------------------------------------------
        //<funDef>//
        public void Visit(FunDef node, int i)
        {
            //visits id
            Visit((dynamic) node[0], 2);
            //visits param list
            Visit((dynamic) node[1], 3);
        }

#endregion

#region 2ndRun

        int parameterCounter = 0;
        int parameterCounter2 = 0;
        int loopCounter = 0;
        //-----------------------------------------------------------
        public void Visit(Program node) 
        {
            Visit((dynamic) node[0]);            
        }
        
        //-----------------------------------------------------------
        //<def>//
        public void Visit(Def node) {
            VisitChildren(node);
        }

        //-----------------------------------------------------------
        //<vardef>//
        public void Visit(VarDef node) {
            //Already visited in first run
        }
        
        //<vardef>//
        public void Visit(VarDef node,char i) 
        {
            Visit((dynamic) node[0],i);     
        }
        
        //<vardef-list>//
        public void Visit(VarDefList node, char i) {
            VisitChildren(node,i);
        }     

        //-----------------------------------------------------------
        //<var-list>// <-- //<id-list>//
        //<paramList>// <-- //<id-List>
        public void Visit(IdList node, char i) {
            VisitChildren(node,i); 
        }

        //-----------------------------------------------------------
        //<assign>//
        public void Visit(Assign node, char i) {
            var variableName = node.AnchorToken.Lexeme;            
            var fctemp = FunMethods[currentFunc];
            if (!fctemp.ParticularFunction.Contains(variableName) && !GlobalVarsTable.Contains(variableName)) {
                throw new SemanticError(
                    "Undeclared var: " + variableName,
                    node.AnchorToken);
            }
            VisitChildren(node,i); 
        }
        //-----------------------------------------------------------
        //<increase>//
        public void Visit(Increase node, char i) {
            var variableName = node.AnchorToken.Lexeme;            
            var fctemp = FunMethods[currentFunc];
            if (!fctemp.ParticularFunction.Contains(variableName) && !GlobalVarsTable.Contains(variableName)) {
                throw new SemanticError(
                    "Undeclared var: " + variableName,
                    node.AnchorToken);
            }
        }
        //-----------------------------------------------------------
        //<decrease>//
        public void Visit(Decrease node, char i) {
            var variableName = node.AnchorToken.Lexeme;            
            var fctemp = FunMethods[currentFunc];
            if (!fctemp.ParticularFunction.Contains(variableName) && !GlobalVarsTable.Contains(variableName)) {
                throw new SemanticError(
                    "Undeclared var: " + variableName,
                    node.AnchorToken);
            }
        }

        //-----------------------------------------------------------
        //<id>//
        public void Visit(Identifier node, char i) {

            var variableName = node.AnchorToken.Lexeme;
            if(i.Equals('f')){
                if (FunctionTable.Contains(variableName)) 
                {
                    currentFunc = variableName;
                    FunMethods[variableName] = new FunContainer();
                } 
            }
            else if (i.Equals('p'))
            {
                var fctemp = FunMethods[currentFunc];
                fctemp.ParticularFunction[variableName] = new DataOfFunc("param",parameterCounter); 
                FunMethods[currentFunc] = fctemp;
                parameterCounter++;
            }
            else if (i.Equals('v'))
            {
                
                var fctemp = FunMethods[currentFunc];
                if (fctemp.ParticularFunction.Contains(variableName)) {
                    throw new SemanticError(
                        "Duplicated var: " + variableName,
                        node.AnchorToken);
                } 
                else
                {
                    fctemp.ParticularFunction[variableName] = new DataOfFunc("local",-1); 
                    FunMethods[currentFunc] = fctemp;  
                }
            }            
            else if (i.Equals('s'))
            {
                var fctemp = FunMethods[currentFunc];
                if (!fctemp.ParticularFunction.Contains(variableName) && !GlobalVarsTable.Contains(variableName)) {
                    throw new SemanticError(
                        "Undeclared var: " + variableName,
                        node.AnchorToken);
                } 
            } 
            else if (i.Equals('c')||i.Equals('q'))
            {
                var fctemp = FunMethods[currentFunc];
                if (!fctemp.ParticularFunction.Contains(variableName) && !GlobalVarsTable.Contains(variableName)) {
                    throw new SemanticError(
                        "Undeclared var: " + variableName,
                        node.AnchorToken);
                }          
                
            }        
        }

        //-----------------------------------------------------------
        //<funDef>//
        public void Visit(FunDef node)
        {
            //visits id
            Visit((dynamic) node[0], 'f');
            //visits param list
            parameterCounter = 0;
            Visit((dynamic) node[1], 'p');
            //visits var def list
            Visit((dynamic) node[2], 'v');
            //visits stmt list
            Visit((dynamic) node[3], 's');
        }

        //-----------------------------------------------------------
        //<Loop>//
        public void Visit(Loop node,char i)
        {
            loopCounter += 1;
            VisitChildren(node, i);
            loopCounter -= 1;
        }

        //-----------------------------------------------------------
        //<Break>//
        public void Visit(Break node, char i)
        {
            var variableName = node.AnchorToken.Lexeme;
            if(loopCounter<=0)
            {

                throw new SemanticError(
                    "Break statement can only be inside a loop: " + variableName,
                    node.AnchorToken);
            }
        }

        //-----------------------------------------------------------
        //<Return>//
        public void Visit(Return node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<stmt-list>//
        public void Visit(StmtList node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<funcall>//
        public void Visit(FunCall node, char i)
        {
            var variableName = node.AnchorToken.Lexeme;
            var fctemp = FunMethods[currentFunc];
            if (!FunMethods.Contains(variableName) && !FunctionTable.Contains(variableName)) {
                throw new SemanticError(
                    "Func Not declared: " + variableName,
                    node.AnchorToken);
            } 
            else
            {
                if (i.Equals('c'))
                {
                    parameterCounter2 = 0;
                    VisitChildren(node, 'q');
                    GFuncStruct temp = FunctionTable[variableName];
                    //Console.WriteLine(variableName);
                    if(parameterCounter2 != temp.arity )
                    {
                        throw new SemanticError(
                        "Incorrect parameters arity in function call: " + variableName + ", expected:"+ temp.arity + ", actual:" +parameterCounter2 ,
                        node.AnchorToken);
                    }
                }
                else
                {
                    parameterCounter = 0;
                    VisitChildren(node, 'c');
                    GFuncStruct temp = FunctionTable[variableName];
                    if(parameterCounter != temp.arity )
                    {
                        throw new SemanticError(
                        "Incorrect parameters arity in function call: " + variableName+  ", expected:"+ temp.arity + ", actual:" +parameterCounter ,
                        node.AnchorToken);
                    }
                }
            }
        }

        //-----------------------------------------------------------
        //<expr-list>//
        public void Visit(ExpressionList node, char i)
        {
            if(i.Equals('c')||i.Equals('q'))
            {
                CountChildren(node,i);
            }
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr><-<expr-or>//
        public void Visit(Or node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //ARRAYS//
        public void Visit(ArrayNode node, char i)
        {
            VisitChildren(node, 'A');
        }

        //-----------------------------------------------------------
        //<expr><-<expr-and>//
        public void Visit(And node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr><-<equal-to>//
        public void Visit(EqualTo node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr><-<not-equal-to>//
        public void Visit(NotEqualTo node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr-rel><--<LessThan>//
        public void Visit(LessThan node, char i)
        {
            VisitChildren(node, i);
        }
                
        //-----------------------------------------------------------
        //<expr-rel><--<LessThanorequal>//
        public void Visit(LessThanOrEqual node, char i)
        {
            VisitChildren(node, i);
        }

                //-----------------------------------------------------------
        //<expr-rel><--<GreaterThan>//
        public void Visit(GreaterThan node, char i)
        {
            VisitChildren(node, i);
        }
                
        //-----------------------------------------------------------
        //<expr-rel><--<GreaterThanorequal>//
        public void Visit(GreaterThanOrEqual node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr-rel><--<ExprAdd>//
        public void Visit(Add node, char i)
        {
            VisitChildren(node, i);
        }

         //-----------------------------------------------------------
        //<expr-rel><--<ExprRes>//
        public void Visit(Res node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr-mul><--<Mul>//
        public void Visit(Multiply node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr-mul><--<Rem>//
        public void Visit(Remainder node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr-mul><--<Div>//
        public void Visit(Div node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr-unary><--<NOT>//
        public void Visit(Not node, char i)
        {
            VisitChildren(node, i);
        }

        //-----------------------------------------------------------
        //<expr-primary><--<str>//
        public void Visit(StringLiteral node, char i)
        {
            ;
        }

        //-----------------------------------------------------------
        //<expr-primary><--<char>//
        public void Visit(CharLiteral node, char i)
        {
            ;
        }
        //-----------------------------------------------------------
        //<expr-primary><--<int>//
        public void Visit(IntLiteral node, char i)
        {
            var thisIntString = node.AnchorToken.Lexeme;
            char charChar = thisIntString[0];
            if (charChar.Equals('-') || charChar.Equals('+') )
            {
                if (charChar.Equals('-'))
                {
                    try
                    {
                       var oneInt = Int32.Parse(thisIntString.Substring(1));     
                       oneInt = 0-oneInt-1;
                       Int32.Parse(oneInt.ToString());  
                    }
                    catch (System.Exception)
                    {
                        throw new SemanticError(
                            "Int value too big: " + node.AnchorToken.Lexeme,
                            node.AnchorToken);
                    }
                }
                else
                {
                    try
                    {
                       Int32.Parse(thisIntString.Substring(1));     
                    }
                    catch (System.Exception)
                    {
                        throw new SemanticError(
                            "Int value too big: " + node.AnchorToken.Lexeme,
                            node.AnchorToken);
                    }
                }
            }
            else
            {
                try
                {
                   Int32.Parse(thisIntString);     
                }
                catch (System.Exception)
                {
                    throw new SemanticError(
                        "Int value too big: " + node.AnchorToken.Lexeme,
                        node.AnchorToken);
                }
            }
        }

        //<if>//
        public void Visit(If node, char i)
        {
            VisitChildren(node, i);
        }

        //<elseif>//
        public void Visit(ElseIfList node, char i)
        {
            VisitChildren(node, i);
        }

        //<elseif>//
        public void Visit(ElseIf node, char i)
        {
            VisitChildren(node, i);
        }

        //<else>//
        public void Visit(Else node, char i)
        {
            VisitChildren(node, i);
        }

 

#endregion

#region independent funcs

        //-----------------------------------------------------------
        int CountChildren(Node node) 
        {
            int i = 0;
            foreach (var n in node) 
            {
                i++;
            }
            return i;
        }

        //-----------------------------------------------------------
        void CountChildren(Node node, char c) 
        {
            int i = 0;
            foreach (var n in node) 
            {
                i++;
            }
            //Console.WriteLine("Children:"+i+"Char:"+c);
            if(c.Equals('c'))
            {
                parameterCounter = i;
            }else if (c.Equals('q'))
            {
                parameterCounter2 = i;    
            }
        }
        //-----------------------------------------------------------
        void VisitChildren(Node node) 
        {
            foreach (var n in node) 
            {
                Visit((dynamic) n);
            }
        }
        //-----------------------------------------------------------
        void VisitChildren(Node node, int i) 
        {
            int counter = 0;
            foreach (var n in node) 
            {
                if(i == 3 )
                {
                    counter++;
                }
                Visit((dynamic) n, i);                
            }
            if(i == 3 )
            {
                GFuncStruct oneStruct = FunctionTable[currentFunc];
                oneStruct.arity = counter;
                FunctionTable[currentFunc] = oneStruct;
            }
        }
        //-----------------------------------------------------------
        void VisitChildren(Node node, char i) 
        {
            foreach (var n in node) 
            {
                Visit((dynamic) n, i);                
            }
        }
        //-----------------------------------------------------------
        void VisitBinaryOperator(char op, Node node, Type type) {
            if (Visit((dynamic) node[0]) != type || 
                Visit((dynamic) node[1]) != type) {
                throw new SemanticError(
                    String.Format(
                        "Operator {0} requires two operands",
                        op),
                    node.AnchorToken);
            }
        }
        //----------------------------------------------------------
        public void FillTheRefs()
        {
            foreach (var item in FunctionTable.getkeys())
            {
                var temp = FunctionTable[item];
                if (temp.p_or_u.Equals("u"))
                {
                    var fctemp = FunMethods[item];
                    temp.refToFunc = fctemp;
                    //Console.WriteLine(item);
                    //Console.WriteLine(fctemp.ParticularFunction.ToString());
                    FunctionTable[item] = temp;
                }
            }
        }
#endregion
    
    }
}
