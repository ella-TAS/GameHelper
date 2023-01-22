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
    private bool dead = false;
    private Collider bounceCollider;
    private float noGravityTimer;
    public Holdable Hold;
    public Solid solid;
    public Vector2 previousPosition = new Vector2(0f, 0f); // this will be used to go back to previous position if somehow it gets out of water
    public Vector2 originalPosition = new Vector2(0f, 0f); // this will be used to go back to the original position if the player is not inside the water

    // Constructor
    public WaterEnemy(EntityData data, Vector2 offset)
    : base(data.Position + offset)
    {
        base.Collider = new Hitbox(46f, 12f, -22f, -8f);
        Position = data.Position + offset;
        previousPosition = Position;
        originalPosition = Position;
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
        if (player != null)
        {
            if ((ExactPosition.X - player.Position.X) > 0) //This is to flip the sprite
                left = true;
            else
                left = false;
            //Acceleration + moving one pixel at a time
            if (SwimCheck() && playerOnWaterCheck())
            {
                float targetPositionX = Calc.Approach(ExactPosition.X, player.Position.X, speedX * Engine.DeltaTime);
                float toX = ExactPosition.X;
                while (toX != targetPositionX)
                {
                    toX = Calc.Approach(toX, targetPositionX, 2f);
                }
                //Actually move
                MoveToX(toX);
                //Same but for the Y coordinates
                float targetPositionY = Calc.Approach(ExactPosition.Y, player.Position.Y, speedY * Engine.DeltaTime);
                float toY = ExactPosition.Y;
                while (toY != targetPositionY)
                {
                    toY = Calc.Approach(toY, targetPositionY, 1f);
                }
                //Actually move
                MoveToY(toY);
            }
            else//Same but for going back to the originalPosition if you're outside of water
            {
                if ((ExactPosition.X - originalPosition.X) > 0) //This is to flip the sprite
                    left = true;
                else
                    left = false;
                float backToX = Calc.Approach(ExactPosition.X, originalPosition.X, speedX * Engine.DeltaTime);
                float goesBackToX = ExactPosition.X;
                while (goesBackToX != backToX)
                {
                    goesBackToX = Calc.Approach(goesBackToX, backToX, 1f);
                }
                MoveToX(goesBackToX);

                float backToY = Calc.Approach(ExactPosition.Y, originalPosition.Y, speedY * Engine.DeltaTime);
                float goesBackToY = ExactPosition.Y;
                while (goesBackToY != backToY)
                {
                    goesBackToY = Calc.Approach(goesBackToY, backToY, 1f);
                }
                MoveToY(goesBackToY);
            }
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

    private bool playerOnWaterCheck()
    {
        Player player = Scene.Tracker.GetEntity<Player>(); // Searchs for Madeline inside the room
        if (CollideCheck<Water>(player.Position + Vector2.UnitY * -8f)) //And checks if she's inside water
        {
            return CollideCheck<Water>(Position);
        }
        return false;
    }
}