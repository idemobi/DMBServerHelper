#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBserverHelperUnitTest.Helpers;

internal sealed class InMemorySession : ISession
{
    #region Instance fields and properties

    private readonly Dictionary<string, byte[]> values = new Dictionary<string, byte[]>();

    #region From interface ISession

    string ISession.Id { get; } = Guid.NewGuid().ToString("N");

    bool ISession.IsAvailable => true;

    IEnumerable<string> ISession.Keys => values.Keys;

    #endregion

    #endregion

    #region Instance methods

    #region From interface ISession

    void ISession.Clear()
    {
        values.Clear();
    }

    Task ISession.CommitAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    Task ISession.LoadAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    void ISession.Remove(string key)
    {
        values.Remove(key);
    }

    void ISession.Set(string key, byte[] value)
    {
        values[key] = value;
    }

    bool ISession.TryGetValue(string key, out byte[] value)
    {
        return values.TryGetValue(key, out value!);
    }

    #endregion

    #endregion
}