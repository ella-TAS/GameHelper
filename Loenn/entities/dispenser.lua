local dispenser = {}

dispenser.name = "GameHelper/Dispenser"
dispenser.depth = 8998
dispenser.texture = "objects/GameHelper/dispenser/dispenser"
dispenser.placements = {
    name = "dispenser",
    data = {
        flag = "dispenser",
        faceLeft = false
    }
}

function dispenser.scale(room, entity)
    return {entity.faceLeft and -1 or 1, 1}
end

function dispenser.justification(room, entity)
    return {entity.faceLeft and 1 or 0, 0}
end

return dispenser