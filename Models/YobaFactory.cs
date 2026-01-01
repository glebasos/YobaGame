using System.Collections.Generic;
using Godot;

namespace YobaGame.Models;

public class YobaFactory
{
   
    private readonly Dictionary<string, PackedScene> _yobaScenes;

    public YobaFactory(Dictionary<string, PackedScene> yobaScenes)
    {
        _yobaScenes = yobaScenes;
    }

    public Yoba CreateYoba(string yobaType)
    {
        if (_yobaScenes.TryGetValue(yobaType, out var yobaScene))
        {
            var yobaInstance = (Yoba)yobaScene.Instantiate();
            yobaInstance.YobaName = yobaType;
            yobaInstance.Freeze = true;
            return yobaInstance;
        }

        GD.PrintErr($"YobaFactory: Unable to create Yoba of type '{yobaType}'");
        return null;
    }
    
    // public Yoba CreateYoba(string yobaType)
    // {
    //     if (_yobaScenes.TryGetValue(yobaType, out var yobaScene))
    //     {
    //         switch (yobaType)
    //         {
    //             case "YobaClassic":
    //                 YobaClassic yobaClassic = (YobaClassic)yobaScene.Instantiate();
    //                 yobaClassic.YobaName = "YobaClassic";
    //                 return yobaClassic;
    //                 break;
    //             case "YobaAnime":
    //                 YobaAnime yobaAnime = (YobaAnime)yobaScene.Instantiate();
    //                 yobaAnime.YobaName = "YobaAnime";
    //                 return yobaAnime;
    //                 break;
    //             case "YobaBatya":
    //                 YobaBatya yobaBatya = (YobaBatya)yobaScene.Instantiate();
    //                 yobaBatya.YobaName = "YobaBatya";
    //                 return yobaBatya;
    //                 break;
    //             case "YobaSpurdo":
    //                 YobaSpurdo yobaSpurdo = (YobaSpurdo)yobaScene.Instantiate();
    //                 yobaSpurdo.YobaName = "YobaSpurdo";
    //                 return yobaSpurdo;
    //                 break;
    //             case "YobaButthurt":
    //                 YobaButthurt yobaButthurt = (YobaButthurt)yobaScene.Instantiate();
    //                 yobaButthurt.YobaName = "YobaButthurt";
    //                 return yobaButthurt;
    //                 break;
    //         }
    //     }
    //
    //     GD.PrintErr($"YobaFactory: Unable to create Yoba of type '{yobaType}'");
    //     return null;
    // }
    
    public T CreateYoba<T>(string yobaType) where T : Yoba
    {
        if (_yobaScenes.TryGetValue(yobaType, out var yobaScene))
        {
            var instance = yobaScene.Instantiate<T>();
            instance.YobaName = yobaType;
            return instance;
        }

        GD.PrintErr($"YobaFactory: Unable to create Yoba of type '{yobaType}'");
        return null;
    }
    
    public Yoba CreateYoba(string yobaType, Vector2 position)
    {
        if (_yobaScenes.TryGetValue(yobaType, out var yobaScene))
        {
            var yobaInstance = (Yoba)yobaScene.Instantiate();
            yobaInstance.GlobalPosition = position;
            yobaInstance.YobaName = yobaType;
            return yobaInstance;
        }

        GD.PrintErr($"YobaFactory: Unable to create Yoba of type '{yobaType}'");
        return null;
    }
}