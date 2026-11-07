namespace EquationSolver

open System

[<Struct>]
type internal Sample = {
    Point: float
    Value: float
}

module internal Sample =
    let midpoint first second = Math.ScaleB(first.Point + second.Point, -1)

    let hasDifferentSign first second =
        not (Double.IsNaN first.Value || Double.IsNaN second.Value)
        && Math.Sign first.Value <> Math.Sign second.Value
