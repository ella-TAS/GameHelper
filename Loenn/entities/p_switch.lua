local pswitch = {}

pswitch.name = "GameHelper/PSwitch"
pswitch.depth = 8998
pswitch.texture = "objects/GameHelper/p_switch/normal00"
pswitch.justification = {0.5, 1.0}
pswitch.placements = {
    name = "normal",
    data = {
        showTutorial = false,
        stationary = false,
        flag = "pswitch",
        flagDuration = 7
    }
}

return pswitch