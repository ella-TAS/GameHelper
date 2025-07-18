local cdc = {}

cdc.name = "GameHelper/ColorfulDebugController"
cdc.depth = -999999999
cdc.texture = "loenn/GameHelper/colorful_debug_controller"
cdc.justification = {0.0, 0.0}
cdc.placements = {
    {
        name = "normal",
        data = {
            spawnColor = "FF0000",
            bgTileColor = "2F4F4F",
            fgTileColor = "FFFFFF",
            roomBgColor = "000000",
            berryColor = "FFB6C1",
            checkpointColor = "00FF00",
            jumpthruColor = "FFFF00",
            roomPrefix = ""
        }
    }
}

cdc.fieldInformation = {
    spawnColor =  { fieldType = "color" },
    bgTileColor = { fieldType = "color" },
    fgTileColor = { fieldType = "color" },
    roomBgColor = { fieldType = "color" },
    jumpthruColor =  { fieldType = "color" },
    berryColor =  { fieldType = "color" },
    checkpointColor =  { fieldType = "color" }
}

return cdc