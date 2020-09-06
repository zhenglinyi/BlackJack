using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategyTipPanel : UIBase
{

    private void Awake()
    {
        Bind(UIEvent.STRATEGY_SHOW_TIP_PANEL);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.STRATEGY_SHOW_TIP_PANEL:
                refreshPanel();
                break;
            default:
                break;
        }
    }

    private Button btnBack;

    void Start()
    {
        
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(backClick);

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
    private void backClick()
    {
        setPanelActive(false);

    }
}
