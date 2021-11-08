using System;

using LibGit2Sharp;

using NCheck;

using NUnit.Framework;

namespace WikiMerge.Test
{
    [TestFixture]
    public class MergerOptionsExtensionsFixture
    {
        [Test]
        public void ToCredentials()
        {
            var options = new MergerOptions
            {
                Username = "Foo",
                Password = "Bar"
            };

            var expected = new Credentials
            {
                Username = "Foo",
                Password = "Bar"
            };

            var candidate = options.ToCredentials();

            var checker = new CheckerFactory();

            checker.Check(expected, candidate);
        }

        [TestCase(null)]
        [TestCase("")]
        public void ToCredentialsNoUser(string userName)
        {
            var options = new MergerOptions
            {
                Username = userName,
                Password = "Bar"
            };

            var expected = new Credentials
            {
                Username = "pat",
                Password = "Bar"
            };

            var candidate = options.ToCredentials();

            var checker = new CheckerFactory();

            checker.Check(expected, candidate);
        }

        [Test]
        public void ToSignature()
        {
            var options = new MergerOptions
            {
                Committer = "WikiMerge",
                CommitterEmail = "wikimerge@devops.com"
            };

            var expected = new Signature("WikiMerge", "wikimerge@devops.com", DateTimeOffset.UtcNow);

            var candidate = options.ToSignature();

            var checker = new CheckerFactory();
            checker.Compare<Signature>(x => x.When).Ignore();

            checker.Check(expected, candidate);
        }

        [TestCase("My Title", "My-Title")]
        [TestCase("Conceptual Level: Behaviour", "Conceptual-Level%3A-Behaviour")]
        [TestCase("Non-Functional Requirements", "Non%2DFunctional-Requirements")]
        public void WikiEncode(string name, string expected)
        {
            var candidate = name.WikiEncode();

            Assert.That(candidate, Is.EqualTo(expected), "Name differs");
        }

        [TestCase("C:\\Sample%20Wiki\\S1", "C:\\Sample%20Wiki\\S1")]
        [TestCase("C:\\Sample.wiki\\S2-Foo/S3: Bar", "C:\\Sample.wiki\\S2%2DFoo/S3%3A-Bar")]
        [TestCase("C:\\Sample.wiki\\S2-Foo\\S3: Bar", "C:\\Sample.wiki\\S2%2DFoo\\S3%3A-Bar")]
        public void FixupPath(string name, string expected)
        {
            var candidate = name.WikiEncode().FixupPath();

            Assert.That(candidate, Is.EqualTo(expected), "Name differs");
        }

        [TestCase("My-Title", "My Title")]
        [TestCase("Conceptual-Level%3A-Behaviour", "Conceptual Level: Behaviour")]
        [TestCase("Non%2DFunctional-Requirements", "Non-Functional Requirements")]
        public void WikiDecode(string name, string expected)
        {
            var candidate = name.WikiDecode();

            Assert.That(candidate, Is.EqualTo(expected), "Name differs");
        }

        [TestCase("http://github.com/phatcher/wikimerge.wiki", true)]
        [TestCase("https://github.com/phatcher/wikimerge.wiki", true)]
        [TestCase("git://github.com/phatcher/wikimerge.wiki", true)]

        [TestCase("git@github.com:phatcher/wikimerge.wiki", false)]
        [TestCase("C:\\Devel\\wikimerge.wiki", false)]
        public void IsRepo(string path, bool expected)
        {
            var candidate = path.IsRepo();

            Assert.That(candidate, Is.EqualTo(expected));
        }

        [Test]
        public void RepoToWiki()
        {
            var source = "https://github.com/phatcher/wikimerge.wiki";
            var path = "Foo";
            var branch = "wikiMaster";

            var options = new MergerOptions
            {
                WorkingDirectory = "C:\\Devel",
                Username = "Foo",
                Password = "Bar"
            };

            var expected = new Wiki
            {
                Name = "wikimerge.wiki",
                BranchName = "wikiMaster",
                Url = source,
                Path = "Foo",
                CheckoutDirectory = "C:\\Devel\\wikimerge.wiki",
                Credentials = new Credentials
                {
                    Username = "Foo",
                    Password = "Bar"
                }
            };

            var candidate = options.ToWiki(source, path, branch);

            var checker = new CheckerFactory();

            checker.Check(expected, candidate);
        }

        [Test]
        public void PathToWiki()
        {
            var source = "C:\\Devel\\wikimerge.wiki";
            var path = "Foo";
            var branch = "wikiMaster";

            var options = new MergerOptions
            {
                Username = "Foo",
                Password = "Bar"
            };

            var expected = new Wiki
            {
                Name = "wikimerge.wiki",
                BranchName = "wikiMaster",
                Path = "Foo",
                CheckoutDirectory = source,
                Credentials = new Credentials
                {
                    Username = "Foo",
                    Password = "Bar"
                }
            };

            var candidate = options.ToWiki(source, path, branch);

            var checker = new CheckerFactory();

            checker.Check(expected, candidate);
        }
    }
}
