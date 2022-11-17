using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet7.Service;

//NEW file keyword to scope the class to the file
file class AnotherClass
{
    
}

internal class TestService : IService
{
    public void Test()
    {
        var a = new AnotherClass();
    }
}
