using System.Collections.Generic;
using System.IO;
using Godot;
using FileAccess = Godot.FileAccess;
using NotImplementedException = System.NotImplementedException;

namespace YobaGame.Models.System;

public partial class ConfigFileHandler : Node
{
    public static ConfigFileHandler Instance { get; private set; }
    
    private static string CONFIG_PATH;
    private static ConfigFile _config = new();
    
    private static Dictionary<string, Variant> _settings = new()
    {
        // { "audio/master_volume", 0.0f },
        { "audio/music_volume", 100.0d },
        { "audio/sfx_volume", 100.0d },
        { "system/vibration", true },
        //{ "video/fullscreen", false },
        { "video/screen_shake", true },
        { "game_data/high_score", 0 },
    };
    
    public ConfigFileHandler()
    {
        if (OS.HasFeature("android"))
        {
            CONFIG_PATH = "user://settings.ini";  // Android uses internal storage
            //GD.PrintErr($"Working on android");
        }
        else
        {
            CONFIG_PATH = ProjectSettings.GlobalizePath("settings.ini");  // PC stores in game folder
            //GD.PrintErr($"Working on not android");
        }
        
        LoadSettings();
        ApplySettings();
    }

    public override void _Ready()
    {
        Instance = this;
    }

    private void ApplySettings()
    {
        // AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.LinearToDb((float)_settings["audio/master_volume"]));
        // DisplayServer.WindowSetMode((bool)_settings["video/fullscreen"] ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
    }
    
    public void SaveSettings()
    {
        foreach (var key in _settings.Keys)
        {
            string section = key.Split('/')[0];
            string property = key.Split('/')[1];
            _config.SetValue(section, property, _settings[key]);
        }

        Error err = _config.Save(CONFIG_PATH);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to save settings: {err} + {CONFIG_PATH}");
        }
    }

    private void LoadSettings()
    {
        // Use Godot's file existence check
        if (!FileAccess.FileExists(CONFIG_PATH))
        {
            SaveSettings();  // Create default settings
            return;
        }

        Error err = _config.Load(CONFIG_PATH);
        if (err == Error.Ok)
        {
            foreach (var key in _settings.Keys)
            {
                string section = key.Split('/')[0];
                string property = key.Split('/')[1];

                if (_config.HasSectionKey(section, property))
                    _settings[key] = _config.GetValue(section, property, _settings[key]);
            }
        }
        else
        {
            GD.PrintErr($"Failed to load settings: {err}");
        }
    }
    
    public void SetSetting(string key, Variant value)
    {
        _settings[key] = value;
        ApplySettings();
        SaveSettings();
    }

    public Variant GetSetting(string key)
    {
        return _settings.ContainsKey(key) ? _settings[key] : default(Variant);;
    }
    
}