namespace WikiMerge
{
    /// <summary>
    /// Wiki information
    /// </summary>
    public class Wiki
    {
        /// <summary>
        /// Get or set the name of the wiki
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Get or set the path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Get or set the directory we are checked out to.
        /// </summary>
        public string CheckoutDirectory { get; set; }

        /// <summary>
        /// Get or set the branch name we are operating on.
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Get or set the credentials for authentication
        /// </summary>
        public Credentials Credentials { get; set; }
    }
}
