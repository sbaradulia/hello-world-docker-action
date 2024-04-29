﻿using System.Runtime.InteropServices;
using FluentAssertions.Execution;

namespace ConsumerTests.StringComparisons;

public class ForStructs
{
    [Fact]
    public void Comparison_omitted_but_specifying_own_comparer()
    {
        using var _ = new AssertionScope();

        StringVo_Struct_NothingSpecified left = StringVo_Struct_NothingSpecified.From("abc");
        StringVo_Struct_NothingSpecified right = StringVo_Struct_NothingSpecified.From("abc");

        left.Should().Be(right);
        (left == right).Should().BeTrue();

        IEqualityComparer<StringVo_Struct_NothingSpecified> myComparer = new MyComparer_Class();
        left.Equals(right, myComparer).Should().BeTrue();
        Dictionary<StringVo_Struct_NothingSpecified, int> d = new(myComparer);
        d.Add(StringVo_Struct_NothingSpecified.From("one"), 1);
        d.ContainsKey(StringVo_Struct_NothingSpecified.From("ONE")).Should().BeTrue();
    }

    [Fact]
    public void OrdinalIgnoreCase()
    {
        using var _ = new AssertionScope();

        StringVo_Struct left = StringVo_Struct.From("abc");
        StringVo_Struct right = StringVo_Struct.From("AbC");

        left.Equals(right, StringVo_Struct.Comparers.OrdinalIgnoreCase).Should().BeTrue();
        (left.GetHashCode() != right.GetHashCode()).Should().BeTrue();

        left.Should().NotBe(right);
        (left == right).Should().BeFalse();
    }

    [Fact]
    public void OrdinalIgnoreCase_For_ReadOnly_Struct()
    {
        using var _ = new AssertionScope();

        StringVo_ReadOnly_Struct left = StringVo_ReadOnly_Struct.From("abc");
        StringVo_ReadOnly_Struct right = StringVo_ReadOnly_Struct.From("AbC");

        left.Equals(right, StringVo_ReadOnly_Struct.Comparers.OrdinalIgnoreCase).Should().BeTrue();
        (left.GetHashCode() != right.GetHashCode()).Should().BeTrue();

        left.Should().NotBe(right);
        (left == right).Should().BeFalse();
    }

    [Fact]
    public void OrdinalIgnoreCase_Generic()
    {
        using var _ = new AssertionScope();

        var left = StringVo_Struct_Generic.From("abc");
        var right = StringVo_Struct_Generic.From("AbC");

        var comparer = StringVo_Struct_Generic.Comparers.OrdinalIgnoreCase;

        left.Equals(right, comparer).Should().BeTrue();

        (comparer.GetHashCode(left) == comparer.GetHashCode(right)).Should().BeTrue();

        left.Should().NotBe(right);
        (left == right).Should().BeFalse();
    }

    [Fact]
    public void OrdinalIgnoreCase_in_a_dictionary()
    {
        Dictionary<StringVo_Struct, int> d = new(StringVo_Struct.Comparers.OrdinalIgnoreCase);

        using var _ = new AssertionScope();

        StringVo_Struct key1Lower = StringVo_Struct.From("abc");
        StringVo_Struct key2Mixed = StringVo_Struct.From("AbC");

        d.Add(key1Lower, 1);
        d.Should().ContainKey(key2Mixed);
    }

    [SkippableFact]
    public void Size_is_not_bigger()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
        
        var s1 = Marshal.SizeOf<StringVo_Struct_NothingSpecified>();
        var s2 = Marshal.SizeOf<StringVo_Struct>();

        s1.Should().Be(s2);
    }
}

public class MyComparer_Class : IEqualityComparer<StringVo_Struct_NothingSpecified>
{
    public bool Equals(StringVo_Struct_NothingSpecified x, StringVo_Struct_NothingSpecified y) =>
        StringComparer.OrdinalIgnoreCase.Equals(x.Value, y.Value);

    public int GetHashCode(StringVo_Struct_NothingSpecified obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
}
