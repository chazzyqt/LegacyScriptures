using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LegacyScriptures.Content.Items
{
	public class surgearray : ModItem
	{
		public override void SetStaticDefaults()
		{
			glowMaskAddon.AddGlowMask(Item.type, "LegacyScriptures/Content/Items/surgearray_glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 92;
			Item.DamageType = DamageClass.Ranged;
			Item.crit = 4;
			Item.noMelee = true;

			Item.width = 40;
			Item.height = 20;
			Item.scale = 1.25f;

			Item.useTime = 4;
			Item.useAnimation = 4;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item91 with {Volume = 0.15f, Pitch = -0.7f};
			Item.autoReuse = true;

			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 19f;
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
			return new Vector2(-18, -3);
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return Main.rand.NextFloat() >= 0.10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float numberProjectiles = 3;
			float rotation = MathHelper.ToRadians(0.33f);

			position += Vector2.Normalize(velocity) * 45f;

			for (int i = 0; i < numberProjectiles; i++)
			{
				float angleOffset = -rotation + (rotation * 2f * i / (numberProjectiles - 1));
				Vector2 perturbedSpeed = velocity.RotatedBy(angleOffset);

				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
			}

			return false;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * -45f;
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(1f));
			
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {	
				position += muzzleOffset;
			}
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 1.5f;
            return true;
        }
	}
}
