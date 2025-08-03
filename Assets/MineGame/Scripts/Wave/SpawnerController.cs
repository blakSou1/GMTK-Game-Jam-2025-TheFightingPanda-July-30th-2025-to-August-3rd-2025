using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    #region Param
    [SerializeField] private List<Transform> spawnPoint;
    [SerializeField] private GameObject enemyPrefab;
    public static GameObject EnemyPrefab;
    private ObjectPool pool;
    private Wave wave;
    [ReadOnly] public float time = 0;
    private int coroutineCount = 0;

    public static SpawnerController spawnerController;
    private Coroutine coroutineSpawn;
    #endregion

    private void Awake()
    {
        if (spawnerController == null)
            spawnerController = this;
        else
        {
            Destroy(this);
            return;
        }

        pool = new();
        pool.Init();
        EnemyPrefab = enemyPrefab;
        pool.DoneCreatePool += i => coroutineSpawn = StartCoroutine(SpawnMobs());
    }

    public void StartSpawmWave(Wave wave)
    {
        this.wave = wave;
        StartCoroutine(pool.InitializePool(wave));
    }
    private IEnumerator SpawnMobs()
    {
        coroutineCount = wave.mobs.Count;
        time = 0;

        while (true)
        {
            time += .2f;

            foreach (Mobs mobs in wave.mobs)
            {
                if (mobs.isSpawn || mobs.startSpawnTime > time) continue;
                mobs.isSpawn = true;

                if (mobs.spawnInterval > 0)
                    StartCoroutine(SpawnInterval(mobs));
                else if (mobs.spawnDuration > 0)
                    StartCoroutine(SpawnDuration(mobs));
            }

            foreach (Event events in wave.waveEvents)
            {
                if (events.isWave) continue;
                if (events.eventTime > time) continue;
                if (events.endOfWave)
                {
                    if (coroutineCount != 0)
                        continue;
                    if (!HasNoActiveChildren())
                        continue;
                }

                events.isWave = true;
                events.waveEvent.StartEvent(new StateEvent
                {
                    coroutine = coroutineSpawn,
                    monoBehaviour = this
                });
            }

            yield return new WaitForSeconds(.2f);
        }
    }
    public void EndSpawnWoln(Wave wave)
    {
        spawnerController.StopAllCoroutines();
        pool.Resetpool(wave);
    }

    private IEnumerator SpawnInterval(Mobs mobs)
    {
        int count = 0;

        while (count < mobs.count)
        {
            if (SpawnRandomObject(mobs))
                count++;

            yield return new WaitForSeconds(mobs.spawnInterval);
        }
        coroutineCount--;
    }
    private IEnumerator SpawnDuration(Mobs mobs)
    {
        int count = 0;
        float spawnInterval = mobs.spawnDuration / mobs.count;

        while (count < mobs.count)
        {
            if (SpawnRandomObject(mobs))
                count++;

            yield return new WaitForSeconds(spawnInterval);
        }
        coroutineCount--;
    }

    /// <summary>
    /// Метод для спавна объекта из пула в случайной точке
    /// </summary>
    private bool SpawnRandomObject(Mobs mobs)
    {
        // Выбираем случайную точку спавна
        Transform randomSpawnPoint = spawnPoint[Random.Range(0, spawnPoint.Count)];

        // Ищем первый неактивный объект в пуле
        GameObject objectToSpawn = pool.GetFromPool(mobs, wave);

        // Если нашли неактивный объект
        if (objectToSpawn != null)
        {
            objectToSpawn.transform.position = randomSpawnPoint.position;

            objectToSpawn.SetActive(true);

            return true;
        }
        else
            return false;
    }

    public bool HasNoActiveChildren()
    {
        // Получаем все дочерние Transform компоненты
        Transform[] childTransforms = pool.parent.GetComponentsInChildren<Transform>();

        // Исключаем сам родительский объект и проверяем активность дочерних
        bool noActiveChildren = childTransforms
            .Where(child => child != pool.parent) // Исключаем родительский объект
            .All(child => !child.gameObject.activeInHierarchy);

        return noActiveChildren;
    }
}