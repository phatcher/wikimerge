# Introduction

The [WikiMerge project](https://github.com/phatcher/wikimerge) it a dotnet tool that allows you to merge part of, or an entire Azure DevOps wiki into another Azure DevOps wiki. It will correctly fix up the `.order` file in the target wiki and copy, and optionally rename, any referenced images.

This can be useful when initializing new projects into which you wish to copy reference or templated documentation.

# Installation

This will install the latest official version from nuget

`dotnet tool install dotnet-wikimerge -g`

# Getting started

Once you have installed the tool, clone your wiki repository and then enter the following command

`wikimerge -s <sourceRepo> -p <sourcePath> -t <targetRepo> -q <targetPath> --username <username> --password <password>`

## Options

The options default settings have been initialized to values that "just work", if need something different you should be able to adjust the option settings to suit; if not raise a ticket and we can see how to help.

* *-s*, *--sourceWiki*: Required. Source wiki to copy from, either url or directory name
* *--sourceBranch*: (Default: wikiMaster) Source branch to use
* *-p*, *--sourcePath*: Required. Source path to process
* *-t*, *--targetWiki*: Required. Target wiki to merge into, either url or directory name
* *--targetBranch*: (Default: wikiMaster) Target base branch to merge into
* *-q*, *--targetPath:* Required. Target path within wiki
* *-u*, *--username*: Username for git authentication
* *--password*: Password for git authentication, Required if passing repository urls
* *-c*, *--committer:* (Default: WikiMerge) Committer for git
* *-e*, *--email*: (Default: wikimerge@devops) Committer email for git
* *-w*, *--workingDirectory*: Working directory to checkout wikis
* *-o*, *--override*: Override existing files
* *-i*, *--renameImages*: Whether we rename images when merging
* *-m*, *--mergeBranch*: Specific branch to put the merge in

## Scenarios

The tool has been designed to support multiple scenarios...

1. Checkout the source and target repositories, perform the merge and commit the changes
1. Work on existing repositories, perform the merge but leave the git state alone

Depending on where you are running this from and whether you want to split the process into separate stages you can adopt the appropriate mode

## Local builds

After building from the root directory of the project 

`dotnet tool install dotnet-wikimerge -g --add-source src\WikiMerge\bin\Debug`

or 

`dotnet tool update dotnet-wikimerge -g --add-source src\WikiMerge\bin\Debug`


# License

The code is available under the [MIT License](http://en.wikipedia.org/wiki/MIT_License), for more information see the [License file][1] in the GitHub repository.

 [1]: https://github.com/phatcher/wikimerge/blob/main/License.md
