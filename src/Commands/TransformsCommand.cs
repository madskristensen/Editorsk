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
            SetupCommand(PackageIds.cmdTitleCase, new Replacement(x => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(x)));
            SetupCommand(PackageIds.cmdReverse, new Replacement(x => new string(x.Reverse().ToArray())));
            SetupCommand(PackageIds.cmdNormalize, new Replacement(x => RemoveDiacritics(x)));
            SetupCommand(PackageIds.cmdMd5, new Replacement(x => Hash(x, new MD5CryptoServiceProvider())));
            SetupCommand(PackageIds.cmdSha1, new Replacement(x => Hash(x, new SHA1CryptoServiceProvider())));
            SetupCommand(PackageIds.cmdSha256, new Replacement(x => Hash(x, new SHA256CryptoServiceProvider())));
            SetupCommand(PackageIds.cmdSha384, new Replacement(x => Hash(x, new SHA384CryptoServiceProvider())));
            SetupCommand(PackageIds.cmdSha512, new Replacement(x => Hash(x, new SHA512CryptoServiceProvider())));
        }

        private void SetupCommand(int commandId, Replacement callback)
        {
            RegisterCommand(PackageGuids.guidTransformCmdSet, commandId, () => Execute(callback));
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

        private void Execute(Replacement callback)
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
