using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.JScript;
using System.Diagnostics;

namespace dotSC2TV
{
    public class JSEvaluator
    {

        #region Private Members

        public static Type _evaluatorType;
        private static object _evaluatorInstance;
        private static readonly string _jscriptEvalClass =
        @"import System;
        public class JScriptEvaluator
        {
            public static function Eval(expression : String) : Object
            {
                return eval(expression);
            }
        }";

        #endregion

        #region Private Methods

        public static void Initialize()
        {

            CodeDomProvider compiler = new JScriptCodeProvider();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("system.dll");

            CompilerResults results = compiler.CompileAssemblyFromSource(parameters, _jscriptEvalClass);

            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("JScriptEvaluator");
            _evaluatorInstance = Activator.CreateInstance(_evaluatorType);
        }

        #endregion

        #region Public Methods

        public static object EvalObject(string expression)
        {
            if (_evaluatorInstance == null)
                Initialize();
            object result = null;
            try
            {
                result = _evaluatorType.InvokeMember(
                "Eval",
               BindingFlags.InvokeMethod,
                null,
               _evaluatorInstance,
                new object[] { expression }
               );
            }
            catch (TargetInvocationException e)
            {
                Debug.Print("JScript EvalObject: " + e.InnerException.Message);
            }

            return result;
        }
        public static object GetProperty(object obj, string propertyName)
        {
            return (object)obj.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
        }
        public static object GetField(object obj, string fieldName)
        {
            return (object)obj.GetType().InvokeMember(fieldName, BindingFlags.GetField, null, obj, null);
        }
        public static List<object> EvalArrayObject( string expression)
        {
            List<object> list = new List<object>();
            ArrayObject ar = EvalObject(expression) as ArrayObject;
            if (ar == null)
                return null;

            foreach (object i in ar)
            {
                list.Add(ar[i]);
            }
            return list;
        }
        public static string ReadPropertyValue(object obj, string propertyName)
        {
            return (string)((JSObject)obj)[propertyName];
        }
        #endregion
    }
}