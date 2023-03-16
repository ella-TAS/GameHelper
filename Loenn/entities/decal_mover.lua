local moving_decal = {}

moving_decal.name = "GameHelper/DecalMover"
moving_decal.depth = -9999999
moving_decal.texture = "loenn/GameHelper/decal_mover"
moving_decal.justification = {0.0, 0.0}
moving_decal.nodeLimits = {1, 999}
moving_decal.placements = {
    name = "moving_decal",
    data = {
        speed = 60.0,
        flag = "",
        loop = true
    }
}

return moving_decal