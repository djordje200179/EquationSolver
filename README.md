Numerical solver of mathematical equations that works by
finding regions where the function changes sign and then
applying bisection method to find the roots of the function.

It is possible to run the calculation in parallel by splitting
the interval into smaller parts.

## Usage
```fsharp
open EquationSolver.Solver

let config = {
    Function = fun x -> sin(1.0/x)
    Accuracy = 1e-10
    Resolution = 1e-5
}

(-1.0, 1.0)
|> Parallel.FindSolutions config
|> Seq.iter (printfn "%.15f")