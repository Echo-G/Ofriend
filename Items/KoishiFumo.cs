using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Ofriend.Items
{
    public class KoishiFumo : ModItem
    {
        public override void SetDefaults()
        {
//安德拉还
            // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.KoishiFumoTile>());
            Item.width = 36;
            Item.height = 38;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 1);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SummerHat);
            recipe.AddIngredient(ItemID.GreenThread);
            recipe.AddIngredient(ItemID.StoneBlock);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
