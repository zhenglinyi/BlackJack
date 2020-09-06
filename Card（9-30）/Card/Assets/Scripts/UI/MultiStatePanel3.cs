using Protocol.Dto;
using Protocol.Dto.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiStatePanel3 : MultiStatePanel
{

    protected override void Awake()
    {
        base.Awake();
        Bind(UIEvent.SET_3_PLAYER_DATA);

    }

    public override void Execute(int eventCode, object message)
    {
        base.Execute(eventCode, message);
        switch (eventCode)
        {
            case UIEvent.SET_3_PLAYER_DATA:
                this.userDto = message as UserDto;
                break;
            default:
                break;
        }
    }

    protected override void Start()
    {
        base.Start();

        int userId = -1;
        //如果！= -1 就代表 有角色
        MutiRoomDto room = Models.GameModel.MutiRoomDto;
        foreach (KeyValuePair<int, int> up in room.UIdPositionDict)
        {
            if (up.Value == 3)
            {
                userId = up.Key;
                break;
            }

        }

        if (userId != -1)
        {
            this.userDto = room.UIdUserDict[userId];
            if (Models.GameModel.UserDto.Id == userDto.Id)
            {
                imgIdentity.sprite = Resources.Load<Sprite>("Identity/Landlord");
            }
            //fixbug923
            if (room.ReadyUIdList.Contains(userId))
            {
                readyState();
            }
            else
            {
                //nothing 可以显示一个未准备
            }
        }
        else
        {
            setPanelActive(false);
        }
    }


}
