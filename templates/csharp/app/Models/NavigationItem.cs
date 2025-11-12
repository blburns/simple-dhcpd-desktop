namespace AvaloniaAppTemplate.Models;

public sealed record NavigationItem(
    string Id,
    string Title,
    string Subtitle,
    string Icon,
    bool IsPrimary = true);

