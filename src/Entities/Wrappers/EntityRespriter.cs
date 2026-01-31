using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[Tracked]
[CustomEntity("GameHelper/EntityRespriter")]
public class EntityRespriter : Wrapper {
    private List<Entity> targets;
    private readonly Vector2[] nodes;
    private readonly Vector2 levelOffset, spriteOffset;
    private readonly string fieldName, spriteFolder, spriteName, xmlPath, spriteID, onlyType, flag;
    private readonly float delay;
    private readonly bool flipX, flipY, allEntities, debug, removeAllComponents, invertFlag, doNewlyAddedEntities;
    private bool firstInjectDone, wasFlag;

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
        flag = data.Attr("activationFlag");
        invertFlag = data.Bool("invertFlag");
        doNewlyAddedEntities = data.Bool("doNewlyAddedEntities");
    }

    public override void Update() {
        base.Update();

        if (getFlag() && (!firstInjectDone || (!wasFlag && allEntities))) {
            foreach (Entity e in targets) {
                injectSprite(e);
            }
            firstInjectDone = true;
        }

        if (getFlag() && !wasFlag) {
            wasFlag = true;
        }
        if (!getFlag() && wasFlag) {
            wasFlag = false;
        }
    }

    private Sprite createSprite() {
        Sprite sprite;
        if (spriteFolder != "") {
            // direct
            sprite = new Sprite(GFX.Game, spriteFolder);
            sprite.AddLoop("idle", spriteName, delay);
            sprite.Play("idle");
        } else {
            // xml
            sprite = new SpriteBank(GFX.Game, xmlPath).Create(spriteID);
        }
        sprite.RenderPosition = spriteOffset;
        sprite.FlipX = flipX;
        sprite.FlipY = flipY;
        return sprite;
    }

    private void injectSprite(Entity targetEntity) {
        if (targetEntity == null) {
            return;
        }
        if (debug) {
            Logger.Info("GameHelper", "Respriting entity " + targetEntity.GetType());
        }

        DynamicData targetData = DynamicData.For(targetEntity);

        //exchange component
        Sprite localSprite = createSprite();
        if (removeAllComponents) {
            targetEntity.Components.RemoveAll<Image>();
        } else if (fieldName?.Length == 0) {
            targetEntity.Get<Sprite>()?.RemoveSelf();
        } else {
            foreach (Image s in targetEntity.Components.GetAll<Image>()) {
                if (s == targetData.Get(fieldName)) {
                    s?.RemoveSelf();
                    break;
                }
            }
        }
        targetEntity.Add(localSprite);

        //set reference
        if (fieldName != "") {
            targetData.Set(fieldName, localSprite);
        }
    }

    private void handleSceneAdd(Entity t) {
        if (onlyType.Length > 0 && (t.GetType().FullName == onlyType || t.GetType().Name == onlyType) && !targets.Contains(t)) {
            targets.Add(t);
            if (debug) {
                Logger.Info("GameHelper", "Newly added entity added: " + t.GetType());
            }
            if (getFlag()) {
                injectSprite(t);
            }
        }
    }

    private static void OnSceneAdd(On.Monocle.Scene.orig_Add_Entity orig, Scene s, Entity t) {
        orig(s, t);
        if (s is Level && s.Tracker.IsEntityTracked<EntityRespriter>()) {
            foreach (EntityRespriter r in s.Tracker.GetEntities<EntityRespriter>()) {
                if (r.doNewlyAddedEntities) {
                    r.handleSceneAdd(t);
                }
            }
        }
    }

    private bool getFlag() {
        return Util.GetFlag(flag, Scene, true, invertFlag);
    }


    public override void Awake(Scene scene) {
        base.Awake(scene);

        targets = FindTargets(Position, nodes, levelOffset, allEntities, onlyType);
        if (targets.Count == 0 && !doNewlyAddedEntities) {
            ComplainEntityNotFound("Entity Respriter");
            return;
        }
        if (getFlag()) {
            foreach (Entity e in targets) {
                injectSprite(e);
            }
            firstInjectDone = true;

            // not needed anymore
            if (!doNewlyAddedEntities) {
                RemoveSelf();
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