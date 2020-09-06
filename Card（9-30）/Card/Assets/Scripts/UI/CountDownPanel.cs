using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.SHOW_COUNT_DOWN_PANEL,
            UIEvent.STOP_COUNT_DOWN_PANEL);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SHOW_COUNT_DOWN_PANEL:

                countDownMessage();
                break;
            case UIEvent.STOP_COUNT_DOWN_PANEL:
                stopCountDownMessage();
                break;
            default:
                break;
        }
    }

    private Text txt;
    private CanvasGroup cg;

    [SerializeField]
    [Range(0, 3)]
    private float showTime = 0.6f;

    private float timer = 0f;
    private IEnumerator coroutine;
    // Use this for initialization
    void Start()
    {
        txt = transform.Find("Text").GetComponent<Text>();
        cg = transform.Find("Text").GetComponent<CanvasGroup>();

        cg.alpha = 0;
    }
    private void countDownMessage()
    {
        gameObject.SetActive(true);
        StartCoroutine("promptAnim2");
    }
    private void stopCountDownMessage()
    {
        gameObject.SetActive(false);
        StopCoroutine("promptAnim2");
        //StopCoroutine(coroutine);
        //StopAllCoroutines();
    }
    IEnumerator promptAnim2()
    {
        for (int i = 3; i >= 0; i--)
        {
            if(i==0)
            {
                txt.text = "开始";
            }
            else
            {
                txt.text = i.ToString();
            }
            
            cg.alpha = 0;
            timer = 0;
            while (cg.alpha < 1f)
            {
                cg.alpha += Time.deltaTime * 5;
                yield return new WaitForEndOfFrame();
            }
            while (timer < showTime)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            while (cg.alpha > 0)
            {
                cg.alpha -= Time.deltaTime * 5;
                yield return new WaitForEndOfFrame();
            }
        }

    }
    

    

}
