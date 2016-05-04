using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class TransformCommand : BaseCommand<TransformCommand>
    {
        private delegate string Replacement(string original);

        protected override void SetupCommands()
        {
            SetupCommand(PackageIds.cmdTitleCaseTransform, new Replacement(x => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(x)));
            SetupCommand(PackageIds.cmdReverseTransform, new Replacement(x => new string(x.Reverse().ToArray())));
            SetupCommand(PackageIds.cmdNormalizeTransform, new Replacement(x => RemoveDiacritics(x)));
            SetupCommand(PackageIds.cmdMd5Transform, new Replacement(x => Hash(x, new MD5CryptoServiceProvider())));
            SetupCommand(PackageIds.cmdSha1Transform, new Replacement(x => Hash(x, new SHA1CryptoServiceProvider())));
            SetupCommand(PackageIds.cmdSha256Transform, new Replacement(x => Hash(x, new SHA256CryptoServiceProvider())));
            SetupCommand(PackageIds.cmdSha384Transform, new Replacement(x => Hash(x, new SHA384CryptoServiceProvider())));
            SetupCommand(PackageIds.cmdSha512Transform, new Replacement(x => Hash(x, new SHA512CryptoServiceProvider())));
        }

        private void SetupCommand(int command, Replacement callback)
        {
            CommandID commandId = new CommandID(PackageGuids.guidTransformCmdSet, command);
            OleMenuCommand menuCommand = new OleMenuCommand((s, e) => Replace(callback), commandId);
            CommandService.AddCommand(menuCommand);
        }

        private static string RemoveDiacritics(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        private static string Hash(string original, HashAlgorithm algorithm)
        {
            byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(original));
            algorithm.Dispose();

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture).ToLowerInvariant());
            }

            return sb.ToString();
        }

        private void Replace(Replacement callback)
        {
            var document = GetTextDocument();
            string result = callback(document.Selection.Text);

            if (result == document.Selection.Text)
                return;

            using (UndoContext(callback.Method.Name))
            {
                document.Selection.Insert(result, 0);
            }
        }
    }
}
