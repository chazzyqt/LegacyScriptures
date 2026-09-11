using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using LegacyScriptures.Content.Projectiles;

namespace LegacyScriptures.Content.Items
{
    public class heatflare : ModItem
    {
        private int bulletTimer = 0;

        public override void SetDefaults()
        {
            Item.damage = 103; 
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 8;
            Item.noMelee = true;

            Item.width = 40;
            Item.height = 20;
            Item.scale = 1.25f;

            Item.useTime = 4;
            Item.useAnimation = 4;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item91 with { Volume = 0.15f, Pitch = -0.7f };
            Item.autoReuse = true;

            Item.channel = true; 
            Item.noUseGraphic = false; 
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
            return Main.rand.NextFloat() >= 0.10f;//
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int laserType = ModContent.ProjectileType<heatflarebeam>();
            int manaCost = 5;

            bool hasMana = player.CheckMana(manaCost, pay: true);

            if (hasMana)
            {
                if (player.ownedProjectileCounts[laserType] < 1)
                {
                    Projectile laser = Projectile.NewProjectileDirect(source, position, velocity, laserType,
                        (int)(damage * 1.5f),
                        knockback * 1.5f, player.whoAmI
                    );
                    laser.DamageType = DamageClass.Ranged;
                }
            }
            else
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.owner == player.whoAmI && p.type == laserType)
                    {
                        p.Kill();
                    }
                }
            }

            // Bullet Spread Logic: Continues firing bullets
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(0.33f);
            Vector2 spawnPosition = position + Vector2.Normalize(velocity) * 45f;

            for (int i = 0; i < numberProjectiles; i++)
            {
                float angleOffset = -rotation + (rotation * 2f * i / (numberProjectiles - 1));
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(1f)).RotatedBy(angleOffset);

                Projectile.NewProjectile(source, spawnPosition, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }

            return false;
        }

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 1.5f;
            return true;
        }
    }
}