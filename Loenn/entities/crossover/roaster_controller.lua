local rc = {}

rc.name = "GameHelper/RoasterController"
rc.depth = -9999999
rc.texture = "loenn/GameHelper/roaster_controller"
rc.justification = {0.0, 0.0}
rc.placements = {
    name = "rc",
    data = {
        timer = 1.5,
        flag = "roasted",
        OnlyExtinguishInWater = false
    }
}

return rc