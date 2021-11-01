using CommandLine;

namespace WikiMerge
{
    /// <summary>
    /// Options to control the wiki merging process
    /// </summary>
    public class MergerOptions
    {
        [Option('s', "sourceWiki", Required = true, HelpText = "Source wiki to copy from, either url or directory name")]
        public string SourceWiki { get; set; }

        [Option("sourceBranch", Default = "wikiMaster", Required = false, HelpText = "Source branch to process")]
        public string SourceBranch { get; set; }

        /// <summary>
        /// Get or set the source path we are processing
        /// </summary>
        [Option('p', "sourcePath", Required = true, HelpText = "Source path to process")]
        public string SourcePath { get; set; }

        [Option('t', "targetWiki", Required = true, HelpText = "Target wiki to merge into, either url or directory name")]
        public string TargetWiki { get; set; }

        [Option("targetBranch", Default = "wikiMaster", Required = false, HelpText = "Target base branch to merge into")]
        public string TargetBranch { get; set; }

        /// <summary>
        /// Get or set the target path we will use
        /// </summary>
        [Option('q', "targetPath", Required = true, HelpText = "Target path within wiki")]
        public string TargetPath { get; set; }

        /// <summary>
        /// Get or set user name for git
        /// </summary>
        [Option('u', "username", HelpText = "Username for git authentication")]
        public string Username { get; set; }

        /// <summary>
        /// Get or set password for git
        /// </summary>
        [Option("password", HelpText = "Password for git authentication")]
        public string Password { get; set; }

        /// <summary>
        /// Get or set the committer name
        /// </summary>
        [Option('c', "committer", Default = "WikiMerge", Required = false, HelpText = "Committer name for git")]
        public string Committer { get; set; }

        /// <summary>
        /// Get or set the committer email
        /// </summary>
        [Option('e', "email", Default = "wikimerge@devops", Required = false, HelpText = "Committer email for git")]
        public string CommitterEmail { get; set; }

        [Option('w', "workingDirectory", Required = false, HelpText = "Working directory to checkout wikis")]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Get or set the position in the Target order file
        /// </summary>
        public string Position { get; set; }

        [Option('o', "override", HelpText = "Override existing files")]
        public bool OverrideExisting { get; set; }

        /// <summary>
        /// Get or set whether we rename images when merging
        /// </summary>
        [Option('i', "renameImages", HelpText = "Whether we rename images when merging")]
        public bool RenameImages { get; set; }

        /// <summary>
        /// Get or set the name of branch to use for merging
        /// </summary>
        [Option('m', "mergeBranch", Required = false, HelpText = "Specific branch to put the merge in")]
        public string MergeBranch { get; set; }
    }
}