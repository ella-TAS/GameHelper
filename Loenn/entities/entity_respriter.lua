local respriter = {}

respriter.name = "GameHelper/EntityRespriter"
respriter.depth = -9999999
respriter.texture = "loenn/GameHelper/entity_respriter"
respriter.justification = {0.0, 0.0}
respriter.placements = {
    {
        name = "direct",
        data = {
            spriteFolder = "",
            spriteName = "",
            delay = 0.0,
            fieldName = "sprite"
        }
    },
    {   name = "xml",
        data = {
            xmlPath = "",
            spriteID = "",
            fieldName = "sprite"
        }
    }
}
respriter.fieldOrder = {
    "x", "y",
    "spriteFolder", "spriteName", "delay",
    "xmlPath", "spriteID",
    "fieldName"
}

return respriter