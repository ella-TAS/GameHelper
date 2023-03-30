local rotator = {}

rotator.name = "GameHelper/MouseRotator"
rotator.depth = 8998
rotator.texture = "objects/GameHelper/chainsaw/idle00"
rotator.justification = {0.0, 0.0}
rotator.placements = {
    name = "rotator",
    data = {
        clockwise = false
    }
}

function rotator.texture(room, entity)
    return "objects/GameHelper/mouse_puzzle/rotator_" .. (entity.clockwise and "left" or "right")
end

return rotator