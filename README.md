# OpenSsl.DynamicEngine
A package to add support for dynamic OpenSSL engines to .NET Core.

Usage:
```csharp
using (var engine = new DynamicEngine("cloudhsm"))
{
    engine.Initialize();
    engine.SetDefaults(EngineDefaults.RSA); // <- Select the engine features you intend to use
    // ...
    // Perform cryptographic operations
    // ...
}
```