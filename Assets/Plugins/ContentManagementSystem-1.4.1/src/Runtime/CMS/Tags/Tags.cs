using System;
using System.Collections;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public class TagNearAtack : EntityComponentDefinition, IAtack, IFixedUpdate, IAnimationEvent
{
    [SerializeField] private string animEventKey = "Atack";
    [SerializeField] private int damage = 5;
    [SerializeField] private string paramName = "Atack";
    public float distanceToAtack = .5f;

    public void Execute(State owner)
    {
        if (owner.distancePlayer.distanceIsPlayer < distanceToAtack)
            owner.anim.SetBool(paramName, true);
        else
            owner.anim.SetBool(paramName, false);
    }

    public void Event(State owner)
    {
        if (owner.AnimEvent == animEventKey)
            CMSEntityPfb.TriggerScoreChanged(damage);
    }
}

[Serializable]
public class TagMoveGround : EntityComponentDefinition, IMove, IFixedUpdate
{
    public float speed;
    public float distanceToAtack = .5f;

    public void Execute(State owner)
    {
        if (owner.distancePlayer.distanceIsPlayer > distanceToAtack)
            owner.rb.linearVelocity = Vector2.left * speed;
        else
            owner.rb.linearVelocity = Vector2.zero;
    }
}

public class DistancePlayer : EntityComponentDefinition, IRuntimeStats
{
    [HideInInspector] public float distanceIsPlayer;

}
[Serializable]
public class TagDistanceDetecterToPlayer : DistancePlayer, IFixedUpdate, IStartUpdate
{
    private GameObject player = null;

    public void Execute(State owner)
    {
        distanceIsPlayer = Mathf.Abs(owner.gameObjectC.transform.position.x - player.transform.position.x);
    }

    public void Start(State owner)
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
    }
}

[Serializable]
public class TagHealth : EntityComponentDefinition, IDamage, IRuntimeStats, IAnimationEvent, IEnableUpdate
{
    private int health;
    public int maxHealth = 5;

    [SerializeField] private string animEventKey = "Deach";
    [SerializeField] private string paramName = "Deach";

    public void Dead(State owner)
    {
        owner.gameObjectC.SetActive(false);
    }

    public void Event(State owner)
    {
        if (owner.AnimEvent == animEventKey)
            Dead(owner);
    }

    public void Execute(State owner)
    {
        health -= owner.damage;
        if (health <= 0)
        {
            owner.anim.SetTrigger(paramName);
            owner.gameObjectC.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void OnEnable(State owner)
    {
        owner.gameObjectC.GetComponent<Collider2D>().enabled = true;
        health = maxHealth;
    }
}

[Serializable]
public class TagGun : EntityComponentDefinition, IShoot
{
    public LayerMask mask;
    public int damage;
    public float speed;
    public GameObject bulletPrefab;
    public int pool;
    public float pause;

    public IEnumerator Atack(State owner)
    {
        owner.inputPosition = Camera.main.ScreenToWorldPoint(owner.inputPosition);
        Vector2 startPosition = owner.gameObjectC.transform.position;

        float totalDistance = Vector3.Distance(startPosition, owner.inputPosition);
        float distanceTraveled = 0;

        while (distanceTraveled < totalDistance)
        {
            float moveDistance = speed * Time.fixedDeltaTime;
            distanceTraveled += moveDistance;

            // Ограничиваем перемещение, чтобы не перескочить цель
            distanceTraveled = Mathf.Min(distanceTraveled, totalDistance);

            float progress = distanceTraveled / totalDistance;
            owner.gameObjectC.transform.position = Vector3.Lerp(startPosition, owner.inputPosition, progress);

            yield return new WaitForEndOfFrame();
        }

        RaycastHit2D hit = Physics2D.CircleCast(owner.inputPosition, .2f, Vector2.zero,mask);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out CMSEntityPfb inter))
            {
                inter.state.damage = damage;
                inter.Components.OfType<IDamage>().FirstOrDefault()?.Execute(inter.state);
            }
        }

        owner.gameObjectC.SetActive(false);
    }

    public void Execute(State owner)
    {
        owner.monoBehaviour.StartCoroutine(Atack(owner));
    }
}

[Serializable]
public class TagAnimator : EntityComponentDefinition, IStartUpdate
{
    public AnimatorController animController;

    public void Start(State owner)
    {
        owner.gameObjectC.GetComponent<Animator>().runtimeAnimatorController = animController;
    }
}

public class State : UnityEngine.Object
{
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