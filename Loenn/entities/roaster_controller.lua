local rc = {}

rc.name = "GameHelper/RoasterController"
rc.depth = -9999999
rc.texture = "loenn/GameHelper/roaster_controller"
rc.justification = {0.0, 0.0}
rc.fieldInformation = {
    timer = {
        fieldType = "integer"
    }
}
rc.placements = {
    name = "rc",
    data = {
        timer = 90,
        flag = "roasted",
        OnlyExtinguishInWater = false
    }
}

return rc