using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dotnet7.Models;

public enum VariableType
{
    String,
    Int,
    Boolean,
    Date,
    Time
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(VariableString), (int)VariableType.String)]
[JsonDerivedType(typeof(VariableInt), (int)VariableType.Int)]
[JsonDerivedType(typeof(VariableBool), (int)VariableType.Boolean)]
[JsonDerivedType(typeof(VariableDate), (int)VariableType.Date)]
[JsonDerivedType(typeof(VariableTime), (int)VariableType.Time)]
internal class VariableBase
{
    protected VariableBase(VariableType type)
    {
        Type = type;
    }
    
     public VariableType Type { get; set; }
}

internal class VariableString : VariableBase
{
    public VariableString() : base(VariableType.String) { }
    public string? DerivedProperty { get; set; }
}

internal class VariableInt : VariableBase
{
    public VariableInt() : base(VariableType.Int) { }
    public int? DerivedProperty { get; set; }
}

internal class VariableBool : VariableBase
{
    public VariableBool() : base(VariableType.Boolean) { }
    public bool? DerivedProperty { get; set; }
}
internal class VariableDate : VariableBase
{
    public VariableDate() : base(VariableType.Date) { }
    public DateOnly? DerivedProperty { get; set; }
}

internal class VariableTime : VariableBase
{
    public VariableTime() : base(VariableType.Time) { }
    public TimeOnly? DerivedProperty { get; set; }
}


internal class Step
{
    public List<VariableBase> Variables { get; set; } = new List<VariableBase>();
    public void Init()
    {
        Variables.Add(new VariableBool { DerivedProperty = true });
        Variables.Add(new VariableInt { DerivedProperty = 123 });
        Variables.Add(new VariableString { DerivedProperty = $"""
    Hi!
    there
    "big"
    dog
    """ });
        Variables.Add(new VariableString { DerivedProperty = $"""this "is" on a single line""" });
        Variables.Add(new VariableDate { DerivedProperty = DateOnly.FromDateTime(DateTime.UtcNow)});
        Variables.Add(new VariableTime { DerivedProperty = TimeOnly.FromDateTime(DateTime.UtcNow)});
    }
}


