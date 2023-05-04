local modifier = {}

modifier.name = "GameHelper/EntityModifier"
modifier.depth = -9999999
modifier.texture = "loenn/GameHelper/entity_modifier"
modifier.justification = {0.0, 0.0}
modifier.nodeLimits = {0, 999}
modifier.placements = {
    {
        name = "number",
        data = {
            valueType = "",
            fieldName = "",
            onlyType = "",
            allEntities = false,
            debug = false
        }
    }
}
modifier.fieldOrder = {
    "x", "y",

    "fieldName", "onlyType", "allEntities", "debug"
}

return modifier