using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LegacyScriptures.Content.Utilities;
using Microsoft.Xna.Framework;

namespace LegacyScriptures.Content.Items
{
    public class armorReforgekit : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 15;
            Item.useAnimation = 15;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(20);
            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.AddRecipeGroup(RecipeGroupID.Wood, 5);
			recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe(20);
            recipe2.AddIngredient(ItemID.LeadBar, 5);
            recipe2.AddRecipeGroup(RecipeGroupID.Wood, 5);
			recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }

        public override bool CanUseItem(Player player)
        {
            var reforgePlayer = player.GetModPlayer<ReforgePlayer>();

            if (!reforgePlayer.IsArmorReforgingActive && !Main.playerInventory)
            {
                Main.NewText("Please open inventory to reforge armor.", 255, 100, 100);
                return false;
            }

            reforgePlayer.IsArmorReforgingActive = !reforgePlayer.IsArmorReforgingActive;
            
            Main.NewText(reforgePlayer.IsArmorReforgingActive 
                ? "Armor reforging mode: ON. Right-click an armor piece to reforge." 
                : "Armor reforging mode: OFF.", 50, 255, 50);

            return false;
        }
    }
}