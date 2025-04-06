namespace EquationSolver

open System

module Solver =
    let inline private IsDifferentSign num1 num2 =
        if Double.IsNaN num1 || Double.IsNaN num2 then
            false
        else
            Math.Sign num1 <> Math.Sign num2

    let inline private Average num1 num2 = Math.ScaleB(num1 + num2, -1)

    let private FindRegions resolution func limits =
        let (lowerLimit, upperLimit) = limits

        seq { lowerLimit..resolution..upperLimit }
        |> Seq.map (fun x -> x, func x)
        |> Seq.pairwise
        |> Seq.filter (fun ((_, y1), (_, y2)) -> y1 = 0.0 || y2 = 0.0 || IsDifferentSign y1 y2)
        |> Seq.map (fun ((x1, _), (x2, _)) -> x1, x2)

    [<TailCall>]
    let rec private FindPoint accuracy func limits =
        let (lowerLimit, upperLimit) = limits

        match func lowerLimit, func upperLimit with
        | lowerPointValue, _ when abs(lowerPointValue) < accuracy -> lowerLimit
        | _, upperPointValue when abs(upperPointValue) < accuracy -> upperLimit
        | lowerPointValue, upperPointValue ->
            let midPoint = Average lowerLimit upperLimit

            if lowerLimit = midPoint || upperLimit = midPoint then
                raise (InvalidOperationException("Stuck in a local minimum. Decrease resolution value"))

            FindPoint accuracy func (
                match func midPoint with
                | midPointValue when IsDifferentSign lowerPointValue midPointValue -> (lowerLimit, midPoint)
                | midPointValue when IsDifferentSign upperPointValue midPointValue -> (midPoint, upperLimit)
                | _ -> raise (InvalidOperationException("Function is not continuous"))
            )

    type Config = {
        Function: double -> double
        Resolution: double
        Accuracy: double
    }

    let FindSolutions config limits =
        limits
        |> FindRegions config.Resolution config.Function
        |> Seq.map  (FindPoint config.Accuracy config.Function)
