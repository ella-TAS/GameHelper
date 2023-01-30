local pigarithm = {}

local spriteOptions = {
    Small = "pigarithm_small",
    Medium = "pigarithm_medium",
    Big = "pigarithm_big"
}

pigarithm.name = "GameHelper/Pigarithm"
pigarithm.depth = 8998
pigarithm.justification = {0.14, 0.0}
pigarithm.minimumSize = {16, 16}
pigarithm.maximumSize = {48, 32}
pigarithm.canResize = {true, true}
pigarithm.placements = {
    {
        name = "pigarithm",
        data = {
            width = 48,
            height = 32,
            sprite = "pigarithm_big",
            speed = 60.0,
            startRight = true,
            kill = true,
            flag = ""
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
    return "objects/GameHelper/pigarithm/" .. (entity.sprite == "pigarithm_small" ? "small" : (entity.sprite == "pigarithm_medium" ? "medium" : "big")) .. "_idle"
end

return pigarithm