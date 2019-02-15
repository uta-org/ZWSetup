using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWSetup.Shell.Controller
{
    using Lib.Controller;

    public static class TesterController
    {
        public static void GenerateSetupInTester(string name)
        {
            string testerPath = SetupController.TesterPath;

            // First, we will create the file

            var c = new CodeTypeDeclaration("MyType")
            {
                Attributes = MemberAttributes.Public,
                IsPartial = true,
                IsStruct = true,
                Members =
                {
                    new CodeMemberField(new CodeTypeReference(typeof(int)), "Test") {InitExpression = new CodePrimitiveExpression(1)}
                }
            };

            var ns = new CodeNamespace("MyProject.MyNameSpace") { Types = { c } };

            var cu = new CodeCompileUnit() { Namespaces = { ns } };

            var provider = CodeDomProvider.CreateProvider("CSharp");
            var sb = new StringBuilder();
            using (var sourceWriter = new StringWriter(sb))
            {
                provider.GenerateCodeFromCompileUnit(cu, sourceWriter, new CodeGeneratorOptions());
            }
            var text = sb.ToString();
            Console.WriteLine(text);

            // Then, we will save it on the csproj
        }
    }
}