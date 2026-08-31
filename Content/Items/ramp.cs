using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LegacyScriptures.Content.Items
{
	// This is a basic item template.
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class ramp : ModItem
	{
		//How many total game frames (60 = 1 second) of firing to reach full ramp
        public static readonly int MaxRampFrames = 180; // 3 seconds of continuous fire
        //Maximum multiplier applied to base damage at full ramp (e.g., 2.0f = +100% damage, 1.5f = +50% damage)
        public static readonly float MaxDamageMultiplier = 2.0f;

		// The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.BulletHoseP90.hjson' file.
		public override void SetDefaults()
		{
			Item.damage = 4;
			Item.DamageType = DamageClass.Ranged;
			Item.crit = -2;
			Item.noMelee = true;

			Item.width = 40;
			Item.height = 20;
			Item.scale = 1.75f; //0.75f;

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

            //Dynamic damage ramp calculation            
            var rampPlayer = player.GetModPlayer<RampUpPlayer>();
            float rampProgress = System.Math.Min((float)rampPlayer.firingTimer / MaxRampFrames, 1.0f);
            float currentMultiplier = MathHelper.Lerp(1.0f, MaxDamageMultiplier, rampProgress);
            damage = (int)(damage * currentMultiplier);
		}
	}

	public class RampUpPlayer : ModPlayer
    {
        // Tracks time spent firing/using the current item in ticks (60 = 1 second)
        public int firingTimer;

        public override void PreUpdate()
        {
            // If the player is actively swinging/shooting ANY weapon, increment the timer.
            // Otherwise, reset it back to 0.
            if (Player.itemAnimation > 0)
            {
                firingTimer++;
            }
            else
            {
                firingTimer = 0;
            }
        }
    }
}
