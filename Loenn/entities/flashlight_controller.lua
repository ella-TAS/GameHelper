local flc = {}

flc.name = "GameHelper/FlashlightController"
flc.depth = -9999999
flc.texture = "loenn/GameHelper/flashlight_controller"
flc.justification = {0.0, 0.0}
flc.fieldInformation = {
    cooldown = {
        fieldType = "integer",
    }
}
flc.placements = {
    name = "flc",
    data = {
        fadeSpeed = 0.03,
        cooldown = 180
    }
}

return flc