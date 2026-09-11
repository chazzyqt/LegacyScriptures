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
        private const float max_laser_dist = 1600f;
        private const float laserpos_x = 55f; 
        private const float laserpos_y = -5f; 

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5; 
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

            Vector2 aimDir = Vector2.Normalize(Main.MouseWorld - player.MountedCenter);
            Vector2 perpendicular = new Vector2(-aimDir.Y, aimDir.X);
            
            float currentOffsetY = (player.direction == -1) ? -laserpos_y : laserpos_y;
            Vector2 startPos = player.MountedCenter + (aimDir * laserpos_x) + (perpendicular * currentOffsetY);

            Projectile.Center = startPos;
            Projectile.velocity = aimDir;
            Projectile.rotation = aimDir.ToRotation();

            float targetLaserLength = max_laser_dist;
            bool hitTile = false;

            for (float d = 0f; d < max_laser_dist; d += 16f)
            {
                Vector2 checkPos = startPos + aimDir * d;
                if (!Collision.CanHitLine(startPos, 1, 1, checkPos, 1, 1))
                {
                    targetLaserLength = d;
                    hitTile = true;
                    break;
                }
            }

            float extensionSpeed = 150f; // Speed how laser travels
            
            Projectile.ai[1] += extensionSpeed;

            float laserLength = Math.Min(Projectile.ai[1], targetLaserLength);

            Projectile.ai[0] = laserLength;

            Vector2 impactPoint = startPos + aimDir * laserLength;
            
            if (hitTile && Projectile.ai[1] >= targetLaserLength)
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

            if (Main.rand.NextBool(3) && laserLength > 50f)
            {
                Vector2 dustPos = startPos + aimDir * Main.rand.NextFloat(laserLength);
                Dust dust = Dust.NewDustPerfect(dustPos, DustID.Electric, aimDir * 1.5f, 100, default, 0.9f);
                dust.noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] < 30f)
                return false;

            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * Projectile.ai[0];
            float collisionPoint = 0f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 26f, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero || Projectile.ai[0] <= 10f)
                return false;

            Texture2D startTex = ModContent.Request<Texture2D>("LegacyScriptures/Content/Projectiles/CustomLaserBeam_Start").Value;
            Texture2D middleTex = ModContent.Request<Texture2D>("LegacyScriptures/Content/Projectiles/CustomLaserBeam_Middle").Value;
            Texture2D endTex = ModContent.Request<Texture2D>("LegacyScriptures/Content/Projectiles/CustomLaserBeam_End").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 unitDirection = Projectile.velocity;
            float totalLaserLength = Projectile.ai[0];
            Color beamColor = Color.White;
            float scaleFactor = 1f;

            Main.EntitySpriteDraw(startTex, drawPosition, null, beamColor, Projectile.rotation, new Vector2(0, startTex.Height / 2f), new Vector2(1f, scaleFactor), SpriteEffects.None, 0); // Draw Start Laser
            float currentDistance = startTex.Width;

            while (currentDistance < totalLaserLength - endTex.Width)
            {
                Vector2 bodyPos = drawPosition + unitDirection * currentDistance;
                Main.EntitySpriteDraw(middleTex, bodyPos, null, beamColor, Projectile.rotation, new Vector2(0, middleTex.Height / 2f), new Vector2(1f, scaleFactor), SpriteEffects.None, 0); // Draw Middle Laser
                currentDistance += middleTex.Width;
            }

            Vector2 endPos = drawPosition + unitDirection * Math.Max(startTex.Width, totalLaserLength - endTex.Width);
            Main.EntitySpriteDraw(endTex, endPos, null, beamColor, Projectile.rotation, new Vector2(0, endTex.Height / 2f), new Vector2(1f, scaleFactor), SpriteEffects.None, 0); // Draw End Laser

            return false;
        }
    }
}