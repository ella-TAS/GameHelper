module GameHelperEnemy

using ..Ahorn, Maple

@mapdef Entity "GameHelper/Enemy" Enemy(
    x::Integer, y::Integer, speedX::Number=50.0, speedY::Number=200.0,
    hitboxWidth::Number=8.0, hitboxHeight::Number=16.0,
    hitboxXOffset::Number=-3.0, hitboxYOffset::Number=0.0,
    bounceHitboxWidth::Number=8.0, bounceHitboxHeight=4.0,
    bounceHitboxXOffset::Number=-3.0, bounceHitboxYOffset::Number=-3.0,
    fallSpeedLimit::Number=400.0, canDie::Bool=true, drawOutline::Bool=false,
    customSpritePath::String="objects/GameHelper/Enemy"
)

const placements = Ahorn.PlacementDict(
    "Enemy (GameHelper)" => Ahorn.EntityPlacement(
        Enemy,
        "rectangle"
    ),
)

function Ahorn.selection(entity::Enemy)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x, y, 16, 16)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Enemy, room::Maple.Room)
customSpritePath = get(entity.data,"customSpritePath", "objects/GameHelper/Enemy")
if customSpritePath == ""
    customSpritePath = "objects/GameHelper/Enemy"
end
Ahorn.drawSprite(ctx, "$(customSpritePath)/walking00.png", 16, 8)
end

end