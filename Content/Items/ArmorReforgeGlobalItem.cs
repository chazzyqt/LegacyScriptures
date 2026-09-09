using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LegacyScriptures.Content.Players;

namespace LegacyScriptures.Content.Items
{
    public class ReforgeOption
    {
        public Action ApplyAction { get; set; }
        public int Weight { get; set; }

        public ReforgeOption(Action action, int weight)
        {
            ApplyAction = action;
            Weight = weight;
        }
    }
    
    public class ArmorReforgeGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public int reforgeLifeBonus = 0;
        public int reforgeManaBonus = 0;
        public int reforgeDefenseBonus = 0;
        public float reforgeDamageBonus = 0f;
        public int reforgeCritBonus = 0;
        public float reforgeMoveSpeedBonus = 0f;
        public int reforgeMinionSlots = 0;
        public bool reforgeKnockbackImmune = false;
        public float reforgeThornsPercent = 0f;
        public int reforgeExtraIFrameBonus = 0;
        public string reforgeName = "";
        public int reforgeRarityBonus = 0;
        public int originalRare = -1;

        public override GlobalItem Clone(Item item, Item target)
        {
            var clone = (ArmorReforgeGlobalItem)base.Clone(item, target);
            clone.reforgeLifeBonus = reforgeLifeBonus;
            clone.reforgeManaBonus = reforgeManaBonus;
            clone.reforgeDefenseBonus = reforgeDefenseBonus;
            clone.reforgeDamageBonus = reforgeDamageBonus;
            clone.reforgeCritBonus = reforgeCritBonus;
            clone.reforgeMoveSpeedBonus = reforgeMoveSpeedBonus;
            clone.reforgeMinionSlots = reforgeMinionSlots;
            clone.reforgeKnockbackImmune = reforgeKnockbackImmune;
            clone.reforgeThornsPercent = reforgeThornsPercent;
            clone.reforgeExtraIFrameBonus = reforgeExtraIFrameBonus;
            clone.reforgeName = reforgeName;
            clone.reforgeRarityBonus = reforgeRarityBonus;
            clone.originalRare = originalRare;
            return clone;
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            tag["reforgeLifeBonus"] = reforgeLifeBonus;
            tag["reforgeManaBonus"] = reforgeManaBonus;
            tag["reforgeDefenseBonus"] = reforgeDefenseBonus;
            tag["reforgeDamageBonus"] = reforgeDamageBonus;
            tag["reforgeCritBonus"] = reforgeCritBonus;
            tag["reforgeMoveSpeedBonus"] = reforgeMoveSpeedBonus;
            tag["reforgeMinionSlots"] = reforgeMinionSlots;
            tag["reforgeKnockbackImmune"] = reforgeKnockbackImmune;
            tag["reforgeThornsPercent"] = reforgeThornsPercent;
            tag["reforgeExtraIFrameBonus"] = reforgeExtraIFrameBonus;
            tag["reforgeName"] = reforgeName;
            tag["reforgeRarityBonus"] = reforgeRarityBonus;
            tag["reforgeOriginalRare"] = originalRare;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            reforgeLifeBonus = tag.GetInt("reforgeLifeBonus");
            reforgeManaBonus = tag.GetInt("reforgeManaBonus");
            reforgeDefenseBonus = tag.GetInt("reforgeDefenseBonus");
            reforgeDamageBonus = tag.GetFloat("reforgeDamageBonus");
            reforgeCritBonus = tag.GetInt("reforgeCritBonus");
            reforgeMoveSpeedBonus = tag.GetFloat("reforgeMoveSpeedBonus");
            reforgeMinionSlots = tag.GetInt("reforgeMinionSlots");
            reforgeKnockbackImmune = tag.GetBool("reforgeKnockbackImmune");
            reforgeThornsPercent = tag.GetFloat("reforgeThornsPercent");
            reforgeExtraIFrameBonus = tag.GetInt("reforgeExtraIFrameBonus");
            reforgeName = tag.GetString("reforgeName");
            reforgeRarityBonus = tag.GetInt("reforgeRarityBonus");
            originalRare = tag.ContainsKey("reforgeOriginalRare") ? tag.GetInt("reforgeOriginalRare") : -1;
        }

