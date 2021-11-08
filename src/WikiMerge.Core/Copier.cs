using System.IO;

namespace WikiMerge
{
    public class Copier
    {
        public Copier(string sourceImagePath, string targetImagePath, bool renameImages)
        {
            SourceImagePath = sourceImagePath;
            TargetImagePath = targetImagePath;
            RenameImages = renameImages;
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
            // TODO: Detect if destination file exists and obey overwrite options
            if (!File.Exists(targetFile))
            {
                // Detect whether we need to process the file e.g. just markdown otherwise just copy it?
                if (fileName.IsMarkdown())
                {
                    // Get the data
                    var source = File.ReadAllText(fileName);

                    static void Copy(string sourceImage, string targetImage)
                    {
                        var targetDirectory = Path.GetDirectoryName(targetImage);
                        // Handles images in subfolders of the root attachments folder
                        Directory.CreateDirectory(targetDirectory);
                        File.Copy(sourceImage, targetImage, true);
                    }

                    // And fix it's images
                    var result = source.FixAttachmentReferences(SourceImagePath, TargetImagePath, RenameImages, Copy);

                    // Write out the modified version
                    File.WriteAllText(targetFile, result);
                }
                else
                {
                    File.Copy(fileName, targetFile);
                }
            }
        }
    }
}