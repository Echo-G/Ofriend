using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ofriend.Items
{
    public class BuleBullet : ModItem
    {
        private const int BlueEggItemId = 5297;

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2f;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.ammo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<global::Ofriend.Projectiles.BuleBullet>();
            Item.shootSpeed = 12f;
            Item.value = Item.sellPrice(copper: 2);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(4000);
            recipe.AddIngredient(BlueEggItemId);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
