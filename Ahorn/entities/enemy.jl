module GameHelperEnemy

using ..Ahorn, Maple

@mapdef Entity "GameHelper/Enemy" Enemy(
    x::Integer, y::Integer, speedX::Number=50.0, speedY::Number=200.0
)

const placements = Ahorn.PlacementDict(
    "Enemy (GameHelper)" => Ahorn.EntityPlacement(
        Enemy
    )
)

function Ahorn.selection(entity::Enemy)
    x, y = Ahorn.position(entity)
    sprite = "objects/GameHelper/Enemy/walking00.png"
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Enemy, room::Maple.Room)
theSprite = "objects/GameHelper/Enemy/walking00.png"
sprite = theSprite
Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end