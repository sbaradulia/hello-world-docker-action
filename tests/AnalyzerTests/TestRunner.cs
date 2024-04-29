using System;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Shared;

namespace AnalyzerTests
{
    public class TestRunner<T> where T : IIncrementalGenerator, new()
    {
        private readonly TargetFramework[] _allFrameworks = {
            TargetFramework.Net6_0,
            TargetFramework.Net7_0,
            TargetFramework.Net8_0,
#if THOROUGH
            TargetFramework.Net4_6_1,
            TargetFramework.Net4_8,
            TargetFramework.NetCoreApp3_1,
            TargetFramework.Net5_0,
#endif
        };

        private Action<ImmutableArray<Diagnostic>>? _validationMethod;
        private string? _source;

        public void RunOnAllFrameworks(bool ignoreInitialCompilationErrors = false)
        {
            RunOn(
                ignoreInitialCompilationErrors,
                _allFrameworks);
        }

        public TestRunner<T> WithSource(string source)
        {
            _source = source;
            return this;
        }

        public TestRunner<T> ValidateWith(Action<ImmutableArray<Diagnostic>> method)
        {
            _validationMethod = method;

            return this;
        }

        private void RunOn(bool ignoreInitialCompilationErrors,
            params TargetFramework[] frameworks)
        {
            _ = _source ?? throw new InvalidOperationException("No source!");
            _ = _validationMethod ?? throw new InvalidOperationException("No validation method!");

// #if NET7_0_OR_GREATER
//             // Only run .NET 7 tests when using the .NET 7 SDK (prevents assembly versioning issues with <6.0)
//             frameworks = frameworks.Where(framework => framework >= TargetFramework.Net7_0).ToArray();
// #elif NET6_0
//             // Alternatively, only run non-net7 tests when using the .NET 6 target
//             // as .NET 6 will use the .NET standard Vogen binary (without C#11 support)
//             frameworks = frameworks.Where(framework => framework != TargetFramework.Net7_0).ToArray();
// #endif

            // Skips tests targeting specific frameworks that were excluded above
            // NOTE: Requires [SkippableFact] attribute to be added to single-framework tests
            Skip.If(frameworks.Length == 0);

            foreach (var eachFramework in frameworks)
            {
                using var scope = new AssertionScope();

                var (diagnostics, _) = GetGeneratedOutput(
                    _source,
                    eachFramework,
                    ignoreInitialCompilationErrors);

                _validationMethod(diagnostics);
            }
        }

        private static (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput(
            string source,
            TargetFramework targetFramework, 
            bool ignoreInitialCompilationErrors)
        {
            var results = new ProjectBuilder()
                .WithSource(source)
                .WithTargetFramework(targetFramework)
                .GetGeneratedOutput<T>(ignoreInitialCompilationErrors);

            return results;
        }

    }
}