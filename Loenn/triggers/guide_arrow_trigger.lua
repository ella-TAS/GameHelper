local gat = {}

gat.name = "GameHelper/GuideArrowTrigger"
gat.depth = 8998
gat.nodeLimits = { 0, -1 }
gat.nodeLineRenderType = "fan"
gat.placements = {
    name = "normal",
    data = {
        onlyOnce = false,
        duration = 0.0
    }
}

return gat
