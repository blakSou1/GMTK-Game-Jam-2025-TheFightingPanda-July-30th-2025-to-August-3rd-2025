using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Param
    public static Player player;
    [SerializeField] private CanvasGroup canvasGroupLouse;

    public CMSEntityPfb cMSEntityPfb;
    private TagGun GunModel;
    private List<GameObject> bulletsPool = new();
    [SerializeField] private LayerMask layerMask;

    private Coroutine coroutine;
    private float timePause = 0;

    #region Components
    private Transform parent; // Родитель для объектов
    public Transform startPos;
    private Animator anim;
    private AudioSource audioSourceShot;

    private List<IDisebleUpdate> ComponentsDisableUpdate;
    private State state;

    public Param param;
    #endregion
    #endregion

    private void OnDead()
    {
        StartCoroutine(Controller.controller.UpdatePanel(canvasGroupLouse));
    }
    public void Start()
    {
        state = new State()
        {
            monoBehaviour = this
        };

        Param.param = param;
        param.RedactCoin(0);
        param.player = gameObject;
        param.monoBehaviour = this;

        param.Dead += OnDead;

        GunModel = cMSEntityPfb.Components.OfType<TagGun>().FirstOrDefault();

        audioSourceShot = GetComponent<AudioSource>();
        audioSourceShot.clip = GunModel.audioClip;

        if (player != null)
            Debug.Log("Far!");
        player = this;

        timePause = Time.time;

        anim = GetComponent<Animator>();


        parent = new GameObject("BulletPoolParent").transform;

        for (int i = 0; i < GunModel.pool; i++)
        {
            GameObject obj = Instantiate(GunModel.bulletPrefab, parent);
            obj.SetActive(false);
            bulletsPool.Add(obj);
        }

        ComponentsDisableUpdate = cMSEntityPfb.Components?
        .Where(c => c is IDisebleUpdate)
        .Cast<IDisebleUpdate>()
        .ToList() ?? new List<IDisebleUpdate>();

    }

    private void OnEnable()
    {
        ApplicationController.inputPlayer.Player.Attack.performed += i => coroutine = StartCoroutine(Shooting());
        ApplicationController.inputPlayer.Player.Attack.canceled += i => StopCoroutine(coroutine);
    }
    private void OnDisable()
    {
        ApplicationController.inputPlayer.Player.Attack.performed -= i => coroutine = StartCoroutine(Shooting());
        ApplicationController.inputPlayer.Player.Attack.canceled -= i => StopAllCoroutines();

        foreach (IDisebleUpdate enableUpdate in ComponentsDisableUpdate)
            enableUpdate.OnDisable(state);
    }
    private void OnDestroy()
    {
        param.Dead -= OnDead;
    }

    public IEnumerator Shooting()
    {
        while (true)
        {
            if (Time.time - timePause < GunModel.pause)
                yield return new WaitForSeconds(GunModel.pause - (Time.time - timePause));

            GameObject bullets = bulletsPool.FirstOrDefault(go => !go.activeInHierarchy);
            if (bullets != null)
            {
                bullets.transform.position = startPos.position;
                bullets.SetActive(true);

                anim.SetTrigger("Atack");
                audioSourceShot.Play();

                GunModel.Execute(new State
                {
                    gameObjectC = bullets,
                    inputPosition = ApplicationController.inputPlayer.Player.Aim.ReadValue<Vector2>(),
                    monoBehaviour = state.monoBehaviour
                });
            }

            timePause = Time.time;
        }
    }
}