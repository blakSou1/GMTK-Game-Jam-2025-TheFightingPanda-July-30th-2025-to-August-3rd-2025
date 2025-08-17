using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Wave/WaveData")]
public class Wave : ScriptableObject
{
    public List<Mobs> mobs;
    public List<Event> waveEvents;

    public List<ObjectPoolItem> objectPools;

    public int numberWave;

    public Wave DeepCopy()
    {
        Wave copy = ScriptableObject.CreateInstance<Wave>();

        var fields = typeof(Wave).GetFields(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance
        );

        foreach (var field in fields)
            field.SetValue(copy, field.GetValue(this));

        return copy;
    }
}

[Serializable]
public class Mobs
{
    public string objectPoolName;

    public int count;

    public float startSpawnTime;
    [NonSerialized] public bool isSpawn = false;

    [Header("интервал между спавном моба")]
    public float spawnInterval;

    [Header("если > 0 - распределить спавн равномерно в течение этого времени")]
    public float spawnDuration;
}

[Serializable]
public class ObjectPoolItem
{
    public string objectPoolName;

    [NonSerialized] public List<GameObject> item = new();
    public CMSEntityPfb obraz;

    [Header("можно ли расширять пул при необходимости")]
    public bool expandable = true;

    [Header("начальный размер пула")]
    public int initialPoolSize = 10;
}