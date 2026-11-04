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
open EquationSolver.Solver

let config = {
    Function = fun x -> sin (1.0 / x)
    Accuracy = 1e-10
    Resolution = 1e-5
}

match findSolutions config (-1.0, 1.0) with
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
    EquationSolver.Parallel.findSolutions config (-1.0, 1.0)
```

## Configuration

| Property | Description |
| --- | --- |
| `Function` | Function whose real roots should be found. |
| `Accuracy` | Maximum accepted function-value error at a root. Must be positive and finite. |
| `Resolution` | Distance between samples used to locate root regions. Must be positive and finite. |

Interval limits must be finite, and the lower limit must be less than the
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
