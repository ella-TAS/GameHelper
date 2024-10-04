local pushbox = {}

pushbox.name = "GameHelper/PushBox"
pushbox.depth = 8999
pushbox.fillColor = {251/255, 185/255, 84/255}
pushbox.borderColor = {229/255, 129/255, 37/255}
pushbox.justification = {0.0, 0.0}
pushbox.minimumSize = {16, 16}
pushbox.canResize = {true, true}
pushbox.placements = {
    name = "pushbox",
    data = {
        width = 16,
        height = 16,
        speedX = 30.0
    }
}

return pushbox