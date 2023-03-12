using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* using UnityEngine.Events; */
using System;

/* [System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }; */


public class MouseManager : Singleton<MouseManager>
{

    /* 代理模式 */
    /*    public static MouseManager Instance; */
    /* 鼠标图片变量 */
    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitInfo;
    /* 保存射线碰撞到的物体的相关信息 */

    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;


    /*     void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }
     */
    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    /*鼠标在射线触碰到不同东西的时候，会有鼠标的变化 */
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;


                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    /* 点击的那一下返回的信息 */
    void MouseControl()
    {
        /* 左键0,同时点击的碰撞体不为空*/
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);


        }
    }
}
