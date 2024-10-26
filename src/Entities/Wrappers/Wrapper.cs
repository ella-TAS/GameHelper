using Monocle;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

public class Wrapper : Entity {
    private static bool RoomLogged;

    public Wrapper(Vector2 position) : base(position) { }

    public List<Entity> FindTargets(Vector2 node, Vector2[] nodes, Vector2 nodeOffset, bool allEntities, string onlyType) {
        List<Entity> entities = new();
        //don't look for entity if allEntities and type is set
        Entity targetEntity = null;
        if(!allEntities || onlyType?.Length == 0) {
            targetEntity = FindNearest(node, onlyType);
        }

        if(allEntities) {
            foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
                if((onlyType?.Length == 0 && e.GetType() == targetEntity?.GetType()) ||
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
            bool typeCorrect = e.GetType().ToString() == type;
            if(
                e is not Wrapper &&
                (e is not Player || typeCorrect) &&
                (type?.Length == 0 || typeCorrect) &&
                Vector2.Distance(e.Center, pos) < minDistance
            ) {
                entity = e;
                minDistance = Vector2.Distance(e.Center, pos);
            }
        }
        return entity;
    }

    public T FindNearest<T>(Vector2 pos) where T : Entity {
        Entity entity = null;
        float minDistance = float.MaxValue;
        foreach(T e in SceneAs<Level>().Entities.FindAll<T>()) {
            if(Vector2.Distance(e.Center, pos) < minDistance) {
                entity = e;
                minDistance = Vector2.Distance(e.Center, pos);
            }
        }
        return (T) entity;
    }

    public void ComplainEntityNotFound(string wrapperName) {
        Logger.Warn("GameHelper", wrapperName + " found no target in room " + SceneAs<Level>().Session.LevelData.Name);
        RemoveSelf();
    }

    public void LogAllEntities() {
        //only do so once per room
        if(RoomLogged) {
            return;
        }
        Logger.Info("GameHelper", "List of all entities in the room:");
        foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
            Logger.Info("GameHelper", e.GetType().ToString());
        }
        RoomLogged = true;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        RoomLogged = false;
    }
}