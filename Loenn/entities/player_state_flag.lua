local psf = {}

psf.name = "GameHelper/PlayerStateFlag"
psf.depth = -999999999
psf.texture = "loenn/GameHelper/flag_controller"
psf.justification = {0.0, 0.0}
psf.placements = {
    name = "psf",
    data = {
        flag = "playerstate",
        state = "0",
        useStateName = false,
        invert = false,
        dashAttack = false,
        debug = false,
    }
}

return psf