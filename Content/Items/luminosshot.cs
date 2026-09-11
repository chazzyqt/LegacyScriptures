using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LegacyScriptures.Content.Items
{
	public class luminosshot : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 32;
			Item.DamageType = DamageClass.Ranged;
			Item.crit = 3;
			Item.noMelee = true;

			Item.width = 40;
			Item.height = 20;
			Item.scale = 1.3f;

			Item.useTime = 4;
			Item.useAnimation = 4;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item11 with { Volume = 0.3f };
			Item.autoReuse = true;

			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 12f;
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
			return new Vector2(-7, 0);
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return Main.rand.NextFloat() >= 0.33f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 5f;
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(1.5f));
			
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			int manaCost = 2;
			if (player.CheckMana(manaCost, pay: true))
    		{
				int laserType = ProjectileID.LaserMachinegunLaser;
				float laserSpeed = 16f; 
				Vector2 laserVelocity = Vector2.Normalize(velocity) * laserSpeed;
				laserVelocity = laserVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f));

				int laserDamage = (int)(damage * 0.75f); // Laser is 75% of the gun's damage

				Projectile.NewProjectile(source, position, laserVelocity, laserType, laserDamage, knockback, player.whoAmI);
			}
            return true; 
        }

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 1.5f;
            return true;
        }
	}
}
