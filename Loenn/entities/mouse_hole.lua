local hole = {}

hole.name = "GameHelper/MouseHole"
hole.depth = 8998
hole.justification = {0.0, 0.0}
hole.placements = {
    {
        name = "hole_spawner",
        data = {
            spawner = true,
            flag = "mice"
        }
    },
    {
        name = "hole_exit",
        data = {
            spawner = false,
            flag = "mice"
        }
    }
}

function hole.texture(room, entity)
    return "objects/GameHelper/mouse_hole/" .. entity.spawner ? "hole_closed" : "exit"
end

return hole