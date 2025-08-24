using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Ofriend.Systems;
using Ofriend.UI;

namespace Ofriend
{
    public class OfriendConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Power_UI_Settings")]
        [Label("Show_Power_Text")]
        [Tooltip("Whether to show the power text on the UI")]
        [DefaultValue(false)]
        public bool ShowPowerText;

        [Label("UI_Position_X")]
        [Tooltip("Distance from left edge of screen in pixels")]
        [Range(0, 2000)]
        [DefaultValue(580)]
        [Slider]
        public int PowerUIPositionX;

        [Label("UI_Position_Y")]
        [Tooltip("Distance from top edge of screen in pixels")]
        [Range(0, 1000)]
        [DefaultValue(30)]
        [Slider]
        public int PowerUIPositionY;

        public override void OnChanged()
        {
            // 获取 PowerUISystem 实例
            var powerUISystem = ModContent.GetInstance<PowerUISystem>();
            if (powerUISystem?.PowerInterface?.CurrentState is PowerUI powerUI)
            {
                powerUI.UpdatePosition();
            }
        }
    }
} 