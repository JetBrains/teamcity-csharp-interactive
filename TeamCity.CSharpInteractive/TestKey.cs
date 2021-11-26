namespace TeamCity.CSharpInteractive;

using System;
using JetBrains.TeamCity.ServiceMessages;

internal readonly struct TestKey
{
    private readonly string _flowId;
    public readonly string TestName;

    public TestKey(IServiceMessage message)
    {
        _flowId = message.GetValue("flowId") ?? string.Empty;
        TestName = message.GetValue("name") ?? string.Empty;
    }

    public override bool Equals(object? obj) =>
        obj is TestKey other && _flowId == other._flowId && TestName == other.TestName;

    public override int GetHashCode() =>
        HashCode.Combine(_flowId, TestName);
}