using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace WikiMerge
{
    public class AttachmentFixer
    {
        private readonly string sourcePath;
        private readonly string targetPath;
        private readonly bool rename;
        private readonly Action<string, string> fileAction;

        const string attachmentFinder = @"\[(?<caption>.+)\]\((?<path>/.attachments)/(?<name>.+)\)";

        public AttachmentFixer(string sourcePath, string targetPath, bool rename, Action<string, string> fileAction = null)
        {
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;
            this.rename = rename;
            this.fileAction = fileAction ?? ((x, y) => { });
        }

        public string Fix(string source)
        {
            // Fix up the references
            var result = Regex.Replace(source, attachmentFinder, Replace);

            return result;
        }

        public string Replace(Match match)
        {
            var attachmentName = match.Groups["name"].Value;
            var caption = match.Groups["caption"].Value;

            // Decode it so we can find it
            // Can have %20 etc
            var fileName = HttpUtility.UrlDecode(attachmentName);
            var fi = new FileInfo(fileName);

            // Generate a new name if we need to, otherwise us the original
            var targetFile = rename ? $"image-{Guid.NewGuid()}{fi.Extension}" : fileName;

            // Do the action, typically copy the file
            fileAction(Path.Combine(sourcePath, fileName), Path.Combine(targetPath, targetFile));

            // And change the path to relative to where we want to be with the encoded name
            var directory = new DirectoryInfo(targetPath);
            return $"[{caption}]({Path.Combine(directory.Name, HttpUtility.UrlEncode(targetFile))})";
        }
    }
}