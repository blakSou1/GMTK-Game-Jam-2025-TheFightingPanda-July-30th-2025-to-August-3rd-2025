using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
            Param.param.IsDamage(damage);
    }
}

[Serializable]
public class TagBombAtack : EntityComponentDefinition, IAtack, IFixedUpdate, IAnimationEvent
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private int damage = 15;
    [SerializeField] private string paramName = "Atack";
    public float distanceToAtack = .4f;
    [SerializeField] private string animEventKey = "Atack";

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
        {
            owner.audio.clip = clip;
            owner.audio.Play();

            Param.param.IsDamage(damage);
        }
    }
}

[Serializable]
public class TagGunAtack : EntityComponentDefinition, IAtack, IFixedUpdate, IAnimationEvent
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private int damage = 15;
    [SerializeField] private string paramName = "Atack";
    public float distanceToAtack = 5;
    [SerializeField] private string animEventKey = "Atack";
    public GameObject bulletPrefab;
    public Transform bulletStartPos;
    public float speed = 30;

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

        float totalDistance = Vector3.Distance(startPosition, Param.param.player.transform.position);
        float distanceTraveled = 0;

        while (distanceTraveled < totalDistance)
        {
            float moveDistance = speed * Time.fixedDeltaTime;
            distanceTraveled += moveDistance;

            // Ограничиваем перемещение, чтобы не перескочить цель
            distanceTraveled = Mathf.Min(distanceTraveled, totalDistance);

            float progress = distanceTraveled / totalDistance;
            bullet.transform.position = Vector3.Lerp(startPosition, Param.param.player.transform.position, progress);

            yield return new WaitForEndOfFrame();
        }

        Param.param.IsDamage(damage);

        GameObject.Destroy(bullet);
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
        distanceIsPlayer = 500;
    }
}

[Serializable]
public class TagHealth : EntityComponentDefinition, IDamage, IRuntimeStats, IAnimationEvent, IEnableUpdate
{
    private int health;
    public int maxHealth = 5;
    public Image hpBar;
    public int weigthCoin = 3;
    public int maxWeigthCoin = 10;

    [SerializeField] private string animEventKey = "Deach";
    [SerializeField] private string paramName = "Deach";
    [SerializeField] private string animAddCoinName = "IsCoin";

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
        hpBar.fillAmount = Mathf.Clamp01((float)health / (float)maxHealth);

        if (health <= 0)
        {
            int i = UnityEngine.Random.Range(0, maxWeigthCoin);
            if (i < weigthCoin)
            {
                owner.gameObjectC.GetComponentInChildren<Animator>().SetTrigger(animAddCoinName);   
                Param.param.RedactCoin(1);
            }

            owner.anim.SetTrigger(paramName);
            owner.gameObjectC.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void OnEnable(State owner)
    {
        hpBar.fillAmount = 0;
        owner.gameObjectC.GetComponent<Collider2D>().enabled = true;
        health = maxHealth;
    }
}

[Serializable]
public class TagGun : EntityComponentDefinition, IShoot, IDisebleUpdate
{
    public event Action<State> IsAtack;

    public AudioClip audioClip;
    public LayerMask mask;
    public int damage;
    public float speed;
    public GameObject bulletPrefab;
    public int pool;
    public float pause;

