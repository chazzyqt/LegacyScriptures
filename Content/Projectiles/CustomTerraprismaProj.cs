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
    public class CustomTerraprismaProj : ModProjectile
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
        
        /*public override void AI()
        {
            // Decrement your custom hit cooldown timers every frame
            for (int i = 0; i < hitCooldowns.Length; i++)
            {
                if (hitCooldowns[i] > 0)
                {
                    hitCooldowns[i]--;
                }
            }
            
            Player player = Main.player[Projectile.owner];
            if (!player.HasBuff(BuffType<CustomTerraprismaBuff>()))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 10;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
            }

            // 1 & 2. If in guard mode, check if there is an enemy within 50 block range (800f) and in line of sight of the player
            if (swordGuardmode)
            {
                bool foundTriggerEnemy = false;
                float triggerRange = 800f;

                // Check manual target first if within range and line of sight
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC targetedNpc = Main.npc[player.MinionAttackTargetNPC];
                    if (targetedNpc.active && Vector2.Distance(player.Center, targetedNpc.Center) <= triggerRange && Collision.CanHitLine(player.Center, 0, 0, targetedNpc.Center, 0, 0))
                    {
                        foundTriggerEnemy = true;
                    }
                }

                if (!foundTriggerEnemy)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy())
                        {
                            float dist = Vector2.Distance(player.Center, npc.Center);
                            if (dist <= triggerRange && Collision.CanHitLine(player.Center, 0, 0, npc.Center, 0, 0))
                            {
                                foundTriggerEnemy = true;
                                break;
                            }
                        }
                    }
                }

                if (foundTriggerEnemy)
                {
                    swordAttackmode = true; // swordGuardmode becomes false automatically via property
                }
            }

            // 3 & 4. If swordAttackmode = true, ignore line of sight and attack everything in the screen (~960f) until no enemy is present
            if (swordAttackmode)
            {
                bool hasScreenTarget = false;
                int selectedTarget = -1;
                float screenRange = 960f; // 960f screen range or around 60 blocks
                float closestDist = screenRange;

                // Check manual target first
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC targetedNpc = Main.npc[player.MinionAttackTargetNPC];
                    if (targetedNpc.active && Vector2.Distance(player.Center, targetedNpc.Center) <= screenRange)
                    {
                        selectedTarget = targetedNpc.whoAmI;
                        hasScreenTarget = true;
                    }
                }

                // Otherwise, search for closest enemy in screen range (ignoring line of sight)
                if (!hasScreenTarget)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy())
                        {
                            float dist = Vector2.Distance(player.Center, npc.Center);
                            if (dist < closestDist)
                            {
                                closestDist = dist;
                                selectedTarget = i;
                                hasScreenTarget = true;
                            }
                        }
                    }
                }

                // If no enemy is present left within the screen, set swordAttackmode = false (swordGuardmode = true)
                if (!hasScreenTarget || selectedTarget == -1)
                {
                    swordAttackmode = false;
                }
                else
                {
                    // Lock vanilla attack onto the target and execute base.AI
                    Projectile.ai[1] = selectedTarget;
                    base.AI();
                    return; // 5. Loop back condition met / keep attacking until clear
                }
            }

            // If we reach here, we are in guard mode (idle state) and do not use base.AI
            Projectile.ai[1] = 0;

            int dynamicIndex = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (other.active && other.owner == Projectile.owner && other.type == Projectile.type)
                {
                    if (other.identity < Projectile.identity)
                    {
                        dynamicIndex++;
                    }
                }
            }
            Projectile.ai[0] = dynamicIndex; 

            int swordIndex = (int)Projectile.ai[0];

            // 1. Spacing multiplier between swords (smaller = tighter grouping)
            float offsetAngle = swordIndex * 0.3f;
            // 2. Base distance behind the player (change -40 to move the whole cluster closer/further); Multiplied by player.direction so it flips left/right based on where the player looks
            float baseDistanceX = -30f * player.direction;
            // 3. Step distance between each individual sword (change 15 to space them wider apart on X); Multiplied by player.direction so the fan extends backward relative to facing direction
            float swordSpacingX = 8f * player.direction;
            // 4. Vertical height offset (change -30 to move the cluster higher or lower)
            float heightOffsetY = 0f;
            // 5. Floating bob speed and amplitude
            float bobSpeed = 15f;
            float bobHeight = 2f;
            float floatingBob = (float)Math.Sin(Main.time / bobSpeed + offsetAngle) * bobHeight;

            Vector2 idlePosition = player.Center + new Vector2(baseDistanceX - (swordIndex * swordSpacingX), heightOffsetY + floatingBob);
            Vector2 vectorToIdle = idlePosition - Projectile.Center;
            
            // Smooth floating pull towards the idle position (30% spring force for smooth rubber-band movement), 
            // blended with 40% of the player's velocity so the swords dynamically inherit momentum and don't lag behind when moving fast
            Projectile.velocity = (vectorToIdle * 0.3f) + (player.velocity * 0.4f);

            // 6. Base tilt angle offset in radians (multiplied by player.direction so it mirrors when turning around)
            float baseTiltAngle = 3.28122f * player.direction; 
            // 7. Incremental wing spread (multiplied by player.direction so each successive sword fans outward while flipping correctly when turning around)
            float incrementalWingSpread = 0.05f * player.direction; 

            // Scales horizontal velocity by 0.01f to tilt the sword during movement, clamped between a minimum of -0.1f and maximum of 0.1f radians
            float velocityTilt = MathHelper.Clamp(Projectile.velocity.X * 0.01f, -0.1f, 0.1f);

            Projectile.rotation = velocityTilt + baseTiltAngle + (swordIndex * incrementalWingSpread);
        }*/

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
            if (!player.HasBuff(BuffType<CustomTerraprismaBuff>()))
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
                "LegacyScriptures/Content/Projectiles/CustomTerraprismaProj",
                "LegacyScriptures/Content/Projectiles/CustomTerraprismaProj_Variant2",
                "LegacyScriptures/Content/Projectiles/CustomTerraprismaProj_Variant3",
                "LegacyScriptures/Content/Projectiles/CustomTerraprismaProj_Variant4"
            };

            string[] glowTexturePaths = new string[]
            {
                "LegacyScriptures/Content/Projectiles/CustomTerraprismaProj_Glow",
                "LegacyScriptures/Content/Projectiles/CustomTerraprismaProj_Variant2_Glow",
                "LegacyScriptures/Content/Projectiles/CustomTerraprismaProj_Variant3_Glow",
                "LegacyScriptures/Content/Projectiles/CustomTerraprismaProj_Variant4_Glow"
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