local pigarithm = {}

local spriteOptions = {
    Small = "pigarithm_small",
    Medium = "pigarithm_medium",
    Big = "pigarithm_big"
}

pigarithm.name = "GameHelper/Pigarithm"
pigarithm.depth = 8998
pigarithm.justification = {0.0, 0.0}
pigarithm.placements = {
    {
        name = "pigarithm",
        data = {
            width = 48,
            height = 32,
            sprite = "pigarithm_big",
            speed = 60.0,
            startRight = true
        }
    }
}
pigarithm.fieldInformation = {
    sprite = {
        options = spriteOptions,
        editable = false
    }
}

function pigarithm.texture(room, entity)
    return "objects/GameHelper/pigarithm/small_idle" and entity.sprite == "pigarithm_small" or "objects/GameHelper/pigarithm/medium_idle" and entity.size == "pigarithm_medium" or "objects/GameHelper/pigarithm/big_idle"
end

return pigarithm