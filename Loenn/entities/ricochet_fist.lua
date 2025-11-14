local fist = {}

fist.name = "GameHelper/RicochetFist"
fist.depth = 8998
fist.texture = "objects/GameHelper/ricochet_fist"
fist.justification = { 0.0, 0.0 }
fist.nodeLimits = { 1, 1 }
fist.placements = {
    name = "fist",
    data = {
        speed = 100.0,
        sprite = "objects/GameHelper/ricochet_fist"
    }
}

return fist
