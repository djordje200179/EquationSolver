open System
open EquationSolver

[<EntryPoint>]
let main argv =
    let results = 
        EquationSolver.FindSolutions (Math.Exp >> Math.Sin) (uint64 1e6) (-10.0, 3.0)
        |> Seq.sort
        |> Seq.cache
    
    printfn "*************************************************"
    printfn "*          Solutions of the function             "
    printfn "*------------------------------------------------"
    Seq.iteri (fun index element -> 
        printfn $"*  {index + 1}. zero: %.6f{element}"
    ) results
    printfn "*------------------------------------------------"
    printfn $"*  Zeroes found:  {Seq.length results}"
    printfn "*************************************************"

    0