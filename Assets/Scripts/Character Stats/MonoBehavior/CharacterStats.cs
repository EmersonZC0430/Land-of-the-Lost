using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;


    void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    /* 读写数据 */
    #region read from Data_SO
    public int MaxHealth
    {
        get
        {
            if (characterData != null)
            {
                return characterData.maxHealth;
            }
            else return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }
    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentHealth;
            }
            else return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }
    public int BaseDefence
    {
        get
        {
            if (characterData != null)
            {
                return characterData.baseDefence;
            }
            else return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }
    }
    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentDefence;
            }
            else return 0;
        }
        set
        {
            characterData.currentDefence = value;
        }
    }

    #endregion


    #region Character Combat
    public void TakeDamge(CharacterStats attacker, CharacterStats defener)
    {

        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);


        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");



        }

        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        //UI面板
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.KillPoint);
        }
    }
    public void TakeDamge(int damage, CharacterStats defener)
    {
        int CurrentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - CurrentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStates.characterData.UpdateExp(characterData.KillPoint);
        }

    }
    private int CurrentDamage()
    {
        /* 伤害随机 */
        float coreDamage = UnityEngine.Random.Range(attackData.minDamge, attackData.maxDamge);
        Debug.Log("攻击" + coreDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击" + coreDamage);
        }


        return (int)coreDamage;
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
