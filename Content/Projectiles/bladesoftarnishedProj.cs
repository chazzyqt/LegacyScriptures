using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using LegacyScriptures.Content.Buffs;


namespace LegacyScriptures.Content.Projectiles
{
    public class bladesoftarnishedProj : ModProjectile
    {
        public bool swordAttackmode = false;
        public bool swordGuardmode => !swordAttackmode;
        private static readonly int[] hitCooldowns = new int[Main.maxNPCs];

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EmpressBlade);
            AIType = ProjectileID.EmpressBlade; //The same with ID 946
            Projectile.width = 36;
            Projectile.height = 105;
            Projectile.scale = 1f;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 999;
            Projectile.hide = false;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 30;
            Projectile.alpha = 255;
            DrawOffsetX = 0; //-7

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.empressBlade = false;
            return true;
        }

        public override void AI()
        {
            for (int i = 0; i < hitCooldowns.Length; i++)
            {
                if (hitCooldowns[i] > 0)
                {
                    hitCooldowns[i]--;
                }
            }
            
            Player player = Main.player[Projectile.owner];
            if (!player.HasBuff(BuffType<bladesoftarnishedBuff>()))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 10;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            string[] swordTexturePaths = new string[]
            {
                "LegacyScriptures/Content/Projectiles/bladesoftarnishedProj",
                "LegacyScriptures/Content/Projectiles/bladesoftarnishedProj_Variant2",
                "LegacyScriptures/Content/Projectiles/bladesoftarnishedProj_Variant3",
                "LegacyScriptures/Content/Projectiles/bladesoftarnishedProj_Variant4"
            };

            string[] glowTexturePaths = new string[]
            {
                "LegacyScriptures/Content/Projectiles/bladesoftarnishedProj_Glow",
                "LegacyScriptures/Content/Projectiles/bladesoftarnishedProj_Variant2_Glow",
                "LegacyScriptures/Content/Projectiles/bladesoftarnishedProj_Variant3_Glow",
                "LegacyScriptures/Content/Projectiles/bladesoftarnishedProj_Variant4_Glow"
            };

            // Remember the sprite of the projectile instead of cycling
            if (Projectile.ai[2] == 0f)
            {
                Projectile.ai[2] = Main.rand.Next(1, swordTexturePaths.Length + 1);
            }

            int variantIndex = Math.Clamp((int)(Projectile.ai[2] - 1), 0, swordTexturePaths.Length - 1);
            
            Texture2D texture = ModContent.Request<Texture2D>(swordTexturePaths[variantIndex]).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>(glowTexturePaths[variantIndex]).Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            // Adjust the sprite scale of the projectile without affecting its hitbox
            float visualSpriteScale = 0.5f; 

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Deferred, 
                BlendState.Additive, 
                SamplerState.LinearClamp, 
                DepthStencilState.None, 
                RasterizerState.CullNone, 
                null, 
                Main.GameViewMatrix.TransformationMatrix
            );

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;

                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(DrawOffsetX, Projectile.gfxOffY) + new Vector2(-22f, 25f); // Offset Vector(X, Y) to fine-tune trail position relative to the projectile center

                float trailProgress = 1f - (i / (float)Projectile.oldPos.Length);

                Color color = Color.DarkRed * 0.7f * trailProgress; // Use Main.DiscoColor for rainbow

                // Scale the trail relative to your downscaled visual size
                float enlargedScale = visualSpriteScale * 1.1f;
    
                // Draw the matching variant trail outline
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[i], drawOrigin, enlargedScale, SpriteEffects.None, 0);
                // Optional glowmask trail
                //Main.EntitySpriteDraw(glowTexture, drawPos, null, Color.White * 0.5f * trailProgress, Projectile.oldRot[i], drawOrigin, enlargedScale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Deferred, 
                BlendState.AlphaBlend, 
                SamplerState.LinearClamp, 
                DepthStencilState.None, 
                RasterizerState.CullNone, 
                null, 
                Main.GameViewMatrix.TransformationMatrix
            );

            // Draw the main active sword sprite normally on top using the downscaled visual size
            Vector2 mainDrawPos = Projectile.Center - Main.screenPosition + new Vector2(DrawOffsetX, Projectile.gfxOffY + 25f);
            Main.EntitySpriteDraw(texture, mainDrawPos, null, lightColor, Projectile.rotation, drawOrigin, visualSpriteScale, SpriteEffects.None, 0);
            // Draw the main sword's glowmask directly on top so it always glows bright regardless of ambient lighting
            Main.EntitySpriteDraw(glowTexture, mainDrawPos, null, Color.White, Projectile.rotation, drawOrigin, visualSpriteScale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (hitCooldowns[target.whoAmI] > 0) // If cooldown is active, ignore/block hit
            {
                return false;
            }
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitCooldowns[target.whoAmI] = 30; // 60 = 1 second cooldown
        }
    }
}