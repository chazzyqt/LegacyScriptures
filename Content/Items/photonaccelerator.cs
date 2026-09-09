using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LegacyScriptures.Content.Items
{
	public class photonaccelerator : ModItem
	{
		public override void SetStaticDefaults()
		{
			glowMaskAddon.AddGlowMask(Item.type, "LegacyScriptures/Content/Items/photonaccelerator_glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.DamageType = DamageClass.Ranged;
			Item.crit = 2;
			Item.noMelee = true;

			Item.width = 40;
			Item.height = 20;
			Item.scale = 1.75f;

			Item.useTime = 5;
			Item.useAnimation = 5;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Lime;
			Item.UseSound = SoundID.Item11 with { Volume = 0.3f };
			Item.autoReuse = true;

			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 15f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-11, 1);
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return Main.rand.NextFloat() >= 0.25f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 5f;
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(1.5f));
			
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 2.0f;
            return true;
        }
	}
}
