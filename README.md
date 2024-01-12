# arc-validate

**This is the v2 branch of arc-validate that currently undergoes significant changes.**

Home of all the tools and libraries to create and run validation of ARCs:

### ARCExpect

`ARCExpect` offers easy to use and understand APIs to create and execute validation cases. The main intention of this library is offering a framework for validation of [ControlledVocabulary](https://github.com/nfdi4plants/ARCTokenization) tokens in **Research Data management (RDM)** based on testing principles from the world of software development:
- a `validation case` is the equivalent of a unit test
- `Validate.*` functions are the equivalent of `Assert.*`, `Expect.*`, or equivalent functions
- A `BadgeCreation` API based on [AnyBadge.NET](https://github.com/kMutagene/AnyBadge.NET) enables creation of badges that visualize the validation results.
- export of the validation results as a `junit` xml file enables further integration into e.g. CI/CD pipelines

**User-facing APIs:**

- `Validate` :
  - validate **ControlledVocabulary** tokens, e.g. for their compliance with a reference ontology or for the type and shape of annotated value
- `BadgeCreation`:
  - Create and style small .svg images that visualize the validation results of a validation suite

**Additional APIs:**

- `OBOGraph`
  - Create, complete, and query the graph representation of an ontology
- `ARCGraph`
  - Create, complete, and query a graph of ControlledVocabulary tokens based on the relations in a reference ontology

Example:

```fsharp
open ARCExpect

ARCExpect.validationCase (TestID.Name INVMSO.``Investigation Metadata``.INVESTIGATION.``Investigation Title``.Name) {
    params
    |> Validate.ParamCollection.ContainsParamWithTerm 
        INVMSO.``Investigation Metadata``.INVESTIGATION.``Investigation Title``
}
```

this will create a test case that checks if one of the `params` contain the term "Investigation Title" from the INVMSO ontology.

Let's take a closer look at some API specifics:

- `ARCExpect.validationCase` creates a validation case that can be evaluated
- The `Validate` API is designed to be very close to natural language. `Validate.ParamCollection.ContainsParamWithTerm` means "validate (for a) ParamCollection (that it) contains (a) param with (the given) term"

### ARCValidationPackages

API for installing, updating, and executing ARC validation packages. Validation packages are scripts containing validation cases created with ARCExpect to validate an ARC for a specific target (e.g. for publishing with invenio, PRIDE compatibility, etc.). Packages are currently hosted at [](https://github.com/nfdi4plants/arc-validate-packages) and can be managed locally via the ARCValidationPackages library.

Example:

```fsharp
open ARCValidationPackages
open ARCValidationPackages.API

let verbose = true // verbosity prints diagnostic info
let config = Config.get() // either get config from default path or initialize new
let cache = PackageCache.get() // either get package cache from default path or initialize new
let packageName = "invenio" // name of the package to install
installPackage verbose config cache packageName // installs package and adds it to the package cache
```

### arc-validate

`arc-validate` is a CLI tool that offers commands for:
- validating ARCs via `arc-validate validate`
- manage validation packages via `arc-validate package`

CLI structure will be put here and documented once more finalized

### Docker container

This repository provides a docker container that has the `arc-validate` tool pre-installed for using it in [DataHUB](https://git.nfdi4plants.org/explore)-CI jobs.  


## Project aim

Validation of ARCs based on:
- [**ARCTokenization**](): Structural ontologies for file formats (for parsing/tokenizing files): INVMSO, STDMSO, ASSMSO
- [**OBO.NET**]():
    - parsing ontologies, generation of **OBO graphs** based on ontology term relation
    - code genearation of ontology modules with accessible terms
- [**ARCGraph**](): Graph representation of file content based on structural ontologies via **OBO graph**
- [**Graph-based**]() completion of File content (missing cells -> empty tokens) via **ARCGraph**
- [**ARCExpect**](): Expecto-like API for creating validation cases
- [**Validation Packages**](): API for installing and executing additional validation packages

## Project layout

```mermaid
flowchart TD

ControlledVocabulary("<b>ControlledVocabulary:</b><br>Data model for CVs")
ARCTokenization("<b>ARCTokenization:</b><br>Tokenization of ARCs into CVs")
OBO.NET("<b>OBO.NET:</b><br>OBO Ontology data model and parsing")
ARCGraph("<b>ARCGraph:</b><br>Graph based on structural ontologies")
ARCExpect("<b>ARCExpect:</b><br>Expecto-like API for validation")
ARCValidationPackages("<b>ARCValidationPackages:</b><br>API for additional validation packages")
arc-validate("<b>arc-validate:</b><br>validation CLI tool")

arc-validate --depends on--> ARCExpect
arc-validate --depends on--> ARCValidationPackages
ARCTokenization --depends on--> ControlledVocabulary
ARCTokenization --depends on--> OBO.NET
ARCExpect --depends on--> ARCGraph
ARCExpect --depends on--> ARCTokenization
ARCGraph --depends on--> ARCTokenization
ARCGraph --depends on--> OBO.NET
```