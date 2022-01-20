namespace TeamCity.CSharpInteractive.Tests;

public class ExitCodeParserTests
{
    [Theory]
    [InlineData(0, true, 0)]
    [InlineData(3, true, 3)]
    [InlineData(-3, true, -3)]
    [InlineData((uint)0, true, 0)]
    [InlineData((uint)3, true, 3)]
    [InlineData((short)0, true, 0)]
    [InlineData((short)3, true, 3)]
    [InlineData((short)-3, true, -3)]
    [InlineData((ushort)0, true, 0)]
    [InlineData((ushort)3, true, 3)]
    [InlineData((sbyte)0, true, 0)]
    [InlineData((sbyte)3, true, 3)]
    [InlineData((sbyte)-3, true, -3)]
    [InlineData((byte)0, true, 0)]
    [InlineData((byte)3, true, 3)]
    [InlineData((long)0, true, 0)]
    [InlineData((long)3, true, 3)]
    [InlineData((long)-3, true, -3)]
    [InlineData(long.MaxValue, true, -1)]
    [InlineData(long.MaxValue - 1, true, -2)]
    [InlineData((ulong)0, true, 0)]
    [InlineData((ulong)3, true, 3)]
    [InlineData(ulong.MaxValue, true, -1)]
    [InlineData(ulong.MaxValue - 1, true, -2)]
    [InlineData("Abc", false, 0)]
    [InlineData((float)1, false, 0)]
    [InlineData((double)1, false, 0)]
    public void ShouldParseReturnValue(object returnValue, bool success, int expectedExitCode)
    {
        // Given
        var parser = CreateInstance();

        // When
        parser.TryParse(returnValue, out var actualExitCode).ShouldBe(success);

        // Then
        if (success)
        {
            actualExitCode.ShouldBe(expectedExitCode);
        }
    }

    private static ExitCodeParser CreateInstance() => new();
}