# arc-validate

Home of all the tools and libraries to create and run validation of ARCs:

### ARCExpect

Expecto-like API for creating validation cases.

Example:

```fsharp
open ARCExpect

ARCExpect.test (TestID.Name INVMSO.``Investigation Metadata``.INVESTIGATION.``Investigation Title``.Name) {
    cvParams
    |> ARCExpect.ByTerm.contains INVMSO.``Investigation Metadata``.INVESTIGATION.``Investigation Title``
}
```

this will create a test case that checks if the `cvParams` contain the term "Investigation Title" from the INVMSO ontology.

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

**This is the v2 branch of arc-validate that currently undergoes significant changes.**

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