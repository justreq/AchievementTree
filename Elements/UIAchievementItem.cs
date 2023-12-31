using AchievementTree.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AchievementTree.Elements;
public class UIAchievementItem : UIElement
{
    public bool completed = false;
    public float opacity = 1f;

    Rectangle frame;

    UIImageFramed Icon { get; set; }
    UIImage Border { get; set; }

    public readonly LocalAchievement localAchievement;

    public UIAchievementItem(LocalAchievement localAchievement) : base()
    {
        Width = StyleDimension.FromPixels(72f);
        Height = StyleDimension.FromPixels(72f);

        this.localAchievement = localAchievement;
        frame = localAchievement.icons.IncompleteFrame;

        UpdateFrame();

        Icon = this.AddElement(new UIImageFramed(localAchievement.modded ? ModContent.Request<Texture2D>(localAchievement.icons.TexturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad) : Main.Assets.Request<Texture2D>(localAchievement.icons.TexturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad), frame).With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 0.5f;
        }));

        Border = this.AddElement(new UIImage(Main.Assets.Request<Texture2D>("Images/UI/Achievement_Borders", ReLogic.Content.AssetRequestMode.ImmediateLoad)));
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        completed = Main.LocalPlayer.GetModPlayer<AchievementTreeModPlayer>().FindAchievement(localAchievement.name).isCompleted;
        UpdateFrame();

        Icon.Color = Border.Color = Color.White * opacity;
    }

    private void UpdateFrame()
    {
        frame = completed ? localAchievement.icons.CompleteFrame : localAchievement.icons.CompleteFrame;
        Icon?.SetFrame(frame);
    }
}
