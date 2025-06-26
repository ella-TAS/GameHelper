// code borrowed from https://github.com/EverestAPI/Everest/blob/dev/Celeste.Mod.mm/Mod/UI/OuiMapSearch.cs
// The MIT License (MIT), Copyright (c) 2018 Everest Team

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.GameHelper.Utils;

public class SearchMenu : Entity {
    public bool leftFocused {
        get => leftMenu.Focused;
        set {
            leftMenu.Focused = value;
            rightMenu.Focused = !value;
        }
    }

    public TextMenu leftMenu;
    public TextMenu rightMenu;

    public int Selection {
        get => currentMenu.Selection;
        set => currentMenu.Selection = value;
    }

    public TextMenu currentMenu => leftFocused ? leftMenu : rightMenu;

    public bool Focused {
        get => leftMenu.Focused || rightMenu.Focused;
        set {
            if(value) {
                leftFocused = true;
            } else {
                leftMenu.Focused = false;
                rightMenu.Focused = false;
            }
        }
    }

    public SearchMenu() {
        Position = Vector2.Zero;
        leftMenu = new();
        rightMenu = new();
        rightMenu.InnerContent = TextMenu.InnerContentMode.TwoColumn;
        leftMenu.Position.X = Engine.Width / 5f;
        rightMenu.Position.X = 0.65f * Engine.Width;
        rightMenu.Focused = false;
    }

    public override void Update() {
        base.Update();
        leftMenu.Update();
        rightMenu.Update();
    }

    public override void Render() {
        base.Render();
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
        leftMenu.Render();
        rightMenu.Render();
        Draw.SpriteBatch.End();
    }
}