using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

using Microsoft.Extensions.Logging;

namespace WikiMerge
{
    public class AttachmentFixer
    {
        private readonly string sourcePath;
        private readonly string targetPath;
        private bool rename;
        private DirectoryInfo directory;
        private ILogger logger;

        public AttachmentFixer(string sourcePath, string targetPath, bool rename, ILogger logger)
        {
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;
            this.rename = rename;
            this.logger = logger;
        }

        public string Replace(Match match)
        {
            if (directory == null)
            {
                // Create/assign on capture only - avoids empty directory if no attachments
                directory = Directory.CreateDirectory(targetPath);
                logger.LogInformation($"Local attachments folder: ${targetPath}");
            }

            var attachmentName = match.Groups["name"].Value;
            var caption = match.Groups["caption"].Value;

            // Copy it so we can find it
            // Can have %20 etc
            var fileName = HttpUtility.UrlDecode(attachmentName);
            var fi = new FileInfo(fileName);

            var targetFile = rename ? $"image-{Guid.NewGuid()}{fi.Extension}" : fileName;
            var targetName = Path.Combine(targetPath, targetFile);

            // Handles images in subfolders of the root attachments folder
            var targetDirectory = Path.GetDirectoryName(targetName);
            Directory.CreateDirectory(targetDirectory);
            File.Copy(Path.Combine(sourcePath, fileName), targetName, true);

            // And change the path to relative to where we are and potentially a new name
            return $"[{caption}]({Path.Combine(directory.Name, HttpUtility.UrlEncode(targetFile))})";
        }
    }
}
