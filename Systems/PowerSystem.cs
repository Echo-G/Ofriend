using Terraria.ModLoader;

namespace Ofriend.Systems
{
    public class PowerSystem : ModSystem
    {
        public const int MaxPower = 4000;
        public const int BombCost = 1000;

        public static int GetPowerLevel(int power)
        {
            if (power >= 3000)
            {
                return 4;
            }

            if (power >= 2000)
            {
                return 3;
            }

            if (power >= 1000)
            {
                return 2;
            }

            return 1;
        }
    }
}
