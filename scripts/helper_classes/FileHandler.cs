using Godot;
using Godot.Collections;

namespace RPG.scripts.helper_classes;

/// <summary>
/// Allows for the storing and retrieving of Dictionaries saved in the Godot Binary Method
/// </summary>
public partial class FileHandler: Node 
{
    public static Error StoreBinaryFile(Dictionary data, string filePath, bool createDirectory = false)
    {
        var result = OpenFileForWrite(filePath, createDirectory);
        var err = result.ErrorType;
        var file = result.File;

        if (err != Error.Ok)
        {
            return err;
        }

        file.StoreVar(data, false);
        file.Close();
        return err;
    }

    public static Error OpenBinaryFile(string filePath, Dictionary outData)
    {
        outData.Clear();
        
        var result = OpenFileForRead(filePath);
        var err = result.ErrorType;
        var file = result.File;

        if (err != Error.Ok)
        {
            return err;
        }

        var value = file.GetVar(false);
        file.Close();

        if (value.VariantType != Variant.Type.Dictionary)
        {
            return Error.InvalidData;
        }

        var fileData = value.AsGodotDictionary();

        outData.Merge(fileData, true);
        return Error.Ok;
    }

    static FileHandlerErrorCheck OpenFileForRead(string filePath)
    {
        if (!FileAccess.FileExists(filePath))
        {
            return new FileHandlerErrorCheck { ErrorType = Error.FileNotFound, File = null};
        }
        
        var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            return new FileHandlerErrorCheck { ErrorType = Error.CantOpen, File = null};
        }
        
        return new FileHandlerErrorCheck { ErrorType = Error.Ok, File = file };
    }

    static FileHandlerErrorCheck OpenFileForWrite(string filePath, bool createDirectory = false)
    {
        Error err = CheckAndCreateDirectory(filePath, createDirectory);
        if (err != Error.Ok)
        {
            return new FileHandlerErrorCheck { ErrorType = err, File = null};
        }

        FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            return new FileHandlerErrorCheck { ErrorType = Error.CantOpen, File = null};
        }

        return new FileHandlerErrorCheck { ErrorType = Error.Ok, File = file };
    }

    static Error CheckAndCreateDirectory(string filePath, bool createDirectory)
    {
        string dirPath = filePath.GetBaseDir();
        if (DirAccess.DirExistsAbsolute(dirPath))
        {
            return Error.Ok;
        }

        if (!createDirectory)
        {
            return Error.CantCreate;
        }

        return DirAccess.MakeDirRecursiveAbsolute(dirPath);
    }
}

class FileHandlerErrorCheck
{
    public Error ErrorType { get;  set; }
    public FileAccess File { get; set; }
}