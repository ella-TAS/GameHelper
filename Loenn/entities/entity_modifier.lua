local modifier = {}

local booleanOptions = {
    SetTrue = "set_true",
    SetFalse = "set_false",
    Invert = "invert",
    SetToFlag = "set_flag",
    Ignore = "ignore"
}

modifier.name = "GameHelper/EntityModifier"
modifier.depth = -9999999
modifier.justification = {0.0, 0.0}
modifier.nodeLimits = {0, -1}
modifier.nodeLineRenderType = "fan"
modifier.placements = {
    {
        name = "number",
        data = {
            valueType = "number",
            valueNumber = 0.0,
            integer = false,
            fieldName = "",
            onlyType = "",
            activationFlag = "",
            invertFlag = false,
            allEntities = false,
            doNewlyAddedEntities = false,
            onlyOnce = true,
            everyFrame = false,
            debug = false
        }
    },
    {
        name = "string",
        data = {
            valueType = "string",
            valueString = "",
            fieldName = "",
            onlyType = "",
            activationFlag = "",
            invertFlag = false,
            allEntities = false,
            doNewlyAddedEntities = false,
            onlyOnce = true,
            everyFrame = false,
            debug = false
        }
    },
    {
        name = "bool",
        data = {
            valueType = "bool",
            valueBool = false,
            fieldName = "",
            onlyType = "",
            activationFlag = "",
            invertFlag = false,
            allEntities = false,
            doNewlyAddedEntities = false,
            onlyOnce = true,
            everyFrame = false,
            debug = false
        }
    },
    {
        name = "common",
        data = {
            valueType = "common",
            changeActive = "ignore",
            changeCollidable = "ignore",
            changeVisible = "ignore",
            onlyType = "",
            activationFlag = "",
            invertFlag = false,
            allEntities = false,
            doNewlyAddedEntities = false,
            onlyOnce = true,
            everyFrame = false,
            debug = false
        }
    }
}
modifier.fieldOrder = {
    "x", "y",
    "fieldName", "valueNumber", "valueString", "valueBool", "changeActive", "changeCollidable", "changeVisible",
    "onlyType", "allEntities", "doNewlyAddedEntities", "activationFlag", "invertFlag", "onlyOnce", "everyFrame", "integer", "debug"
}
modifier.fieldInformation = {
    changeActive = {
        options = booleanOptions,
        editable = false
    },
    changeCollidable = {
        options = booleanOptions,
        editable = false
    },
    changeVisible = {
        options = booleanOptions,
        editable = false
    }
}
modifier.ignoredFields = {
    "_id", "_name", "originX", "originY", "valueType"
}

function modifier.texture(room, entity)
    return "loenn/GameHelper/entity_modifier_" .. entity.valueType
end

return modifier