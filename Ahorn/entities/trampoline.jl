module GameHelperTrampoline

using ..Ahorn, Maple

@mapdef Entity "GameHelper/Trampoline" Trampoline(
    x::Integer, y::Integer, speedBoost::Number=40.0, facingUpLeft::Bool=true, refillDash::Bool=true, oneUse::Bool=false
)

const placements = Ahorn.PlacementDict(
    "Trampoline (GameHelper)" => Ahorn.EntityPlacement(
        Trampoline
    )
)

function Ahorn.selection(entity::Trampoline)
    x, y = Ahorn.position(entity)
    sprite = "objects/GameHelper/trampoline/idle.png"
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Trampoline, room::Maple.Room)
theSprite = "objects/GameHelper/trampoline/idle.png"
sprite = theSprite
Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end