using AhpilyServer;
using GameServer.Logic;
using System;
using System.Collections.Generic;
using Protocol.Code;

namespace GameServer
{
    /// <summary>
    /// 网络的消息中心
    /// </summary>
    public class NetMsgCenter : IApplication
    {
        IHandler account = new AccountHandler();
        IHandler user = new UserHandler();
        MatchHandler match = new MatchHandler();
        IHandler chat = new ChatHandler();
        FightHandler fight = new FightHandler();

        _21SingleHandler single = new _21SingleHandler();

        _21MultiHandler multi = new _21MultiHandler();

        public NetMsgCenter()
        {
            match.startFight += fight.StartFight;
        }

        public void OnDisconnect(ClientPeer client)
        {
            single.OnDisconnect(client);
            multi.OnDisconnect(client);
            fight.OnDisconnect(client);
            chat.OnDisconnect(client);
            match.OnDisconnect(client);
            user.OnDisconnect(client);
            account.OnDisconnect(client);
            
        }

        public void OnReceive(ClientPeer client, SocketMsg msg)
        {
            switch (msg.OpCode)
            {
                case OpCode.ACCOUNT:
                    account.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.USER:
                    user.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.MATCH:
                    match.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.CHAT:
                    chat.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.FIGHT:
                    fight.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode._21Single:
                    single.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode._21Multi:
                    multi.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                default:
                    break;
            }
        }
    }
}
