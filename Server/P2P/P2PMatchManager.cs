using System;
using System.Collections.Generic;

namespace FanJun.P2PSample.Server
{
    public class P2PMatchManager
    {
        static Dictionary<string, P2PMatchPair> s_matchPairs = new Dictionary<string, P2PMatchPair>();


        public static string AddNew(string sourceUser, string targetUser)
        {
            string key = Guid.NewGuid().ToString("n");
            P2PMatchPair pair = new P2PMatchPair(key, sourceUser, targetUser);
            lock (s_matchPairs)
            {
                s_matchPairs.Add(key, pair);
            }
            return key;
        }

        public static P2PMatchPair Get(string key)
        {
            P2PMatchPair val;
            lock (s_matchPairs)
            {
                s_matchPairs.TryGetValue(key, out val);
            }
            return val;
        }
    }
}
