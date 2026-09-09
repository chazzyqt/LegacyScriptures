using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LegacyScriptures.Content.Items
{
	// This is a basic item template. and sparknlead
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class corep90 : ModItem
	{

		// The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.BulletHoseP90.hjson' file.
		public override void SetDefaults()
		{
			Item.damage = 4;
			Item.DamageType = DamageClass.Ranged;
			Item.crit = -2;
			Item.noMelee = true;

			Item.width = 40;
			Item.height = 20;
			Item.scale = 1.75f;

			Item.useTime = 6;
			Item.useAnimation = 6;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item11 with { Volume = 0.3f };
			Item.autoReuse = true;

			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 12f; //19f Projectile speed
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
			// Vector2(X, Y)
			// X: Higher = move Right, Lower = move Left
			// Y: Higher = move Down, Lower = move Up
			return new Vector2(-11, 1);
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return Main.rand.NextFloat() >= 0.33f; //Chance not to consume ammo
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 5f; //Position of the projectile from the muzzle
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(1.5f)); //Angle of inaccuracy of the gun
			
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 2.5f; // 0.5 = half size, 2.0f = double size
            return true;
        }
	}
}
