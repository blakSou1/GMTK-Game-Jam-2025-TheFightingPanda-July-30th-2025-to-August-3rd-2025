using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[Serializable]
public class TagUpGun : EntityComponentDefinition, IBoost
{
    public int coin = 1;
    public CMSEntityPfb GunModel;
    public float timePause = .1f;
    public int DebafDamage = 2;

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        Start(cMSEntityPfb.state);
        cMSEntityPfb.Update += Execute;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.state.gameObjectC.GetComponentInChildren<GeneralButton>().onClick.RemoveAllListeners();
        cMSEntityPfb.Update -= Execute;
    }

    public void Boost(State owner)
    {
        G.param.RedactCoin(-coin);

        G.param.upDamage -= DebafDamage;
        GunModel.Components.OfType<TagGun>().FirstOrDefault().IsAtack += i => G.monoBehaviour.StartCoroutine(UpAtack(i));
        G.param.coin -= coin;

        owner.gameObjectC.SetActive(false);
    }
    private IEnumerator UpAtack(State owner)
    {
        GameObject bullets = GameObject.Instantiate(owner.gameObjectC, owner.gameObjectC.transform.parent);
        bullets.transform.position = bullets.transform.position;
        bullets.SetActive(true);

        yield return new WaitForSeconds(timePause);

        GameObject.Destroy(bullets, 1);

        G.monoBehaviour.StartCoroutine(GunModel.Components.OfType<TagGun>().FirstOrDefault().Atack(new State
        {
            gameObjectC = bullets,
            inputPosition = owner.inputPosition,
            monoBehaviour = G.monoBehaviour
        }));
    }

    private void Execute(State owner)
    {
        if (G.param.coin < coin)
            owner.gameObjectC.GetComponentInChildren<GeneralButton>().UpdateInteractable(false);
    }

    private void Start(State owner)
    {
        GeneralButton generalButton = owner.gameObjectC.GetComponentInChildren<GeneralButton>();
        generalButton.UpdateText($"{coin}");

        if (G.param.coin < coin)
        {
            generalButton.UpdateInteractable(false);
            return;
        }

        generalButton.onClick.AddListener(() => Boost(owner));
    }
}

[Serializable]
public class TagHeling : EntityComponentDefinition, IBoost
{
    public int hp = 20;
    public int coin = 3;
    
    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        Start(cMSEntityPfb.state);
        cMSEntityPfb.Update += Execute;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.state.gameObjectC.GetComponentInChildren<GeneralButton>().onClick.RemoveAllListeners();
        cMSEntityPfb.Update -= Execute;
    }

    public void Boost(State owner)
    {
        G.param.RedactCoin(-coin);

        G.param.AddHealth(hp);
        owner.gameObjectC.SetActive(false);
    }

    private void Execute(State owner)
    {
        if (G.param.coin < coin || G.param.health == G.param.maxHealth)
            owner.gameObjectC.GetComponentInChildren<GeneralButton>().UpdateInteractable(false);
    }

    private void Start(State owner)
    {
        GeneralButton generalButton = owner.gameObjectC.GetComponentInChildren<GeneralButton>();
        generalButton.UpdateText($"{coin}");

        if (G.param.coin < coin || G.param.health == G.param.maxHealth)
        {
            generalButton.UpdateInteractable(false);
            return;
        }

        generalButton.onClick.AddListener(() => Boost(owner));
    }
}

[Serializable]
public class TagUpDamage : EntityComponentDefinition, IBoost
{
    public int upDamage = 1;
    public int coin = 8;

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        Start(cMSEntityPfb.state);
        cMSEntityPfb.Update += Execute;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.state.gameObjectC.GetComponentInChildren<GeneralButton>().onClick.RemoveAllListeners();
        cMSEntityPfb.Update -= Execute;
    }

    public void Boost(State owner)
    {
        G.param.RedactCoin(-coin);

        G.param.upDamage += upDamage;
        owner.gameObjectC.SetActive(false);
    }

    private void Execute(State owner)
    {
        if (G.param.coin < coin)
            owner.gameObjectC.GetComponentInChildren<GeneralButton>().UpdateInteractable(false);
    }

    private void Start(State owner)
    {
        GeneralButton generalButton = owner.gameObjectC.GetComponentInChildren<GeneralButton>();
        generalButton.UpdateText($"{coin}");

        if (G.param.coin < coin)
        {
            generalButton.UpdateInteractable(false);
            return;
        }

        generalButton.onClick.AddListener(() => Boost(owner));
    }
}

[Serializable]
public class TagUpSpeedGun : EntityComponentDefinition, IBoost
{
    public float upSpeed = .2f;
    public int coin = 5;

    public override void Init(CMSEntityPfb cMSEntityPfb)
    {
        Start(cMSEntityPfb.state);
        cMSEntityPfb.Update += Execute;
    }
    
    public override void OnDestroy(CMSEntityPfb cMSEntityPfb)
    {
        cMSEntityPfb.state.gameObjectC.GetComponentInChildren<GeneralButton>().onClick.RemoveAllListeners();
        cMSEntityPfb.Update -= Execute;
    }

    public void Boost(State owner)
    {
        G.param.RedactCoin(-coin);

        G.param.speed += upSpeed;
        owner.gameObjectC.SetActive(false);
    }

    private void Execute(State owner)
    {
        if (G.param.coin < coin)
            owner.gameObjectC.GetComponentInChildren<GeneralButton>().UpdateInteractable(false);
    }

    private void Start(State owner)
    {
        GeneralButton generalButton = owner.gameObjectC.GetComponentInChildren<GeneralButton>();
        generalButton.UpdateText($"{coin}");

        if (G.param.coin < coin)
        {
            generalButton.UpdateInteractable(false);
            return;
        }

        generalButton.onClick.AddListener(() => Boost(owner));
    }
}
