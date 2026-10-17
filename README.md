# Neomoth.IoC

A small static IoC container with `[Dependency]` field injection.

```
dotnet add package Neomoth.IoC
```

## Usage

```csharp
using IoC;

IoCManager.InitThread();
IoCManager.Register<ILogger, ConsoleLogger>();
IoCManager.Register<SomeService>();
IoCManager.BuildGraph();

var logger = IoCManager.Resolve<ILogger>();

public sealed class SomeService : IPostInjectInit
{
    [Dependency] private readonly ILogger _logger = default!;

    public void PostInject() => _logger.Log("ready");
}
```

Objects not created by the container can be injected with `IoCManager.InjectDependencies(obj)`.

## Analyzers

The package includes Roslyn analyzers:

| ID      | Description                                     |
|---------|-------------------------------------------------|
| IOC0001 | Assignment to a `[Dependency]` field            |
| IOC0002 | Duplicate `[Dependency]` field of the same type |
