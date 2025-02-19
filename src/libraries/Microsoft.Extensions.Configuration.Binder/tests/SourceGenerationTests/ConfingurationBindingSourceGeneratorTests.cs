﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;
#endif
using SourceGenerators.Tests;
using Xunit;

namespace Microsoft.Extensions.Configuration.Binder.SourceGeneration.Tests
{
#if NETCOREAPP
    [ActiveIssue("https://github.com/dotnet/runtime/issues/52062", TestPlatforms.Browser)]
    public class ConfingurationBindingSourceGeneratorTests
    {
        private const string BindCallSampleCode = @"
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

public class Program
{
	public static void Main()
	{
		ConfigurationBuilder configurationBuilder = new();
		IConfigurationRoot config = configurationBuilder.Build();

		MyClass configObj = new();
		config.Bind(configObj);
        config.Bind(configObj, options => { })
        config.Bind(""key"", configObj);
	}

	public class MyClass
	{
		public string MyString { get; set; }
		public int MyInt { get; set; }
		public List<int> MyList { get; set; }
		public Dictionary<string, string> MyDictionary { get; set; }
        public Dictionary<string, MyClass2> MyComplexDictionary { get; set; }
	}

    public class MyClass2 { }
}";

        [Theory]
        [InlineData(LanguageVersion.Preview)]
        [InlineData(LanguageVersion.CSharp11)]
        public async Task TestBaseline_TestBindCallGen(LanguageVersion langVersion) =>
            await VerifyAgainstBaselineUsingFile("TestBindCallGen.generated.txt", BindCallSampleCode, langVersion);

        [Fact]
        public async Task TestBaseline_TestGetCallGen()
        {
            string testSourceCode = @"
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

public class Program
{
	public static void Main()
	{
		ConfigurationBuilder configurationBuilder = new();
		IConfigurationRoot config = configurationBuilder.Build();

		MyClass configObj = config.Get<MyClass>();
        configObj = config.Get(typeof(MyClass2));
        configObj = config.Get<MyClass>(binderOptions => { });
        configObj = config.Get(typeof(MyClass2), binderOptions => { });
	}

	public class MyClass
	{
		public string MyString { get; set; }
		public int MyInt { get; set; }
		public List<int> MyList { get; set; }
        public int[] MyArray { get; set; }
		public Dictionary<string, string> MyDictionary { get; set; }
	}

    public class MyClass2
    {
        public int MyInt { get; set; }
    }

    public class MyClass3
    {
        public int MyInt { get; set; }
    }

    public class MyClass4
    {
        public int MyInt { get; set; }
    }
}";

            await VerifyAgainstBaselineUsingFile("TestGetCallGen.generated.txt", testSourceCode);
        }

        [Fact]
        public async Task TestBaseline_TestGetValueCallGen()
        {
            string testSourceCode = @"
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;

public class Program
{
	public static void Main()
	{
		ConfigurationBuilder configurationBuilder = new();
		IConfigurationRoot config = configurationBuilder.Build();

		config.GetValue<int>(""key"");
        config.GetValue(typeof(bool?), ""key"");
        config.GetValue<MyClass>(""key"", new MyClass());
        config.GetValue<byte[]>(""key"", new byte[] { });
        config.GetValue(typeof(CultureInfo), ""key"", CultureInfo.InvariantCulture);
	}
	
	public class MyClass
	{
		public string MyString { get; set; }
		public int MyInt { get; set; }
		public List<int> MyList { get; set; }
        public int[] MyArray { get; set; }
		public Dictionary<string, string> MyDictionary { get; set; }
	}
}";

            await VerifyAgainstBaselineUsingFile("TestGetValueCallGen.generated.txt", testSourceCode);
        }

        [Fact]
        public async Task TestBaseline_TestConfigureCallGen()
        {
            string testSourceCode = @"
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
	public static void Main()
	{
        ConfigurationBuilder configurationBuilder = new();
		IConfiguration config = configurationBuilder.Build();
        IConfigurationSection section = config.GetSection(""MySection"");

		ServiceCollection services = new();
        services.Configure<MyClass>(section);
	}

	public class MyClass
	{
		public string MyString { get; set; }
		public int MyInt { get; set; }
		public List<int> MyList { get; set; }
        public List<MyClass2> MyList2 { get; set; }
		public Dictionary<string, string> MyDictionary { get; set; }
	}

    public class MyClass2
    {
        public int MyInt { get; set; }
    }
}";

            await VerifyAgainstBaselineUsingFile("TestConfigureCallGen.generated.txt", testSourceCode);
        }

        [Fact]
        public async Task TestBaseline_TestPrimitivesGen()
        {
            string testSourceCode = """
                using System;
                using System.Globalization;
                using Microsoft.Extensions.Configuration;

                public class Program
                {
                    public static void Main()
                    {
                        ConfigurationBuilder configurationBuilder = new();
                        IConfigurationRoot config = configurationBuilder.Build();

                        MyClass obj = new();
                        config.Bind(obj);
                    }

                    public class MyClass
                    {
                        public bool Prop0 { get; set; }
                        public byte Prop1 { get; set; }
                        public sbyte Prop2 { get; set; }
                        public char Prop3 { get; set; }
                        public double Prop4 { get; set; }
                        public string Prop5 { get; set; }
                        public int Prop6 { get; set; }
                        public short Prop8 { get; set; }
                        public long Prop9 { get; set; }
                        public float Prop10 { get; set; }
                        public ushort Prop13 { get; set; }
                        public uint Prop14 { get; set; }
                        public ulong Prop15 { get; set; }
                        public object Prop16 { get; set; }
                        public CultureInfo Prop17 { get; set; }
                        public DateTime Prop19 { get; set; }
                        public DateTimeOffset Prop20 { get; set; }
                        public decimal Prop21 { get; set; }
                        public TimeSpan Prop23 { get; set; }
                        public Guid Prop24 { get; set; }
                        public Uri Prop25 { get; set; }
                        public Version Prop26 { get; set; }
                        public DayOfWeek Prop27 { get; set; }
                        public Int128 Prop7 { get; set; }
                        public Half Prop11 { get; set; }
                        public UInt128 Prop12 { get; set; }
                        public DateOnly Prop18 { get; set; }
                        public TimeOnly Prop22 { get; set; }
                        public byte[] Prop22 { get; set; }
                        public int Prop23 { get; set; }
                        public DateTime Prop24 { get; set; }
                    }
                }
                """;

            await VerifyAgainstBaselineUsingFile("TestPrimitivesGen.generated.txt", testSourceCode);
        }

