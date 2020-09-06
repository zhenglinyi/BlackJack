using Protocol.Constant.Train;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunErrorPanel : UIBase
{

    private void Awake()
    {
        Bind(UIEvent.RUN_SHOW_ERROR_PANEL);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.RUN_SHOW_ERROR_PANEL:
                refreshPanel(message as RunError);
                break;
            default:
                break;
        }
    }

    private Text txtError;
    private Button btnBack;

    void Start()
    {
        txtError = transform.Find("txtError").GetComponent<Text>();
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(backClick);

        setPanelActive(false);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="re"></param>
    private void refreshPanel(RunError re)
    {
        //显示面板
        setPanelActive(true);
        txtError.text = "错 误";
        txtError.text += "之前的流水数 ：" + re.lastCount+"\n";
        txtError.text += "当前的流水数 ：" + re.nowCount + "\n";
        for(int i=0;i< re.cardList.Count;i++)
        {
            if (i % 5 == 0)
                txtError.text += "\n";
            int w = re.cardList[i].Weight;
            txtError.text += w + " ( ";
            if(w >= 10||w==1)
            {
                txtError.text += "1";
            }
            else if(w>=7)
            {
                txtError.text += "0";
            }
            else
            {
                txtError.text += "-1";
            }
            txtError.text +=  " ) ";
        }
        
        

    }
        /// <summary>
        /// 返回，显示继续按钮
        /// </summary>
    private void backClick()
    {
        setPanelActive(false);
        Dispatch(AreaCode.UI, UIEvent.RUN_SHOW_NEXT_BUTTON, null);

    }
}
