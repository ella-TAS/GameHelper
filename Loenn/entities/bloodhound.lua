local bloodhound = {}

bloodhound.name = "GameHelper/Bloodhound"
bloodhound.depth = -9999999
bloodhound.texture = "loenn/GameHelper/flashlight_controller"
bloodhound.justification = {0.0, 0.0}
bloodhound.fieldInformation = {
    moveRange = {
        fieldType = "integer",
    }
}
bloodhound.placements = {
    name = "bloodhound",
    data = {
        moveRange = 48,
        horizontal = true
    }
}

return bloodhound