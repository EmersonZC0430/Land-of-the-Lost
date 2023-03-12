using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMunie : MonoBehaviour
{
    Button newGameBtn, quitBtn;
    void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        quitBtn = transform.GetChild(2).GetComponent<Button>();

        quitBtn.onClick.AddListener(QuitGame);
        newGameBtn.onClick.AddListener(stargame);
    }
    void stargame()
    {
        SceneManager.LoadScene("1");//引号内为要跳转的场景名

    }
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("esc");
    }

}
