using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityRespriter")]
public class EntityRespriter : Entity {
    private Vector2[] nodes;
    private Vector2 levelOffset, spriteOffset;
    private string fieldName, spriteFolder, spriteName, xmlPath, spriteID;
    private float delay;
    private bool allEntities, debug;

    public EntityRespriter(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        nodes = data.Nodes;
        this.levelOffset = levelOffset;
        spriteOffset = new Vector2(data.Float("offsetX"), data.Float("offsetY"));
        allEntities = data.Bool("allEntities");
        fieldName = data.Attr("fieldName", "sprite");
        spriteFolder = data.Attr("spriteFolder");
        spriteName = data.Attr("spriteName");
        delay = data.Float("delay");
        xmlPath = data.Attr("xmlPath");
        spriteID = data.Attr("spriteID");
        debug = data.Bool("debug");
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

    private Entity nearestEntity(Vector2 pos) {
        Entity entity = null;
        float minDistance = float.MaxValue;
        foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
            if(e.GetType() != typeof(EntityRespriter) && Vector2.Distance(e.Position, pos) < minDistance) {
                entity = e;
                minDistance = Vector2.Distance(e.Position, pos);
            }
        }
        return entity;
    }

    private void injectSprite(Entity targetEntity) {
        if(targetEntity == null) {
            Logger.Log(LogLevel.Warn, "GameHelper", "Entity Respriter found no entity in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
            return;
        }
        if(debug) {
            Logger.Log("GameHelper", "Respriting entity " + targetEntity.GetType().ToString() + " in room " + SceneAs<Level>().Session.LevelData.Name);
        }
        //exchange component
        Sprite localSprite = createSprite();
        Sprite targetSprite = targetEntity.Get<Sprite>();
        if(targetSprite != null) {
            targetSprite.RemoveSelf();
        }
        targetEntity.Add(localSprite);
        //set reference
        DynamicData.For(targetEntity).Set(fieldName, localSprite);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        Entity targetEntity = nearestEntity(Position);
        if(allEntities) {
            foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
                if(e.GetType() == targetEntity.GetType()) {
                    injectSprite(e);
                }
            }
        } else {
            injectSprite(targetEntity);
            foreach(Vector2 n in nodes) {
                injectSprite(nearestEntity(n + levelOffset));
            }
        }
    }
}