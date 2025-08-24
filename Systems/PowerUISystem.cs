using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Ofriend.UI;
using System.Collections.Generic;

namespace Ofriend.Systems
{
    public class PowerUISystem : ModSystem
    {
        public UserInterface PowerInterface;

        public override void Load()
        {
            // 初始化 UI
            PowerInterface = new UserInterface();
            PowerInterface.SetState(new PowerUI());
        }

        public override void Unload()
        {
            // 清理 UI
            PowerInterface?.SetState(null);
            PowerInterface = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // 更新 UI
            PowerInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // 找到绘制 HUD 的层
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                // 在鼠标文本层之前插入我们的 UI
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Ofriend: Power UI",
                    delegate
                    {
                        PowerInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
    }
} 