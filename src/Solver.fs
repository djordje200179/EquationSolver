namespace EquationSolver

open System
open FsToolkit.ErrorHandling

module Solver =
    type SolveError =
        | DidNotConverge
        | PossibleDiscontinuity

    type internal Bracket = {
        LowerLimit: float
        LowerValue: float
        UpperLimit: float
        UpperValue: float
    }

    let private isDifferentSign first second =
        not (Double.IsNaN first || Double.IsNaN second)
        && Math.Sign first <> Math.Sign second

    let private midpoint lower upper = Math.ScaleB(lower + upper, -1)

    let internal findRegions config functionToSolve interval =
        interval
        |> Interval.points (Config.resolution config)
        |> Seq.map (fun point -> point, functionToSolve point)
        |> Seq.pairwise
        |> Seq.choose (fun ((firstPoint, firstValue), (secondPoint, secondValue)) ->
            if firstValue = 0.0 || secondValue = 0.0 || isDifferentSign firstValue secondValue then
                Some {
                    LowerLimit = firstPoint
                    LowerValue = firstValue
                    UpperLimit = secondPoint
                    UpperValue = secondValue
                }
            else
                None)

    [<TailCall>]
    let rec internal findRoot accuracy functionToSolve bracket =
        if abs bracket.LowerValue < accuracy then
            Ok bracket.LowerLimit
        elif abs bracket.UpperValue < accuracy then
            Ok bracket.UpperLimit
        else
            let middle = midpoint bracket.LowerLimit bracket.UpperLimit

            if bracket.LowerLimit = middle || bracket.UpperLimit = middle then
                Error DidNotConverge
            else
                let middleValue = functionToSolve middle

                if isDifferentSign bracket.LowerValue middleValue then
                    findRoot accuracy functionToSolve {
                        bracket with
                            UpperLimit = middle
                            UpperValue = middleValue
                    }
                elif isDifferentSign bracket.UpperValue middleValue then
                    findRoot accuracy functionToSolve {
                        bracket with
                            LowerLimit = middle
                            LowerValue = middleValue
                    }
                else
                    Error PossibleDiscontinuity

    let findSolutions config functionToSolve interval =
        interval
        |> findRegions config functionToSolve
        |> Seq.toList
        |> List.traverseResultM (findRoot (Config.accuracy config) functionToSolve)
        |> Result.map List.distinct
