namespace EquationSolver

open System
open FSharp.Collections.ParallelSeq

module EquationSolver =
    let private numOfSegments = Environment.ProcessorCount

    let inline private DifferentSign (num1: double) (num2: double) =
        num1 * num2 < 0.0

    let FindPairs (func: double -> double) (steps: uint64) (limits: double * double) =
        let (lowerLimit, upperLimit) = limits
        let step = (upperLimit - lowerLimit) / double steps

        let mutable previousX = lowerLimit
        let mutable previousY = func previousX
        
        seq {
            for x in lowerLimit..step..upperLimit do
                let y = func x

                if not(Double.IsNaN y) then
                    if y = 0.0 then
                        yield (x - step, x + step)
                    
                    if DifferentSign previousY y then
                        yield (previousX, x)

                    previousX <- x
                    previousY <- y
        }

    let Bisect (func: double -> double) (steps: uint64) (limits: double * double) =
        let mutable (lowerLimit, upperLimit) = limits

        for _ in 1UL..steps do
            let midPoint = (lowerLimit + upperLimit) / 2.0
            
            let lowerPointValue = func lowerLimit
            let midPointValue = func midPoint
            let upperPointValue = func upperLimit
            
            if DifferentSign lowerPointValue midPointValue then
                upperLimit <- midPoint
            else if DifferentSign upperPointValue midPointValue then
                lowerLimit <- midPoint

        (lowerLimit + upperLimit) / 2.0  

    let FindSolutions (func: double -> double) (steps: uint64) (limits: double * double) =
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
        |> PSeq.map (FindPairs func steps)
        |> PSeq.concat
        |> PSeq.map (Bisect func steps)