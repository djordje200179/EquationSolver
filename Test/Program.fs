open EquationSolver.Solver
open EquationSolver
open System.Diagnostics

let config = {
    Function = fun x -> sin(1.0/x)
    Accuracy = 1e-10
    Resolution = 1e-5
}

let stopwatch = Stopwatch.StartNew()

let zeroes = 
    (-1.0, 1.0)
    |> Parallel.FindSolutions config
    |> Seq.toList

printfn "*************************************************"
printfn "*          Solutions of the function             "
printfn "*------------------------------------------------"
printfn $"*  Elapsed time: {stopwatch.ElapsedMilliseconds} ms"
printfn "*------------------------------------------------"
for zero in zeroes do
    printfn "* %.15f" zero
printfn "*************************************************"