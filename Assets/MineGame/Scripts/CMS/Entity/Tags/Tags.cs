using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TagMoveGround : EntityComponentDefinition
{
    public float speed;
    public float distanceToAtack = .5f;

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update += Execute;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update -= Execute;
    }

    private void Execute(State owner)
    {
        if (owner.distancePlayer.distanceIsPlayer > distanceToAtack)
            owner.rb.linearVelocity = Vector2.left * speed;
        else
            owner.rb.linearVelocity = Vector2.zero;
    }
}

public class DistancePlayer : EntityComponentDefinition
{
    [ReadOnly] public float distanceIsPlayer;
}
[Serializable]
public class TagDistanceDetecterToPlayer : DistancePlayer
{
    private GameObject player = null;

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        Start(cMSEntityPfb.state);
        cMSEntityPfb.Update += Execute;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.Update -= Execute;
    }

    private void Execute(State owner)
    {
        distanceIsPlayer = Mathf.Abs(owner.gameObjectC.transform.position.x - player.transform.position.x);
    }

    private void Start(State owner)
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        distanceIsPlayer = 500;
    }
}

[Serializable]
public class TagHealth : EntityComponentDefinition, IDamage
{
    private int health;
    public int maxHealth = 5;
    public Image hpBar;
    public int weigthCoin = 3;
    public int maxWeigthCoin = 10;

    [SerializeField] private string animEventKey = "Deach";
    [SerializeField] private string paramName = "Deach";
    [SerializeField] private string animAddCoinName = "IsCoin";

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.OnEnables += OnEnable;
        cMSEntityPfb.AnimationEvent += Event;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.OnEnables -= OnEnable;
        cMSEntityPfb.AnimationEvent -= Event;
    }

    public void Dead(State owner)
    {
        owner.gameObjectC.SetActive(false);
    }

    private void Event(State owner)
    {
        if (owner.AnimEvent == animEventKey)
            Dead(owner);
    }

    public void Execute(State owner)
    {
        health -= owner.damage;
        hpBar.fillAmount = Mathf.Clamp01((float)health / (float)maxHealth);

        if (health <= 0)
        {
            int i = UnityEngine.Random.Range(0, maxWeigthCoin);
            if (i < weigthCoin)
            {
                owner.gameObjectC.GetComponentInChildren<Animator>().SetTrigger(animAddCoinName);   
                G.param.RedactCoin(1);
            }

            owner.anim.SetTrigger(paramName);
            owner.gameObjectC.GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnEnable(State owner)
    {
        hpBar.fillAmount = 0;
        owner.gameObjectC.GetComponent<Collider2D>().enabled = true;
        health = maxHealth;
    }
}

[Serializable]
public class TagGun : EntityComponentDefinition
{
    public event Action<State> IsAtack;

    public AudioClip audioClip;
    public LayerMask mask;
    public int damage;
    public float speed;
    public GameObject bulletPrefab;
    public int pool;
    public float pause;

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        IsAtack = null;
    }
    
    public IEnumerator Atack(State owner)
    {
        Vector2 inputPosition = Camera.main.ScreenToWorldPoint(owner.inputPosition);
        Vector2 startPosition = owner.gameObjectC.transform.position;

        float totalDistance = Vector3.Distance(startPosition, inputPosition);
        float distanceTraveled = 0;

        while (distanceTraveled < totalDistance)
        {
            float moveDistance = (speed + G.param.speed) * Time.fixedDeltaTime;
            distanceTraveled += moveDistance;

            // Ограничиваем перемещение, чтобы не перескочить цель
            distanceTraveled = Mathf.Min(distanceTraveled, totalDistance);

            float progress = distanceTraveled / totalDistance;
            owner.gameObjectC.transform.position = Vector3.Lerp(startPosition, inputPosition, progress);

            yield return new WaitForEndOfFrame();
        }

        RaycastHit2D hit = Physics2D.CircleCast(inputPosition, .2f, Vector2.zero, mask);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out CMSEntityPfb inter))
            {
                inter.state.damage = damage + G.param.upDamage;
                inter.Components.OfType<IDamage>().FirstOrDefault()?.Execute(inter.state);
            }
        }

        owner.gameObjectC.SetActive(false);
    }

    public void Execute(State owner)
    {
        IsAtack?.Invoke(owner);
        owner.monoBehaviour.StartCoroutine(Atack(owner));
    }
}

public class State
{
    public AudioSource audio;
    public string AnimEvent;
    public DistancePlayer distancePlayer;
    public MonoBehaviour monoBehaviour;
    public Vector2 inputPosition;
    public GameObject gameObjectC;
    public int damage;
    public CMSEntityPfb cms;
    public Rigidbody2D rb;
    public Animator anim;
}