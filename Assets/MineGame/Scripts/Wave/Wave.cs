using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Wave/WaveData")]
public class Wave : ScriptableObject
{
    public List<Mobs> mobs;
    public List<WaveEvent> waveEvents;

    public List<ObjectPoolItem> objectPools;

    public int numberWave;

    public Wave DeepCopy()
    {
        // Создаем новый экземпляр Wave
        Wave copiedWave = ScriptableObject.CreateInstance<Wave>();

        // Копируем простые поля
        copiedWave.numberWave = this.numberWave;

        // Глубокое копирование WaveEvents
        if (waveEvents != null)
        {
            copiedWave.waveEvents = new List<WaveEvent>();
            foreach (var waveEvent in waveEvents)
            {
                copiedWave.waveEvents.Add(new WaveEvent
                {
                    eventTime = waveEvent.eventTime,
                    endOfWave = waveEvent.endOfWave,
                    onEvent = waveEvent.onEvent // Shallow copy, так как это reference type
                });
            }
        }

        // Глубокое копирование ObjectPoolItems
        if (objectPools != null)
        {
            copiedWave.objectPools = new List<ObjectPoolItem>();
            foreach (var poolItem in objectPools)
            {
                copiedWave.objectPools.Add(new ObjectPoolItem
                {
                    objectPoolName = poolItem.objectPoolName,
                    obraz = poolItem.obraz, // Shallow copy
                    expandable = poolItem.expandable,
                    initialPoolSize = poolItem.initialPoolSize,
                    item = new List<GameObject>(poolItem.item) // Создаем новый список с теми же ссылками
                });
            }
        }

        // Глубокое копирование Mobs
        if (mobs != null)
        {
            copiedWave.mobs = new List<Mobs>();
            foreach (var mob in mobs)
            {
                copiedWave.mobs.Add(new Mobs
                {
                    objectPoolName = mob.objectPoolName,
                    count = mob.count,
                    startSpawnTime = mob.startSpawnTime,
                    isSpawn = false, // Сбрасываем флаг спавна
                    spawnInterval = mob.spawnInterval,
                    spawnDuration = mob.spawnDuration
                });
            }
        }

        return copiedWave;
    }
}

[Serializable]
public class WaveEvent
{
    public bool endOfWave;
    public float eventTime;
    public CMSEntityPfb onEvent;
}

[Serializable]
public class Mobs
{
    public string objectPoolName;

    public int count;

    public float startSpawnTime;
    [NonSerialized] public bool isSpawn = false;

    /// <summary>
    /// если 0 - спавнить всех сразу
    /// </summary>
    public float spawnInterval;
    /// <summary>
    /// если > 0 - распределить спавн равномерно в течение этого времени
    /// </summary>
    public float spawnDuration;
}

[Serializable]
public class ObjectPoolItem
{
    public string objectPoolName;

    public List<GameObject> item = new();
    public CMSEntityPfb obraz;

    /// <summary>
    /// можно ли расширять пул при необходимости
    /// </summary>
    public bool expandable = true;
    /// <summary>
    /// начальный размер пула
    /// </summary>
    public int initialPoolSize = 10;
}