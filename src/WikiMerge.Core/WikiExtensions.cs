using System.IO;

using LibGit2Sharp;

namespace WikiMerge
{
    public static class WikiExtensions
    {
        public static Repository Clone(this Wiki wiki)
        {
            var gitPath = Path.Combine(wiki.CheckoutDirectory, ".git");

            // See if we are already checked out
            if (!Repository.IsValid(gitPath))
            {
                if (string.IsNullOrEmpty(wiki.Url))
                {
                    // Don't attempt to clone if we don't have a url
                    return null;
                }

                // Ensure the target exists
                Directory.CreateDirectory(wiki.CheckoutDirectory);

                var options = new CloneOptions
                {
                    BranchName = wiki.BranchName,
                    CredentialsProvider = (url, user, cred) => wiki.Credentials.ToGitCredentials()
                };

                gitPath = Repository.Clone(wiki.Url, wiki.CheckoutDirectory, options);
            }

            return new Repository(gitPath);
        }

        public static string AttachmentPath(this Wiki wiki)
        {
            return wiki.CheckoutDirectory.AttachmentPath();
        }

        public static UsernamePasswordCredentials ToGitCredentials(this Credentials credentials)
        {
            return new UsernamePasswordCredentials
            {
                Username = credentials.Username,
                Password = credentials.Password
            };
        }
    }
}