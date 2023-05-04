using Monocle;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

public class Wrapper : Entity {
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
            if(!(e is Wrapper) &&
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
        foreach(Entity e in SceneAs<Level>().Entities.FindAll<Decal>()) {
            if(Vector2.Distance(e.Position, pos) < minDistance) {
                entity = e;
                minDistance = Vector2.Distance(e.Position, pos);
            }
        }
        return entity as T;
    }

    public string EntityStamp(Entity e) {
        return e.GetType().ToString() + " [" + SceneAs<Level>().Session.LevelData.Name + "]";
    }

    public void ComplainEntityNotFound(string wrapperName) {
        Logger.Log(LogLevel.Warn, "GameHelper", wrapperName + " found no target in room " + SceneAs<Level>().Session.LevelData.Name);
        RemoveSelf();
    }

    public void LogAllEntities() {
        Logger.Log("GameHelper", "List of all entities in the room:");
        foreach(Entity e in SceneAs<Level>().Entities.FindAll<Entity>()) {
            Logger.Log("GameHelper", EntityStamp(e));
        }
    }
}