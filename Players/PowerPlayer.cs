using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ModLoader.IO;
using Ofriend.Systems;

namespace Ofriend.Players
{
    public class PowerPlayer : ModPlayer
    {
        public int power = 0;
        private int lastPower = 0;
        private readonly int[] powerThresholds = new int[] { 1000, 2000, 3000, 4000 };

        public override void PostUpdate()
        {
            lastPower = power;
        }

        // 增加能力值
        public void AddPower(int amount)
        {
            int oldPower = power;
            power += amount;
            ClampPower();
            CheckThresholds(oldPower, power);
        }

        // 减少能力值
        public void ReducePower(int amount)
        {
            int oldPower = power;
            power -= amount;
            ClampPower();
            CheckThresholds(oldPower, power);
        }

        public bool Bomb()
        {
            if(power >= 1000){
                power -= 1000;
                BombSound();
                return true;
            }
            return false;
        }
        // 设置能力值
        public void SetPower(int value)
        {
            int oldPower = power;
            power = value;
            ClampPower();
            CheckThresholds(oldPower, power);
        }

        // 边界检查
        private void ClampPower()
        {
            if (power < 0) power = 0;
            if (power > PowerSystem.MaxPower) power = PowerSystem.MaxPower;
        }

        // 检查阈值
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
        //播放放符卡音效
        private static void BombSound()
        {
            // 播放音效
            SoundEngine.PlaySound(new SoundStyle("Ofriend/Assets/Sounds/Bomb"));
        }

        // 播放能力值增加音效
        private static void PlayPowerupSound()
        {
            // 播放音效
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