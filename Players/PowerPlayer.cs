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
            // 检查是否跨过任何阈值
            foreach (int threshold in powerThresholds)
            {
                if (lastPower < threshold && power >= threshold)
                {
                    PlayPowerupSound();
                    break;
                }
            }
            lastPower = power;
        }

        private void PlayPowerupSound()
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