using FrameworkBase.Automation.Core.Abstractions;

namespace FrameworkBase.Automation.Infrastructure.Logging;

public sealed class ArtifactLogger : IArtifactLogger
{
    private readonly string artifactRootDirectory;

    public ArtifactLogger(string artifactRootDirectory)
    {
        this.artifactRootDirectory = artifactRootDirectory;
    }

    public string CreateRunDirectory(string scopeName)
    {
        Directory.CreateDirectory(artifactRootDirectory);

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeScopeName = string.Concat(scopeName.Select(ch => invalidChars.Contains(ch) ? '_' : ch));
        var runDirectory = Path.Combine(
            artifactRootDirectory,
            safeScopeName,
            DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));

        Directory.CreateDirectory(runDirectory);
        return runDirectory;
    }

    public string WriteText(string scopeName, string fileName, string content)
    {
        var runDirectory = CreateRunDirectory(scopeName);
        var fullPath = Path.Combine(runDirectory, fileName);
        File.WriteAllText(fullPath, content);
        return fullPath;
    }
}
