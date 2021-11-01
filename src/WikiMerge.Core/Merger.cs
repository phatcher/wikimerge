using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using LibGit2Sharp;

using Microsoft.Extensions.Logging;

namespace WikiMerge
{
    public class Merger
    {
        private readonly ILogger logger;

        public Merger(ILogger<Merger> logger)
        {
            this.logger = logger;
        }

        public Wiki Source { get; set; }

        public Wiki Target { get; set; }

        public void Merge(MergerOptions options)
        {
            if (string.IsNullOrEmpty(options.WorkingDirectory))
            {
                options.WorkingDirectory = Directory.GetCurrentDirectory();
            }
            else if (!Directory.Exists(options.WorkingDirectory))
            {
                logger.LogError($"Working directory '{options.WorkingDirectory}' does not exist");
                return;
            }

            Source = options.ToSourceWiki();
            Source.Clone();

            var sourcePath = Path.Combine(Source.CheckoutDirectory, Source.Path);

            Target = options.ToTargetWiki();
            var targetRepo = Target.Clone();

            // See if we need to create a target directory and associated order files
            var targetPath = Path.Combine(Target.CheckoutDirectory, Target.Path);

            // Make it as might need an intermediate directory
            var targetDir = Directory.CreateDirectory(targetPath);

            // Copy the tree
            CopyFolder(sourcePath, targetPath, options);

            // Fix up the target directory order file
            VerifyOrder(targetDir);

            BranchAndCommit(targetRepo, options);
        }

        private void BranchAndCommit(Repository targetRepo, MergerOptions options)
        {
            // TODO: Is this sufficient might we want to clone but not do the branch/stage/commit in the tool?
            if (string.IsNullOrEmpty(Target.Url))
            {
                return;
            }

            // Create the branch if we want a separate merge branch
            if (!string.IsNullOrEmpty(options.MergeBranch))
            {
                var branch = targetRepo.CreateBranch(options.MergeBranch);

                Commands.Checkout(targetRepo, branch);
            }

            // Add all of the files to git repo - will pick up attachments etc
            Commands.Stage(targetRepo, "*");
            targetRepo.Commit($"Imported `{Source.Name}\\{Source.Path}` to {Target.Path}", options.ToSignature(), options.ToSignature());
        }

        private void CopyFolder(string sourceFolder, string destFolder, MergerOptions options)
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
                CopyFile(file, dest, options);
            }

            var folders = Directory.GetDirectories(sourceFolder);
            foreach (var folder in folders)
            {
                var name = Path.GetFileName(folder);
                var dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest, options);
            }
        }

        private void VerifyOrder(DirectoryInfo path)
        {
            var name = path.Name;
            var parent = path.Parent;
            var order = parent.GetFiles(".order").ToList().FirstOrDefault();
            if (order == null)
            {
                using (var writer = new StreamWriter(Path.Combine(parent.FullName, ".order")))
                {
                    writer.Write($"{name}");
                }
                VerifyOrder(parent);
            }
            else
            {
                // TODO: Check the name is in there if not add it at the end/position
            }
        }

        private void CopyFile(string fileName, string targetFile, MergerOptions options)
        {
            // TODO: Detect .order files - needed?
            // TODO: Parse the file and grab images as well
            // TODO: Detect if destination file exists and obey overwrite options
            if (!File.Exists(targetFile))
            {
                File.Copy(fileName, targetFile);
                FixAttachmentReferences(targetFile, options);
            }
        }

        private void FixAttachmentReferences(string fileName, MergerOptions options)
        {
            const string attachmentFinder = @"\[(?<caption>.+)\]\((?<path>/.attachments)/(?<name>.+)\)";

            // Find the .attachments folders
            var attachmentSourcePath = Source.CheckoutDirectory.AttachmentPath();
            var attachmentTargetPath = Target.CheckoutDirectory.AttachmentPath();

            logger.LogInformation($"Attachments will be output to ${attachmentTargetPath}");

            var fixer = new AttachmentFixer(attachmentSourcePath, attachmentTargetPath, options.RenameImages, logger);

            // Get the data
            var source = File.ReadAllText(fileName);

            // Fix up the references, plus copy the files
            var result = Regex.Replace(source, attachmentFinder, fixer.Replace);

            // And write back the updated references
            File.WriteAllText(fileName, result);
        }
    }
}