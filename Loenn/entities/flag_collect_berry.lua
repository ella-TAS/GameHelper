local strawberry = {}

strawberry.name = "GameHelper/FlagCollectBerry"
strawberry.depth = -100
strawberry.texture = "collectables/strawberry/normal00"
strawberry.placements = {
    {
        name = "normal",
        data = {
            collectFlag = "collect",
            loseFlag = "",
            keepOnDeath = false
        }
    }
}

return strawberry
