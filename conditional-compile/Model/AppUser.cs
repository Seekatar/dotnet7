﻿using Seekatar.OptionToStringGenerator;
namespace Model;

[OptionsToString]
public partial class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
}


