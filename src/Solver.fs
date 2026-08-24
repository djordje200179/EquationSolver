namespace EquationSolver

open System

module Solver =
    let private isDifferentSign first second =
        not (Double.IsNaN first || Double.IsNaN second)
        && Math.Sign first <> Math.Sign second

    let private midpoint lower upper = Math.ScaleB(lower + upper, -1)

    let private findRegions resolution functionToSolve (lowerLimit, upperLimit) =
        seq { lowerLimit..resolution..upperLimit }
        |> Seq.map (fun point -> point, functionToSolve point)
        |> Seq.pairwise
        |> Seq.choose (fun ((firstPoint, firstValue), (secondPoint, secondValue)) ->
            if firstValue = 0.0 || secondValue = 0.0 || isDifferentSign firstValue secondValue then
                Some(firstPoint, firstValue, secondPoint, secondValue)
            else
                None)

    [<TailCall>]
    let rec private findRoot accuracy functionToSolve (lowerLimit, lowerValue, upperLimit, upperValue) =
        if abs lowerValue < accuracy then
            lowerLimit
        elif abs upperValue < accuracy then
            upperLimit
        else
            let middle = midpoint lowerLimit upperLimit

            if lowerLimit = middle || upperLimit = middle then
                invalidOp "Unable to converge within the requested accuracy."

            let middleValue = functionToSolve middle

            let nextRegion =
                if isDifferentSign lowerValue middleValue then
                    lowerLimit, lowerValue, middle, middleValue
                elif isDifferentSign upperValue middleValue then
                    middle, middleValue, upperLimit, upperValue
                else
                    invalidOp "Unable to isolate a root. The function may be discontinuous within the interval."

            findRoot accuracy functionToSolve nextRegion

    type Config = {
        Function: double -> double
        Accuracy: double
        Resolution: double
    }

    let internal validate config (lowerLimit, upperLimit) =
        if not (Double.IsFinite config.Accuracy) || config.Accuracy <= 0.0 then
            invalidArg (nameof config.Accuracy) "Accuracy must be a positive finite number."

        if not (Double.IsFinite config.Resolution) || config.Resolution <= 0.0 then
            invalidArg (nameof config.Resolution) "Resolution must be a positive finite number."

        if not (Double.IsFinite lowerLimit) || not (Double.IsFinite upperLimit) then
            invalidArg "limits" "Limits must be finite numbers."

        if lowerLimit >= upperLimit then
            invalidArg "limits" "The lower limit must be less than the upper limit."

    let findSolutions config limits =
        validate config limits

        limits
        |> findRegions config.Resolution config.Function
        |> Seq.map (findRoot config.Accuracy config.Function)
        |> Seq.distinct
