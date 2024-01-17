using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AchievementTree
{
    public class AchievementTree : Mod
    {
        public override void Load()
        {
            Main.Achievements.ClearAll();
            IL_Main.CanPauseGame += IL_Main_CanPauseGame;
            IL_IngameOptions.Draw += IL_IngameOptions_Draw;
            IL_AchievementAdvisor.DrawOneAchievement += IL_AchievementAdvisor_DrawOneAchievement;
        }

        private void IL_Main_CanPauseGame(ILContext il)
        {
            ILCursor c = new(il);

            c.TryGotoNext(i => i.MatchLdsfld<Main>("AchievementsMenu"));
            c.Remove();

            c.EmitDelegate(() =>
            {
                return new AchievementTreeUI();
            });
        }

        private void IL_IngameOptions_Draw(ILContext il)
        {
            ILCursor c = new(il);

            c.TryGotoNext(i => i.MatchCall<IngameFancyUI>("OpenAchievements"));
            c.Remove();

            c.EmitDelegate(() =>
            {
                IngameFancyUI.OpenUIState(new AchievementTreeUI());
            });
        }

        private void IL_AchievementAdvisor_DrawOneAchievement(ILContext il)
        {
            ILCursor c = new(il);

            c.TryGotoNext(MoveType.After, i => i.MatchStsfld<Main>("ingameOptionsWindow"));
            c.RemoveRange(4);

            c.EmitDelegate(() =>
            {
                IngameFancyUI.OpenUIState(new AchievementTreeUI());
            });
        }
    }
}