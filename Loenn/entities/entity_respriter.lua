local respriter = {}

respriter.name = "GameHelper/EntityRespriter"
respriter.depth = -9999999
respriter.texture = "loenn/GameHelper/entity_respriter"
respriter.justification = {0.0, 0.0}
respriter.nodeLimits = {0, 999}
respriter.placements = {
    {
        name = "direct",
        data = {
            spriteFolder = "",
            spriteName = "",
            offsetX = 0.0,
            offsetY = 0.0,
            flipX = false,
            flipY = false,
            delay = 1/60,
            fieldName = "sprite",
            onlyType = "",
            allEntities = false,
            debug = false,
            removeAllComponents = false
        }
    },
    {   name = "xml",
        data = {
            xmlPath = "",
            spriteID = "",
            offsetX = 0.0,
            offsetY = 0.0,
            flipX = false,
            flipY = false,
            fieldName = "sprite",
            onlyType = "",
            allEntities = false,
            debug = false,
            removeAllComponents = false
        }
    }
}
respriter.fieldOrder = {
    "x", "y",
    "spriteFolder", "spriteName", "offsetX", "offsetY", "delay",
    "xmlPath", "spriteID",
    "fieldName", "onlyType", "allEntities", "debug", "removeAllComponents", "flipX", "flipY"
}

return respriter