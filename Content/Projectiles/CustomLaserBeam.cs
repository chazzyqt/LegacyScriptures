using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LegacyScriptures.Content.Projectiles
{
    public class CustomLaserBeam : ModProjectile
    {
        private const float MAX_DISTANCE = 1600f;
        private const float OFFSET_X = 55f; // X: Higher = move Right, Lower = move Left
        private const float OFFSET_Y = -5f; // Y: Higher = move Down, Lower = move Up

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;

            // --- FIX FOR IFRAME CONFLICTS ---
            // This makes the laser track immunity per-NPC independently using a local timer instead of global IFrames
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2; //10; Hits the same enemy once every 10 frames (adjust lower for faster hits, higher to leave room for bullets)
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || !player.channel)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            // --- CHARGE-UP ANIMATION LOGIC ---
            // 60 ticks = 1 second delay
            const float chargeTime = 60f; //60f
            if (Projectile.ai[1] < chargeTime)
            {
                Projectile.ai[1] += 1f; // Increment charge timer
            }
            
            // Calculate charge percentage from 0.0 (empty/thin) to 1.0 (fully charged)
            float chargeProgress = MathHelper.Clamp(Projectile.ai[1] / chargeTime, 0f, 1f);
            // Smooth out the scaling curve using Sine (starts slow, speeds up, eases out)
            float currentScale = (float)Math.Sin(chargeProgress * MathHelper.PiOver2);

            // Pass the scaling factor to ai[2] so PreDraw can use it
            Projectile.ai[2] = currentScale;

            // If still charging, you can choose to disable collision/damage until it's fully grown
            // (Optional: remove this check if you want it to deal damage immediately while charging)
            if (chargeProgress < 1f)
            {
                // Optional: you can spawn charging dust here around the player/muzzle
            }

            Vector2 aimDir = Vector2.Normalize(Main.MouseWorld - player.MountedCenter);
            Vector2 perpendicular = new Vector2(-aimDir.Y, aimDir.X);
            
            float currentOffsetY = (player.direction == -1) ? -OFFSET_Y : OFFSET_Y;
            Vector2 startPos = player.MountedCenter + (aimDir * OFFSET_X) + (perpendicular * currentOffsetY);

            Projectile.Center = startPos;
            Projectile.velocity = aimDir;
            Projectile.rotation = aimDir.ToRotation();

            float laserLength = MAX_DISTANCE;
            bool hitTile = false;

            for (float d = 0f; d < MAX_DISTANCE; d += 16f)
            {
                Vector2 checkPos = startPos + aimDir * d;
                if (!Collision.CanHitLine(startPos, 1, 1, checkPos, 1, 1))
                {
                    laserLength = d;
                    hitTile = true;
                    break;
                }
            }

            Projectile.ai[0] = laserLength;

            Vector2 impactPoint = startPos + aimDir * laserLength;
            
            // Only deal damage/effects when fully charged (or scale it with charge if you prefer)
            if (chargeProgress >= 1f)
            {
                if (hitTile)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 dustVel = -aimDir.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(2f, 5f);
                        Dust d = Dust.NewDustPerfect(impactPoint, DustID.Electric, dustVel, 100, default, 1.3f);
                        d.noGravity = true;
                    }

                    if (Main.rand.NextBool(2))
                    {
                        Dust cyanDust = Dust.NewDustPerfect(impactPoint, DustID.SnowflakeIce, -aimDir * 2f, 100, default, 1.5f);
                        cyanDust.noGravity = true;
                    }
                }

                if (Main.rand.NextBool(3))
                {
                    Vector2 dustPos = startPos + aimDir * Main.rand.NextFloat(laserLength);
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.Electric, aimDir * 1.5f, 100, default, 0.9f);
                    dust.noGravity = true;
                }
            }
        }

        // Optional: Prevent dealing damage while charging up
        public override bool ? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[1] < 60f) // If less than 1 second has passed, no collision
                return false;

            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * Projectile.ai[0];
            float collisionPoint = 0f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 26f, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Texture2D startTex = ModContent.Request<Texture2D>("LegacyScriptures/Content/Projectiles/CustomLaserBeam_Start").Value;
            Texture2D middleTex = ModContent.Request<Texture2D>("LegacyScriptures/Content/Projectiles/CustomLaserBeam_Middle").Value;
            Texture2D endTex = ModContent.Request<Texture2D>("LegacyScriptures/Content/Projectiles/CustomLaserBeam_End").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 unitDirection = Projectile.velocity;
            float totalLaserLength = Projectile.ai[0];
            Color beamColor = Color.White;

            // Grab the scaling factor calculated in AI (ranges from 0.0 to 1.0)
            float scaleFactor = Projectile.ai[2];
            if (scaleFactor <= 0f)
                return false; // Don't draw if scale is 0

            // 1. DRAW START CAP (Scale the Y-axis so it grows vertically from a line)
            Main.EntitySpriteDraw(
                startTex,
                drawPosition,
                null,
                beamColor,
                Projectile.rotation,
                new Vector2(0, startTex.Height / 2f),
                new Vector2(1f, scaleFactor), // Scales thickness vertically
                SpriteEffects.None,
                0
            );

            float currentDistance = startTex.Width;

            // 2. TILE MIDDLE SEGMENT
            while (currentDistance < totalLaserLength - endTex.Width)
            {
                Vector2 bodyPos = drawPosition + unitDirection * currentDistance;

                Main.EntitySpriteDraw(
                    middleTex,
                    bodyPos,
                    null,
                    beamColor,
                    Projectile.rotation,
                    new Vector2(0, middleTex.Height / 2f),
                    new Vector2(1f, scaleFactor), // Scales thickness vertically
                    SpriteEffects.None,
                    0
                );

                currentDistance += middleTex.Width;
            }

            // 3. DRAW END CAP
            Vector2 endPos = drawPosition + unitDirection * (totalLaserLength - endTex.Width);

            Main.EntitySpriteDraw(
                endTex,
                endPos,
                null,
                beamColor,
                Projectile.rotation,
                new Vector2(0, endTex.Height / 2f),
                new Vector2(1f, scaleFactor), // Scales thickness vertically
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}