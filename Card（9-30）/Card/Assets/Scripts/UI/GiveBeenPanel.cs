using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiveBeenPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.SHOW_GIVE_PANEL);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SHOW_GIVE_PANEL:
                refreshPanel();
                break;
            default:
                break;
        }
    }

    
    private Button btnOK;

    void Start()
    {
        
        btnOK = transform.Find("btnOK").GetComponent<Button>();
        btnOK.onClick.AddListener(okClick);

        setPanelActive(false);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="re"></param>
    private void refreshPanel()
    {
        //显示面板
        setPanelActive(true);

    }
    /// <summary>
    /// 返回，显示继续按钮
    /// </summary>
    private void okClick()
    {
        setPanelActive(false);

    }

}
