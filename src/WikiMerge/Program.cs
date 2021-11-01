using System;

using CommandLine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WikiMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Based on https://wildermuth.com/2020/08/02/NET-Core-Console-Apps---A-Better-Way
                Parser.Default.ParseArguments<MergerOptions>(args)
                    .WithParsed(options =>
                    {
                        Console.WriteLine("wikimerge");
                        Console.WriteLine(
                            $"Merging {options.SourceWiki}/{options.SourcePath} to {options.TargetPath}");
                        Console.WriteLine();

                        var host = Host.CreateDefaultBuilder()
                            .ConfigureServices((b, c) => { c.AddSingleton(options); })
                            .ConfigureLogging(bldr =>
                            {
                                bldr.ClearProviders();
                                bldr.AddConsole()
                                    .SetMinimumLevel(LogLevel.Error);
                            })
                            .Build();

                        var merger = ActivatorUtilities.CreateInstance<Merger>(host.Services);
                        merger.Merge(options);
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}