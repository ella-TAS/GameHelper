local bridge = {}

bridge.name = "GameHelper/HangingBridge"
bridge.depth = 8998
bridge.nodeLimits = {1, 1}
bridge.nodeLineRenderType = "line"
bridge.nodeVisibility = "always"
bridge.placements = {
    name = "normal",
    data = {
        parabolaExtremity = 1.0
    }
}

return bridge