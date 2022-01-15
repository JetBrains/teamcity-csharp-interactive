namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using System.Linq;
    using Shouldly;
    using Xunit;

    public class StatisticsTests
    {
        [Fact]
        public void ShouldTrackTimeElapsed()
        {
            // Given
            var statistics = new Statistics();

            // When
            using (statistics.Start())
            {
                System.Threading.Thread.Sleep(2);
            }
            
            // Then
            statistics.TimeElapsed.ShouldNotBe(TimeSpan.Zero);
        }
        
        [Fact]
        public void ShouldRegisterError()
        {
            // Given
            var statistics = new Statistics();

            // When
            statistics.RegisterError("error1");
            statistics.RegisterError("error2");

            // Then
            statistics.Errors.ToArray().ShouldBe(new []{"error1", "error2"});
        }
        
        [Fact]
        public void ShouldRegisterWarning()
        {
            // Given
            var statistics = new Statistics();

            // When
            statistics.RegisterWarning("warning1");
            statistics.RegisterWarning("warning2");

            // Then
            statistics.Warnings.ToArray().ShouldBe(new []{"warning1", "warning2"});
        }
    }
}