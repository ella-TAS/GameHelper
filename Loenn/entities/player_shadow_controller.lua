local psc = {}

psc.name = "GameHelper/PlayerShadowController"
psc.depth = -9999999
psc.texture = "loenn/GameHelper/player_shadow_controller"
psc.justification = {0.0, 0.0}
psc.placements = {
    name = "normal",
    data = {
        uses = 3,
        oneUse = false,
        sprite = "objects/GameHelper/player_shadow",
        freezeFrames = true,
        clipToTop = true
    }
}
psc.fieldInformation = {
    uses = {
        fieldType = "integer"
    }
}

return psc