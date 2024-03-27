local psf = {}

psf.name = "GameHelper/PlayerStateFlag"
psf.depth = -9999999
psf.texture = "loenn/GameHelper/flag_controller"
psf.justification = {0.0, 0.0}
psf.placements = {
    name = "psf",
    data = {
        flag = "playerstate",
        state = "0",
        invert = false,
        dashAttack = false,
    }
}
psf.fieldInformation = {
    state = {
        fieldType = "integer"
    }
}

return psf