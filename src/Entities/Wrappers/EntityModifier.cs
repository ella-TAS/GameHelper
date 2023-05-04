using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections.Generic;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityModifier")]
public class EntityModifier : Wrapper {
    private Vector2[] nodes;
    private Vector2 levelOffset;
    private bool debug, allEntities;
    private string onlyType, fieldName;
    private object value;

    public EntityModifier(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        nodes = data.Nodes;
        this.levelOffset = levelOffset;
        debug = data.Bool("debug");
        allEntities = data.Bool("allEntities");
        onlyType = data.Attr("onlyType");
        fieldName = data.Attr("fieldName");
        switch(data.Attr("valueType")) {
            case "int":
                value = data.Int("valueNumber");
                break;
            case "float":
                value = data.Float("valueNumber");
                break;
            case "string":
                value = data.Attr("valueString");
                break;
            case "bool":
                value = data.Bool("valueBool");
                break;
        }
    }

    private void modify(Entity targetEntity) {
        if(targetEntity == null) {
            ComplainEntityNotFound("Entity Modifier");
        }
        if(debug) {
            Logger.Log("GameHelper", "Modifying entity " + EntityStamp(targetEntity));
        }

        DynamicData.For(targetEntity).Set(fieldName, value);
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
            modify(e);
        }
    }
}