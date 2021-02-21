﻿[<AutoOpen>]
module Finelines.Tasks.DotNetCoreCli.DotNetCoreCliRunTask

open Finelines
open Finelines.Tasks

type DotNetCoreCliRunRawTask =
    { Task: Task
      DotNetCoreCliRawTask: DotNetCoreCliRawTask }

type DotNetCoreCliRunTask =
    { Task: Task
      DotNetCoreCliTask: DotNetCoreCliTask }

    interface IYamlTask with
        member task.AsYamlTask = {
            Task = "DotNetCoreCLI@2"
            DisplayName = task.Task.DisplayName
            Inputs = 
                [
                    "command", Text "run"
                ] @ DotNetCoreCliTask.getInputs task.DotNetCoreCliTask 
        }

type DotNetCoreCliRunTaskBuilder() =
    member _.Yield _ : DotNetCoreCliRunRawTask =
        { Task = Task.Default
          DotNetCoreCliRawTask = DotNetCoreCliRawTask.Default }

    member _.Run (task: DotNetCoreCliRunRawTask) =
        { Task = task.Task
          DotNetCoreCliTask = DotNetCoreCliTask.convert task.DotNetCoreCliRawTask }

    [<CustomOperation "addTarget">]
    member _.AddTarget(task: DotNetCoreCliRunRawTask, target) =
        if task.DotNetCoreCliRawTask.Targets |> Parameter.exists (List.contains target) then invalidArg (nameof target) "Duplicate target."

        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Targets = task.DotNetCoreCliRawTask.Targets |> Parameter.bind (fun targets ->  targets @ [target]) } }

    [<CustomOperation "arguments">]
    member _.AddArguments(task: DotNetCoreCliRunRawTask, arguments) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with Arguments = Parameter.Set arguments } }

    [<CustomOperation "workingDirectory">]
    member _.AddWorkingDirectory(task: DotNetCoreCliRunRawTask, directory) =
        { task with DotNetCoreCliRawTask = { task.DotNetCoreCliRawTask with WorkingDirectory = Parameter.Set directory } }

    interface ITaskBuilder<DotNetCoreCliRunRawTask> with
        member _.DisplayName task name =
            { task with
                  Task = { task.Task with DisplayName = Some name } }

        member _.Condition task condition =
            { task with
                  Task = { task.Task with Condition = condition } }

        member _.ContinueOnError task =
            { task with
                  Task = { task.Task with ContinueOnError = true } }

let dotNetCoreCliRun = DotNetCoreCliRunTaskBuilder()
