local swing = {}

swing.name = "GameHelper/SwingSolid"
swing.depth = 8999
swing.justification = {0.1, 0.12}
swing.texture = "objects/GameHelper/swing/swing_solid"
swing.canResize = {false, false}
swing.nodeLimits = {1, 1}
swing.nodeLineRenderType = "line"
swing.nodeVisibility = "always"
swing.nodeTexture = "objects/GameHelper/swing/chain00"
swing.nodeJustification = {-1, -1}
swing.placements = {
    name = "solid",
    data = {
        width = 48,
        height = 48,
        sprite = "objects/GameHelper/swing/swing_solid",
        chainSpritePrefix = "objects/GameHelper/swing/chain0",
        maxAngle = 30.0,
        swingSpeed = 1.0,
        accelerationTime = 377.0,
        startRight = false
    }
}

return swing