    public IEnumerator Atack(State owner)
    {
        Vector2 inputPosition = Camera.main.ScreenToWorldPoint(owner.inputPosition);
        Vector2 startPosition = owner.gameObjectC.transform.position;

        float totalDistance = Vector3.Distance(startPosition, inputPosition);
        float distanceTraveled = 0;

        while (distanceTraveled < totalDistance)
        {
            float moveDistance = (speed + Param.param.speed) * Time.fixedDeltaTime;
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
                inter.state.damage = damage + Param.param.upDamage;
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

    public void OnDisable(State owner)
    {
        IsAtack = null;
    }
}

[Serializable]
public class TagUpGun : EntityComponentDefinition, IBoost, IStartUpdate, IDisebleUpdate, IFixedUpdate
{
    public int coin = 1;
    public CMSEntityPfb GunModel;
    public float timePause = .1f;
    public int DebafDamage = 2;

    public void Boost(State owner)
    {
        Param.param.upDamage -= DebafDamage;
        GunModel.Components.OfType<TagGun>().FirstOrDefault().IsAtack += i => Param.param.monoBehaviour.StartCoroutine(UpAtack(i));
        Param.param.coin -= coin;

        owner.gameObjectC.SetActive(false);
    }
    private IEnumerator UpAtack(State owner)
    {
        GameObject bullets = GameObject.Instantiate(owner.gameObjectC, owner.gameObjectC.transform.parent);
        bullets.transform.position = bullets.transform.position;
        bullets.SetActive(true);

        yield return new WaitForSeconds(timePause);

        GameObject.Destroy(bullets, 1);

        Param.param.monoBehaviour.StartCoroutine(GunModel.Components.OfType<TagGun>().FirstOrDefault().Atack(new State
        {
            gameObjectC = bullets,
            inputPosition = owner.inputPosition,
            monoBehaviour = Param.param.monoBehaviour
        }));
    }

    public void Execute(State owner)
    {
        if (Param.param.coin < coin)
            owner.gameObjectC.GetComponentInChildren<GeneralButton>().UpdateInteractable(false);
    }

    public void OnDisable(State owner)
    {
        owner.gameObjectC.GetComponentInChildren<GeneralButton>().onClick.RemoveAllListeners();
    }

    public void Start(State owner)
    {
        GeneralButton generalButton = owner.gameObjectC.GetComponentInChildren<GeneralButton>();
        generalButton.UpdateText($"{coin}");

        if (Param.param.coin < coin)
        {
            generalButton.UpdateInteractable(false);
            return;
        }

        generalButton.onClick.AddListener(() => Boost(owner));
    }
}

[Serializable]
public class TagHeling : EntityComponentDefinition, IBoost, IStartUpdate, IDisebleUpdate, IFixedUpdate
{
    public int hp = 20;
    public int coin = 3;
    
    public void Boost(State owner)
    {
        Param.param.RedactCoin(-coin);

        Param.param.AddHealth(hp);
        owner.gameObjectC.SetActive(false);
    }

    public void Execute(State owner)
    {
        if (Param.param.coin < coin || Param.param.health == Param.param.maxHealth)
            owner.gameObjectC.GetComponentInChildren<GeneralButton>().UpdateInteractable(false);
    }

    public void OnDisable(State owner)
    {
        owner.gameObjectC.GetComponentInChildren<GeneralButton>().onClick.RemoveAllListeners();
    }

    public void Start(State owner)
    {
        GeneralButton generalButton = owner.gameObjectC.GetComponentInChildren<GeneralButton>();
        generalButton.UpdateText($"{coin}");

        if (Param.param.coin < coin || Param.param.health == Param.param.maxHealth)
        {
            generalButton.UpdateInteractable(false);
            return;
        }

        generalButton.onClick.AddListener(() => Boost(owner));
    }
}

[Serializable]
public class TagUpDamage : EntityComponentDefinition, IBoost, IStartUpdate, IDisebleUpdate, IFixedUpdate
{
    public int upDamage = 1;
    public int coin = 8;

    public void Boost(State owner)
    {
        Param.param.RedactCoin(-coin);

        Param.param.upDamage += upDamage;
        owner.gameObjectC.SetActive(false);
    }

    public void Execute(State owner)
    {
        if (Param.param.coin < coin)
            owner.gameObjectC.GetComponentInChildren<GeneralButton>().UpdateInteractable(false);
    }

    public void OnDisable(State owner)
    {
        owner.gameObjectC.GetComponentInChildren<GeneralButton>().onClick.RemoveAllListeners();
    }

    public void Start(State owner)
    {
        GeneralButton generalButton = owner.gameObjectC.GetComponentInChildren<GeneralButton>();
        generalButton.UpdateText($"{coin}");

        if (Param.param.coin < coin)
        {
            generalButton.UpdateInteractable(false);
            return;
        }

        generalButton.onClick.AddListener(() => Boost(owner));
    }
}

[Serializable]
public class TagUpSpeedGun : EntityComponentDefinition, IBoost, IStartUpdate, IDisebleUpdate, IFixedUpdate
{
    public float upSpeed = .2f;
    public int coin = 5;

    public void Boost(State owner)
    {
        Param.param.speed += upSpeed;
        owner.gameObjectC.SetActive(false);
    }

    public void Execute(State owner)
    {
        if (Param.param.coin < coin)
            owner.gameObjectC.GetComponentInChildren<GeneralButton>().UpdateInteractable(false);
    }

    public void OnDisable(State owner)
    {
        owner.gameObjectC.GetComponentInChildren<GeneralButton>().onClick.RemoveAllListeners();
    }

    public void Start(State owner)
    {
        GeneralButton generalButton = owner.gameObjectC.GetComponentInChildren<GeneralButton>();
        generalButton.UpdateText($"{coin}");

        if (Param.param.coin < coin)
        {
            generalButton.UpdateInteractable(false);
            return;
        }

        generalButton.onClick.AddListener(() => Boost(owner));
    }
}

public class State : UnityEngine.Object
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