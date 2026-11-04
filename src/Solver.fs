namespace EquationSolver

open System
open FsToolkit.ErrorHandling

module Solver =
    type SolveError =
        | DidNotConverge
        | PossibleDiscontinuity

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
            Ok lowerLimit
        elif abs upperValue < accuracy then
            Ok upperLimit
        else
            let middle = midpoint lowerLimit upperLimit

            if lowerLimit = middle || upperLimit = middle then
                Error DidNotConverge
            else
                let middleValue = functionToSolve middle

                if isDifferentSign lowerValue middleValue then
                    findRoot accuracy functionToSolve (lowerLimit, lowerValue, middle, middleValue)
                elif isDifferentSign upperValue middleValue then
                    findRoot accuracy functionToSolve (middle, middleValue, upperLimit, upperValue)
                else
                    Error PossibleDiscontinuity

    let findSolutions config functionToSolve interval =
        let limits = Interval.limits interval
        let accuracy = Config.accuracy config
        let resolution = Config.resolution config

        findRegions resolution functionToSolve limits
        |> Seq.toList
        |> List.traverseResultM (findRoot accuracy functionToSolve)
        |> Result.map (List.distinct >> Seq.ofList)
