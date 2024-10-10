using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;

namespace TGZG.CSharpScript.Console {
    static class ExtensionMethods {
        private static Action<object> LogAct => System.Console.WriteLine;
        private static Action<object> LogWarningAct => t => {
            // 输出编译警告信息
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(t);
            System.Console.ResetColor();
        };
        private static Action<object> LogErrorAct = t => {
            // 输出编译错误信息
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(t);
            System.Console.ResetColor();
        };
        public static void Log(this object message) {
            LogAct?.Invoke(message);
        }
        public static void LogWarning(this object message) {
            LogWarningAct?.Invoke(message);
        }
        public static void LogError(this object message) {
            LogErrorAct?.Invoke(message);
        }
    }
    public class Environment<T> {
        public CSharpScriptConsole<T> ThisConsole { get; }
        public Dictionary<string, Action> Operations { get; private set; } = new();
        public bool ReadyToExit { get; private set; } = false;
        public bool ReadyToOutputVars { get; private set; } = false;
        public Environment(CSharpScriptConsole<T> console) {
            ThisConsole = console;
            InitOperations();
        }
        void InitOperations() {
            Operations["exit"] = () => ReadyToExit = true;
            Operations["vars"] = () => ReadyToOutputVars = true;
        }
        public Func<string> ReadLine = () => {
            System.Console.Write("> ");
            return System.Console.ReadLine();
        };
        public void ReSetReadyToOutputVars() {
            ReadyToOutputVars = false;
        }
    }
    public class Runner<T> {
        public CSharpScriptConsole<T> ThisConsole { get; }
        public Runner(CSharpScriptConsole<T> console) {
            ThisConsole = console;
        }
        public void Run() {
            while (!ThisConsole.environment.ReadyToExit) {
                var input = ThisConsole.environment.ReadLine();
                MatchOperations(input);
                if (ThisConsole.environment.ReadyToExit) continue;
                if (ThisConsole.environment.ReadyToOutputVars) {
                    ListVariables();
                    continue;
                }
                RunScriptLine(input);
            }
        }
        void MatchOperations(string input) {
            foreach (var operation in ThisConsole.environment.Operations) {
                if (input.Equals(operation.Key, StringComparison.OrdinalIgnoreCase)) {
                    operation.Value?.Invoke();
                    break;
                }
            }
        }
        void ListVariables() {
            if (ThisConsole.context?.Variables != null && ThisConsole.context.Variables.Any()) {
                "== Variables ==".Log();
                foreach (var variable in ThisConsole.context.Variables) {
                    $"{variable.Name} ({variable.Type}): {variable.Value ?? "null"}".Log();
                }
                "===============".Log();
            } else {
                "No variables defined.".Log();
            }
            ThisConsole.environment.ReSetReadyToOutputVars();
        }
        void RunScriptLine(string input) {
            try {
                if (ThisConsole.context == null) {
                    ThisConsole.context = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.RunAsync(input, ThisConsole.options, ThisConsole.globals, typeof(T)).Result;
                    ThisConsole.context.ReturnValue?.Log();
                } else
                    ThisConsole.context = ThisConsole.context.ContinueWithAsync(input).Result;
            } catch (CompilationErrorException e) {
                string.Join(Environment.NewLine, e.Diagnostics).LogError();
            }
        }
    }
    public class CSharpScriptConsole<T> {
        //Other Components
        public Environment<T> environment { get; }
        public Runner<T> runner { get; }
        //Options:before Run
        public T? globals { get; private set; }
        public ScriptOptions options => ScriptOptions.Default
                .WithImports(usings)
                .WithReferences(references);
        List<string> usings = [];
        List<Assembly> references = [];
        //Context:after Run
        public ScriptState<object> context = null;

        public CSharpScriptConsole() {
            this.environment = new Environment<T>(this);
            this.runner = new Runner<T>(this);
        }
        public CSharpScriptConsole(T? globals) {
            this.environment = new Environment<T>(this);
            this.runner = new Runner<T>(this);
            this.globals = globals;
        }
        public CSharpScriptConsole<T> Import(string namespaceName) {
            if (context != null) "Alread Created. Can't Import on Run-Time.".LogError();
            usings.Add(namespaceName);
            return this;
        }
        public CSharpScriptConsole<T> Reference(Assembly assembly) {
            if (context != null) "Alread Created. Can't Reference on Run-Time.".LogError();
            references.Add(assembly);
            return this;
        }
        public CSharpScriptConsole<T> SetGlobals(T globals) {
            if (context != null) "Alread Created. Can't Set Public Variables and Methods on Run-Time.".LogError();
            this.globals = globals;
            return this;
        }
        public void Run() {
            RunChecks();
            Tips();
            runner.Run();
        }
        void Tips() {
            "C# Interactive Console. Enter your code below:".Log();
            "===== Tips =====".Log();
            "[vars]: List all defined Variables and Methods.".Log();
            "[exit]: Exit the console.".Log();
            "================".Log();
        }
        void RunChecks() {
            var type = typeof(T);
            if (usings.Contains("System.Linq") && !references.Contains(typeof(Enumerable).Assembly)) {
                $"Warning: Cannot import 'System.Linq' without adding 'typeof(Enumerable).Assembly' Assembly to References.".LogWarning();
            }
            if (!type.IsVisible) {
                $"Warning: Cannot access '{type.Name}'. Make it public and not internal or it cannot be used.".LogWarning();
            }
            foreach (var field in type.GetFields()) {
                if (!field.IsPublic) {
                    $"Warning: Cannot access '{field.Name}'. Make it Public or it cannot be used.".LogWarning();
                }
            }
        }
    }
}