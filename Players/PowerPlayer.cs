using Ofriend.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Ofriend.Players
{
    public class PowerPlayer : ModPlayer
    {
        public int power = 0;

        private readonly int[] powerThresholds = new int[] { 1000, 2000, 3000, 4000 };

        public int PowerLevel => PowerSystem.GetPowerLevel(power);

        public void AddPower(int amount)
        {
            int oldPower = power;
            power += amount;
            ClampPower();
            CheckThresholds(oldPower, power);
        }

        public void ReducePower(int amount)
        {
            int oldPower = power;
            power -= amount;
            ClampPower();
            CheckThresholds(oldPower, power);
        }

        public bool Bomb()
        {
            if (power >= PowerSystem.BombCost)
            {
                power -= PowerSystem.BombCost;
                BombSound();
                return true;
            }

            return false;
        }

        public void SetPower(int value)
        {
            int oldPower = power;
            power = value;
            ClampPower();
            CheckThresholds(oldPower, power);
        }

        private void ClampPower()
        {
            if (power < 0)
            {
                power = 0;
            }

            if (power > PowerSystem.MaxPower)
            {
                power = PowerSystem.MaxPower;
            }
        }

        private void CheckThresholds(int oldPower, int newPower)
        {
            foreach (int threshold in powerThresholds)
            {
                if (oldPower < threshold && newPower >= threshold)
                {
                    PlayPowerupSound();
                    break;
                }
            }
        }

        private static void BombSound()
        {
            SoundEngine.PlaySound(new SoundStyle("Ofriend/Assets/Sounds/Bomb"));
        }

        private static void PlayPowerupSound()
        {
            SoundEngine.PlaySound(new SoundStyle("Ofriend/Assets/Sounds/Powerup"));
        }

        public override void SaveData(TagCompound tag)
        {
            tag["power"] = power;
        }

        public override void LoadData(TagCompound tag)
        {
            power = tag.Get<int>("power");
        }
    }
}
