using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityRespriter")]
public class EntityRespriter : Wrapper {
    private Vector2[] nodes;
    private Vector2 levelOffset, spriteOffset;
    private string fieldName, spriteFolder, spriteName, xmlPath, spriteID, onlyType;
    private float delay;
    private bool allEntities, debug;

    public EntityRespriter(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        nodes = data.Nodes;
        this.levelOffset = levelOffset;
        debug = data.Bool("debug");
        allEntities = data.Bool("allEntities");
        onlyType = data.Attr("onlyType");
        spriteOffset = new Vector2(data.Float("offsetX"), data.Float("offsetY"));
        fieldName = data.Attr("fieldName", "sprite");
        spriteFolder = data.Attr("spriteFolder");
        spriteName = data.Attr("spriteName");
        delay = data.Float("delay");
        xmlPath = data.Attr("xmlPath");
        spriteID = data.Attr("spriteID");
    }

    private Sprite createSprite() {
        Sprite sprite;
        if(spriteFolder != "") {
            //direct
            sprite = new Sprite(GFX.Game, spriteFolder);
            sprite.AddLoop("idle", spriteName, delay);
            sprite.RenderPosition = spriteOffset;
            sprite.Play("idle");
        } else {
            //xml
            sprite = new SpriteBank(GFX.Game, xmlPath).Create(spriteID);
        }
        return sprite;
    }

    private void injectSprite(Entity targetEntity) {
        if(targetEntity == null) {
            ComplainEntityNotFound("Entity Respriter");
            return;
        }
        if(debug) {
            Logger.Log("GameHelper", "Respriting entity " + EntityStamp(targetEntity));
        }

        //exchange component
        Sprite localSprite = createSprite();
        targetEntity.Get<Sprite>()?.RemoveSelf();
        targetEntity.Add(localSprite);

        //set reference
        if(fieldName != "") {
            DynamicData.For(targetEntity).Set(fieldName, localSprite);
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
    }
}