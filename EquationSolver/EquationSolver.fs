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

        let mutable previousX = lowerLimit
        let mutable previousY = func lowerLimit

        let pairs = seq {
            for x in lowerLimit..step..upperLimit do
                let y = func x

                if DifferentSign previousY y then
                    yield (previousX, x)

                previousX <- x
                previousY <- y
        }

        pairs

    let private Bisect (delta: double) (func: double -> double) (limits: double * double) =
        let mutable (lowerLimit, upperLimit) = limits

        let mutable midPoint = Average lowerLimit upperLimit

        let mutable lowerPointValue = func lowerLimit
        let mutable upperPointValue = func upperLimit

        while upperLimit - lowerLimit >= delta do
            let midPointValue = func midPoint

            if DifferentSign lowerPointValue midPointValue then
                upperLimit <- midPoint
                upperPointValue <- midPointValue
            else if DifferentSign upperPointValue midPointValue then
                lowerLimit <- midPoint
                lowerPointValue <- midPointValue

            midPoint <- Average lowerLimit upperLimit

        midPoint

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
