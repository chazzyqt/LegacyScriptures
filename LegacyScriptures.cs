using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using LegacyScriptures.Content.Players;
using LegacyScriptures.Content.Items;

namespace LegacyScriptures
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class LegacyScriptures : Mod
	{
		
    }

    public static class glowMasklib
    {
        public static void drawItemglowMask(Texture2D texture, PlayerDrawSet info)
        {
            Item item = info.drawPlayer.HeldItem;
            if (info.shadow != 0f || info.drawPlayer.frozen || ((info.drawPlayer.itemAnimation <= 0 || item.useStyle == ItemUseStyleID.None) && (item.holdStyle <= 0 || info.drawPlayer.pulley)) || info.drawPlayer.dead || item.noUseGraphic || (info.drawPlayer.wet && item.noWet))
                return;

            Vector2 offset = Vector2.Zero;
            Vector2 origin = Vector2.Zero;
            float rotOffset = 0;

            if (item.useStyle == ItemUseStyleID.Shoot)
            {
                if (Item.staff[item.type])
                {
                    rotOffset = 0.785f * info.drawPlayer.direction;
                    if (info.drawPlayer.gravDir == -1f)
                        rotOffset -= 1.57f * info.drawPlayer.direction;

                    origin = new Vector2(texture.Width * 0.5f * (1 - info.drawPlayer.direction), (info.drawPlayer.gravDir == -1f) ? 0 : texture.Height);

                    int oldOriginX = -(int)origin.X;
                    ItemLoader.HoldoutOrigin(info.drawPlayer, ref origin);
                    offset = new Vector2(origin.X + oldOriginX, 0);
                }
                else
                {
                    offset = new Vector2(10, texture.Height / 2);
                    ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);

                    origin = new Vector2((int)-offset.X, texture.Height / 2);
                    if (info.drawPlayer.direction == -1)
                        origin.X = texture.Width + offset.X;

                    offset = new Vector2(0, offset.Y);
                }
            }
            else
            {
                origin = new Vector2(texture.Width * 0.5f * (1 - info.drawPlayer.direction), (info.drawPlayer.gravDir == -1f) ? 0 : texture.Height);
            }

            info.DrawDataCache.Add(new DrawData(
                texture,
                new Vector2((int)(info.ItemLocation.X - Main.screenPosition.X + offset.X), (int)(info.ItemLocation.Y - Main.screenPosition.Y + offset.Y)),
                texture.Bounds,
                Color.White * ((255f - item.alpha) / 255f),
                info.drawPlayer.itemRotation + rotOffset,
                origin,
                item.scale,
                info.playerEffect,
                0
            ));
        }
    }

    public class glowMaskAddon : ModSystem
    {
        internal static readonly Dictionary<int, Texture2D> ItemGlowMask = new();

        public override void Unload() => ItemGlowMask.Clear();

        public static void AddGlowMask(int itemType, string texturePath) => ItemGlowMask[itemType] = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
    }

    public class glowMaskItemLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Item item = drawInfo.drawPlayer.HeldItem;

            if (item.type >= ItemID.Count && glowMaskAddon.ItemGlowMask.TryGetValue(item.type, out Texture2D textureItem) && (drawInfo.drawPlayer.itemTime > 0 || item.useStyle != ItemUseStyleID.None))
                glowMasklib.drawItemglowMask(textureItem, drawInfo);
        }
    }

    public class ArmorReforgingSystem : ModSystem
    {
        public override void PostUpdatePlayers()
        {
            Player player = Main.LocalPlayer;
            var reforgePlayer = player.GetModPlayer<ReforgePlayer>();

            if (!reforgePlayer.IsArmorReforgingActive) return;

            // If mode is active, but the player closes their inventory UI, turn it off
            if (!Main.playerInventory)
            {
                reforgePlayer.IsArmorReforgingActive = false;
                Main.NewText("Armor reforging disabled: Inventory closed.", 255, 100, 100);
                return;
            }

            int kitType = ModContent.ItemType<ArmorReforgeKit>();

            // Check if kit exists in inventory OR is currently picked up on the mouse cursor
            bool hasKitInInventory = player.HasItem(kitType);
            bool hasKitOnCursor = Main.mouseItem.type == kitType && !Main.mouseItem.IsAir;

            // Turn off mode if out of kits everywhere
            if (!hasKitInInventory && !hasKitOnCursor)
            {
                reforgePlayer.IsArmorReforgingActive = false;
                Main.NewText("All reforge kits consumed. Reforging disabled.", 255, 100, 100);
            }
        }
    }
}