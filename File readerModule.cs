using System;
using VRCOSC.Game.Modules;
using VRCOSC.Game.Modules.Avatar;

namespace VRCOSC.Modules.FileReader;

[ModuleTitle("File reader")]
[ModuleDescription("Reads the contents of a text file")]
[ModuleAuthor("YukiyaVR", "https://github.com/YukiyaVR")]
[ModuleGroup(ModuleType.General)]

public sealed class FileReaderModule : ChatBoxModule
{
    private string? filePath;
    private string? fileContent;
    protected override void CreateAttributes()
    {
        CreateSetting(FileReaderSetting.FilePath, "File Path", "The path to the text file", string.Empty);
        CreateVariable(FileReaderVariable.FileContent, "File Content", "content");
        CreateState(FileReaderState.Default, "Default", $"{GetVariableFormat(FileReaderVariable.FileContent)}");
        CreateEvent(FileVariableChanged.TextUpdate, "Content Changed", $"{GetVariableFormat(FileReaderVariable.FileContent)}", 4);

    }

    protected override void OnModuleStart()
    {
        ChangeStateTo(FileReaderState.Default);
        filePath = GetSetting<string>(FileReaderSetting.FilePath);
        if (IsValidFilePath())
        {
            Log("File path is valid. Reading file content...");
        }
        else
        {
            Log("Invalid file path. Please provide a valid file path in the module settings.");
        }
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void updateVariables()
    {
        if (IsValidFilePath())
        {
            ReadFileContent();
        }
    }

    private bool IsValidFilePath()
    {
        return !string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath);
    }
    private void ReadFileContent()
    {
        try
        {
            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                string newFileContent = System.IO.File.ReadAllText(filePath);
                if (newFileContent != fileContent)
                {
                    TriggerEvent(FileVariableChanged.TextUpdate);
                    fileContent = newFileContent;
                    SetVariableValue(FileReaderVariable.FileContent, fileContent);
                }
            }
            else
            {
                fileContent = string.Empty;
                SetVariableValue(FileReaderVariable.FileContent, fileContent);
            }
        }
        catch (Exception ex)
        {
            Log($"Error reading file: {ex.Message}");
            fileContent = string.Empty;
            SetVariableValue(FileReaderVariable.FileContent, fileContent);
        }
    }

    private enum FileReaderSetting
    {
        FilePath
    }

    private enum FileReaderState
    {
        Default
    }

    private enum FileReaderVariable
    {
        FileContent
    }
    private enum FileVariableChanged
    {
        TextUpdate
    }
