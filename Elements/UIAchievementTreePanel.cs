using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace AchievementTree.Elements;
public class UIAchievementTreePanel : UIElement
{
    private Vector2 offset;
    private bool dragging;

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        DragStart(evt);
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
        DragEnd(evt);
    }

    private void DragStart(UIMouseEvent evt)
    {
        offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
        dragging = true;
    }

    private void DragEnd(UIMouseEvent evt)
    {
        Vector2 endMousePosition = evt.MousePosition;
        Vector2 newPanelPosition = endMousePosition - offset;
        dragging = false;

        var parentSpace = Parent.GetDimensions().ToRectangle();
        var panelSpace = GetDimensions().ToRectangle();

        if (parentSpace.Width - Parent.PaddingLeft - Parent.PaddingRight < panelSpace.Width)
        {
            Left.Pixels = Utils.Clamp(newPanelPosition.X, parentSpace.Width - (Parent.PaddingLeft + Parent.PaddingRight) - panelSpace.Width, 0);
        }

        if (parentSpace.Height - Parent.PaddingTop - Parent.PaddingBottom < panelSpace.Height)
        {
            Top.Pixels = Utils.Clamp(newPanelPosition.Y, parentSpace.Height - (Parent.PaddingTop + Parent.PaddingBottom) - panelSpace.Height, 0);
        }

        Recalculate();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        var parentSpace = Parent.GetDimensions().ToRectangle();
        var panelSpace = GetDimensions().ToRectangle();

        if (dragging)
        {
            offset = new Vector2(Main.lastMouseX - Left.Pixels, Main.lastMouseY - Top.Pixels);

            if (parentSpace.Width - Parent.PaddingLeft - Parent.PaddingRight < panelSpace.Width)
            {
                Left.Pixels = Utils.Clamp(Main.mouseX - offset.X, parentSpace.Width - (Parent.PaddingLeft + Parent.PaddingRight) - panelSpace.Width, 0);
            }

            if (parentSpace.Height - Parent.PaddingTop - Parent.PaddingBottom < panelSpace.Height)
            {
                Top.Pixels = Utils.Clamp(Main.mouseY - offset.Y, parentSpace.Height - (Parent.PaddingTop + Parent.PaddingBottom) - panelSpace.Height, 0);
            }

            Recalculate();
        }

        if (!GetDimensions().ToRectangle().Intersects(parentSpace))
        {
            Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
            Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
            Recalculate();
        }
    }
}
