# .NET 7 Playground <!-- omit in toc -->

- [/net7 Endpoints](#net7-endpoints)
  - [GET /net7/step/{id}](#get-net7stepid)
  - [POST /net7/derived](#post-net7derived)
  - [GET /net7/derived/{id}](#get-net7derivedid)
  - [/net7/string/{id}](#net7stringid)
  - [Creating the JWT](#creating-the-jwt)
  - [Errors without IProblemDetails](#errors-without-iproblemdetails)
  - [Error with IProblemDetails](#error-with-iproblemdetails)
  - [Calling it with Auth](#calling-it-with-auth)
- [/weatherforecast Endpoint](#weatherforecast-endpoint)
- [FeatureManagement Test](#featuremanagement-test)
  - [Configuration Provider](#configuration-provider)
  - [Filters](#filters)

This was created with this command

```powershell
dotnet new webapi -o dotnet7 -minimal
```

Then the `Program.cs` was edited to add some additional endpoints.

Features:

- [IProblemDetailsService](https://devblogs.microsoft.com/dotnet/asp-net-core-updates-in-dotnet-7-preview-7/#new-problem-details-service) for errors in [RFC7807](https://www.rfc-editor.org/rfc/rfc7807) format
- [String literals](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/strings/#raw-string-literals)
- [Simplified Auth for Minimal API](https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization/)
- [dotnet user-jwts](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [Polymorphic JSON Serialization](https://devblogs.microsoft.com/dotnet/system-text-json-in-dotnet-7/#type-hierarchies)
- [file](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/file) keyword (`AnotherClass` in TestService.cs)

## /net7 Endpoints

These are secured and require a JWT. Use this command to generate one.

### GET /net7/step/{id}

Demonstrates JSON Serialization of derived types and if id < 100 throws an error

### POST /net7/derived

Adds a new derived type to the list with polymorphic deserialization

### GET /net7/derived/{id}

Demonstrates JSON Serialization of derived types added with the post

### /net7/string/{id}

Demostrated new string features. 1-5 for id. Anything else is 404

### Creating the JWT

```powershell
dotnet user-jwts create --role admin -o token
```

### Errors without IProblemDetails

```text
System.ArgumentException: id
   at Program.<>c__DisplayClass0_0.<<Main>$>b__4(Int32 id) in /workspaces/dotnet7/Program.cs:line 73
   at lambda_method3(Closure, Object, HttpContext)
   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)
--- End of stack trace from previous location ---
   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

HEADERS
=======
Accept: application/json
Connection: keep-alive
Host: 127.0.0.1:5070
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US,en;q=0.9
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InJvb3QiLCJzdWIiOiJyb290IiwianRpIjoiY2Q1OGVhNzYiLCJyb2xlIjoiYWRtaW4iLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo2NDQ5NSIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzU2IiwiaHR0cDovL2xvY2FsaG9zdDo1MDcwIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE3MSJdLCJuYmYiOjE2Njg3MDg4OTEsImV4cCI6MTY3NjY1NzY5MSwiaWF0IjoxNjY4NzA4ODkyLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.BZzAHxIQM-zF7pYWU9Dj-CJVAOUvwzslh8rqo_vWICo
Referer: http://127.0.0.1:5070/swagger/index.html
sec-ch-ua: "Google Chrome";v="107", "Chromium";v="107", "Not=A?Brand";v="24"
sec-ch-ua-mobile: ?0
sec-ch-ua-platform: "macOS"
Sec-Fetch-Site: same-origin
Sec-Fetch-Mode: cors
Sec-Fetch-Dest: empty
```

### Error with IProblemDetails

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "System.ArgumentException",
  "status": 500,
  "detail": "id",
  "exception": {
    "details": "System.ArgumentException: id\n   at Program.<>c__DisplayClass0_0.<<Main>$>b__4(Int32 id) in /workspaces/dotnet7/Program.cs:line 73\n   at lambda_method3(Closure, Object, HttpContext)\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)\n--- End of stack trace from previous location ---\n   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)\n   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)\n   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)\n   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)\n   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)",
    "headers": {
      "Accept": [
        "application/json"
      ],
      "Connection": [
        "keep-alive"
      ],
      "Host": [
        "127.0.0.1:5070"
      ],
      "User-Agent": [
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36"
      ],
      "Accept-Encoding": [
        "gzip, deflate, br"
      ],
      "Accept-Language": [
        "en-US,en;q=0.9"
      ],
      "Authorization": [
        "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InJvb3QiLCJzdWIiOiJyb290IiwianRpIjoiNTM5M2E5ZDQiLCJyb2xlIjoiYWRtaW4iLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo2NDQ5NSIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzU2IiwiaHR0cDovL2xvY2FsaG9zdDo1MDcwIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE3MSJdLCJuYmYiOjE2Njg3MDgyMTUsImV4cCI6MTY3NjY1NzAxNSwiaWF0IjoxNjY4NzA4MjE2LCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.me8ZkF5gl1cLokPCOFfPNOspGytAAQb5ElR2C5mF5nA"
      ],
      "Referer": [
        "http://127.0.0.1:5070/swagger/index.html"
      ],
      "sec-ch-ua": [
        "\"Google Chrome\";v=\"107\", \"Chromium\";v=\"107\", \"Not=A?Brand\";v=\"24\""
      ],
      "sec-ch-ua-mobile": [
        "?0"
      ],
      "sec-ch-ua-platform": [
        "\"macOS\""
      ],
      "Sec-Fetch-Site": [
        "same-origin"
      ],
      "Sec-Fetch-Mode": [
        "cors"
      ],
      "Sec-Fetch-Dest": [
        "empty"
      ]
    },
    "path": "/net7/derived/1",
    "endpoint": "HTTP: GET /net7/derived/{id:int}",
    "routeValues": {
      "id": "1"
    }
  }
}
```

### Calling it with Auth

Call all the string variations. The last one will get a 404

```powershell
$token = dotnet user-jwts create --role admin --output token
$headers = @{Authorization="Bearer $token";Accept="application/json"}
$port = 5070
$token

1..6 | % { "-----" && irm "http://localhost:$port/net7/string/$_" -Headers $headers }
```

Create a derived object.

```powershell
# important to be orders type the discrimator must be the first property
$v = [Ordered]@{
    type="int"
    derivedPropertyInt = 123
    message="should be an int"
}
$r = iwr "http://localhost:$port/net7/derived" -Headers $headers -Method Post -Body ($v |ConvertTo-Json -Depth 10) -ContentType "application/json"
$r.Content | ConvertFrom-Json
$location = $r.Headers['Location']

"Location is $location"

# get it back
$x = irm "http://localhost:$port$location" -Headers $headers
$x.derivedPropertyInt.GetType()
# get formatted error
irm  "http://localhost:$port/net7/step/1" -Headers $headers -SkipHttpErrorCheck
```
## /weatherforecast Endpoint

This is from the generated code and is not secure

## FeatureManagement Test

| Key        | SetIn                          | Value               |
| ---------- | ------------------------------ | ------------------- |
| PLAIN.KEYA | appsettings.json               | true                |
| CNTXT.KEYA | appsettings.json               | Enabled for context |
| PLAIN.KEYB | FeatureFlagConfiguration class | true                |
| CNTXT.KEYB | FeatureFlagConfiguration class | Enabled for context |
| PLAIN.KEYC | Environment                    | true                |
| CNTXT.KEYC | Environment                    | Enabled for context |
| CNTXT.KEYD | Added after 10 seconds[^1]     | Enabled for context |

[^1]: This is to test the exception if not found

### Configuration Provider

I added a configuration provider to supply the values for adding the filter names and configuring the filters.

`Load` gets all the keys using options passed in and calls sets the filters on all of them. If a new filter is added later, it registers it.

```csharp
 Data[$"FeatureManagement:{key}:EnabledFor:0:Name"] = "TestFeatureFilter";
```

Note the provider is only needed for registering the filters, not the value. Evaluation got to the filters.

As any new keys get added, update `Data`

### Filters

Gets the `IFeatureFlags` injected in and on `EvaluateAsync` checks the context for the key.

There are two filters, one for context and one for no-context. All the keys are configured to use them in that order.

Since FeatureManagement calls filters until one returns true. This works fine, **except** in the case where no-context is true, and you want to turn off a flag via context. Since context filter is first and returns false, it'd fall through to the no-context filter which would return true. So no short-circuiting a value of false, only true.

> NOTE that the `FeatureManager` caches the values, so you have to do something to trigger it to update with your configuration provider.