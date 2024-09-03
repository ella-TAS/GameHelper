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
function pigarithm.canResize(room, entity)
    return entity.sprite == "pigarithm_mole" and {false, false} or {true, true}
end
pigarithm.fieldInformation = {
    sprite = {
        options = spriteOptions,
        editable = false
    }
}
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
            gravity = false,
            flipSprite = false,
            flag = ""
        }
    }, {
        name = "mole",
        data = {
            width = 24,
            height = 32,
            sprite = "pigarithm_mole",
            speed = 60.0,
            startRight = true,
            kill = true,
            gravity = true,
            flipSprite = false,
            flag = ""
        }
    }
}

function pigarithm.justification(room, entity)
    y = entity.flipSprite and 1.0 or 0.0
    if entity.sprite == "pigarithm_small" then
        return {0.28, y}
    elseif entity.sprite == "pigarithm_medium" then
        return {0.18, y}
    elseif entity.sprite == "pigarithm_big" then
        return {0.14, y}
    else
        return entity.startRight and {0.1, y} or {0.9, y}
    end
end

function pigarithm.scale(room, entity)
    return {(entity.startRight or entity.sprite ~= "pigarithm_mole") and 1 or -1, entity.flipSprite and -1 or 1}
end

function pigarithm.texture(room, entity)
    local sprite = string.sub(entity.sprite, 11)
    return "objects/GameHelper/pigarithm/" .. (sprite == "mole" and "mole00" or sprite .. "/idle")
end

return pigarithm