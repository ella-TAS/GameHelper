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
    private readonly string collectFlag, loseFlag;
    private readonly EntityData data;
    private readonly Vector2 levelOffset;
    private readonly bool keepOnDeath;
    private DynamicData componentData;
    private float wobble;
    private bool hadLeader;

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
        //add only if keep on death
        if(keepOnDeath) {
            GameHelper.Instance.Session.StoredBerries.Add(this);
        }
    }

    public virtual void RemoveStored() {
        //find the list clone with the correct ID
        foreach(FlagCollectBerry f in GameHelper.Instance.Session.StoredBerries) {
            if(ID.Equals(f.ID)) {
                GameHelper.Instance.Session.StoredBerries.Remove(f);
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
        clone.ReturnHomeWhenLost = true;
        p.Leader.GainFollower(clone.Follower);
        clone.hadLeader = true;
        clone.Depth = -1000000;
        clone.Position = p.Center;
    }

    public virtual FlagCollectBerry Clone() {
        return new(data, levelOffset, ID);
    }

    private IEnumerator routineReturn() {
        //patch to avoid spam collecting on lose flag
        yield return 0.5f;
        Collidable = true;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        GameHelper.Instance.Session.StoredBerries ??= new();
        foreach(FlagCollectBerry f in GameHelper.Instance.Session.StoredBerries) {
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
        GameHelper.Instance.Session.StoredBerries ??= new();
        foreach(FlagCollectBerry f in GameHelper.Instance.Session.StoredBerries) {
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