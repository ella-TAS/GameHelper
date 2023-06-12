local pushboxbutton = {}

pushboxbutton.name = "GameHelper/PushBoxButton"
pushboxbutton.depth = 8998
pushboxbutton.justification = {0.0, 0.0}
pushboxbutton.texture = "objects/GameHelper/push_box_button/idle"
pushboxbutton.placements = {
    name = "pushboxbutton",
    data = {
        flag = "button",
        playerActivates = true,
        resetFlagOnDeath = true
    }
}

return pushboxbutton