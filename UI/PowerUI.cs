using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Ofriend.Systems;
using Ofriend.Players;
using System;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.GameContent;

namespace Ofriend.UI
{
    public class PowerUI : UIState
    {
        private PowerBar powerBar;

        public override void OnInitialize()
        {
            powerBar = new PowerBar();
            UpdatePosition();
            Append(powerBar);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            var config = ModContent.GetInstance<OfriendConfig>();
            // 更新位置（使用像素单位）
            powerBar.Left.Set(config.PowerUIPositionX, 0f);
            powerBar.Top.Set(config.PowerUIPositionY, 0f);
        }
    }

    public class PowerBar : UIElement
    {
        private const int BackgroundWidth = 164;
        private const int BackgroundHeight = 28;
        private const int FillerWidth = 138;
        private const int FillerHeight = 20;
        private const int FillerPaddingX = 22;
        private const int FillerPaddingY = 4;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.active)
            {
                var powerPlayer = Main.LocalPlayer.GetModPlayer<PowerPlayer>();
                DrawPowerBar(spriteBatch, powerPlayer.power);
            }
        }

        private void DrawPowerBar(SpriteBatch spriteBatch, int power)
        {
            var config = ModContent.GetInstance<OfriendConfig>();
            // 获取元素位置
            Vector2 position = GetDimensions().Position();

            // 绘制背景
            Rectangle backgroundRect = new Rectangle((int)position.X, (int)position.Y, BackgroundWidth, BackgroundHeight);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Ofriend/Assets/UI/PowerBarBackground").Value, 
                backgroundRect, Color.White);

            // 绘制填充物
            float powerPercentage = (float)power / PowerSystem.MaxPower;
            int fillerWidth = (int)(FillerWidth * powerPercentage);
            if (fillerWidth > 0)
            {
                Rectangle fillerRect = new Rectangle(
                    (int)position.X + FillerPaddingX,
                    (int)position.Y + FillerPaddingY,
                    fillerWidth,
                    FillerHeight
                );
                spriteBatch.Draw(ModContent.Request<Texture2D>("Ofriend/Assets/UI/PowerBarFiller").Value, 
                    fillerRect, Color.White);
            }

            // 绘制框架
            Rectangle frameRect = new Rectangle((int)position.X, (int)position.Y, BackgroundWidth, BackgroundHeight);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Ofriend/Assets/UI/PowerBarFrame").Value, 
                frameRect, Color.White);

            // 如果配置允许，绘制文本
            if (config.ShowPowerText)
            {
                string powerText = $"Power: {power}/{PowerSystem.MaxPower}";
                Vector2 textPosition = position + new Vector2(BackgroundWidth / 2f, BackgroundHeight + 9); // 向下移动4像素
                Vector2 textOrigin = FontAssets.MouseText.Value.MeasureString(powerText) / 2f;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, powerText, textPosition, Color.White, 0f, textOrigin, Vector2.One * 0.8f);
            }
        }
    }
} 