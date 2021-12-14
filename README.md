# SubtleConduit: A RealWorld app called "Conduit" built using Sutil

## About

An attempt at building a [RealWorld](https://github.com/gothinkster/realworld) application called Conduit using F# and the [Sutil](https://github.com/davedawkins/Sutil) framework. RealWorld is a project that aims to compare different frontend and backend frameworks by building the _exact same_ Medium.com clone (called "Conduit") using a framework of the developers choice. Sutil is a F# framework for the Fable compiler to build dynamic and interactive web apps. Its philosophy is inspired by [Svelte](https://svelte.dev/) in that it doesn't have a runtime library, and instead makes all necessary function connections at compile time.

---

## View the live demo here

<a href="https://cubeofshame.codeberg.page/SubtleConduit/" target="_blank">https://cubeofshame.codeberg.page/SubtleConduit/</a>

---

## Development

The following tools are required:

* .Net 5.0
* Node.js 14.x
* PNPM 6.x

For first time setup, run the following 

    dotnet tool restore
    dotnet restore
    
    # if you don't have pnpm installed:
    npm i -g pnpm

    pnpm install

To run the project:

    pnpm run start

To build for prod:

    pnpm run build

---

## TODO

* [ ] Unit/Integration tests
* [ ] Error handling within the application itself
* [ ] Commenting system
* [ ] Icons for header menu items when logged in
* [ ] Routing (URL) is all sorts of janky