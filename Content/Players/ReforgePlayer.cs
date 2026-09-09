using Terraria;
using Terraria.ModLoader;
using LegacyScriptures.Content.Items;

namespace LegacyScriptures.Content.Players
{
    public class ReforgePlayer : ModPlayer
    {
        public bool IsArmorReforgingActive = false;

        public override void UpdateEquips()
        {
            // Loop through head (0), body (1), and legs (2)
            for (int i = 0; i < 3; i++)
            {
                Item armorPiece = Player.armor[i];
                if (armorPiece.IsAir) continue;

                var globalItem = armorPiece.GetGlobalItem<ArmorReforgeGlobalItem>();
                if (globalItem == null) continue;

                Player.statLifeMax2 += globalItem.reforgeLifeBonus;
                Player.statManaMax2 += globalItem.reforgeManaBonus;
                Player.statDefense += globalItem.reforgeDefenseBonus;
                
                Player.GetDamage(DamageClass.Generic) += globalItem.reforgeDamageBonus;
                Player.GetCritChance(DamageClass.Generic) += globalItem.reforgeCritBonus;
                
                Player.moveSpeed += globalItem.reforgeMoveSpeedBonus;
                Player.maxMinions += globalItem.reforgeMinionSlots;

                if (globalItem.reforgeKnockbackImmune)
                {
                    Player.noKnockback = true;
                }

                if (globalItem.reforgeThornsPercent > 0f)
                {
                    Player.thorns += globalItem.reforgeThornsPercent;
                }

                if (globalItem.reforgeExtraIFrameBonus > 0)
                {
                    Player.longInvince = true;
                    Player.immuneTime += globalItem.reforgeExtraIFrameBonus;
                }
            }
        }
    }
}