local trampoline = {}

trampoline.name = "GameHelper/Trampoline"
trampoline.depth = 8998
trampoline.texture = "objects/GameHelper/trampoline"
trampoline.justification = {0.5, 1.0}
trampoline.placements = {
    name = "trampoline",
    data = {
        speedBoost = 40.0,
        facingUpLeft = true,
        refillDash = true,
        oneUse = false
    }
}

return trampoline