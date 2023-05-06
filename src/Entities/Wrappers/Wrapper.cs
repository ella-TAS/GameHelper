using Monocle;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

public class Wrapper : Entity {
    private static bool RoomLogged;

    public Wrapper(Vector2 position) : base(position) { }

    public List<Entity> FindTargets(Vector2 node, Vector2[] nodes, Vector2 nodeOffset, bool allEntities, string onlyType) {
        List<Entity> entities = new List<Entity>();
        //don't look for entity if allEntities and type is set
        Entity targetEntity = null;
        if(!allEntities || onlyType == "") {
            targetEntity = FindNearest(Position, onlyType);
        }

        if(allEntities) {
            foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
                if((onlyType == "" && e.GetType() == targetEntity.GetType()) ||
                e.GetType().ToString() == onlyType) {
                    entities.Add(e);
                }
            }
        } else {
            entities.Add(targetEntity);
            foreach(Vector2 n in nodes) {
                entities.Add(FindNearest(n + nodeOffset, onlyType));
            }
        }
        return entities;
    }

    public Entity FindNearest(Vector2 pos, string type) {
        Entity entity = null;
        float minDistance = float.MaxValue;
        foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
            if(!(e is Wrapper) && !(e is Player) &&
            (type == "" || e.GetType().ToString() == type) &&
            Vector2.Distance(e.Position, pos) < minDistance) {
                entity = e;
                minDistance = Vector2.Distance(e.Position, pos);
            }
        }
        return entity;
    }

    public T FindNearest<T>(Vector2 pos) where T : Entity {
        Entity entity = null;
        float minDistance = float.MaxValue;
        foreach(Entity e in SceneAs<Level>().Entities.FindAll<T>()) {
            if(Vector2.Distance(e.Position, pos) < minDistance) {
                entity = e;
                minDistance = Vector2.Distance(e.Position, pos);
            }
        }
        return entity as T;
    }

    public void ComplainEntityNotFound(string wrapperName) {
        Logger.Log(LogLevel.Warn, "GameHelper", wrapperName + " found no target in room " + SceneAs<Level>().Session.LevelData.Name);
        RemoveSelf();
    }

    public void LogAllEntities() {
        //only do so once per room
        if(RoomLogged) {
            return;
        }
        Logger.Log("GameHelper", "List of all entities in the room:");
        foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
            Logger.Log("GameHelper", e.GetType().ToString());
        }
        RoomLogged = true;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        RoomLogged = false;
    }
}