        public static bool IsArmor(Item item)
        {
            return item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!string.IsNullOrEmpty(reforgeName))
            {
                if (originalRare != -1)
                {
                    item.rare = originalRare + reforgeRarityBonus;
                }

                var nameLine = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
                if (nameLine != null)
                {
                    nameLine.Text = $"{reforgeName} {nameLine.Text}";
                }

                List<string> statText = new List<string>();
                if (reforgeLifeBonus != 0) statText.Add($"{(reforgeLifeBonus > 0 ? "+" : "")}{reforgeLifeBonus} Increased Max HP");
                if (reforgeManaBonus != 0) statText.Add($"{(reforgeManaBonus > 0 ? "+" : "")}{reforgeManaBonus} Increased Max Mana");
                if (reforgeDefenseBonus != 0) statText.Add($"{(reforgeDefenseBonus > 0 ? "+" : "")}{reforgeDefenseBonus} Additional Defense");
                if (reforgeDamageBonus != 0f) statText.Add($"{(reforgeDamageBonus > 0 ? "+" : "")}{(int)(reforgeDamageBonus * 100)}% Damage");
                if (reforgeCritBonus != 0) statText.Add($"{(reforgeCritBonus > 0 ? "+" : "")}{reforgeCritBonus}% Critical Chance");
                if (reforgeMoveSpeedBonus != 0f) statText.Add($"{(reforgeMoveSpeedBonus > 0 ? "+" : "")}{(int)(reforgeMoveSpeedBonus * 100)}% Movement Speed");
                if (reforgeMinionSlots != 0) statText.Add($"{(reforgeMinionSlots > 0 ? "+" : "")}{reforgeMinionSlots} Minion Slots");
                if (reforgeKnockbackImmune) statText.Add("Grants Knockback Immunity");
                if (reforgeThornsPercent > 0f) statText.Add($"+{(int)(reforgeThornsPercent * 100)}% Thorns");
                if (reforgeExtraIFrameBonus > 0) statText.Add("Grants a minor increase in invincibility frames");//$"+{reforgeExtraIFrameBonus} I-Frames");

                tooltips.Add(new TooltipLine(Mod, "ArmorReforgeStats", $"{string.Join(", ", statText)}")
                {
                    OverrideColor = Color.Cyan
                });
            }
        }

        public override bool CanRightClick(Item item)
        {
            Player player = Main.LocalPlayer;
            var reforgePlayer = player.GetModPlayer<ReforgePlayer>();
            bool hasKit = player.HasItem(ModContent.ItemType<ArmorReforgeKit>());
            
            return reforgePlayer.IsArmorReforgingActive && IsArmor(item) && hasKit;
        }

        private void Add(List<ReforgeOption> list, Action action, int weight)
        {
            list.Add(new ReforgeOption(action, weight));
        }

        public override void RightClick(Item item, Player player)
        {
            player.ConsumeItem(ModContent.ItemType<ArmorReforgeKit>());

            reforgeLifeBonus = 0;
            reforgeManaBonus = 0;
            reforgeDefenseBonus = 0;
            reforgeDamageBonus = 0f;
            reforgeCritBonus = 0;
            reforgeMoveSpeedBonus = 0f;
            reforgeMinionSlots = 0;
            reforgeKnockbackImmune = false;
            reforgeThornsPercent = 0f;
            reforgeExtraIFrameBonus = 0;
            reforgeRarityBonus = 0;
            reforgeThornsPercent = 0f;
            reforgeExtraIFrameBonus = 0;
            reforgeRarityBonus = 0;

            List<ReforgeOption> availableReforges = new List<ReforgeOption>();

            Action normalTier = () => reforgeRarityBonus = 1;

            // --- LIFE REFORGES ---
            Add(availableReforges, () => { reforgeName = "Healthy"; reforgeLifeBonus = 5; normalTier(); }, 20);
            Add(availableReforges, () => { reforgeName = "Sturdy"; reforgeLifeBonus = 10; normalTier(); }, 15);
            Add(availableReforges, () => { reforgeName = "Robust"; reforgeLifeBonus = 15; normalTier(); }, 15);
            Add(availableReforges, () => { reforgeName = "Resilient"; reforgeLifeBonus = 20; normalTier(); }, 10);

            // --- MANA REFORGES ---
            Add(availableReforges, () => { reforgeName = "Smart"; reforgeManaBonus = 10; normalTier(); }, 20);
            Add(availableReforges, () => { reforgeName = "Lucid"; reforgeManaBonus = 20; normalTier(); }, 15);
            Add(availableReforges, () => { reforgeName = "Resonant"; reforgeManaBonus = 30; normalTier(); }, 15);
            Add(availableReforges, () => { reforgeName = "Astral"; reforgeManaBonus = 40; normalTier(); }, 10);

            // --- DEFENSE REFORGES ---
            Add(availableReforges, () => { reforgeName = "Protected"; reforgeDefenseBonus = 1; normalTier(); }, 20);
            Add(availableReforges, () => { reforgeName = "Hardened"; reforgeDefenseBonus = 2; normalTier(); }, 15);
            Add(availableReforges, () => { reforgeName = "Fortified"; reforgeDefenseBonus = 4; normalTier(); }, 15);

            // --- DAMAGE REFORGES ---
            Add(availableReforges, () => { reforgeName = "Sharp"; reforgeDamageBonus = 0.01f; normalTier(); }, 20);
            Add(availableReforges, () => { reforgeName = "Fierce"; reforgeDamageBonus = 0.02f; normalTier(); }, 15);
            Add(availableReforges, () => { reforgeName = "Brutal"; reforgeDamageBonus = 0.03f; normalTier(); }, 10);

            // --- CRITICAL STRIKE CHANCE REFORGES ---
            Add(availableReforges, () => { reforgeName = "Focused"; reforgeCritBonus = 1; normalTier(); }, 20);
            Add(availableReforges, () => { reforgeName = "Precise"; reforgeCritBonus = 2; normalTier(); }, 15);

            // --- MOVEMENT SPEED REFORGES ---
            Add(availableReforges, () => { reforgeName = "Swift"; reforgeMoveSpeedBonus = 0.02f; normalTier(); }, 20);
            Add(availableReforges, () => { reforgeName = "Agile"; reforgeMoveSpeedBonus = 0.04f; normalTier(); }, 15);

            // --- KNOCKBACK IMMUNITY REFORGE ---
            Add(availableReforges, () => { reforgeName = "Unshakable"; reforgeKnockbackImmune = true; normalTier(); }, 15);

            // --- THORNS REFORGE ---
            Add(availableReforges, () => { reforgeName = "Spiked"; reforgeThornsPercent = 0.10f; normalTier(); }, 15);

            if (Main.hardMode)
            {
                Action hardmodeTier = () => reforgeRarityBonus = 1;

                Add(availableReforges, () => { reforgeName = "Lethal"; reforgeCritBonus = 3; reforgeDefenseBonus = -2; hardmodeTier(); }, 15);
                Add(availableReforges, () => { reforgeName = "Hasty"; reforgeMoveSpeedBonus = 0.06f; reforgeDefenseBonus = -2; hardmodeTier(); }, 15);
                Add(availableReforges, () => { reforgeName = "Leading"; reforgeMinionSlots = 1; hardmodeTier(); }, 15);
            }

            if (Main.hardMode && NPC.downedPlantBoss)
            {
                Action plantTier = () => reforgeRarityBonus = 2;

                Add(availableReforges, () => { reforgeName = "Bastion"; reforgeDefenseBonus = 8; plantTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Vicious"; reforgeDamageBonus = 0.04f; reforgeDefenseBonus = -3; plantTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Fatal"; reforgeCritBonus = 4; reforgeDefenseBonus = -3; plantTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Zephyr"; reforgeMoveSpeedBonus = 0.08f; reforgeDefenseBonus = -3; plantTier(); }, 10);
            }
            
            if (NPC.downedMoonlord)
            {
                Action moonlordTier = () => reforgeRarityBonus = 2;

                Add(availableReforges, () => { reforgeName = "Vigorous"; reforgeLifeBonus = 25; moonlordTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Mastiff"; reforgeLifeBonus = 500; reforgeDefenseBonus = -50; moonlordTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Boundless"; reforgeManaBonus = 50; moonlordTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Impenetrable"; reforgeDefenseBonus = 16; reforgeLifeBonus = -20; moonlordTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Bulwark"; reforgeDefenseBonus = 24; reforgeLifeBonus = -30; moonlordTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Apocalyptic"; reforgeDamageBonus = 0.05f; reforgeDefenseBonus = -4; moonlordTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Oracle"; reforgeCritBonus = 5; reforgeDefenseBonus = -4; moonlordTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Transient"; reforgeMoveSpeedBonus = 0.10f; reforgeDefenseBonus = -4; moonlordTier(); }, 10);
                Add(availableReforges, () => { reforgeName = "Commanding"; reforgeMinionSlots = 2; reforgeDamageBonus = -0.25f; moonlordTier(); }, 10);
                // --- EXTRA I-FRAMES REFORGE ---
                Add(availableReforges, () => { reforgeName = "Elusive"; reforgeExtraIFrameBonus = 15; moonlordTier(); }, 10); // 0.25 Extra i-frames
            }

            // Weighted random selection implementation
            if (availableReforges.Count > 0)
            {
                int totalWeight = 0;
                foreach (var option in availableReforges)
                {
                    totalWeight += option.Weight;
                }

                int randomValue = Main.rand.Next(totalWeight);
                int currentWeightSum = 0;

                foreach (var option in availableReforges)
                {
                    currentWeightSum += option.Weight;
                    if (randomValue < currentWeightSum)
                    {
                        option.ApplyAction.Invoke();
                        break;
                    }
                }
            }

            // Capture the base rarity the very first time this item is reforged
            if (originalRare == -1)
            {
                originalRare = item.rare;
            }

            // Error catch: If the base rarity is 10 (Red) or higher, cap the rarity bonus to 1 instead of 2
            if (originalRare >= 9 && reforgeRarityBonus > 1)
            {
                reforgeRarityBonus = 1;
            }

            // Cleanly calculate rarity based on the saved baseline instead of compounding
            item.rare = originalRare + reforgeRarityBonus;

            CombatText.NewText(player.getRect(), Color.Gold, $"Reforged: {reforgeName}!", true);
            item.stack++;
        }
    }
}