using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[Tracked]
[CustomEntity("GameHelper/EntityModifier")]
public class EntityModifier : Wrapper {
    private List<Entity> targets;
    private readonly Vector2[] nodes;
    private readonly Vector2 levelOffset;
    private bool wasFlag;
    private readonly bool debug, allEntities, invertFlag, onlyOnce, everyFrame, doNewlyAddedEntities;
    private readonly string onlyType, fieldName, flag;
    private readonly object value;
    private readonly bool isCommon;
    private readonly string doActive, doCollidable, doVisible;

    public EntityModifier(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        Depth = int.MinValue + 77;
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
        doNewlyAddedEntities = data.Bool("doNewlyAddedEntities");
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
        bool isFlag = getFlag();
        if(isFlag && !wasFlag) {
            modify(targets);
            wasFlag = true;
        } else if(isFlag && everyFrame) {
            modify(targets);
        } else if(!isFlag && wasFlag) {
            wasFlag = false;
            modify(targets, true);
        }
    }

    private void modify(List<Entity> targetEntities, bool flagDisabled = false) {
        targetEntities.RemoveAll(gone => gone == null);
        if(targetEntities.Count == 0 && !doNewlyAddedEntities) {
            ComplainEntityNotFound("Entity Modifier");
        }

        foreach(Entity target in targetEntities) {
            if(debug) {
                Logger.Info("GameHelper", "Modifying entity " + target.GetType());
            }

            if(isCommon) {
                modCommonBool(target, "Active", doActive, flagDisabled);
                modCommonBool(target, "Collidable", doCollidable, flagDisabled);
                modCommonBool(target, "Visible", doVisible, flagDisabled);
            } else if(!flagDisabled) {
                DynamicData.For(target).Set(fieldName, value);
            }
        }

        if(onlyOnce) {
            RemoveSelf();
        }
    }

    private void modCommonBool(Entity target, string name, string mode, bool flagDisabled) {
        if(mode == "ignore" || (flagDisabled && mode != "set_flag")) {
            return;
        }
        bool val = false;
        if(mode == "set_true") {
            val = true;
        } else if(mode == "set_flag") {
            val = getFlag();
        } else if(mode == "invert") {
            val = !DynamicData.For(target).Get<bool>(name);
        }
        DynamicData.For(target).Set(name, val);
    }

    private bool getFlag() {
        return Util.GetFlag(flag, Scene, true, invertFlag);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(debug) {
            LogAllEntities();
        }

        targets = FindTargets(Position, nodes, levelOffset, allEntities, onlyType);
        if(targets.Count == 0 && !doNewlyAddedEntities) {
            ComplainEntityNotFound("Entity Modifier");
        }

        if(!getFlag()) {
            modify(targets, true);
            return;
        }

        modify(targets);
        wasFlag = true;

        if(flag?.Length == 0 && !everyFrame && !doNewlyAddedEntities) {
            RemoveSelf();
        }
    }

    private void handleSceneAdd(Entity t) {
        if(onlyType.Length > 0 && t.GetType().ToString() == onlyType && !targets.Contains(t)) {
            targets.Add(t);
            if(debug) {
                Logger.Info("GameHelper", "Newly added entity added: " + t.GetType());
            }
            if(flag?.Length == 0) {
                modify([t]);
            }
        }
    }

    private static void OnSceneAdd(On.Monocle.Scene.orig_Add_Entity orig, Scene s, Entity t) {
        orig(s, t);
        if(s is Level) {
            foreach(EntityModifier m in s.Tracker.GetEntities<EntityModifier>()) {
                if(m.doNewlyAddedEntities) {
                    m.handleSceneAdd(t);
                }
            }
        }
    }

    public static void Hook() {
        On.Monocle.Scene.Add_Entity += OnSceneAdd;
    }

    public static void Unhook() {
        On.Monocle.Scene.Add_Entity -= OnSceneAdd;
    }
}