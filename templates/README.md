# Avalonia Template Snapshot

This folder mirrors the official **Avalonia .NET App** template published in the [`AvaloniaUI/avalonia-dotnet-templates`](https://github.com/AvaloniaUI/avalonia-dotnet-templates) repository. The content was copied from `templates/csharp/app` (commit pinned via `git clone --depth 1`) so we can:

- compare our boilerplate against the upstream starter,
- experiment with customisations before packaging our own template, and
- keep template metadata (`.template.config`) ready if we decide to ship a `dotnet new` experience.

To install this snapshot locally for testing:

```bash
dotnet new install ./templates/csharp/app
dotnet new avalonia.app -o MyPrototype
```

> The upstream documentation for managing and publishing Avalonia templates is available in the official repository README.[^1]

[^1]: AvaloniaUI, “Avalonia Templates for `dotnet new`,” GitHub, accessed Nov 2025, https://github.com/AvaloniaUI/avalonia-dotnet-templates. 

