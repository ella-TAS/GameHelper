local psc = {}

psc.name = "GameHelper/PlayerShadowController"
psc.depth = -9999999
psc.texture = "loenn/GameHelper/super_hot_controller"
psc.justification = {0.0, 0.0}
psc.placements = {
    name = "normal",
    data = {
        uses = 3
    }
}
psc.fieldInformation = {
    uses = {
        fieldType = "integer"
    }
}

return psc