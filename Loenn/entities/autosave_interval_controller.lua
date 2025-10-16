local aic = {}

aic.name = "GameHelper/AutoSaveIntervalController"
aic.depth = -999999999
aic.texture = "loenn/GameHelper/controller"
aic.justification = { 0.0, 0.0 }
aic.placements = {
    name = "normal",
    data = {
        interval = 10
    }
}
aic.fieldInformation = {
    intervalMinutes = {
        fieldType = "integer"
    }
}

return aic
