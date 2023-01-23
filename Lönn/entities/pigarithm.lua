local pigarithm = {}

pigarithm.name = "GameHelper/Pigarithm"
pigarithm.depth = 8998
pigarithm.texture = "objects/GameHelper/pigarithm/idle"
pigarithm.justification = {0.0, 0.0}
pigarithm.minimumSize = {24, 24}
pigarithm.placements = {
    name = "pigarithm",
    data = {
        width = 24,
        height = 24,
        speed = 60.0,
        startRight = true
    }
}

return pigarithm