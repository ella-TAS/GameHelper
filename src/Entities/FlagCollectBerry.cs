using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using Monocle;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/FlagCollectBerry")]
[RegisterStrawberry(true, true)]
public class FlagCollectBerry : Strawberry {
    public string collectFlag, loseFlag;
    public readonly EntityData data;
    public readonly Vector2 levelOffset;
    public bool keepOnDeath;
    private DynamicData componentData;
    public new float wobble;
    public bool hadLeader;

    public FlagCollectBerry(EntityData data, Vector2 levelOffset, EntityID id) : base(data, levelOffset, id) {
        this.data = data;
        this.levelOffset = levelOffset;
        collectFlag = data.Attr("collectFlag");
        loseFlag = data.Attr("loseFlag");
        keepOnDeath = data.Bool("keepOnDeath");
    }

    public override void Update() {
        //visual / base update
        wobble += Engine.DeltaTime * 4f;
        Get<Sprite>().Y = Get<BloomPoint>().Y = Get<VertexLight>().Y = (float) Math.Sin(wobble) * 2f;
        componentData.Invoke("Update");

        //lose / collect check
        if(Follower.Leader == null) {
            //berry not picked up
            return;
        }
        if(!hadLeader) {
            //just picked up
            AddStored();
            hadLeader = true;
        }
        if(loseFlag != "" && SceneAs<Level>().Session.GetFlag(loseFlag)) {
            RemoveStored();
            hadLeader = false;
            LoseBerry();
        } else if(collectFlag != "" && SceneAs<Level>().Session.GetFlag(collectFlag)) {
            RemoveStored();
            hadLeader = false;
            CollectBerry();
        }
    }

    //methods can be overridden
    public virtual void AddStored() {
        if(keepOnDeath) {
            GameHelper.Session.StoredBerries.Add(this);
        }
    }

    public virtual void RemoveStored() {
        //find the list clone with the correct ID
        foreach(FlagCollectBerry f in GameHelper.Session.StoredBerries) {
            if(ID.Equals(f.ID)) {
                GameHelper.Session.StoredBerries.Remove(f);
                return;
            }
        }
    }

    public virtual void LoseBerry() {
        Collidable = false;
        Add(new Coroutine(routineReturn()));
        Follower.Leader.LoseFollower(Follower);
    }

    public virtual void CollectBerry() {
        OnCollect();
    }

    public virtual void AddClone(Player p, Scene s) {
        FlagCollectBerry clone = Clone();
        s.Add(clone);
        p.Leader.GainFollower(clone.Follower);
        clone.Position = p.Center;
    }

    public virtual FlagCollectBerry Clone() {
        return new(data, levelOffset, ID) {
            ReturnHomeWhenLost = true,
            Depth = -1000000,
            hadLeader = true
        };
    }

    private IEnumerator routineReturn() {
        //patch to avoid spam collecting on lose flag
        yield return 0.5f;
        Collidable = true;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        GameHelper.Session.StoredBerries ??= new();
        foreach(FlagCollectBerry f in GameHelper.Session.StoredBerries) {
            if(!hadLeader && ID.Equals(f.ID)) {
                //berry exists
                RemoveSelf();
                return;
            }
        }
        componentData = new DynamicData(Components);
    }

    private static void OnPlayerAdded(On.Celeste.Player.orig_Added orig, Player p, Scene s) {
        orig(p, s);
        GameHelper.Session.StoredBerries ??= new();
        foreach(FlagCollectBerry f in GameHelper.Session.StoredBerries) {
            f.AddClone(p, s);
        }
    }

    public static void Hook() {
        On.Celeste.Player.Added += OnPlayerAdded;
    }

    public static void Unhook() {
        On.Celeste.Player.Added -= OnPlayerAdded;
    }
}