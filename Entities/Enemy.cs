//KoseiDiamond
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Enemy")]
public class Enemy : Actor
{
    private Level level;
    private Sprite sprite;
    public Vector2 speed = new Vector2(0f, 0f);
    public Actor actor;
    private float speedX = 50f;
    private float speedY = 200f;
    private bool left = true;
    private bool dead = false;
    private Collider bounceCollider;
    public Holdable Hold;
    public Solid solid;
    public float hitboxWidth = 8;
    public float hitboxHeight = 16;
    public float hitboxXOffset = -3;
    public float hitboxYOffset = 0;
    public float bounceHitboxWidth = 8;
    public float bounceHitboxHeight = 4;
    public float bounceHitboxXOffset = -3;
    public float bounceHitboxYOffset = -3;
    public bool canDie = true;
    public string customSpritePath;
    public bool drawOutline = false;
    public float fallSpeedLimit = 400;

    // Constructor
    public Enemy(EntityData data, Vector2 offset)
    : base(data.Position + offset)
    {
        base.Collider = new Hitbox(hitboxWidth, hitboxHeight, hitboxXOffset, hitboxYOffset);
        bounceCollider = new Hitbox(bounceHitboxWidth, bounceHitboxHeight, bounceHitboxXOffset, bounceHitboxYOffset);
        Position = data.Position + offset;
        speedX = data.Float("speedX");
        speedY = data.Float("speedY");
        fallSpeedLimit = data.Float("fallSpeedLimit");
        customSpritePath = (string.IsNullOrEmpty(data.Attr("customSpritePath")) ? "objects/GameHelper/Enemy" : data.Attr("customSpritePath")); // If no path is introduced, default sprite, else, customSpritePath
        Add(sprite = new Sprite(GFX.Game, customSpritePath + "/"));
        sprite.AddLoop("walking", "walking", 0.08f);
        sprite.Play("walking");
        Add(new PlayerCollider(OnPlayer));
        Add(new PlayerCollider(OnPlayerBounce, bounceCollider));
    }

    //Adds the entity when the room loads
    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
    }

    //Kills you if you touch it, and then it disappears
    private void OnPlayer(Player player)
    {
        player.Die(new Vector2(-1f, 0f));
    }

    //Method to bounce on it and kill it
    private void OnPlayerBounce(Player player)
    {
        Celeste.Freeze(0.1f);
        player.Bounce(base.Top - 2f);
        Die();

    }

    //Every frame this code is run
    public override void Update()
    {
        base.Update();
        Player player = Scene.Tracker.GetEntity<Player>();
        moving(player);
        if (base.Top > (float)SceneAs<Level>().Bounds.Bottom)
        {
            Die();
        }
        sprite.FlipX = !(left);
    }

    private void moving (Player player)
    {
        //Acceleration + moving one pixel at a time
        if (player != null)
        {
            float targetPosition = Calc.Approach(ExactPosition.X, player.Position.X, speedX * Engine.DeltaTime);
            float toX = ExactPosition.X;
            while (toX != targetPosition)
            {
                toX = Calc.Approach(toX, targetPosition, 2f);
            }
            if ((ExactPosition.X - player.Position.X) > 0)
                left = true;
            else
                left = false;
            //Actually move
                MoveToX(toX);
            //Falls to the bottom of the screen if it's not on ground
            if (!OnGround(new Vector2(toX, ExactPosition.Y)))
            {
                // velY = Calc.Approach(velY, fallCap, gravity)
                float falling = Calc.Approach(ExactPosition.Y, fallSpeedLimit, - speedY * Engine.DeltaTime);
                float toY = ExactPosition.Y;
                /*while (toY != falling)
                {
                    toY = Calc.Approach(toY, falling, 2f);
                }*/
                //MoveToY(toY);
                MoveToY(falling);
            }
        }
    }

    //Draws an outline around the sprite
    public override void Render()
    {
        if (sprite != null && drawOutline)
            sprite.DrawOutline();
        base.Render();
    }

    //Creates the death effect and plays a sound before disappearing
    public void Die()
    {
        if (!dead && canDie == true)
        {
            sprite.RemoveSelf();
            Audio.Play("event:/char/madeline/death", Position);
            Collidable = false;
            //Add(new DeathEffect(Calc.HexToColor("212121"), base.Center - Position));
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

    //Idk how to do this tbh
    public void Jump()
    {
    }

    //This is not working either
    public bool HitSpring (Spring spring)
    {
        if (spring.Orientation == Spring.Orientations.Floor && speed.Y >= 0f)
        {
            MoveTowardsX(spring.CenterX, 4f);
            speed.Y = -160f;
            return true;
        }
        else
            return false;
    }
}
