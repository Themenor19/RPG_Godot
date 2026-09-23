using Godot;
using Godot.Collections;

namespace RPG.scripts.helper_classes.save_data;

public abstract class AbcSave
{
    public bool Save(string savePath)
    {
        Dictionary saveData = DataToSave();

        var err = FileHandler.StoreBinaryFile(saveData, savePath, true);
        if (err != Error.Ok)
        {
            GD.PrintErr("Could not save player data binary: " + err);
        }

        return err == Error.Ok;
    }

    public bool Load(string savePath)
    {
        var saveData = new Dictionary();
        var err = FileHandler.OpenBinaryFile(savePath, saveData);
        if (err != Error.Ok)
        {
            GD.PrintErr("Could not load player data binary: " + err);
        }

        err = DataFromSave(saveData);
        if (err != Error.Ok)
        {
            GD.PrintErr("Invalid save data binary: " + err);
        }

        return err == Error.Ok;
    }
    
    protected abstract Dictionary DataToSave();
    protected abstract Error DataFromSave(Dictionary data);
    protected abstract Error VerifySave(Dictionary data);
}