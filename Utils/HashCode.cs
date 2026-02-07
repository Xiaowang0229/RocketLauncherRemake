using System;
using System.Security.Cryptography;

namespace RocketLauncherRemake.Utils
{

    public static class HashCode
    {
        public static string RandomHashGenerate(int byteLength = 16)
        {
            byte[] bytes = new byte[byteLength];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToHexString(bytes);
        }



        public static int FindHashcodeinGameinfosint(MainConfig config, string hashcode)
        {
            for (int i = 0; i < config.GameInfos.Count; i++)
            {
                if (config.GameInfos[i].HashCode == hashcode)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
