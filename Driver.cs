/* Date: 16-Apr-2018
* Authors:
*          A01374526 José Karlo Hurtado Corona
*          A01373890 Gabriela Aguilar Lugo
*          A01375996 Alan Joseph Salazar Romero
*
*          To see special comments regarding this software please refer to the README included.
*/

using System;
using System.IO;
using System.Text;

namespace DeepLingo {

    public class Driver {

        const string VERSION = "0.5";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntactic analysis",
            "AST construction",
            "Semantic analysis",
            "CIL code generation"
        };

        //-----------------------------------------------------------
        void PrintAppHeader() {
            Console.WriteLine("Deep Lingo compiler, version " + VERSION);
        }

        //-----------------------------------------------------------
        void PrintReleaseIncludes() {
            Console.WriteLine("Included in this release:");            
            foreach (var phase in ReleaseIncludes) {
                Console.WriteLine("   * " + phase);
            }
        }

        //-----------------------------------------------------------
        void Run(string[] args) {

            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();

            if (args.Length != 2) {
                Console.Error.WriteLine("Please specify the name of the input and output files.");
                Environment.Exit(1);
            }

            try
            {            
                var inputPath = args[0]; 
                var outputPath = args[1];               
                var input = File.ReadAllText(inputPath);
                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                var program = parser.Program();
                //Console.Write(program.ToStringTree());
                Console.WriteLine("Syntax OK.");

                var semantic = new SemanticAnalyzer();
                //First Run
                semantic.Visit((dynamic) program,1);
                if (!semantic.FunctionTable.Contains("main"))
                {
                    throw new SemanticError("There must be a main function on the program");
                }
                //Second Run
                semantic.Visit((dynamic) program);
                //fill the refs
                semantic.FillTheRefs();
                Console.WriteLine("Semantics OK.");

                 
                //  Console.WriteLine();
                //   Console.WriteLine(semantic.GlobalVarsTable.ToString());
                //   Console.WriteLine(semantic.FunctionTable.ToString());
                //   Console.WriteLine(semantic.FunMethods.ToString());
                
                
                var codeGenerator = new CILGenerator(semantic.GlobalVarsTable,semantic.FunctionTable);
                File.WriteAllText(
                    outputPath,
                    codeGenerator.Visit((dynamic) program));
                Console.WriteLine(
                    "Generated CIL code to '" + outputPath + "'.");
                Console.WriteLine();

            }
            catch (Exception e)
            {

                if (e is FileNotFoundException 
                    || e is SyntaxError 
                    || e is SemanticError)
                {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(1);
                }

                throw;
            }
        }

        //-----------------------------------------------------------
        public static void Main(string[] args) {
            new Driver().Run(args);
        }
    }
}
