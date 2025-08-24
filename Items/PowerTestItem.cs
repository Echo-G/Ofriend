using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Ofriend.Players;
using Ofriend.Systems;

namespace Ofriend.Items
{
    public class PowerTestItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item4;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            var powerPlayer = player.GetModPlayer<PowerPlayer>();
            
            // 右键减少Power
            if (player.altFunctionUse == 2)
            {
                powerPlayer.power -= 100;
                if (powerPlayer.power < 0)
                {
                    powerPlayer.power = 0;
                }
                Main.NewText($"Power decreased to {powerPlayer.power}");
            }
            // 左键增加Power
            else
            {
                powerPlayer.power += 100;
                if (powerPlayer.power > PowerSystem.MaxPower)
                {
                    powerPlayer.power = PowerSystem.MaxPower;
                }
                Main.NewText($"Power increased to {powerPlayer.power}");
            }
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 1);
            recipe.Register();
        }
    }
} 