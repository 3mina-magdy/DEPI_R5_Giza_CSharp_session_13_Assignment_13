# Assignment 02 — LINQ (C#)

## Run the project
1. Open `Assignment02Linq.csproj` in Visual Studio.
2. Build and run the project (Ctrl + F5).
3. Keep `customers.xml` and `dictionary_english.txt` beside the project file; they are included in the ZIP.

## Project structure
- `Program.cs`: solutions for all required LINQ questions, separated into six methods.
- `ListGenerators.cs`: provided product, customer, and order models/data.
- `customers.xml`: customer and order source data.
- `dictionary_english.txt`: source words for dictionary questions.

## Sections and operators
- Element: `First`, `FirstOrDefault`, `ElementAt`
- Aggregate: `Count`, `Sum`, `Min`, `Max`, `Average`
- Set: `Distinct`, `Union`, `Intersect`, `Except`, `Concat`
- Partitioning: `Take`, `Skip`, `TakeWhile`, `SkipWhile`
- Quantifiers: `Any`, `All`
- Grouping: `GroupBy`, plus a custom anagram comparer

## Notes
- Question 11 explicitly uses `let` to save each category's cheapest price.
- Washington is represented in `Customers.xml` by `Region == "WA"`.
- The dictionary grouping displays the count and the first 12 words per letter to keep the console readable; every word is still included in the LINQ grouping.
- The source input contains `" last"` with a leading space. `AnagramComparer` trims it so it correctly groups with `salt`.
