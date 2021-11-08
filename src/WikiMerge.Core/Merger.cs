using System.IO;

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

            // Get the wikis
            Source = options.ToSourceWiki();
            Source.Clone();

            Target = options.ToTargetWiki();
            var targetRepo = Target.Clone();

            // Create a target directory as might need an intermediate directory
            var targetPath = Path.Combine(Target.CheckoutDirectory, Target.Path);
            var targetDir = Directory.CreateDirectory(targetPath);

            // Copy the tree
            logger.LogInformation($"Attachments will be output to ${Target.AttachmentPath()}");
            var sourcePath = Path.Combine(Source.CheckoutDirectory, Source.Path);
            var copier = new Copier(Source.AttachmentPath(), Target.AttachmentPath(), options.RenameImages);
            copier.CopyFolder(sourcePath, targetPath);

            // Fix up the target directory order file
            targetDir.VerifyOrder();

            // And do the git operations
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
    }
}