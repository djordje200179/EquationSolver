namespace EquationSolver

open System
open FSharp.Collections.ParallelSeq

module EquationSolver =
    let inline private DifferentSign (num1: double) (num2: double) =
        if Double.IsNaN num1 || Double.IsNaN num2 then
            false
        else
            Math.Sign num1 <> Math.Sign num2

    let inline private Average (num1: double) (num2: double) = Math.ScaleB(num1 + num2, -1)

    let private FindPairs (step: double) (func: double -> double) (limits: double * double) =
        let (lowerLimit, upperLimit) = limits

        seq { lowerLimit..step..upperLimit }
        |> Seq.map (fun x -> x, func x)
        |> Seq.pairwise
        |> Seq.filter (fun ((_, y1), (_, y2)) -> y1 = 0.0 || y2 = 0.0 || DifferentSign y1 y2)
        |> Seq.map (fun ((x1, _), (x2, _)) -> x1, x2)

    let rec private Bisect (delta: double) (func: double -> double) (limits: double * double) =
        let (lowerLimit, upperLimit) = limits

        let midPoint = Average lowerLimit upperLimit

        match func lowerLimit, func upperLimit, func midPoint with
        | lowerPointValue, _, _ when lowerPointValue = 0.0 -> lowerLimit
        | _, upperPointValue, _ when upperPointValue = 0.0 -> upperLimit
        | lowerPointValue, _, midPointValue when DifferentSign lowerPointValue midPointValue -> (Bisect delta func) (lowerLimit, midPoint)
        | _, upperPointValue, midPointValue when DifferentSign upperPointValue midPointValue -> (Bisect delta func) (midPoint, upperLimit)
        | _ -> 0.0

    let private FindBounds (limits: double * double) =
        let numOfSegments = Environment.ProcessorCount

        let (lowerLimit, upperLimit) = limits

        let step = (upperLimit - lowerLimit) / double numOfSegments

        let bounds = seq {
            for i in 1..numOfSegments do
                let a = lowerLimit + double (i - 1) * step
                let b = a + step

                yield (a, b)
        }

        bounds

    let FindSolutions (step: double) (delta: double) (func: double -> double) (limits: double * double) =
        limits
        |> FindBounds
        |> PSeq.map (FindPairs step func)
        |> PSeq.concat
        |> PSeq.map (Bisect delta func)
