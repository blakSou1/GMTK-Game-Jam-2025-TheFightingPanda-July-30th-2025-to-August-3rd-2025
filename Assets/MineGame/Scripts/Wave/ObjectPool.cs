using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool
{
    [HideInInspector] public Transform parent; // Родитель для объектов
    public delegate void GenericEventHandler<T>(T eventData);
    public event GenericEventHandler<Coroutine> DoneCreatePool;

    private List<GameObject> UsedPool = new();

    public void Init()
    {
        parent = new GameObject("ObjectPoolParent").transform;
    }

    public IEnumerator InitializePool(Wave wave)
    {
        foreach (ObjectPoolItem p in wave.objectPools)
        {
            for (int i = 0; i < p.initialPoolSize; i++)
            {
                p.item.Add(CreateNewObject(p.obraz));
                yield return new WaitForSeconds(.1f);
            }
        }

        foreach (GameObject ob in UsedPool)
            Object.Destroy(ob);

        UsedPool.Clear();

        DoneCreatePool?.Invoke(null);
    }

    public void Resetpool(Wave wave)
    {
        foreach (ObjectPoolItem obp in wave.objectPools)
        {
            foreach (GameObject ob in obp.item)
                UsedPool.Add(ob);

            obp.item.Clear();
        }
    }

    private GameObject CreateNewObject(CMSEntityPfb obraz)
    {
        GameObject obj;
        if (UsedPool.Count > 0)
        {
            obj = UsedPool[0];
            UsedPool.Remove(obj);
        }
        else
            obj = Object.Instantiate(obraz.gameObject, parent);

        obj.SetActive(false);

        return obj;
    }

    public GameObject GetFromPool(Mobs mobs, Wave wave)
    {
        ObjectPoolItem objectPoolItem = wave.objectPools.FirstOrDefault(go => go.objectPoolName == mobs.objectPoolName);
        GameObject obj = objectPoolItem.item.FirstOrDefault(go => !go.activeInHierarchy);

        if (obj == null)
        {
            if (objectPoolItem.expandable && mobs.spawnDuration > 0)
                obj = CreateNewObject(objectPoolItem.obraz);
        }
        return obj;
    }
}