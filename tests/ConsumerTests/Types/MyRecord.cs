namespace ConsumerTests.Types;

[ValueObject]
public partial record MyRecord
{
    private static Validation Validate(int value) => value >= 0 ? Validation.Ok : Validation.Invalid("must be zero or more");
}

[ValueObject(typeof(string))]
public partial record MyStringRecord
{
    private static Validation Validate(string value) => !value.Contains(" ") ? Validation.Ok : Validation.Invalid("no spaces allowed");
    private static string NormalizeInput(string value) => value.Replace(" ", "");
}

[ValueObject]
public partial record MyRecordStruct
{
    private static Validation Validate(int value) => value >= 0 ? Validation.Ok : Validation.Invalid("must be zero or more");
}

[ValueObject(typeof(string))]
public partial record MyStringRecordStruct
{
    private static Validation Validate(string value) => !value.Contains(" ") ? Validation.Ok : Validation.Invalid("no spaces allowed");
    private static string NormalizeInput(string value) => value.Replace(" ", "");
}
