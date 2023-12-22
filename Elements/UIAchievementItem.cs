using AchievementTree.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AchievementTree.Elements;
public class UIAchievementItem : UIElement
{
    public bool locked = false;
    public float opacity = 1f;

    readonly Rectangle unlockedFrame;
    readonly Rectangle lockedFrame;

    Rectangle frame;

    UIImageFramed Icon { get; set; }
    UIImage Border { get; set; }

    public readonly Achievement achievement;

    public UIAchievementItem(Achievement achievement) : base()
    {
        Width = StyleDimension.FromPixels(72f);
        Height = StyleDimension.FromPixels(72f);

        this.achievement = achievement;

        int iconIndex = Main.Achievements.GetIconIndex(achievement.Name);
        unlockedFrame = new(iconIndex % 8 * 66, iconIndex / 8 * 66, 64, 64);
        lockedFrame = unlockedFrame;
        lockedFrame.X += 528;
        frame = lockedFrame;

        UpdateFrame();

        Icon = this.AddElement(new UIImageFramed(Main.Assets.Request<Texture2D>("Images/UI/Achievements"), frame).With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 0.5f;
        }));

        Border = this.AddElement(new UIImage(Main.Assets.Request<Texture2D>("Images/UI/Achievement_Borders", ReLogic.Content.AssetRequestMode.ImmediateLoad)));
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        // locked = !Main.LocalPlayer.GetModPlayer<AchievementTreeModPlayer>().IsAchievementUnlocked[achievement.Id];
        UpdateFrame();

        Icon.Color = Border.Color = Color.White * opacity;
    }

    private void UpdateFrame()
    {
        frame = locked ? lockedFrame : unlockedFrame;
        Icon?.SetFrame(frame);
    }
}
