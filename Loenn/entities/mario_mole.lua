local mole = {}

mole.name = "GameHelper/MarioMole"
mole.depth = 8998
mole.texture = "objects/GameHelper/mario_mole/mole00"
mole.minimumSize = { 16, 16 }
mole.maximumSize = { 48, 32 }
mole.canResize = { false, false }
mole.placements = {
    name = "mole",
    data = {
        width = 24,
        height = 32,
        speed = 60.0,
        startRight = true,
        kill = true,
        gravity = true,
        flipSprite = false,
        flag = ""
    }
}

function mole.justification(room, entity)
    local y = entity.flipSprite and 1.0 or 0.0
    return entity.startRight and { 0.1, y } or { 0.9, y }
end

function mole.scale(room, entity)
    return { entity.startRight and 1 or -1, entity.flipSprite and -1 or 1 }
end

return mole
