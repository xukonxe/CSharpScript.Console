using System;
using System.Dynamic;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using TGZG.CSharpScript.Console;

namespace TestConsole {
    public class Program {
        public class Globals {
            public string name = "undefined";
            public List<string> names = ["John", "Mary", "Peter"];
        }
        public static void Main(string[] args) {
            new CSharpScriptConsole<Globals>()
                .SetGlobals(new Globals())
                .Import("System.Linq")
                .Reference(typeof(Enumerable).Assembly)
                .Run();
        }
    }
}
