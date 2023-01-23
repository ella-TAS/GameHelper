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
    //Some extra variables to make the entity more customizable
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

    // Constructor
    public Enemy(EntityData data, Vector2 offset)
    : base(data.Position + offset)
    {
        base.Collider = new Hitbox(hitboxWidth, hitboxHeight, hitboxXOffset, hitboxYOffset);
        bounceCollider = new Hitbox(bounceHitboxWidth, bounceHitboxHeight, bounceHitboxXOffset, bounceHitboxYOffset);
        Position = data.Position + offset;
        speedX = data.Float("speedX");
        speedY = data.Float("speedY");
        customSpritePath = data.Attr("sprite", "");
        Add(new PlayerCollider(OnPlayer));
        Add(new PlayerCollider(OnPlayerBounce, bounceCollider));
    }

    //Adds the entity when the room loads
    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        //Add(sprite = GameHelperModule.getSpriteBank().Create("enemy"));
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
        moveTowardsPlayer(player);
        if (base.Top > (float)SceneAs<Level>().Bounds.Bottom)
        {
            Die();
        }
        sprite.FlipX = !(left);
    }

    private void moveTowardsPlayer (Player player)
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
            float falling = Calc.Approach(speedY, (float)SceneAs<Level>().Bounds.Bottom, speedY * Engine.DeltaTime);
            float toY = ExactPosition.Y;
            toY = Calc.Approach(toY, 400f, 2f);
            MoveToY(toY);
            }
        }
    }

    //Draws an outline around the sprite
    public override void Render()
    {
        if (sprite != null && drawOutline)
            sprite.DrawOutline();
        //Something about this is not working and if there's some custom path it does weird things :(
        if (customSpritePath == "" || GFX.Game["objects/" + customSpritePath].get_AtlasPath() == null)
        {
            GFX.Game["objects/GameHelper/Enemy/walking00"].DrawCentered(Position);
        }
        else
        {
            GFX.Game["objects/" + customSpritePath].DrawCentered(Position);
        }
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
