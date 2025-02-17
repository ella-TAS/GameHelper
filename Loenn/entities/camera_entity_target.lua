local st = {}

st.name = "GameHelper/CameraEntityTargetController"
st.depth = 8998
st.texture = "loenn/GameHelper/controller"
st.placements = {
    name = "normal",
    data = {
        onlyType = "Celeste.Player",
        flag = "",
        lerp = 1.0,
        offsetX = 0.0,
        offsetY = 0.0,
        resetOnFalse = false,
        debug = false
    }
}

return st