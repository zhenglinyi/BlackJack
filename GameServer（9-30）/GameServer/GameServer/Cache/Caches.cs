using GameServer.Cache.Fight;
using GameServer.Cache.Match;
using GameServer.Cache._21single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Cache._21mutiFight;

namespace GameServer.Cache
{
    public static class Caches
    {
        public static AccountCache Account { get; set; }
        public static UserCache User { get; set; }
        public static MatchCache Match { get; set; }
        public static FightCache Fight { get; set; }

        public static _21SingleCache Single { get; set; }

        public static _21MutiFightCache Multi { get; set; }

        static Caches()
        {
            Account = new AccountCache();
            //Account.Create("1", "12345");
            //Account.Create("2", "12345");
            //Account.Create("3", "12345");
            //Account.Create("4", "12345");
            //Account.Create("5", "12345");
            User = new UserCache();
            Match = new MatchCache();
            Fight = new FightCache();
            Single = new _21SingleCache();
            Multi = new _21MutiFightCache();
            
        }
    }
}
