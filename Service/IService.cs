namespace dotnet7.Service;

internal interface IService
{
    int Add(VariableBase variable);
    VariableBase? GetVariable(int id);
}
