using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections.Generic;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityModifier")]
public class EntityModifier : Wrapper {
    private List<Entity> targets;
    private readonly Vector2[] nodes;
    private Vector2 levelOffset;
    private bool wasFlag;
    private readonly bool debug, allEntities, invertFlag, onlyOnce, everyFrame;
    private readonly string onlyType, fieldName, flag;
    private readonly object value;
    private readonly bool isCommon;
    private readonly string doActive, doCollidable, doVisible;

    public EntityModifier(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        Depth = int.MinValue;
        nodes = data.Nodes;
        this.levelOffset = levelOffset;
        debug = data.Bool("debug");
        allEntities = data.Bool("allEntities");
        onlyType = data.Attr("onlyType");
        fieldName = data.Attr("fieldName");
        flag = data.Attr("activationFlag");
        invertFlag = data.Bool("invertFlag");
        onlyOnce = data.Bool("onlyOnce");
        everyFrame = data.Bool("everyFrame");
        switch(data.Attr("valueType")) {
            case "number":
                if(data.Bool("integer"))
                    value = data.Int("valueNumber");
                else
                    value = data.Float("valueNumber");
                break;
            case "string":
                value = data.Attr("valueString");
                break;
            case "bool":
                value = data.Bool("valueBool");
                break;
            case "common":
                isCommon = true;
                doActive = data.Attr("changeActive");
                doCollidable = data.Attr("changeCollidable");
                doVisible = data.Attr("changeVisible");
                break;
        }
    }

    public override void Update() {
        base.Update();
        bool isFlag = flag?.Length == 0 || getFlag();
        if(isFlag && !wasFlag) {
            modify(targets);
            wasFlag = true;
        } else if(isFlag && everyFrame) {
            modify(targets);
        } else if(!isFlag && wasFlag) {
            wasFlag = false;
        }
    }

    private void modify(List<Entity> targetEntities) {
        foreach(Entity target in targetEntities) {
            if(target == null) {
                ComplainEntityNotFound("Entity Modifier");
            }
            if(debug) {
                Logger.Log("GameHelper", "Modifying entity " + target.GetType().ToString());
            }

            DynamicData.For(target).Set(fieldName, value);
        }

        if(onlyOnce) {
            RemoveSelf();
        }
    }

    private bool getFlag() {
        return invertFlag ^ SceneAs<Level>().Session.GetFlag(flag);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(debug) {
            LogAllEntities();
        }

        targets = FindTargets(Position, nodes, levelOffset, allEntities, onlyType);
        if(targets.Count == 0) {
            ComplainEntityNotFound("Entity Modifier");
        }

        if(flag != "" && !getFlag()) {
            return;
        }

        modify(targets);
        wasFlag = true;

        if(flag?.Length == 0 && !everyFrame) {
            RemoveSelf();
        }
    }
}