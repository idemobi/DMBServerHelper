#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Reflection;
using DMBServerHelper;
using NUnit.Framework;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
internal sealed class ApiDocumentationListTests
{
    [Test]
    public void AddApiAssemblyHandlesConcurrentDuplicateRegistrations()
    {
        Assembly assembly = typeof(ApiDocumentationListTests).Assembly;

        Parallel.For(0, 32, _ => ApiDocumentationList.AddApiAssembly(assembly));

        Assembly[] assemblies = ApiDocumentationList.GetAssemblies();

        Assert.That(assemblies.Count(item => item == assembly), Is.EqualTo(1));
    }

    [Test]
    public void AddApiAssemblyIgnoresDuplicateAssemblyReferences()
    {
        Assembly assembly = typeof(ApiDocumentationListTests).Assembly;

        ApiDocumentationList.AddApiAssembly(assembly);
        ApiDocumentationList.AddApiAssembly(assembly);

        Assembly[] assemblies = ApiDocumentationList.GetAssemblies();

        Assert.That(assemblies.Count(item => item == assembly), Is.EqualTo(1));
    }
}