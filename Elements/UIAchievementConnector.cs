using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AchievementTree.Elements;
public class UIAchievementConnector(UIAchievementItem connectFrom, UIAchievementItem connectTo) : UIElement()
{
    public UIAchievementItem connectFrom = connectFrom;
    public UIAchievementItem connectTo = connectTo;

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 startPosition = connectTo.GetDimensions().Position() + new Vector2(connectTo.GetDimensions().Width, connectTo.GetDimensions().Height) / 2;
        Vector2 endPosition = connectFrom.GetDimensions().Position() + new Vector2(connectFrom.GetDimensions().Width, connectFrom.GetDimensions().Height) / 2;

        float distance = (float)Math.Sqrt(Math.Pow((endPosition.X - startPosition.X), 2) + Math.Pow((endPosition.Y - startPosition.Y), 2));
        float rotation = (endPosition - startPosition).ToRotation();
        Vector2 stepDistance = (endPosition - startPosition) / (float)distance;

        for (int i = 0; i < distance; i++)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{nameof(AchievementTree)}/Assets/AchievementConnector{(connectFrom.complete && connectTo.complete ? "Full" : "")}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, startPosition + stepDistance * i, null, Color.White, rotation + MathHelper.Pi, new(1, 4), 1f, SpriteEffects.None, 0);
        }
    }
}
