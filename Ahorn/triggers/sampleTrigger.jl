module GameHelperSampleTrigger

using ..Ahorn, Maple

@mapdef Trigger "GameHelper/SampleTrigger" SampleTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    sampleProperty::Integer=0
)

const placements = Ahorn.PlacementDict(
    "Sample Trigger (GameHelper)" => Ahorn.EntityPlacement(
        SampleTrigger,
        "rectangle",
    )
)

end