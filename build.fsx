// FAKE build script
//
// Based on https://raw.githubusercontent.com/jet/kafunk/master/build.fsx

#r @"packages/build/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing
open Fake.AssemblyInfoFile
open System

// The name of the project
// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
let project = "DummyApp"

// Short summary of the project
// (used as description in AssemblyInfo and as a short summary for NuGet package)
let summary = "F# tutorial"

// Longer description of the project
// (used as a description for NuGet package; line breaks are automatically cleaned up)
let description = ""

// List of author names (for NuGet package)
let authors = [ "Samuel El-Borai <samuel.elborai@gmail.com>" ]

// Tags for your project (for NuGet package)
let tags = ""

// File system information
let solutionFile  = "src/DummyApp.sln"

// Pattern specifying assemblies to be tested using NUnit
let testAssemblies = "src/**/bin/Release/Test*.dll"



// -----------
// Build steps

// Helper active pattern for project types
let (|Fsproj|Csproj|Vbproj|Shproj|) (projFileName:string) =
    match projFileName with
    | f when f.EndsWith("fsproj") -> Fsproj
    | f when f.EndsWith("csproj") -> Csproj
    | f when f.EndsWith("vbproj") -> Vbproj
    | f when f.EndsWith("shproj") -> Shproj
    | _                           -> failwith (sprintf "Project file %s not supported. Unknown project type." projFileName)


Target "AssemblyInfo" (fun _ ->
    let getAssemblyInfoAttributes projectName =
        [Attribute.Title (projectName)
         Attribute.Product project
         Attribute.Description summary]

    let getProjectDetails projectPath =
        let projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath)
        (projectPath,
         projectName,
         System.IO.Path.GetDirectoryName(projectPath),
         (getAssemblyInfoAttributes projectName))

    !! "src/**/*.fsproj"
    |> Seq.map getProjectDetails
    |> Seq.iter (fun (projFileName, projectName, folderName, attributes) ->
        match projFileName with
        | Fsproj -> CreateFSharpAssemblyInfo (folderName </> "AssemblyInfo.fs") attributes
    )
)

// Copies binaries from default VS location to expected bin folder
// But keeps a subdirectory structure for each project in the
// src folder to support multiple project outputs
Target "CopyBinaries" (fun _ ->
    !! "src/**/*.??proj"
    -- "src/**/*.shproj"
    |>  Seq.map (fun f -> ((System.IO.Path.GetDirectoryName f) </> "bin/Release", "bin" </> (System.IO.Path.GetFileNameWithoutExtension f)))
    |>  Seq.iter (fun (fromDir, toDir) -> CopyDir toDir fromDir (fun _ -> true))
)

// Clean build results
Target "Clean" (fun _ ->
    CleanDirs ["bin"; "temp"]
)

// Build library & test project
Target "Build" (fun _ ->
    !! solutionFile
    |> MSBuildRelease "" "Rebuild"
    |> ignore
)


// Run the unit tests using test runner
Target "RunTests" (fun _ ->
    !! testAssemblies
    |> NUnit3 (fun p ->
        {p with TimeOut = TimeSpan.FromMinutes 20.})
)

// Start build
Target "All" DoNothing

"Clean"
==> "AssemblyInfo"
==> "Build"
==> "CopyBinaries"
==> "RunTests"
==> "All"

RunTargetOrDefault "All"
