using Microsoft.Build.Evaluation;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using uzLib.Lite.Core.Input;
using uzLib.Lite.Extensions;

namespace ZWSetup.Shell.Controller
{
    using Lib.Controller;

    public static class TesterController
    {
        public static void GenerateSetupInTester(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            string prettyName = name.ToLower().FirstCharToUpper();

            string testerPath = SetupController.TesterPath;

            if (string.IsNullOrEmpty(testerPath))
                throw new Exception("Tester path is null. It must be specified by going to 'Enter Package Creator -> Locate Tester Path'.");

            if (!testerPath.IsDirectory() && Path.GetExtension(testerPath) == "csproj" || testerPath.IsDirectory())
                throw new Exception("The stored Tester isn't a csproj file.");

            // First, we will create the template && store it on a file

            var c = new CodeTypeDeclaration($"{prettyName}Setup")
            {
                Attributes = MemberAttributes.Public,
                IsClass = true,
                Members =
                {
                    new CodeMemberMethod() { Name = "OnSetup", Attributes = MemberAttributes.Public | MemberAttributes.Static}
                }
            };

            var ns = new CodeNamespace($"ZWSetup.Package.{prettyName}") { Types = { c } };

            var cu = new CodeCompileUnit() { Namespaces = { ns } };

            var provider = CodeDomProvider.CreateProvider("CSharp");

            var sb = new StringBuilder();
            using (var sourceWriter = new StringWriter(sb))
                provider.GenerateCodeFromCompileUnit(cu, sourceWriter, new CodeGeneratorOptions());

            var text = sb.ToString();

            string testerFolderPath = Path.GetDirectoryName(testerPath),
                   saveFilePath = Path.Combine(testerFolderPath, "Setups", prettyName + ".cs"),
                   saveFolderPath = Path.GetDirectoryName(saveFilePath);

            if (!Directory.Exists(saveFolderPath))
                Directory.CreateDirectory(saveFolderPath);

            bool overwrite = true;
            if (File.Exists(saveFilePath))
                overwrite = SmartInput.NextConfirm("Do you want to overwrite the current file?");

            if (overwrite)
                File.WriteAllText(saveFilePath, text);

            // Then, we will save it on the csproj

            // Solved issue thanks to: https://stackoverflow.com/a/44260284/3286975

            Project project = new Project(testerPath);
            project.AddItem("Compile", IOHelper.MakeRelativePath(testerFolderPath, saveFilePath));
            project.Save();
        }
    }
}