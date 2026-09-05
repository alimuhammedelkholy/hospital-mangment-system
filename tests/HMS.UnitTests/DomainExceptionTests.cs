using Xunit;
using HMS.Domain.Common;
namespace HMS.UnitTests;
public sealed class DomainExceptionTests { [Fact] public void Preserves_message() => Assert.Equal("rule", new DomainException("rule").Message); }
