local modifier = {}

modifier.name = "GameHelper/EntityModifier"
modifier.depth = -9999999
modifier.justification = {0.0, 0.0}
modifier.nodeLimits = {0, -1}
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
            onlyOnce = true,
            everyFrame = false,
            debug = false
        }
    }
}
modifier.fieldOrder = {
    "x", "y",
    "fieldName", "valueNumber", "valueString", "valueBool",
    "onlyType", "allEntities", "debug", "activationFlag", "invertFlag", "onlyOnce", "everyFrame", "integer"
}
modifier.ignoredFields = {
    "_id", "_name", "originX", "originY", "valueType"
}

function modifier.texture(room, entity)
    return "loenn/GameHelper/entity_modifier_" .. entity.valueType
end

return modifier