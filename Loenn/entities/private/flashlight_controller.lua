local flc = {}

flc.name = "GameHelper/FlashlightController"
flc.depth = -999999999
flc.texture = "loenn/GameHelper/flashlight_controller"
flc.justification = { 0.0, 0.0 }
flc.placements = {
    name = "flc",
    data = {
        fadeTime = 1,
        cooldown = 3
    }
}

return flc
