using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[Tracked]
[CustomEntity("GameHelper/McFire")]
public class McFire : Entity {
    private readonly Sprite sprite;
    private readonly int preferredRotation;
    private readonly EntityData data;
    private readonly List<McFlammable> fuels;
    private float delayTimer;
    public EntityID id;

    public McFire(EntityData data, Vector2 levelOffset, EntityID id) : base(data.Position + levelOffset) {
        delayTimer = data.Float("spreadingTime");
        sprite = GameHelper.SpriteBank.Create("fire");
        sprite.RenderPosition = new Vector2(-8, -8);
        Add(sprite);
        Collider = new Hitbox(16, 16, -8, -8);
        preferredRotation = data.Int("direction");
        fuels = new List<McFlammable>();
        Depth = -9;
        this.id = id;
        this.data = data;
        Add(new PlayerCollider(p => p.Die((-Vector2.UnitY).Rotate(sprite.Rotation))));
    }

    public override void Update() {
        base.Update();
        delayTimer -= Engine.DeltaTime;
        if(delayTimer <= 0) {
            fireTick();
        }
    }

    private void fireTick() {
        if(fuels.Count > 0) {
            fuels.ForEach(fuel => {
                McFire nf = new(new EntityData() {
                    Position = fuel.Center,
                    Values = data.Values
                }, Vector2.Zero, new EntityID());
                SceneAs<Level>().Add(nf);
                fuel.RemoveSelf();
            });
            Audio.Play("event:/GameHelper/fire/fire_burn");
        }
        RemoveSelf();
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(CollideAll<McFire>().Any(fire => CollideCheck(fire) && (fire as McFire).id.ID > id.ID)) RemoveSelf();
        int rotation = determineRotation();
        sprite.Rotation = (float) (rotation * 0.5f * Math.PI);
        sprite.RenderPosition += new Vector2(0 < rotation && rotation < 3 ? 16 : 0, 1 < rotation ? 16 : 0);
        Collider = new Hitbox(
            rotation % 2 == 0 ? 16 : 8,
            rotation % 2 == 0 ? 8 : 16,
            rotation == 3 ? 0 : -8,
            rotation == 0 ? 0 : -8
        );
    }

    private int determineRotation() {
        int? foundRotation = null;
        for(int i = 0; i < 4; i++) {
            Vector2 p = Position + (-Vector2.UnitX).Rotate((float) ((preferredRotation + i) * 0.5f * Math.PI));
            McFlammable collide = CollideFirst<McFlammable>(p);
            if(collide != null) {
                if(collide.Claim()) fuels.Add(collide);
                foundRotation = (preferredRotation + i + 1) % 4;
            }
        }
        if(foundRotation != null) return foundRotation.GetValueOrDefault();
        for(int i = 0; i < 4; i++) {
            if(CollideCheck<Solid>(Position + Vector2.UnitY.Rotate((float) ((preferredRotation + i) * 0.5 * Math.PI)))) {
                return (preferredRotation + i) % 4;
            }
        }
        RemoveSelf();
        return 0;
    }
}

[Tracked]
[CustomEntity("GameHelper/McFlammable")]
public class McFlammable : Solid {
    private readonly char tileType;
    private bool claimed;

    public McFlammable(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        tileType = data.Char("tileset", '3');
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
        Depth = -10;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(CollideCheck<Player>()) {
            RemoveSelf();
            return;
        }
        TileGrid tileGrid = GFX.FGAutotiler.GenerateBox(tileType, (int) Width / 8, (int) Height / 8).TileGrid;
        Add(new LightOcclude());
        Add(tileGrid);
        Add(new TileInterceptor(tileGrid, highPriority: true));
    }

    public bool Claim() {
        bool wasClaimed = claimed;
        claimed = true;
        return !wasClaimed;
    }
}