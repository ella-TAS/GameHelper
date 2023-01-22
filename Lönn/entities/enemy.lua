local enemy = {}

enemy.name = "GameHelper/Enemy"
enemy.depth = 8998
enemy.texture = "objects/GameHelper/Enemy/walking00"
enemy.justification = {0.5, 0.5}
enemy.placements = {
    name = "enemy",
    data = {
        speedX = 50.0,
        speedY = 200.0
    }
}

return enemy