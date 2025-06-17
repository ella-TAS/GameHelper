using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class EntityIdComponent(int id) : Component(false, false) {
    public static readonly List<string> ignoreList = ["TAS.EverestInterop.InfoHUD.SelectedAreaEntity", "Monocle.Entity"];

    public int ID { get; private set; } = id;

    public override void Added(Entity entity) {
        base.Added(entity);
        if(ignoreList.Contains(entity.GetType().ToString())) {
            RemoveSelf();
            return;
        }
        if(entity.Components.GetAll<EntityIdComponent>().Count() == 1) {
            // Logger.Info("GameHelper", "ID " + ID + " assigned to " + entity.GetType());
        } else {
            RemoveSelf();
            // Logger.Warn("GameHelper", "double assignment to " + entity.GetType() + " ID " + entity.Get<EntityIdComponent>().ID + " ID " + ID);
        }
    }
}