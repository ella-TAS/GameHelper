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
    private readonly string onlyType, fieldName, flag, doActive, doCollidable, doVisible, vectorMode;
    private readonly bool isCommon, isVector, onlyX, onlyY;
    private readonly float vectorX, vectorY;
    private readonly object value;

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
        switch (data.Attr("valueType")) {
        case "number":
            if (data.Bool("integer"))
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
        case "vector":
            isVector = true;
            vectorX = data.Float("valueX");
            vectorY = data.Float("valueY");
            vectorMode = data.Attr("vectorMode");
            onlyX = data.Bool("onlyX");
            onlyY = data.Bool("onlyY");
            break;
        }
    }

    public override void Update() {
        base.Update();
        bool isFlag = getFlag();
        if (isFlag && !wasFlag) {
            wasFlag = true;
            modify(targets);
        } else if (isFlag && everyFrame) {
            modify(targets);
        } else if (!isFlag && wasFlag) {
            wasFlag = false;
            modify(targets);
        }
    }

    private void modify(List<Entity> targetEntities) {
        bool isFlag = getFlag();

        targetEntities.RemoveAll(e => e?.Scene == null);
        if (targetEntities.Count == 0 && !doNewlyAddedEntities) {
            if (debug) {
                Logger.Info("GameHelper", "All entities were removed from the scene, killing modifier");
            }
            RemoveSelf();
            return;
        }

        if (isCommon || isFlag) {
            foreach (Entity target in targetEntities) {
                if (debug) {
                    Logger.Info("GameHelper", "Modifying entity " + target.GetType());
                }

                if (isCommon) {
                    modCommonBool(target, "Active", doActive, isFlag);
                    modCommonBool(target, "Collidable", doCollidable, isFlag);
                    modCommonBool(target, "Visible", doVisible, isFlag);
                } else if (isVector) {
                    modVector(target, fieldName, new Vector2(vectorX, vectorY), vectorMode, onlyX, onlyY);
                } else {
                    DynamicData.For(target).Set(fieldName, value);
                }
            }
        }

        if (onlyOnce && isFlag) {
            RemoveSelf();
        }
    }

    private static void modCommonBool(Entity target, string name, string mode, bool flag) {
        if (mode == "ignore" || (!flag && mode != "set_flag")) {
            return;
        }
        bool val = false;
        switch (mode) {
        case "set_true":
            val = true;
            break;
        case "set_flag":
            val = flag;
            break;
        case "invert":
            val = !DynamicData.For(target).Get<bool>(name);
            break;
        }
        DynamicData.For(target).Set(name, val);
    }

    private static void modVector(Entity target, string name, Vector2 val, string mode, bool onlyX, bool onlyY) {
        Vector2 previous = DynamicData.For(target).Get<Vector2>(name);
        switch (mode) {
        case "set":
            if (onlyX) {
                val.Y = previous.Y;
            }
            if (onlyY) {
                val.X = previous.X;
            }
            break;
        case "add":
            if (!onlyX) {
                val.Y += previous.Y;
            }
            if (!onlyY) {
                val.X += previous.X;
            }
            break;
        case "multiply":
            if (!onlyX) {
                previous.Y *= val.Y;
            }
            if (!onlyY) {
                previous.X *= val.X;
            }
            val = previous;
            break;
        }
        DynamicData.For(target).Set(name, val);
    }

    private bool getFlag() {
        return Util.GetFlag(flag, Scene, true, invertFlag);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if (debug) {
            LogAllEntities();
        }

        targets = FindTargets(Position, nodes, levelOffset, allEntities, onlyType);
        if (targets.Count == 0 && !doNewlyAddedEntities) {
            ComplainEntityNotFound("Entity Modifier");
            return;
        }

        modify(targets);

        if (getFlag()) {
            wasFlag = true;

            if (flag?.Length == 0 && !everyFrame && !doNewlyAddedEntities) {
                RemoveSelf();
            }
        }
    }

    private void handleSceneAdd(Entity t) {
        if (onlyType.Length > 0 && t.GetType().ToString() == onlyType && !targets.Contains(t)) {
            targets.Add(t);
            if (debug) {
                Logger.Info("GameHelper", "Newly added entity added: " + t.GetType());
            }
            modify([t]);
        }
    }

    private static void OnSceneAdd(On.Monocle.Scene.orig_Add_Entity orig, Scene s, Entity t) {
        orig(s, t);
        if (s is Level && s.Tracker.IsEntityTracked<EntityModifier>()) {
            foreach (EntityModifier m in s.Tracker.GetEntities<EntityModifier>()) {
                if (m.doNewlyAddedEntities) {
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