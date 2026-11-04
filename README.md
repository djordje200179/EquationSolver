# EquationSolver

EquationSolver is a small F# library for finding the real roots of a
single-variable function over a finite interval.

The solver first samples the interval to locate regions where the function
changes sign, then applies the bisection method to approximate each root.
Intervals can be processed sequentially or partitioned across available
processors.

## Requirements

- .NET 10 SDK
- F# 10

## Usage

```fsharp
open System
open EquationSolver
open EquationSolver.Solver

match Config.create 1e-10 1e-5 with
| Error (InvalidAccuracy value) ->
    printfn "Invalid accuracy: %g" value
| Error (InvalidResolution value) ->
    printfn "Invalid resolution: %g" value
| Ok config ->
    match Interval.create -1.0 1.0 with
    | Error (NonFiniteLimit value) ->
        printfn "Invalid limit: %g" value
    | Error (InvalidOrder (lower, upper)) ->
        printfn "Invalid interval: %g to %g" lower upper
    | Ok interval ->
        match findSolutions config (fun x -> sin (1.0 / x)) interval with
        | Ok solutions ->
            solutions
            |> Seq.iter (printfn "%.15f")
        | Error DidNotConverge ->
            printfn "The requested accuracy could not be reached."
        | Error PossibleDiscontinuity ->
            printfn "A root could not be isolated."
```

Use the parallel solver for larger intervals or expensive functions:

```fsharp
let result =
    EquationSolver.Parallel.findSolutions config sin interval
```

## Configuration

| Property | Description |
| --- | --- |
| `Accuracy` | Maximum accepted function-value error at a root. Must be positive and finite. |
| `Resolution` | Distance between samples used to locate root regions. Must be positive and finite. |

`Config.create` validates these values and returns `Result<Config, ConfigError>`.
The resulting configuration can be reused with multiple functions.

`Interval.create` requires finite limits with the lower limit less than the
upper limit.

## Behavior and limitations

- Solutions are returned in ascending order without exact duplicates.
- `Resolution` determines which root regions can be detected. Roots closer
  together than the selected resolution may be missed.
- Roots where the function only touches zero without changing sign are found
  only when the sampling sequence evaluates the root exactly.
- The bisection method assumes continuity within each detected root region.
  `findSolutions` reports `DidNotConverge` or `PossibleDiscontinuity`.
- Samples that evaluate to `NaN` do not create root regions.

## Testing

Run the test suite from the repository root:

```powershell
dotnet test EquationSolver.sln
```
