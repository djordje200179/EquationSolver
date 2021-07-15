open System
open EquationSolver

let Function (x: double) =
    let Um = 5e3
    let Ekr = 100e3 / 1e-2;
    let b = 2.5e-3;
    
    x * Math.Log(b / x) - (Um / Ekr)

[<EntryPoint>]
let main argv =
    let results = 
        (Function, (0.0, 1.0))
        ||> EquationSolver.FindSolutions 1e-7 0.5e-15
        |> Seq.cache

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