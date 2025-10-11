using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public class DebugDecalData {
    public string type;
    public Vector2 position;
    public int width;
    public int height;
    public bool hollow;
    public float thickness;
    public string data;
    public float scaleX;
    public float scaleY;
    public Color color;
    public List<string> textures;
    public float animationSpeed;
    public bool useGui;
    public float rotation;
}