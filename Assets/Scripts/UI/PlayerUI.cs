using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    Text levelText;
    Image healthSlider;
    Image expSlider;
    void Awake()
    {
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();

    }

    void Update()
    {
        levelText.text = "Level" + GameManager.Instance.playerStates.characterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    void UpdateHealth()
    {
        float sliderPerecent = (float)GameManager.Instance.playerStates.CurrentHealth / GameManager.Instance.playerStates.MaxHealth;
        healthSlider.fillAmount = sliderPerecent;
    }

    void UpdateExp()
    {
        float sliderPerecent = (float)GameManager.Instance.playerStates.characterData.currentExp / GameManager.Instance.playerStates.characterData.baseExp;
        expSlider.fillAmount = sliderPerecent;
    }
}
