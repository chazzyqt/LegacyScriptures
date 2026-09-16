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
    public class eternallanternminion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            //DrawOffsetX = -5; Moved to PreDraw
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            float visualOffsetX = 0f;
            float visualOffsetY = -20f; 

            Rectangle frameRect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frameRect.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(visualOffsetX, visualOffsetY);

            Main.EntitySpriteDraw(texture, drawPosition, frameRect, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>("LegacyScriptures/Content/Projectiles/eternallanternminion_Glow").Value;

            float visualOffsetX = 0f;
            float visualOffsetY = -20f; 

            Rectangle frameRect = glowTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frameRect.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(visualOffsetX, visualOffsetY);

            int sliceHeight = 2;
            
            for (int y = 0; y < frameRect.Height; y += sliceHeight)
            {
                float fadeProgress = (float)y / frameRect.Height; 
                float stripAlphaMultiplier = MathHelper.Clamp(fadeProgress * 1.5f, 0f, 1f); 

                Rectangle sliceRect = new Rectangle(frameRect.X, frameRect.Y + y, frameRect.Width, Math.Min(sliceHeight, frameRect.Height - y));
                Vector2 sliceDrawPos = drawPosition + new Vector2(0f, y - origin.Y + (sliceRect.Height / 2f));
                Color stripColor = Color.White * stripAlphaMultiplier * (1f - Projectile.alpha / 255f);

                Main.EntitySpriteDraw(glowTexture, sliceDrawPos, sliceRect, stripColor, Projectile.rotation, new Vector2(origin.X, sliceRect.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                player.ClearBuff(ModContent.BuffType<eternallanternbuff>());
                return;
            }

            if (player.HasBuff(ModContent.BuffType<eternallanternbuff>()))
            {
                Projectile.timeLeft = 2;
            }

            Vector2 targetPosition = player.Center + new Vector2(0, -60);
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, 0.5f);
            Projectile.velocity = Vector2.Zero;

            if (!Main.dedServ && Main.rand.NextBool(2)) // Spawn dust per amount of frame
            {
                Vector2 dustOffset = new Vector2(-4f, 13f); // Offset position of dust
                
                int dustIndex = Dust.NewDust(Projectile.Center + dustOffset, 0, 0, DustID.Vortex, 0f, 0f, 100, default, 1.2f);
                
                Main.dust[dustIndex].velocity *= 0.2f;
                Main.dust[dustIndex].noGravity = true;
            }

            if (Projectile.ai[1] == 0f) 
            {
                Projectile.alpha = 255;
                Projectile.ai[1] = 1f;
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }

            float maxDetectRadius = 800f;
            NPC target = null;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC targetedNPC = Main.npc[player.MinionAttackTargetNPC];
                if (targetedNPC.active && Vector2.Distance(player.Center, targetedNPC.Center) < maxDetectRadius)
                {
                    target = targetedNPC;
                }
            }

            if (target == null)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float distance = Vector2.Distance(Projectile.Center, npc.Center);
                        if (distance < maxDetectRadius)
                        {
                            target = npc;
                            maxDetectRadius = distance;
                        }
                    }
                }
            }

            if (target != null)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 45f)
                {
                    Projectile.ai[0] = 0f;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        int projectileCount = Math.Max(1, (int)Projectile.minionSlots); // Number of projectiles per minion slot

                        for (int i = 0; i < projectileCount; i++)
                        {
                            Vector2 randomSpreadVel = Main.rand.NextVector2CircularEdge(3f, 3f); // Distance of the idle projectiles before homing
                            Vector2 spawnOffset = new Vector2(0f, 15f); // Offset of summon.
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + spawnOffset, randomSpreadVel, ProjectileID.FairyQueenMagicItemShot, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
            }
        }
    }
}