local web = {}

web.name = "GameHelper/SlowdownCobweb"
web.depth = 8999
web.fillColor = { 10 / 10, 10 / 10, 10 / 10, 1 / 2 }
web.borderColor = { 7 / 10, 7 / 10, 7 / 10 }
web.justification = { 0.0, 0.0 }
web.minimumSize = { 8, 8 }
web.maximumSize = { 999, 999 }
web.canResize = { true, true }
web.placements = {
    name = "web",
    data = {
        width = 8,
        height = 8
    }
}

return web
