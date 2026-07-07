using Microsoft.Xna.Framework;
using Ofriend.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ofriend.Items
{
    public class dogdogbark : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.damage = 80;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType<DogDogBarkHoldout>();
            Item.shootSpeed = DogDogBarkHoldout.HoldoutDistance;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<DogDogBarkHoldout>()] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 direction = velocity.SafeNormalize(Vector2.UnitX);

            Projectile.NewProjectile(
                source,
                player.RotatedRelativePoint(player.MountedCenter),
                direction * DogDogBarkHoldout.HoldoutDistance,
                ModContent.ProjectileType<DogDogBarkHoldout>(),
                damage,
                knockback,
                player.whoAmI);

            return false;
        }
    }
}
