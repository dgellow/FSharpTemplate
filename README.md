# Template for F# projects [![CircleCI](https://circleci.com/gh/dgellow/FSharpTemplate.svg?style=svg)](https://circleci.com/gh/dgellow/FSharpTemplate)

F# plus [Paket](http://fsprojects.github.io/Paket/) to manage depedencies and [FAKE](https://fsharp.github.io/FAKE/) to manage builds.

## First step, clone this project

```
# Clone the repository
git clone git@github.com:dgellow/FSharpTemplate.git
cd FSharpTemplate
```

## The easy way (if you have docker)

There is a `Dockerfile`, if you have [docker](http://docker.com/) you can run the project in an F# environment

```
# Build the image & run the project in a container
docker build -t fsharp_app && docker run fsharp_app
```

## The easy way (if you are in an F# friendly environment)

You can bootstrap and run the build process by using the script `build.sh`.
It'll download latest Paket version, depedencies, run tests and build the project.


```
# Run the build process
bash build.sh
``


## Fetch dependencies with Paket

Paket is used to managed dependencies and can fetch from a variety of sources (NuGet packages, git repositories).
Paket tools are installed in the directory `.paket` at the root of the project.

## Build with FAKE

FAKE is a tool similar to Make but using F# scripts. It is considered as a dependency of the project and is installed by Paket.
FAKE tools are installed in `packages/build/FAKE/tools` after a `.paket/paket.exe install` (or a run of `build.sh`).
