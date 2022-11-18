namespace dotnet7.Service;

//NEW file keyword to scope the class to the file
file class AnotherClass
{

}

internal class TestService : IService
{
    static int _id = 0;
    static Dictionary<int,VariableBase> _map = new();
    public int Add(VariableBase variable)
    {
        _id++;
        _map[_id] = variable;
        return _id;
    }

    public VariableBase? GetVariable(int id)
    {
        if (_map.TryGetValue(id, out var variable))
        {
            return variable;
        }
        return null;
    }

    public void Test()
    {
        var a = new AnotherClass();
    }
}
