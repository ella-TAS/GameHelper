using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityRespriter")]
public class EntityRespriter : Entity {
    private Vector2[] nodes;
    private Vector2 levelOffset, spriteOffset;
    private string fieldName, spriteFolder, spriteName, xmlPath, spriteID, onlyType;
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
        onlyType = data.Attr("onlyType");
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

    private Entity findEntity(Vector2 pos, string type) {
        Entity entity = null;
        float minDistance = float.MaxValue;
        foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
            if(e.GetType() != typeof(EntityRespriter) &&
            (type == "" || e.GetType().ToString() == type) &&
            Vector2.Distance(e.Position, pos) < minDistance) {
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
            Logger.Log("GameHelper", "Respriting entity " + entityStamp(targetEntity));
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

    private string entityStamp(Entity e) {
        return e.GetType().ToString() + " [" + SceneAs<Level>().Session.LevelData.Name + "]";
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(debug) {
            Logger.Log("GameHelper", "List of all entities in the room:");
            foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
                Logger.Log("GameHelper", entityStamp(e));
            }
        }

        //don't look for entity if allEntities and type is set
        Entity targetEntity = null;
        if(!allEntities || onlyType == "") {
            targetEntity = findEntity(Position, onlyType);
        }

        if(allEntities) {
            foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
                if((onlyType == "" && e.GetType() == targetEntity.GetType()) ||
                e.GetType().ToString() == onlyType) {
                    injectSprite(e);
                }
            }
        } else {
            injectSprite(targetEntity);
            foreach(Vector2 n in nodes) {
                injectSprite(findEntity(n + levelOffset, onlyType));
            }
        }
    }
}