using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using LegacyScriptures.Content.Projectiles;

namespace LegacyScriptures.Content.Buffs
{
    public class CustomTerraprismaBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<CustomTerraprismaProj>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}