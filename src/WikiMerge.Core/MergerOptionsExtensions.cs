using System;
using System.IO;
using System.Net;

using LibGit2Sharp;

namespace WikiMerge
{
    public static class MergerOptionsExtensions
    {
        public static Credentials ToCredentials(this MergerOptions options)
        {
            return new Credentials
            {
                Username = options.Username ?? "pat",
                Password = options.Password
            };
        }

        public static Signature ToSignature(this MergerOptions options)
        {
            return new Signature(options.Committer, options.CommitterEmail, DateTimeOffset.UtcNow);
        }

        public static Wiki ToSourceWiki(this MergerOptions options)
        {
            return options.ToWiki(options.SourceWiki, options.SourcePath, options.SourceBranch);
        }

        public static Wiki ToTargetWiki(this MergerOptions options)
        {
            return options.ToWiki(options.TargetWiki, options.TargetPath, options.TargetBranch);
        }

        public static Wiki ToWiki(this MergerOptions options, string source, string path, string branch)
        {
            var uri = new Uri(source);
            var url = source.IsRepo() ? source : null;

            // TODO: Is / always the separator?
            var parts = uri.AbsolutePath.Split('/');
            // Nice little C# 8.0 feature gets last array element
            var name = parts[^1];

            return new Wiki
            {
                Name = name,
                Url = url,
                BranchName = branch,
                Path = path.WikiEncode().FixupPath(),
                CheckoutDirectory = source.IsRepo() ? Path.Combine(options.WorkingDirectory, name) : source,
                Credentials = options.ToCredentials()
            };
        }

        public static bool IsRepo(this string source)
        {
            // Bit of a hack but best we could find
            var uri = new Uri(source);
            switch (uri.Scheme.ToLowerInvariant())
            {
                case "http":
                case "https":
                case "git":
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determine the attachments path for a wiki directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string AttachmentPath(this string path)
        {
            // Note that the directory might not exist if we have no attachments
            return Path.Combine(path, ".attachments");
        }

        /// <summary>
        /// Encode to the wiki file naming convention
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string WikiEncode(this string name)
        {
            var result = name;

            if (!string.IsNullOrEmpty(result))
            {
                // Handle special characters 
                result = WebUtility.UrlEncode(result);
                // Specific replaces for wiki names
                result = result?.Replace("-", "%2D");
                result = result?.Replace("+", "-");
            }

            return result;
        }

        /// <summary>
        /// Decode from the wiki file naming convention
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string WikiDecode(this string name)
        {
            var result = name;
            if (!string.IsNullOrEmpty(result))
            {
                // Replace the hyphens first so we don't confuse with %2D
                result = result?.Replace('-', ' ');
                result = WebUtility.UrlDecode(result);
            }

            return result;
        }

        /// <summary>
        /// Fixes up a path after url encoding it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FixupPath(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.IndexOf("%3A", StringComparison.InvariantCultureIgnoreCase) == 1)
                {
                    // Just replace the first %2D with a colon as it's a drive letter
                    value = $"{value[0]}:{value.Substring(4)}";
                }
                // So it works for path
                value = value?.Replace("%5C", "\\");
                value = value?.Replace("%2F", "/");
            }

            return value;
        }
    }
}
