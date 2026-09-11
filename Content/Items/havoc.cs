using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LegacyScriptures.Content.Items
{
	public class havoc : ModItem
	{
        public static readonly int MaxRampFrames = 600; // 60 = 1 second
        public static readonly float MaxDamageMultiplier = 3.0f;

		public override void SetStaticDefaults()
		{
			glowMaskAddon.AddGlowMask(Item.type, "LegacyScriptures/Content/Items/havoc_glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 24;
			Item.DamageType = DamageClass.Ranged;
			Item.crit = -1;
			Item.noMelee = true;

			Item.width = 40;
			Item.height = 20;
			Item.scale = 1.3f;

			Item.useTime = 4;
			Item.useAnimation = 4;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.value = Item.buyPrice(silver: 1);
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
			return new Vector2(-10, 5);
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return Main.rand.NextFloat() >= 0.15f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 5f;
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(1.5f));
			
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}

			//Dynamic damage ramp calculation            
            var rampPlayer = player.GetModPlayer<RampUpPlayer>();
            float rampProgress = System.Math.Min((float)rampPlayer.firingTimer / MaxRampFrames, 1.0f);
            float currentMultiplier = MathHelper.Lerp(1.0f, MaxDamageMultiplier, rampProgress);
            damage = (int)(damage * currentMultiplier);
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 1.5f;
            return true;
        }
	}

	public class RampUpPlayer : ModPlayer
    {
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
