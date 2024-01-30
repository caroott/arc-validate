﻿module CLITests.PackageCommand

open Expecto
open Expecto.Tests
open Expecto.Impl
open Expecto.Logging
open System.IO
open System.Diagnostics
open Fake.Core
open ReferenceObjects

open Common
open Common.TestUtils
open TestUtils

open JUnit

[<Tests>]
let ``PackageCommand CLI Tests`` =
    testSequenced (testList "arc-validate package" [
        testSequenced (testList "list" [
            yield! 
                testFixture (Fixtures.withToolExecution 
                    true
                    "../../../../../publish/arc-validate" 
                    [|"-v"; "-package"; "list";|]
                    (get_gh_api_token())
                ) [
                    "Exit code is 0 (before install)" , 
                        fun tool args proc -> Expect.equal proc.ExitCode 0 (ErrorMessage.withProcessDiagnostics "incorrect exit code" proc tool args)
                    "test package is not listed (before install)" , 
                        fun tool args proc -> Expect.isFalse (proc.Result.Output.Contains("test")) (ErrorMessage.withProcessDiagnostics $"Console output {proc.Result.Output} did contain the package" proc tool args)
                
                ]
        ])
        testSequenced (testList "install test" [
            yield! 
                testFixture (Fixtures.withToolExecution 
                    true
                    "../../../../../publish/arc-validate" 
                    [|"-v"; "package"; "install"; "test"|]
                    (get_gh_api_token())
                ) [
                    "Exit code is 0" , 
                        fun tool args proc -> Expect.equal proc.ExitCode 0 (ErrorMessage.withProcessDiagnostics "incorrect exit code" proc tool args)
                    "Cache folder exists" ,  
                        fun tool args proc -> Expect.isTrue (Directory.Exists(expected_package_cache_folder_path)) (ErrorMessage.withCLIDiagnostics $"package cache folder was not created at {expected_package_cache_folder_path}." tool args)
                    "Cache exists" ,  
                        fun tool args proc -> Expect.isTrue (File.Exists(expected_package_cache_file_path)) (ErrorMessage.withCLIDiagnostics $"package cache was not created at {expected_package_cache_file_path}." tool args)
                    "Package script exists" ,  
                        fun tool args proc -> Expect.isTrue (File.Exists(Path.Combine(expected_package_cache_folder_path, "test.fsx"))) (ErrorMessage.withCLIDiagnostics $"package file was not installed at expected location." tool args)
                ]
        ])
        testSequenced (testList "list" [
            yield! 
                testFixture (Fixtures.withToolExecution 
                    false
                    "../../../../../publish/arc-validate" 
                    [|"-v"; "package"; "list";|]
                    (get_gh_api_token())
                ) [
                    "Exit code is 0 (after install)" , 
                        fun tool args proc -> Expect.equal proc.ExitCode 0 (ErrorMessage.withProcessDiagnostics "incorrect exit code" proc tool args)
                    "test package is listed (after install)" , 
                        fun tool args proc -> Expect.isTrue (proc.Result.Output.Contains("Name: test")) (ErrorMessage.withProcessDiagnostics $"Console output {proc.Result.Output} did not contain the package" proc tool args)
                ]
        ])
        testSequenced (testList "uninstall test" [
            yield! 
                testFixture (Fixtures.withToolExecution 
                    false
                    "../../../../../publish/arc-validate" 
                    [|"-v"; "package"; "uninstall"; "test"|]
                    (get_gh_api_token())
                ) [
                    "Exit code is 0" , 
                        fun tool args proc -> Expect.equal proc.ExitCode 0 (ErrorMessage.withProcessDiagnostics "incorrect exit code" proc tool args)
                    "Cache folder still exists" ,  
                        fun tool args proc -> Expect.isTrue (Directory.Exists(expected_package_cache_folder_path)) (ErrorMessage.withCLIDiagnostics $"package cache folder was not created at {expected_package_cache_folder_path}." tool args)
                    "Cache still exists" ,  
                        fun tool args proc -> Expect.isTrue (File.Exists(expected_package_cache_file_path)) (ErrorMessage.withCLIDiagnostics $"package cache was not created at {expected_package_cache_file_path}." tool args)
                    "test package script does not exist anymore" ,  
                        fun tool args proc -> Expect.isFalse (File.Exists(Path.Combine(expected_package_cache_folder_path, "test.fsx"))) (ErrorMessage.withCLIDiagnostics $"package file was not uninstalled at expected location." tool args)
                ]
        ])
        testSequenced (testList "list" [
            yield! 
                testFixture (Fixtures.withToolExecution 
                    false
                    "../../../../../publish/arc-validate" 
                    [|"-v"; "package"; "list";|]
                    (get_gh_api_token())
                ) [
                    "Exit code is 0 (after uninstall)" , 
                        fun tool args proc -> Expect.equal proc.ExitCode 0 (ErrorMessage.withProcessDiagnostics "incorrect exit code" proc tool args)
                    "test package is not listed (after uninstall)" , 
                        fun tool args proc -> Expect.isFalse (proc.Result.Output.Contains("Name: test")) (ErrorMessage.withProcessDiagnostics $"Console output {proc.Result.Output} did contain the package" proc tool args)
                ]
        ])
        testSequenced (testList "update-index" [
            yield! 
                testFixture (Fixtures.withToolExecution 
                    false
                    "../../../../../publish/arc-validate" 
                    [|"-v"; "package"; "update-index"|]
                    (get_gh_api_token())
                ) [
                    "Exit code is 0" , 
                        fun tool args proc -> Expect.equal proc.ExitCode 0 (ErrorMessage.withProcessDiagnostics "incorrect exit code" proc tool args)
                ]
        ])
    ])