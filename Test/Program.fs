open EquationSolver.Solver

[<EntryPoint>]
let main argv =
    let config = {
        Function = fun x -> sin(1.0/x)
        Accuracy = 1e-10
        Delta = 1e-5
    }

    printfn "*************************************************"
    printfn "*          Solutions of the function             "
    printfn "*------------------------------------------------"
    (-1.0, 1.0)
    |> Parallel.FindSolutions config
    |> Seq.iteri (fun index element -> 
        printfn $"*  {index + 1}. zero: %.15f{element}"
    )
    printfn "*************************************************"

    0