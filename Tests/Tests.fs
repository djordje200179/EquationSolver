module Tests

open EquationSolver.Solver
open EquationSolver
open Xunit

[<Fact>]
let ``Linear equation`` () =
    let config = {
        Function = fun x -> x + 1.0
        Accuracy = 1e-10
        Resolution = 1e-5
    }

    let zeroes = 
        (-2.0, 2.0)
        |> FindSolutions config
        |> Seq.toList

    Assert.Equal(1, zeroes.Length)
    Assert.Equal(-1.0, zeroes[0], 10)