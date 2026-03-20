namespace FrameworkBase.Automation.Core.Abstractions;

public interface IArtifactLogger
{
    string CreateRunDirectory(string scopeName);

    string WriteText(string scopeName, string fileName, string content);
}
