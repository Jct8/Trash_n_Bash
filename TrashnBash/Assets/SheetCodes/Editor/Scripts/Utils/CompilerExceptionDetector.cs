using System;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;

namespace SheetCodesEditor
{
    public class CompilerExceptionDetector
    {
        public static bool containsCompilerErrors { get; private set; }
        public static event Action onCompilerError;

        [DidReloadScripts(1)]
        private static void DetectExceptions()
        {
            containsCompilerErrors = false;
            CompilationPipeline.assemblyCompilationFinished += ProcessBatchModeCompileFinish;
        }

        private static void ProcessBatchModeCompileFinish(string s, CompilerMessage[] compilerMessages)
        {
            if (compilerMessages.Count(m => m.type == CompilerMessageType.Error) > 0)
            {
                containsCompilerErrors = true;
                onCompilerError?.Invoke();
            }
        }
    }
}