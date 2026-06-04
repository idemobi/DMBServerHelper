#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DMBServerHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
internal sealed class GenericConfigurationTests
{
    #region Setup/Teardown

    [SetUp]
    public void SetUp()
    {
        ConcurrentLoadConfiguration.Reset();
    }

    [TearDown]
    public void TearDown()
    {
        ConcurrentLoadConfiguration.Reset();
    }

    #endregion

    [Test]
    public void LoadCommonConfigRunsLifecycleOnlyOnceUnderConcurrency()
    {
        const int taskCount = 16;
        using CountdownEvent readySignal = new CountdownEvent(taskCount);
        using ManualResetEventSlim startSignal = new ManualResetEventSlim(false);
        Task[] tasks = Enumerable.Range(0, taskCount)
            .Select(_ => Task.Run(() =>
            {
                HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder();
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                IConfigurationRoot configurationRoot = configurationBuilder.Build();

                readySignal.Signal();
                startSignal.Wait();
                ConcurrentLoadConfiguration.LoadCommonConfig(hostBuilder, configurationBuilder, configurationRoot);
            }))
            .ToArray();

        readySignal.Wait();
        startSignal.Set();
        Task.WaitAll(tasks);

        Assert.Multiple(() =>
        {
            Assert.That(ConcurrentLoadConfiguration.BeforeCount, Is.EqualTo(1));
            Assert.That(ConcurrentLoadConfiguration.AfterCount, Is.EqualTo(1));
            Assert.That(ConcurrentLoadConfiguration.Config.Loaded, Is.True);
        });
    }

    [Test]
    public void LoadCommonConfigSecondCallDoesNotRunLifecycleAgain()
    {
        HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder();
        ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        IConfigurationRoot configurationRoot = configurationBuilder.Build();

        ConcurrentLoadConfiguration.LoadCommonConfig(hostBuilder, configurationBuilder, configurationRoot);
        ConcurrentLoadConfiguration.LoadCommonConfig(hostBuilder, configurationBuilder, configurationRoot);

        Assert.Multiple(() =>
        {
            Assert.That(ConcurrentLoadConfiguration.BeforeCount, Is.EqualTo(1));
            Assert.That(ConcurrentLoadConfiguration.AfterCount, Is.EqualTo(1));
        });
    }

    private sealed class ConcurrentLoadConfiguration : GenericConfiguration<ConcurrentLoadConfiguration>
    {
        public static int AfterCount;

        public static int BeforeCount;

        public static void Reset()
        {
            BeforeCount = 0;
            AfterCount = 0;
            Config = new ConcurrentLoadConfiguration();
        }

        public override void AfterConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot)
        {
            Interlocked.Increment(ref AfterCount);
        }

        public override bool ApiDescription()
        {
            return false;
        }

        public override void BeforeConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot)
        {
            Interlocked.Increment(ref BeforeCount);
            Thread.Sleep(25);
        }

        public override bool NeedsConfigFileOrAppSettings()
        {
            return false;
        }

        public override void RandomFake()
        {
        }
    }
}
