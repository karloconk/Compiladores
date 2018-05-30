# Date: 16-Apr-2018
# Authors:
#          A01374526 José Karlo Hurtado Corona
#          A01373890 Gabriela Aguilar Lugo
#          A01375996 Alan Joseph Salazar Romero
#
#          To see special comments regarding this software please refer to the README included.
#

all: deepLingo.exe deeplingolib.dll

deepLingo.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs Parser.cs \
	SyntaxError.cs Node.cs SpecificNodes.cs SemanticAnalyzer.cs \
	FuncTable.cs SemanticError.cs SymbolTable.cs ParticularFunc.cs \
	FunContainer.cs CILGenerator.cs
	
	mcs -out:deepLingo.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs SemanticAnalyzer.cs \
	FuncTable.cs SemanticError.cs SymbolTable.cs ParticularFunc.cs \
	FunContainer.cs CILGenerator.cs

deeplingolib.dll: deeplingolib.cs
	mcs /t:library deeplingolib.cs

clean:
	rm deepLingo.exe deeplingolib.dll

