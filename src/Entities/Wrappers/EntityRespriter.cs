using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityRespriter")]
public class EntityRespriter : Entity {
    private Sprite sprite;
    private string fieldName;

    public EntityRespriter(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        fieldName = data.Attr("fieldName", "sprite");
        //prepare sprite
        if(data.Attr("spriteFolder") != "") {
            //direct
            sprite = new Sprite(GFX.Game, data.Attr("spriteFolder"));
            sprite.AddLoop("idle", data.Attr("spriteName"), data.Float("delay"));
            sprite.Play("idle");
        } else {
            //xml
            sprite = new SpriteBank(GFX.Game, data.Attr("xmlPath")).Create(data.Attr("spriteID"));
        }
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
        //exchange component
        Sprite targetSprite = targetEntity.Get<Sprite>();
        if(targetSprite != null) {
            targetSprite.RemoveSelf();
        }
        targetEntity.Add(sprite);
        //set reference
        DynamicData.For(targetEntity).Set(fieldName, sprite);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        injectSprite(nearestEntity(Position));
    }
}