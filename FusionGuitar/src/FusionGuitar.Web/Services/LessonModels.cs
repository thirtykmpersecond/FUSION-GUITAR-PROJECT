namespace FusionGuitar.Web.Services;

public sealed record LessonModule(string Id, string Title, string Description, int Order);

public sealed record LessonMeta(
    string ModuleId,
    string Slug,
    string Title,
    string Summary,
    int Order,
    IReadOnlyList<string> Tags);

public sealed record Lesson(LessonMeta Meta, string Markdown);

public sealed record LessonNavItem(
    string Slug,
    string Title,
    string ModuleId,
    string ModuleTitle,
    int Order);
