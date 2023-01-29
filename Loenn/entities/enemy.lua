local enemy = {}

enemy.name = "GameHelper/Enemy"
enemy.depth = 8998
enemy.texture = "objects/GameHelper/Enemy/walking00"
enemy.justification = {0.5, 0.5}
enemy.placements = {
    name = "enemy",
    data = {
        speedX = 50.0,
        speedY = 200.0,
        hitboxWidth = 8.0,
        hitboxHeight = 16.0,
        hitboxXOffset = -3.0,
        hitboxYOffset = 0.0,
        bounceHitboxWidth = 8.0,
        bounceHitboxHeight = 4.0,
        bounceHitboxXOffset = -3.0,
        bounceHitboxYOffset = -3.0,
        canDie = true,
        drawOutline = false,
        customSpritePath = ""
    }
}

return enemy