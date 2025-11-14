local bridge = {}

bridge.name = "GameHelper/JumpRope"
bridge.depth = 8998
bridge.nodeLimits = { 1, 1 }
bridge.nodeLineRenderType = "line"
bridge.nodeVisibility = "always"
bridge.placements = {
    name = "normal",
    data = {
        renderLeftEnd = true,
        renderRightEnd = true,
        texturePath = "objects/GameHelper/rope/"
    }
}

return bridge