        [Fact]
        public async Task LangVersionMustBeCharp11OrHigher()
        {
            var (d, r) = await RunGenerator(BindCallSampleCode, LanguageVersion.CSharp10);
            Assert.Empty(r);

            Diagnostic diagnostic = Assert.Single(d);
            Assert.True(diagnostic.Id == "SYSLIB1102");
            Assert.Contains("C# 11", diagnostic.Descriptor.Title.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
        }

        [Fact]
        public async Task TestCollectionsGen()
        {
            string testSourceCode = """
                using System.Collections.Generic;
                using Microsoft.Extensions.Configuration;

                public class Program
                {
                    public static void Main()
                    {
                        ConfigurationBuilder configurationBuilder = new();
                        IConfiguration config = configurationBuilder.Build();
                        IConfigurationSection section = config.GetSection(""MySection"");

                        section.Get<MyClassWithCustomCollections>();
                    }

                    public class MyClassWithCustomCollections
                    {
                        public CustomDictionary<string, int> CustomDictionary { get; set; }
                        public CustomList CustomList { get; set; }
                        public ICustomDictionary<string> ICustomDictionary { get; set; }
                        public ICustomSet<MyClassWithCustomCollections> ICustomCollection { get; set; }
                        public IReadOnlyList<int> IReadOnlyList { get; set; }
                        public IReadOnlyDictionary<MyClassWithCustomCollections, int> UnsupportedIReadOnlyDictionaryUnsupported { get; set; }
                        public IReadOnlyDictionary<string, int> IReadOnlyDictionary { get; set; }
                    }

                    public class CustomDictionary<TKey, TValue> : Dictionary<TKey, TValue>
                    {
                    }

                    public class CustomList : List<string>
                    {
                    }

                    public interface ICustomDictionary<T> : IDictionary<T, string>
                    {
                    }

                    public interface ICustomSet<T> : ISet<T>
                    {
                    }
                }
                """;

            await VerifyAgainstBaselineUsingFile("TestCollectionsGen.generated.txt", testSourceCode, assessDiagnostics: (d) =>
            {
                Assert.Equal(6, d.Length);
                Test(d.Where(diagnostic => diagnostic.Id is "SYSLIB1100"), "Did not generate binding logic for a type");
                Test(d.Where(diagnostic => diagnostic.Id is "SYSLIB1101"), "Did not generate binding logic for a property on a type");

                static void Test(IEnumerable<Diagnostic> d, string expectedTitle)
                {
                    Assert.Equal(3, d.Count());
                    foreach (Diagnostic diagnostic in d)
                    {
                        Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
                        Assert.Contains(expectedTitle, diagnostic.Descriptor.Title.ToString(CultureInfo.InvariantCulture));
                    }
                }
            });
        }

        private async Task VerifyAgainstBaselineUsingFile(
            string filename,
            string testSourceCode,
            LanguageVersion languageVersion = LanguageVersion.Preview,
            Action<ImmutableArray<Diagnostic>>? assessDiagnostics = null)
        {
            string baseline = LineEndingsHelper.Normalize(await File.ReadAllTextAsync(Path.Combine("Baselines", filename)).ConfigureAwait(false));
            string[] expectedLines = baseline.Replace("%VERSION%", typeof(ConfigurationBindingSourceGenerator).Assembly.GetName().Version?.ToString())
                                             .Split(Environment.NewLine);

            var (d, r) = await RunGenerator(testSourceCode, languageVersion);
            Assert.Single(r);
            (assessDiagnostics ?? ((d) => Assert.Empty(d))).Invoke(d);

            Assert.True(RoslynTestUtils.CompareLines(expectedLines, r[0].SourceText,
                out string errorMessage), errorMessage);
        }

        private async Task<(ImmutableArray<Diagnostic>, ImmutableArray<GeneratedSourceResult>)> RunGenerator(
            string testSourceCode,
            LanguageVersion langVersion = LanguageVersion.CSharp11) =>
            await RoslynTestUtils.RunGenerator(
                new ConfigurationBindingSourceGenerator(),
                new[] {
                    typeof(ConfigurationBinder).Assembly,
                    typeof(CultureInfo).Assembly,
                    typeof(IConfiguration).Assembly,
                    typeof(IServiceCollection).Assembly,
                    typeof(IDictionary).Assembly,
                    typeof(ServiceCollection).Assembly,
                    typeof(OptionsConfigurationServiceCollectionExtensions).Assembly,
                    typeof(Uri).Assembly,
                },
                new[] { testSourceCode },
                langVersion: langVersion).ConfigureAwait(false);
    }
#endif
}
