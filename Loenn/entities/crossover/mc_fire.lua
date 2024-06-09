local fire = {}

local directions = {
    up = "0",
    right = "1",
    down = "2",
    left = "3"
}

fire.name = "GameHelper/McFire"
fire.depth = -9999999
fire.texture = "objects/GameHelper/mc_fire/fire00"
fire.placements = {
    name = "fire",
    data = {
        spreadingTime = 5.0,
        direction = "0"
    }
}
fire.fieldInformation = {
    direction = {
        options = directions,
        editable = false
    }
}

function fire.rotation(room, entity)
    return 0.5 * 3.141592653589793238462643383 * tonumber(entity.direction)
end

return fire