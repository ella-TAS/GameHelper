using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityRespriter")]
public class EntityRespriter : Wrapper {
    private readonly Vector2[] nodes;
    private readonly Vector2 levelOffset, spriteOffset;
    private readonly string fieldName, spriteFolder, spriteName, xmlPath, spriteID, onlyType;
    private readonly float delay;
    private readonly bool flipX, flipY, allEntities, debug, removeAllComponents;

    public EntityRespriter(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        Depth = int.MinValue;
        nodes = data.Nodes;
        this.levelOffset = levelOffset;
        debug = data.Bool("debug");
        allEntities = data.Bool("allEntities");
        onlyType = data.Attr("onlyType");
        spriteOffset = new Vector2(data.Float("offsetX"), data.Float("offsetY"));
        flipX = data.Bool("flipX");
        flipY = data.Bool("flipY");
        fieldName = data.Attr("fieldName", "sprite");
        spriteFolder = data.Attr("spriteFolder");
        spriteName = data.Attr("spriteName");
        delay = data.Float("delay");
        xmlPath = data.Attr("xmlPath");
        spriteID = data.Attr("spriteID");
        removeAllComponents = data.Bool("removeAllComponents");
    }

    private Sprite createSprite() {
        Sprite sprite;
        if(spriteFolder != "") {
            //direct
            sprite = new Sprite(GFX.Game, spriteFolder);
            sprite.AddLoop("idle", spriteName, delay);
            sprite.Play("idle");
        } else {
            //xml
            sprite = new SpriteBank(GFX.Game, xmlPath).Create(spriteID);
        }
        sprite.RenderPosition = spriteOffset;
        sprite.FlipX = flipX;
        sprite.FlipY = flipY;
        return sprite;
    }

    private void injectSprite(Entity targetEntity) {
        if(targetEntity == null) {
            ComplainEntityNotFound("Entity Respriter");
            return;
        }
        if(debug) {
            Logger.Info("GameHelper", "Respriting entity " + targetEntity.GetType());
        }

        DynamicData targetData = DynamicData.For(targetEntity);

        //exchange component
        Sprite localSprite = createSprite();
        if(removeAllComponents) {
            targetEntity.Components.RemoveAll<Sprite>();
        } else if(fieldName?.Length == 0) {
            targetEntity.Get<Sprite>()?.RemoveSelf();
        } else {
            foreach(Sprite s in targetEntity.Components.GetAll<Sprite>()) {
                if(s == targetData.Get(fieldName)) {
                    s?.RemoveSelf();
                    break;
                }
            }
        }
        targetEntity.Add(localSprite);

        //set reference
        if(fieldName != "") {
            targetData.Set(fieldName, localSprite);
        }
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(debug) {
            LogAllEntities();
        }

        List<Entity> targets = FindTargets(Position, nodes, levelOffset, allEntities, onlyType);
        if(targets.Count == 0) {
            ComplainEntityNotFound("Entity Respriter");
        }
        foreach(Entity e in targets) {
            injectSprite(e);
        }
        RemoveSelf();
    }
}