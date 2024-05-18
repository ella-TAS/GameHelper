local swing = {}

swing.name = "GameHelper/SwingSolid"
swing.depth = 8999
swing.justification = {0.1, 0.12}
swing.texture = "objects/GameHelper/swing/swing_solid"
swing.canResize = {false, false}
swing.minimumSize = {46, 46}
swing.maximumSize = {46, 46}
swing.nodeLimits = {1, 1}
swing.nodeLineRenderType = "line"
swing.nodeVisibility = "always"
swing.nodeTexture = "objects/GameHelper/swing/chain00"
swing.nodeJustification = {-1, -1}
swing.placements = {
    name = "solid",
    data = {
        width = 46,
        height = 46
    }
}

return swing