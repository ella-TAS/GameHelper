local shield = {}

shield.name = "GameHelper/ShieldTrigger"
shield.depth = 8998
shield.justification = { 0.0, 0.0 }
shield.placements = {
    name = "shield",
    data = {
        flashAmount = 3,
        enable = true
    }
}
shield.fieldInformation = {
    flashAmount = {
        fieldType = "integer"
    }
}

return shield
