using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using LegacyScriptures.Content.Projectiles;
using LegacyScriptures.Content.Buffs;

namespace LegacyScriptures.Content.Items
{
	public class eternallantern : ModItem
	{
		public override void SetStaticDefaults()
		{
			glowMaskAddon.AddGlowMask(Item.type, "LegacyScriptures/Content/Items/eternallantern_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 148;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 0.5f;

			Item.width = 20;
            Item.height = 20;
            Item.scale = 0.7f;

			Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.holdStyle = ItemHoldStyleID.HoldLamp;
            Item.noMelee = true;

			Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Cyan;

			Item.UseSound = SoundID.Item82;
			Item.shoot = ModContent.ProjectileType<eternallanternminion>();
			Item.buffType = ModContent.BuffType<eternallanternbuff>();
		}

		public override void HoldItem(Player player)
		{
			if (Main.dedServ) return;

			float offsetY = -14f;
			float offsetX = player.direction == 1 ? 5f : -14f; // Change 5f (left) : -14f (right) if left side needs more/less adjustment

			Vector2 lanternCenter = player.itemLocation + new Vector2(offsetX, offsetY);

			int dustIndex = Dust.NewDust(lanternCenter, 0, 0, DustID.Vortex, 0f, 0f, 100, default, 0.8f);
			
			Main.dust[dustIndex].velocity *= 0.2f;
			Main.dust[dustIndex].noGravity = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			bool minionExists = false;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile p = Main.projectile[i];
				if (p.active && p.owner == player.whoAmI && p.type == type)
				{
					if (p.minionSlots < player.maxMinions)
					{
						p.minionSlots += 1f;
						p.netUpdate = true;
					}
					
					minionExists = true;
					break;
				}
			}

			if (!minionExists)
			{
				var projectile = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
				projectile.originalDamage = Item.damage;
				projectile.minionSlots = 1f;
			}

			return false;
		}
	}
}