namespace EquationSolver

open System
open FsToolkit.ErrorHandling

module Solver =
    type SolveError =
        | DidNotConverge
        | PossibleDiscontinuity

    let internal findRegions config functionToSolve interval =
        interval
        |> Interval.points (Config.resolution config)
        |> Seq.map (fun point -> {
            Point = point
            Value = functionToSolve point
        })
        |> Seq.pairwise
        |> Seq.filter (fun (lower, upper) ->
            lower.Value = 0.0
            || upper.Value = 0.0
            || Sample.hasDifferentSign lower upper)

    [<TailCall>]
    let rec internal findRoot accuracy functionToSolve (lower, upper) =
        if abs lower.Value < accuracy then
            Ok lower.Point
        elif abs upper.Value < accuracy then
            Ok upper.Point
        else
            let middlePoint = Sample.midpoint lower upper

            if lower.Point = middlePoint || upper.Point = middlePoint then
                Error DidNotConverge
            else
                let middle = {
                    Point = middlePoint
                    Value = functionToSolve middlePoint
                }

                if Sample.hasDifferentSign lower middle then
                    findRoot accuracy functionToSolve (lower, middle)
                elif Sample.hasDifferentSign upper middle then
                    findRoot accuracy functionToSolve (middle, upper)
                else
                    Error PossibleDiscontinuity

    let findSolutions config functionToSolve interval =
        interval
        |> findRegions config functionToSolve
        |> Seq.toList
        |> List.traverseResultM (findRoot (Config.accuracy config) functionToSolve)
        |> Result.map List.distinct
