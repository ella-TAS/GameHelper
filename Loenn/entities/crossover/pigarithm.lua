local pigarithm = {}

local spriteOptions = {
    Small = "pigarithm_small",
    Medium = "pigarithm_medium",
    Big = "pigarithm_big",
    MegaMole = "pigarithm_mole"
}

pigarithm.name = "GameHelper/Pigarithm"
pigarithm.depth = 8998
pigarithm.minimumSize = {16, 16}
pigarithm.maximumSize = {48, 32}
pigarithm.canResize = {true, true}
pigarithm.fieldInformation = {
    sprite = {
        options = spriteOptions,
        editable = false
    }
}
pigarithm.placements = {
    name = "pigarithm",
    data = {
        width = 48,
        height = 32,
        sprite = "pigarithm_big",
        speed = 60.0,
        startRight = true,
        kill = true,
        gravity = false,
        flipSprite = false,
        flag = ""
    }
}

function pigarithm.justification(room, entity)
    if entity.sprite == "pigarithm_small" then
        return {0.28, 0.0}
    elseif entity.sprite == "pigarithm_medium" then
        return {0.18, 0.0}
    elseif entity.sprite == "pigarithm_big" then
        return {0.14, 0.0}
    else
        return {0.0, 0.0}
    end
end

function pigarithm.texture(room, entity)
    local sprite = string.sub(entity.sprite, 11)
    return "objects/GameHelper/pigarithm/" .. (sprite == "mole" and "mole00" or sprite .. "/idle")
end

return pigarithm