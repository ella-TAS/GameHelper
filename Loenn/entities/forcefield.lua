local field = {}
local dirs = { "Left", "Up", "Right", "Down" }

field.name = "GameHelper/Forcefield"
field.depth = 8999
field.fillColor = { 10 / 10, 10 / 10, 0 / 10, 1 / 2 }
field.borderColor = { 7 / 10, 7 / 10, 0 / 10 }
field.justification = { 0.0, 0.0 }
field.minimumSize = { 8, 8 }
field.canResize = { true, true }
field.placements = {
    {
        name = "normal",
        data = {
            width = 8,
            height = 8,
            force = 30.0,
            direction = "Right",
            color = "FFFF00",
            speedCheck = false
        }
    },
    {
        name = "speedCheck",
        data = {
            width = 8,
            height = 8,
            force = 40.0,
            direction = "Left",
            color = "FFFF00",
            speedCheck = true
        }
    }
}
field.fieldInformation = {
    direction = {
        options = dirs,
        editable = false
    },
    color = {
        fieldType = "color"
    }
}

return field
