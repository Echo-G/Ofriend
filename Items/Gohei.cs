using Microsoft.Xna.Framework;
using Ofriend.Players;
using Ofriend.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ofriend.Items
{
    public class Gohei : ModItem
    {
        public const int PanelDamage = 40;
        public const float OfudaDamageMultiplier = 0.40f;
        public const float HomingAmuletDamageMultiplier = 0.55f;
        public const float DreamSealDamageMultiplier = 1.50f;

        public const float OfudaSpeed = 20f;
        public const float HomingAmuletInitialSpeed = 12f;
        public const float HomingAmuletKnockBack = 2f;
        public const float DreamSealInitialSpeed = 8f;
        public const float DreamSealKnockBack = 2f;

        private const int OrbFireDelayFrames = 3;

        public override void SetDefaults()
        {
            Item.width = 33;
            Item.height = 33;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.damage = PanelDamage;
            Item.knockBack = 2f;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<Ofuda>();
            Item.shootSpeed = OfudaSpeed;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.WandofSparking)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            GoheiPlayer goheiPlayer = player.GetModPlayer<GoheiPlayer>();

            if (player.altFunctionUse == 2)
            {
                return !goheiPlayer.IsBombActive;
            }

            return !goheiPlayer.IsBombActive;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                return true;
            }

            PowerPlayer powerPlayer = player.GetModPlayer<PowerPlayer>();
            GoheiPlayer goheiPlayer = player.GetModPlayer<GoheiPlayer>();

            if (!goheiPlayer.IsBombActive && powerPlayer.Bomb())
            {
                goheiPlayer.StartBomb();
            }

            return true;
        }

        public override void HoldItem(Player player)
        {
            MaintainYinYangOrbs(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 || player.GetModPlayer<GoheiPlayer>().IsBombActive)
            {
                return false;
            }

            int powerLevel = player.GetModPlayer<PowerPlayer>().PowerLevel;
            int ofudaCount = GetOfudaCount(powerLevel);
            int ofudaDamage = GetOfudaDamage(damage);
            Vector2 shotVelocity = velocity.SafeNormalize(Vector2.UnitX) * OfudaSpeed;
            Vector2 perpendicular = shotVelocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);

            foreach (float offset in GetOfudaOffsets(ofudaCount))
            {
                Projectile.NewProjectile(
                    source,
                    position + perpendicular * offset,
                    shotVelocity,
                    type,
                    ofudaDamage,
                    knockback,
                    player.whoAmI);
            }

            SignalOrbsToFire(player, GetHomingAmuletDamage(damage));
            return false;
        }

        public static int GetOfudaDamage(int panelDamage)
        {
            return (int)(panelDamage * OfudaDamageMultiplier);
        }

        public static int GetHomingAmuletDamage(int panelDamage)
        {
            return (int)(panelDamage * HomingAmuletDamageMultiplier);
        }

        public static int GetDreamSealDamage(int panelDamage)
        {
            return (int)(panelDamage * DreamSealDamageMultiplier);
        }

        public static int GetOfudaCount(int powerLevel)
        {
            return powerLevel >= 3 ? 4 : 2;
        }

        public static int GetOrbCount(int powerLevel)
        {
            if (powerLevel >= 4)
            {
                return 4;
            }

            if (powerLevel >= 2)
            {
                return 2;
            }

            return 0;
        }

        private static float[] GetOfudaOffsets(int count)
        {
            return count == 4
                ? new float[] { -18f, -6f, 6f, 18f }
                : new float[] { -8f, 8f };
        }

        private static void MaintainYinYangOrbs(Player player)
        {
            int desiredCount = GetOrbCount(player.GetModPlayer<PowerPlayer>().PowerLevel);
            int existingCount = 0;

            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active &&
                    projectile.owner == player.whoAmI &&
                    projectile.type == ModContent.ProjectileType<YinYangOrb>())
                {
                    int index = existingCount++;
                    if (index >= desiredCount)
                    {
                        projectile.Kill();
                        continue;
                    }

                    projectile.ai[0] = index;
                    projectile.ai[1] = desiredCount;
                    projectile.timeLeft = 2;
                }
            }

            for (int i = existingCount; i < desiredCount; i++)
            {
                Projectile.NewProjectile(
                    player.GetSource_ItemUse(player.HeldItem),
                    player.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<YinYangOrb>(),
                    0,
                    0f,
                    player.whoAmI,
                    i,
                    desiredCount);
            }
        }

        private static void SignalOrbsToFire(Player player, int damage)
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active &&
                    projectile.owner == player.whoAmI &&
                    projectile.type == ModContent.ProjectileType<YinYangOrb>())
                {
                    YinYangOrb.RequestFire(projectile, OrbFireDelayFrames, damage);
                }
            }
        }
    }
}
