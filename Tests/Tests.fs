module Tests

open EquationSolver.Solver
open Xunit
open System

[<Fact>]
let ``Constant function has no solutions`` () =
    let config = {
        Function = fun x -> 1.0
        Accuracy = 1e-10
        Resolution = 1e-5
    }

    let zeroes = 
        (-2.0, 2.0)
        |> FindSolutions config
        |> Seq.toList

    Assert.Empty(zeroes)

[<Fact>]
let ``Linear equation has one solution`` () =
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

[<Fact>]
let ``Quadratic equation has two solutions`` () =
    let config = {
        Function = fun x -> x * x - 1.0
        Accuracy = 1e-10
        Resolution = 1e-5
    }

    let zeroes = 
        (-2.0, 2.0)
        |> FindSolutions config
        |> Seq.toList

    Assert.Equal(2, zeroes.Length)
    Assert.Equal(-1.0, zeroes.[0], 10)
    Assert.Equal(1.0, zeroes.[1], 10)

[<Fact>]
let ``Discontinuous function causes exception`` () =
    let config = {
        Function = fun x -> 1.0 / x
        Accuracy = 1e-10
        Resolution = 1e-5
    }

    Assert.Throws<InvalidOperationException>(fun () ->
        (-2.0, 2.0)
        |> FindSolutions config
        |> Seq.toList
        :> obj
    )