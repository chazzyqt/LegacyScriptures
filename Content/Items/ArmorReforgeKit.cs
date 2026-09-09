using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LegacyScriptures.Content.Players;
using Microsoft.Xna.Framework;

namespace LegacyScriptures.Content.Items
{
    public class ArmorReforgeKit : ModItem
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

        /*public override bool CanUseItem(Player player)
        {
            var reforgePlayer = player.GetModPlayer<ReforgePlayer>();

            reforgePlayer.IsArmorReforgingActive = !reforgePlayer.IsArmorReforgingActive;
            
            Main.NewText(reforgePlayer.IsArmorReforgingActive 
                ? "Armor Reforging Mode: ON. Right-click an armor piece to reforge." 
                : "Armor Reforging Mode: OFF.", 50, 255, 50);

            // Return false so it doesn't consume the kit just from toggling the mode on/off in your hand
            return false;
        }*/

        public override bool CanUseItem(Player player)
        {
            var reforgePlayer = player.GetModPlayer<ReforgePlayer>();

            // If trying to turn it on, check if inventory UI is open
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