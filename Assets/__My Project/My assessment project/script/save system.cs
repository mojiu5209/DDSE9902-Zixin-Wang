using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string dateISO;
    public float obedience, bond, energy;
    public List<TrickSave> tricks = new();
}

[Serializable]
public class TrickSave
{
    public string trick;
    public int level;
    public float xp;
    public string lastTrainedISO;
}

public static class SaveSystem
{
    static string Path => System.IO.Path.Combine(Application.persistentDataPath, "dog_save.json");

    public static void SaveAll()
    {
        var s = new SaveData();
        s.dateISO = GameClock.I.Now.ToString("o");
        s.obedience = DogStats.I.obedience;
        s.bond = DogStats.I.bond;
        s.energy = DogStats.I.energy;

        foreach (var kv in DogStats.I.tricks)
        {
            s.tricks.Add(new TrickSave
            {
                trick = kv.Key.ToString(),
                level = kv.Value.level,
                xp = kv.Value.xp,
                lastTrainedISO = kv.Value.lastTrained.ToString("o")
            });
        }

        File.WriteAllText(Path, JsonUtility.ToJson(s, true));
    }

    public static bool LoadAll()
    {
        if (!File.Exists(Path)) return false;
        var json = File.ReadAllText(Path);
        var s = JsonUtility.FromJson<SaveData>(json);
        try
        {
            GameClock.I.Now = DateTime.Parse(s.dateISO, null, System.Globalization.DateTimeStyles.RoundtripKind);
            DogStats.I.obedience = s.obedience;
            DogStats.I.bond = s.bond;
            DogStats.I.energy = s.energy;

            foreach (var ts in s.tricks)
            {
                var t = (DogTrick)Enum.Parse(typeof(DogTrick), ts.trick);
                var tp = DogStats.I.tricks[t];
                tp.level = ts.level;
                tp.xp = ts.xp;
                tp.lastTrained = DateTime.Parse(ts.lastTrainedISO, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }
            return true;
        }
        catch { return false; }
    }
}