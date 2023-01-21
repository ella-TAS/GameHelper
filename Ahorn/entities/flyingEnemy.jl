module GameHelperFlyingEnemy

using ..Ahorn, Maple

@mapdef Entity "GameHelper/FlyingEnemy" FlyingEnemy(
    x::Integer, y::Integer, speedX::Number=50.0, speedY::Number=200.0
)

const placements = Ahorn.PlacementDict(
    "Flying enemy (GameHelper)" => Ahorn.EntityPlacement(
        FlyingEnemy
    )
)

function Ahorn.selection(entity::FlyingEnemy)
    x, y = Ahorn.position(entity)
    sprite = "objects/GameHelper/FlyingEnemy/flying00.png"
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FlyingEnemy, room::Maple.Room)
theSprite = "objects/GameHelper/FlyingEnemy/flying00.png"
sprite = theSprite
Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end