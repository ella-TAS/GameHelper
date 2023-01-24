local trampoline = {}

trampoline.name = "GameHelper/Trampoline"
trampoline.depth = 8998
trampoline.texture = "objects/GameHelper/trampoline/idle"
trampoline.placements = {
    name = "trampoline",
    data = {
        speedBoostX = 60.0,
        speedBoostY = 60.0,
        facingUpLeft = true,
        refillDash = true,
        oneUse = false
    }
}

--function trampoline.justification(room, entity)
--    return entity.facingUpLeft and {1.0, 0.5} or {0.0, 0.5}
--end

function trampoline.rotation(room, entity)
    return entity.facingUpLeft and 0 or math.pi / 2
end

return trampoline