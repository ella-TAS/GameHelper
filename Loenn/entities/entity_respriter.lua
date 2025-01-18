local respriter = {}

respriter.name = "GameHelper/EntityRespriter"
respriter.depth = -999999999
respriter.texture = "loenn/GameHelper/entity_respriter"
respriter.justification = {0, 0}
respriter.nodeLimits = {0, -1}
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
            removeAllComponents = false,
            activationFlag = "",
            invertFlag = false,
            doNewlyAddedEntities = false
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
            removeAllComponents = false,
            activationFlag = "",
            invertFlag = false,
            doNewlyAddedEntities = false
        }
    }
}
respriter.fieldOrder = {
    "x", "y",
    "spriteFolder", "spriteName", "offsetX", "offsetY", "delay",
    "xmlPath", "spriteID",
    "fieldName", "onlyType", "activationFlag", "invertFlag", "allEntities",
    "debug", "removeAllComponents", "flipX", "flipY", "doNewlyAddedEntities"
}

return respriter