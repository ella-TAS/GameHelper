local hole = {}

hole.name = "GameHelper/MouseHole"
hole.depth = 8998
hole.justification = {0.0, 0.0}
hole.placements = {
    {
        name = "hole_spawner",
        data = {
            spawner = true,
            flag = "mice",
            resetFlagOnDeath = false
        }
    },
    {
        name = "hole_exit",
        data = {
            spawner = false,
            flag = "mice",
            resetFlagOnDeath = true
        }
    }
}

function hole.texture(room, entity)
    return "objects/GameHelper/mouse_puzzle/" .. (entity.spawner and "hole_closed" or "exit")
end

return hole