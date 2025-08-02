using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Param
{
    public static Param param;
    public int health = 100;
    public int maxHealth = 100;
    public int coin = 0;
    public int upDamage = 0;

    public TextMeshProUGUI textCoin;
    [SerializeField] private Image hpBar;
    public event Action Dead;

    public void RedactCoin(int i)
    {
        coin += i;
        textCoin.text = $"{coin}";
    }
    public void AddHealth(int hp)
    {
        health += hp;
        health = Math.Min(health, maxHealth);
        hpBar.fillAmount = Mathf.Clamp01((float)param.health / (float)param.maxHealth);
    }
    public void IsDamage(int damage)
    {
        if (param.health <= 0)
            return;
            
        param.health -= damage;

        hpBar.fillAmount = Mathf.Clamp01((float)param.health / (float)param.maxHealth);

        if (param.health <= 0)
            Dead?.Invoke();
    }

}
