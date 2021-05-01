open System
open EquationSolver

[<EntryPoint>]
let main argv =
    let results = 
        (Math.Tan, (-10.0, 10.0))
        ||> EquationSolver.FindSolutions (uint64 1e6)
        |> Seq.cache
        |> Seq.sort

    printfn "*************************************************"
    printfn "*          Solutions of the function             "
    printfn "*------------------------------------------------"
    Seq.iteri (fun index element -> 
        printfn $"*  {index + 1}. zero: %.15f{element}"
    ) results
    printfn "*------------------------------------------------"
    printfn $"*  Zeroes found:  {Seq.length results}"
    printfn "*************************************************"

    0