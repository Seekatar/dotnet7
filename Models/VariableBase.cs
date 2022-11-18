using System.Text.Json.Serialization;

namespace dotnet7.Models;

public enum VariableType
{
    String = 1,
    Int = 2,
    Boolean = 3,
    DateOnly = 4,
    TimeOnly = 5,
    DateTime = 6
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(VariableString), "string")]
[JsonDerivedType(typeof(VariableInt), "int")]
[JsonDerivedType(typeof(VariableBool), "boolean")]
[JsonDerivedType(typeof(VariableDate), "dateOnly")]
[JsonDerivedType(typeof(VariableTime), "timeOnly")]
[JsonDerivedType(typeof(VariableDateTime), "dateTime")]
internal class VariableBase
{
    protected VariableBase(VariableType type)
    {
        Type = type;
    }

     public VariableType Type { get; set; }
     public string Message { get; set; } = "Set in base";
}

internal class VariableString : VariableBase
{
    public VariableString() : base(VariableType.String) { }
    public string? DerivedPropertyString { get; set; }
}

internal class VariableInt : VariableBase
{
    public VariableInt() : base(VariableType.Int) { }
    public int? DerivedPropertyInt { get; set; }
}

internal class VariableBool : VariableBase
{
    public VariableBool() : base(VariableType.Boolean) { }
    public bool? DerivedPropertyBool { get; set; }
}
internal class VariableDate : VariableBase
{
    public VariableDate() : base(VariableType.DateOnly) { }
    public DateOnly? DerivedPropertyDateOnly { get; set; }
}

internal class VariableTime : VariableBase
{
    public VariableTime() : base(VariableType.TimeOnly) { }
    public TimeOnly? DerivedPropertyTimeOnly { get; set; }
}

internal class VariableDateTime : VariableBase
{
    public VariableDateTime() : base(VariableType.DateTime) { }
    public DateTime? DerivedPropertyDateTime { get; set; }
}


internal class Step
{
    public List<VariableBase> Variables { get; set; } = new List<VariableBase>();
    public void Init()
    {
        Variables.Add(new VariableBool { DerivedPropertyBool = true });
        Variables.Add(new VariableInt { DerivedPropertyInt = 123 });
        Variables.Add(new VariableString { DerivedPropertyString = $"""this "is" on a single line""" });
        Variables.Add(new VariableDate { DerivedPropertyDateOnly = DateOnly.FromDateTime(DateTime.UtcNow)});
        Variables.Add(new VariableTime { DerivedPropertyTimeOnly = TimeOnly.FromDateTime(DateTime.UtcNow)});
        Variables.Add(new VariableDateTime { DerivedPropertyDateTime = DateTime.UtcNow});
    }
}


