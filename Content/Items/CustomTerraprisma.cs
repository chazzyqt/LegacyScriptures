using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using LegacyScriptures.Content.Projectiles;
using LegacyScriptures.Content.Buffs;

namespace LegacyScriptures.Content.Items
{
    public class CustomTerraprisma : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.mana = 10;
            Item.noMelee = true;

            Item.width = 40;
            Item.height = 40;
            Item.scale = 1.5f; //0.75f;
            
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.holdStyle = ItemHoldStyleID.HoldGuitar;
            Item.useStyle = ItemUseStyleID.Swing;
            
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item82;
            Item.shoot = ProjectileType<CustomTerraprismaProj>();
            Item.DamageType = DamageClass.Summon;
            Item.buffType = BuffType<CustomTerraprismaBuff>();
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.Y += 25f; // Lowers the sprite down vertically
            player.itemLocation.X += -25f * player.direction; // Shifts it horizontally based on player direction
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            Vector2 toPlayer = player.Center - player.itemLocation;
            player.itemLocation += toPlayer * 1f; // Adjust 0.3f to pull it closer (higher = closer)
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            
            bool[] usedIndices = new bool[Main.maxProjectiles];
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == type)
                {
                    int existingIndex = (int)p.ai[0];
                    if (existingIndex >= 0 && existingIndex < Main.maxProjectiles)
                    {
                        usedIndices[existingIndex] = true;
                    }
                }
            }

            int currentIndex = 0;
            for (int i = 0; i < usedIndices.Length; i++)
            {
                if (!usedIndices[i])
                {
                    currentIndex = i;
                    break;
                }
            }

            position = Main.MouseWorld;
            
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, currentIndex);
            
            return false;
        }
    }
}