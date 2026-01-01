using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using YobaGame.Models;
using YobaGame.Models.Managers;

public partial class YobaSpawner : Node2D
{
    private Dictionary<string, PackedScene> yobaScenes = new Dictionary<string, PackedScene>();
    private Dictionary<string, int> yobaValues = new Dictionary<string, int>();
    private LinkedList<string> yobaTypes = new LinkedList<string>();
    Yoba[] yobaHolder = new Yoba[2];
    private Queue<Yoba> yobaQueue = new Queue<Yoba>();
    private int biggestYoba = 0;
    private YobaFactory _yobaFactory;

    private bool playerReady = false;
    private bool nextYobaReady = false;

    private int poolResetCount = 100;
    private int poolResetCounter = 0;
    private HashSet<ulong> yobaIdPool = new ();
    
    private bool _debug = false;
    private int _debugYobaIndex = 5;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var yobaNames = new string[]
        {
            "YobaClassic", "YobaAnime", "YobaBatya", "YobaSpurdo", "YobaButthurt",
            "YobaDerp", "YobaSmile", "YobaNya", "YobaFat", "YobaLmao", "YobaGreen"
        };

        foreach (var name in yobaNames)
        {
            yobaScenes[name] = GD.Load<PackedScene>($"res://Scenes/Yobas/{name}.tscn");
            yobaValues[name] = Array.IndexOf(yobaNames, name);
            yobaTypes.AddLast(name);
        }

        _yobaFactory = new YobaFactory(yobaScenes);

        SignalManager.YobasCollidedSignal += SpawnNextYoba;
        SignalManager.PlayerSpawnedYobaSignal += SpawnParentYoba;
        SignalManager.PlayerReadySignal += PlayerReady;
        SignalManager.NextYobaReadySignal += NextYobaReady;

        GD.Randomize();
    }
    
    public override void _ExitTree()
    {
        SignalManager.YobasCollidedSignal -= SpawnNextYoba;
        SignalManager.PlayerSpawnedYobaSignal -= SpawnParentYoba;
        SignalManager.PlayerReadySignal -= PlayerReady;
        SignalManager.NextYobaReadySignal -= NextYobaReady;
    }

    private void PlayerReady()
    {
        playerReady = true;
        if (playerReady && nextYobaReady)
            SpawnInitialYobas();
    }

    private void NextYobaReady()
    {
        nextYobaReady = true;
        if (playerReady && nextYobaReady)
            SpawnInitialYobas();
    }

    private void SpawnInitialYobas()
    {
        string firstYobaType = GetYobaTypeByIndex(GetYobaIndex());
        string secondYobaType = GetYobaTypeByIndex(GetYobaIndex());
        if (!string.IsNullOrEmpty(firstYobaType) && !string.IsNullOrEmpty(secondYobaType))
        {
            Yoba firstYobaNode = _yobaFactory.CreateYoba(firstYobaType);
            Yoba secondYobaNode = _yobaFactory.CreateYoba(secondYobaType);

            if (firstYobaNode is not null && secondYobaNode is not null)
            {
                yobaHolder[0] = firstYobaNode;
                yobaHolder[1] = secondYobaNode;
            }
        }

        SpawnParentYoba();
    }

    private int GetYobaIndex()
    {
        return _debug ? _debugYobaIndex : GD.RandRange(0, 4);
    }

    private void SpawnParentYoba()
    {
        SignalManager.InvokeGivePlayerYoba(yobaHolder[0]);
        SignalManager.InvokeGiveNextYoba(yobaHolder[1]);

        yobaHolder[0] = yobaHolder[1];
        yobaHolder[1] = GetNextYoba();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void SpawnYobaRandom(Vector2 spawnPosition)
    {
        GD.Print(biggestYoba);
        // Randomly select a Yoba
        //int nextYobaIndex = biggestYoba == 0 ? 0 : GD.RandRange(0, 5);
        int nextYobaIndex = GetYobaIndex();
        string nextYobaType = GetYobaTypeByIndex(nextYobaIndex);

        if (!string.IsNullOrEmpty(nextYobaType))
        {
            Yoba yobaNode = _yobaFactory.CreateYoba(nextYobaType, spawnPosition);

            if (yobaNode != null)
            {
                AddChild(yobaNode);
            }
        }
    }

    private Yoba GetNextYoba()
    {
        int nextYobaIndex = GetYobaIndex();
        string nextYobaType = GetYobaTypeByIndex(nextYobaIndex);
        if (!string.IsNullOrEmpty(nextYobaType))
        {
            Yoba yobaNode = _yobaFactory.CreateYoba(nextYobaType, new Vector2());

            if (yobaNode != null)
            {
                yobaNode.Freeze = true;
                return yobaNode;
            }
        }

        return null;
    }

    private void SpawnNextYoba(Vector2 spawnPosition, Yoba yoba, Yoba otherYoba)
    {
        try
        {
            var yobaId = yoba.GetInstanceId();
            var otherYobaId = otherYoba.GetInstanceId();
            if (yobaIdPool.Contains(yobaId) && yobaIdPool.Contains(otherYobaId))
            {
                ;
                yobaIdPool.Clear();
                poolResetCounter = 0;
                return;
            }
        
            yobaIdPool.Add(yobaId);
            yobaIdPool.Add(otherYobaId);
        
            GD.Print($"Yoba main spawner. Yoba name is {yoba.YobaName}, {yobaId}");

            LinkedListNode<string> currentYobaType = yobaTypes.Find(yoba.YobaName);
            if (currentYobaType != null && currentYobaType != yobaTypes.Last)
            {
                string nextYobaType = currentYobaType.Next.Value;

                GD.Print($@"Next Yoba type: {nextYobaType}");

                Yoba newYobaNode = _yobaFactory.CreateYoba(nextYobaType, spawnPosition);

                if (newYobaNode is not null)
                {
                    this.GetTree().Root.AddChild(newYobaNode);

                    if (yobaValues.TryGetValue(nextYobaType, out int value) && value > biggestYoba)
                    {
                        biggestYoba = value;
                    }
                }
                else
                {
                    GD.Print($"Unknown Yoba type: {nextYobaType}");
                }
            }

            poolResetCounter++;
            if (poolResetCounter > poolResetCount)
            {
                yobaIdPool.Clear();
                poolResetCounter = 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return;
        }
        
    }

    private string GetYobaTypeByIndex(int index)
    {
        if (index >= 0 && index < yobaTypes.Count)
        {
            LinkedListNode<string> node = yobaTypes.First;
            for (int i = 0; i < index; i++)
            {
                node = node.Next;
            }

            return node.Value;
        }

        return null;
    }
}