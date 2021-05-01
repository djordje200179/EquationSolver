namespace EquationSolver

open System
open FSharp.Collections.ParallelSeq

module EquationSolver =
    let inline private AreDifferentSign (num1: double) (num2: double) =
        Math.Sign(num1) * Math.Sign(num2) = -1

    let private FindPairs (numberOfPairs: uint64) (func: double -> double) (limits: double * double) =
        let (lowerLimit, upperLimit) = limits
        let step = (upperLimit - lowerLimit) / double numberOfPairs

        let mutable previousX = lowerLimit
        let mutable previousY = func previousX
        
        seq {
            for x in lowerLimit..step..upperLimit do
                let y = func x

                if not(Double.IsNaN y) then
                    if y = 0.0 then
                        yield (x, x)
                    elif AreDifferentSign previousY y then
                        yield (previousX, x)

                    previousX <- x
                    previousY <- y
        }

    let private Bisect (iterations: uint64) (func: double -> double) (limits: double * double) =
        let mutable (lowerLimit, upperLimit) = limits
        
        let mutable iterationCount = 0UL
        while iterationCount < iterations do
            let midPoint = Math.ScaleB(lowerLimit + upperLimit, -1)
            
            if midPoint = lowerLimit || midPoint = upperLimit then
                iterationCount <- iterations

            let lowerPointValue = func lowerLimit
            let midPointValue = func midPoint
            let upperPointValue = func upperLimit
            
            if AreDifferentSign lowerPointValue midPointValue then
                upperLimit <- midPoint
            else if AreDifferentSign upperPointValue midPointValue then
                lowerLimit <- midPoint

            iterationCount <- iterationCount + 1UL

        Math.ScaleB (lowerLimit + upperLimit, -1)

    let FindSolutions (iterations: uint64) (func: double -> double) (limits: double * double) =
        let numOfSegments = Environment.ProcessorCount

        let (lowerLimit, upperLimit) = limits
        let step = (upperLimit - lowerLimit) / double numOfSegments

        let bounds = 
            seq {
                for i in 1..numOfSegments do
                    let a = lowerLimit + double (i - 1) * step
                    let b = a + step

                    yield (a, b)
            }

        bounds 
        |> PSeq.map (FindPairs iterations func)
        |> PSeq.concat
        |> PSeq.map (Bisect iterations func)