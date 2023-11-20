using Model;
using Seekatar.OptionToStringGenerator;

Console.WriteLine("Hello, World!");

var model = new AppUser();
var myOption = new MyOption();

Console.WriteLine(model.OptionsToString());
Console.WriteLine(myOption.OptionsToString());

[OptionsToString]
class MyOption {
public string Secret { get; set; } = "be quiet";
}