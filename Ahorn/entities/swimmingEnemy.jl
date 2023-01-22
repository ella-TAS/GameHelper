module GameHelperWaterEnemy

using ..Ahorn, Maple

@mapdef Entity "GameHelper/WaterEnemy" WaterEnemy(
    x::Integer, y::Integer, speedX::Number=50.0, speedY::Number=50.0
)

const placements = Ahorn.PlacementDict(
    "Water enemy (GameHelper)" => Ahorn.EntityPlacement(
        WaterEnemy
    )
)

function Ahorn.selection(entity::WaterEnemy)
    x, y = Ahorn.position(entity)
    sprite = "objects/GameHelper/SwimmingEnemy/swimming00.png"
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::WaterEnemy, room::Maple.Room)
theSprite = "objects/GameHelper/SwimmingEnemy/swimming00.png"
sprite = theSprite
Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end