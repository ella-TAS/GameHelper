using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Utils;

public abstract class Wrapper(Vector2 position) : Entity(position) {
    public List<Entity> FindTargets(Vector2 node, Vector2[] nodes, Vector2 nodeOffset, bool allEntities, string onlyType) {
        List<Entity> entities = new();
        //don't look for entity if allEntities and type is set
        Entity targetEntity = null;
        if (!allEntities || onlyType?.Length == 0) {
            targetEntity = FindNearest(node, onlyType);
        }

        if (allEntities) {
            foreach (Entity e in SceneAs<Level>().Entities) {
                if ((onlyType?.Length == 0 && e.GetType() == targetEntity?.GetType()) || e.GetType().FullName == onlyType || e.GetType().Name == onlyType) {
                    entities.Add(e);
                }
            }
        } else {
            entities.Add(targetEntity);
            foreach (Vector2 n in nodes) {
                entities.Add(FindNearest(n + nodeOffset, onlyType));
            }
        }
        return entities;
    }

    public Entity FindNearest(Vector2 pos, string type, Entity notEntity = null) {
        Entity entity = null;
        float minDistance = float.MaxValue;
        foreach (Entity e in SceneAs<Level>().Entities) {
            bool typeCorrect = e.GetType().FullName == type || e.GetType().Name == type;
            if (
                e != notEntity &&
                e is not Wrapper &&
                e is not TrailManager &&
                (typeCorrect || e is not Player || e is not Trigger) &&
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
        foreach (T e in SceneAs<Level>().Entities.FindAll<T>()) {
            if (Vector2.Distance(e.Center, pos) < minDistance) {
                entity = e;
                minDistance = Vector2.Distance(e.Center, pos);
            }
        }
        return (T) entity;
    }

    public Entity FindById(int id) {
        foreach (Entity e in SceneAs<Level>().Entities) {
            if (e.SourceId.ID == id) {
                return e;
            }
        }

        return null;
    }

    public void ComplainEntityNotFound(string wrapperName) {
        Logger.Warn("GameHelper", wrapperName + " found no target in room " + SceneAs<Level>().Session.LevelData.Name);
        RemoveSelf();
    }
}