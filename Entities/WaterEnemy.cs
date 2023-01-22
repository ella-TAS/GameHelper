//KoseiDiamond
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/WaterEnemy")]
public class WaterEnemy : Actor
{
    private Level level;
    private Sprite sprite;
    public Vector2 speed = new Vector2(0f, 0f);
    public Actor actor;
    private float speedX = 50f;
    private float speedY = 50f;
    private bool left = true;
    private bool down = true;
    private bool dead = false;
    private Collider bounceCollider;
    private float noGravityTimer;
    public Holdable Hold;
    public Solid solid;
    public Vector2 previousPosition = new Vector2(0f, 0f); // this will be used to go back to previous position if somehow it gets out of water

    // Constructor
    public WaterEnemy(EntityData data, Vector2 offset)
    : base(data.Position + offset)
    {
        base.Collider = new Hitbox(46f, 12f, -22f, -2f);
        Position = data.Position + offset;
        previousPosition = Position;
        speedX = data.Float("speedX");
        speedY = data.Float("speedY");
        Add(new PlayerCollider(OnPlayer));
    }

    //Adds the entity then the room loads
    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        Add(sprite = GameHelperModule.getSpriteBank().Create("swimmingEnemy"));
    }

    //Kills you if you touch it, and then it disappears
    private void OnPlayer(Player player)
    {
        player.Die(new Vector2(-1f, 0f));
    }

    public override void Update()
    {
        base.Update();
        Player player = Scene.Tracker.GetEntity<Player>();
        moveTowardsPlayer(player);
        /*if (base.Top > (float)SceneAs<Level>().Bounds.Bottom)
        {
            Die();
        }*/
        sprite.FlipX = !(left);
        if (SwimCheck() == true)
        {
        previousPosition = Position; // If it's on water, previousPosition updates
        }
        else
        {
            Position = previousPosition; //If it's not on water, goes back to previousPosition
        }
    }

    private void moveTowardsPlayer (Player player)
    {
        //Acceleration + moving one pixel at a time
        if (player != null && SwimCheck())
        {
            float targetPositionX = Calc.Approach(ExactPosition.X, player.Position.X, speedX * Engine.DeltaTime);
            float toX = ExactPosition.X;
            while (toX != targetPositionX)
            {
                toX = Calc.Approach(toX, targetPositionX, 2f);
            }
            if ((ExactPosition.X - player.Position.X) > 0)
                left = true;
            else
                left = false;
            //Actually move
            MoveToX(toX);
            //Same but for the Y coordinates
            float targetPositionY = Calc.Approach(ExactPosition.Y, player.Position.Y, speedY * Engine.DeltaTime);
            float toY = ExactPosition.Y;
            while (toY != targetPositionY)
            {
                toY = Calc.Approach(toY, targetPositionY, 1f);
            }
            if ((ExactPosition.Y - player.Position.Y) > 0)
                down = true;
            else
                down = false;
            //Actually move
            MoveToY(toY);
        }
    }

    //Draws an outline around the sprite
    /*public override void Render()
    {
        if (sprite != null)
            sprite.DrawOutline();

        base.Render();
    }*/

    public void Die()
    {
        if (!dead)
        {
            sprite.RemoveSelf();
            Audio.Play("event:/char/madeline/death", Position);
            Collidable = false;
            /*Add(new DeathEffect(Calc.HexToColor("212121"), base.Center - Position));
            this.RemoveSelf();*/

            Entity entity = new Entity(Position);
            DeathEffect component = new DeathEffect(Calc.HexToColor("585e58"), base.Center - Position)
            {
                OnEnd = delegate
                {
                    entity.RemoveSelf();
                }
            };
            entity.Add(component);
            entity.Depth = -1000000;
            base.Scene.Add(entity);
            RemoveSelf();

            dead = true;
        }
    }

    private bool SwimCheck()
    {
        if (CollideCheck<Water>(Position + Vector2.UnitY * -8f))
        {
            return CollideCheck<Water>(Position);
        }
        return false;
    }
}