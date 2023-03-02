local pigarithm = {}

local spriteOptions = {
    Small = "pigarithm_small",
    Medium = "pigarithm_medium",
    Big = "pigarithm_big"
}

pigarithm.name = "GameHelper/Pigarithm"
pigarithm.depth = 8998
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
            flipSprite = false,
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

function pigarithm.justification(room, entity)
    return (entity.sprite == "pigarithm_small" ? {0.28, 0.0} : (entity.sprite == "pigarithm_medium" ? {0.18, 0.0} : {0.14, 0.0}))
end

function pigarithm.texture(room, entity)
    return "objects/GameHelper/pigarithm/" .. (entity.sprite == "pigarithm_small" ? "small" : (entity.sprite == "pigarithm_medium" ? "medium" : "big")) .. "_idle"
end

return pigarithm