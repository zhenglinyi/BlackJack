using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiStatePanel : UIBase
{
    protected virtual void Awake()
    {
        Bind(UIEvent.MULTI_PLAYER_HIDE_STATE,
            UIEvent.MULTI_PLAYER_READY,
            UIEvent.MULTI_PLAYER_LEAVE,
            UIEvent.MULTI_PLAYER_ENTER,
            UIEvent.MULTI_PLAYER_CHANGE_IDENTITY
            );
        
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.MULTI_PLAYER_HIDE_STATE:
                {
                    txtReady.gameObject.SetActive(false);
                }
                break;
            case UIEvent.MULTI_PLAYER_READY:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    //如果是自身角色 就显示
                    if (userDto.Id == userId)
                        readyState();
                    break;
                }
            case UIEvent.MULTI_PLAYER_LEAVE:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    if (userDto.Id == userId)
                        setPanelActive(false);
                    break;
                }
            case UIEvent.MULTI_PLAYER_ENTER:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    if (userDto.Id == userId)
                        setPanelActive(true);
                    break;
                }
            
            case UIEvent.MULTI_PLAYER_CHANGE_IDENTITY:
                {
                    if (userDto == null)
                        break;
                    int userId = (int)message;
                    if (userDto.Id == userId)
                        setIdentity(1);
                    break;
                }
            default:
                break;
        }
    }

    /// <summary>
    /// 角色的数据
    /// </summary>
    protected UserDto userDto;

    protected Image imgIdentity;
    protected Text txtReady;

    protected virtual void Start()
    {
        imgIdentity = transform.Find("imgIdentity").GetComponent<Image>();
        
        
        txtReady = transform.Find("txtReady").GetComponent<Text>();
        

        //默认状态
        txtReady.gameObject.SetActive(false);
        
    }

    protected virtual void readyState()
    {
        txtReady.gameObject.SetActive(true);
    }

    protected void setIdentity(int identity)
    {
        //string identityStr = identity == 0 ? "Farmer" : "Landlord";
        if (identity == 0)
        {
            imgIdentity.sprite = Resources.Load<Sprite>("Identity/Farmer");
        }
        else if (identity == 1)
        {
            imgIdentity.sprite = Resources.Load<Sprite>("Identity/Landlord");
        }
    }
}
