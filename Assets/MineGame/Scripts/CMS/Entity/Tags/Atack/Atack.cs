using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class TagNearAtack : EntityComponentDefinition
{
    [SerializeField] private string animEventKey = "Atack";
    [SerializeField] private int damage = 5;
    [SerializeField] private string paramName = "Atack";
    public float distanceToAtack = .5f;

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update += Execute;
        cMSEntityPfb.AnimationEvent += Event;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update -= Execute;
        cMSEntityPfb.AnimationEvent -= Event;
    }

    private void Execute(State owner)
    {
        if (owner.distancePlayer.distanceIsPlayer < distanceToAtack)
            owner.anim.SetBool(paramName, true);
        else
            owner.anim.SetBool(paramName, false);
    }

    private void Event(State owner)
    {
        if (owner.AnimEvent == animEventKey)
            G.param.IsDamage(damage);
    }
}

[Serializable]
public class TagBombAtack : EntityComponentDefinition
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private int damage = 15;
    [SerializeField] private string paramName = "Atack";
    public float distanceToAtack = .4f;
    [SerializeField] private string animEventKey = "Atack";

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update += Execute;
        cMSEntityPfb.AnimationEvent += Event;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update -= Execute;
        cMSEntityPfb.AnimationEvent -= Event;
    }

    private void Execute(State owner)
    {
        if (owner.distancePlayer.distanceIsPlayer < distanceToAtack)
            owner.anim.SetBool(paramName, true);
        else
            owner.anim.SetBool(paramName, false);
    }

    private void Event(State owner)
    {
        if (owner.AnimEvent == animEventKey)
        {
            owner.audio.clip = clip;
            owner.audio.Play();

            G.param.IsDamage(damage);
        }
    }
}

[Serializable]
public class TagGunAtack : EntityComponentDefinition
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private int damage = 15;
    [SerializeField] private string paramName = "Atack";
    public float distanceToAtack = 5;
    [SerializeField] private string animEventKey = "Atack";
    public GameObject bulletPrefab;
    public Transform bulletStartPos;
    public float speed = 30;

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update += Execute;
        cMSEntityPfb.AnimationEvent += Event;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update -= Execute;
        cMSEntityPfb.AnimationEvent -= Event;
    }

    private void Execute(State owner)
    {
        if (owner.distancePlayer.distanceIsPlayer < distanceToAtack)
            owner.anim.SetBool(paramName, true);
        else
            owner.anim.SetBool(paramName, false);
    }

    private void Event(State owner)
    {
        if (owner.AnimEvent == animEventKey)
        {
            owner.audio.clip = clip;
            owner.audio.Play();

            GameObject bul = GameObject.Instantiate(bulletPrefab, bulletStartPos);
            owner.monoBehaviour.StartCoroutine(BulletRun(bul));
        }
    }

    private IEnumerator BulletRun(GameObject bullet)
    {
        Vector2 startPosition = bullet.transform.position;

        float totalDistance = Vector3.Distance(startPosition, G.player.transform.position);
        float distanceTraveled = 0;

        while (distanceTraveled < totalDistance)
        {
            float moveDistance = speed * Time.fixedDeltaTime;
            distanceTraveled += moveDistance;

            // Ограничиваем перемещение, чтобы не перескочить цель
            distanceTraveled = Mathf.Min(distanceTraveled, totalDistance);

            float progress = distanceTraveled / totalDistance;
            bullet.transform.position = Vector3.Lerp(startPosition, G.player.transform.position, progress);

            yield return new WaitForEndOfFrame();
        }

        G.param.IsDamage(damage);

        GameObject.Destroy(bullet);
    }
    
}

