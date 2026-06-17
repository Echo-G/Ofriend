using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Ofriend.Players;
using Ofriend.Systems;
using Ofriend.Projectiles;
using System;

namespace Ofriend.Items
{

    public class Gohei : ModItem
    {
        private int homingCooldown = 0;
        private int bombTimer = 0;
        private int bombState = 0;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.damage = 20;
            Item.knockBack = 2f;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<Ofuda>();
            Item.shootSpeed = 16f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            var powerPlayer = player.GetModPlayer<PowerPlayer>();

            // 右键消耗Power
            if (player.altFunctionUse == 2)
            {
                if (powerPlayer.Bomb())
                {
                    // 初始化Bomb状态和计时器
                    bombState = 1;
                    bombTimer = 0;
                }
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 计算伤害为面板的40%
            int adjustedDamage = (int)(damage * 0.4f);

            // 发射平行的两个符札射弹（不重叠）
            float offsetDistance = 10f; // 偏移距离，确保不重叠
            Vector2 perpendicular = new Vector2(-velocity.Y, velocity.X).SafeNormalize(Vector2.Zero);

            for (int i = 0; i < 2; i++)
            {
                // 计算偏移位置
                Vector2 offset = perpendicular * offsetDistance * (i == 0 ? -1 : 1);
                Vector2 spawnPosition = position + offset;
                // 保持相同的速度向量，实现平行移动
                Projectile.NewProjectile(source, spawnPosition, velocity, type, adjustedDamage, knockback, player.whoAmI);
            }

            // 发射追踪射弹
            homingCooldown--;
            if (homingCooldown <= 0)
            {
                var powerPlayer = player.GetModPlayer<PowerPlayer>();
                int power = powerPlayer.power;
                int homingCount = 0;

                if (power >= 1000 && power < 2000)
                {
                    homingCount = 2;
                }
                else if (power >= 2000 && power < 3000)
                {
                    homingCount = 4;
                }
                else if (power >= 3000 && power <= 4000)
                {
                    homingCount = 6;
                }

                if (homingCount > 0)
                {
                    // 计算伤害为面板的60%
                    int homingDamage = (int)(damage * 0.6f);

                    // 发射追踪射弹
                    float homingSpread = MathHelper.ToRadians(15);
                    float angleStep = homingCount > 1 ? homingSpread * 2 / (homingCount - 1) : 0;

                    for (int i = 0; i < homingCount; i++)
                    {
                        float angle = velocity.ToRotation() - homingSpread + i * angleStep;
                        Vector2 homingVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 12f;
                        Projectile.NewProjectile(source, position, homingVelocity, ModContent.ProjectileType<HomingAmulet>(), homingDamage, knockback, player.whoAmI);
                    }
                }
                homingCooldown = 4;
            }

            return false;
        }

        public override void UpdateInventory(Player player)
        {
            // 处理Bomb弹幕释放
            if (bombState > 0)
            {
                bombTimer++;

                // 第一秒结束释放第一个弹幕
                if (bombState == 1 && bombTimer >= 60)
                {
                    SpawnDreamSeal(player);
                    bombState = 2;
                }
                // 第二秒结束释放第二个弹幕
                else if (bombState == 2 && bombTimer >= 120)
                {
                    SpawnDreamSeal(player);
                    bombState = 3;
                }
                // 第三秒结束释放第三个弹幕
                else if (bombState == 3 && bombTimer >= 180)
                {
                    SpawnDreamSeal(player);
                    bombState = 4;
                }
                // 之后每0.5秒释放一个弹幕，直到6秒结束
                else if (bombState == 4 && bombTimer >= 210 && (bombTimer - 210) % 30 == 0)
                {
                    SpawnDreamSeal(player);
                    // 6秒结束
                    if (bombTimer >= 360)
                    {
                        bombState = 0;
                        bombTimer = 0;
                    }
                }
            }
        }

        private void SpawnDreamSeal(Player player)
        {
            // 释放一个DreamSeal弹幕
            float angle = Main.rand.NextFloat((float)Math.PI * 2);
            Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 8f;
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, velocity, ModContent.ProjectileType<DreamSeal>(), 30, 2f, player.whoAmI);
        }
    }
}
