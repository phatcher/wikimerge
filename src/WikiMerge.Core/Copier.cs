using System.IO;

using Microsoft.Extensions.Logging;

namespace WikiMerge
{
    public class Copier
    {
        private ILogger logger;

        public Copier(string sourceImagePath, string targetImagePath, bool renameImages, ILogger logger)
        {
            SourceImagePath = sourceImagePath;
            TargetImagePath = targetImagePath;
            RenameImages = renameImages;
            this.logger = logger;
        }

        public string SourceImagePath { get; }

        public string TargetImagePath { get; }

        public bool RenameImages { get; }

        public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            var files = Directory.GetFiles(sourceFolder);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var dest = Path.Combine(destFolder, name);
                CopyFile(file, dest);
            }

            var folders = Directory.GetDirectories(sourceFolder);
            foreach (var folder in folders)
            {
                var name = Path.GetFileName(folder);
                var dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        private void CopyFile(string fileName, string targetFile)
        {
            // TODO: Detect .order files - needed?
            // TODO: Detect if destination file exists and obey overwrite options
            if (!File.Exists(targetFile))
            {
                // Copy it
                File.Copy(fileName, targetFile);

                // And fix it's images
                targetFile.FixAttachmentReferences(SourceImagePath, TargetImagePath, RenameImages, logger);
            }
        }
    }
}